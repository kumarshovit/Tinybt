import { mkdir, readFile, writeFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { render } from "../dist-ssr/entry-prerender.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const rootDir = path.resolve(__dirname, "..");
const distDir = path.join(rootDir, "dist");
const template = await readFile(path.join(distDir, "index.html"), "utf8");

const routes = ["/", "/contact", "/terms", "/privacy-policy", "/login", "/register", "/forgot-password"];

function stripClientSeo(html) {
  return html
    .replace(/<title>[\s\S]*?<\/title>\s*/i, "")
    .replace(
      /\s*<meta\s+name="(?:description|robots|keywords|author|twitter:[^"]+)"[^>]*>\s*/gi,
      "",
    )
    .replace(/\s*<meta\s+property="og:[^"]+"[^>]*>\s*/gi, "")
    .replace(/\s*<link\s+rel="canonical"[^>]*>\s*/gi, "")
    .replace(
      /\s*<script\s+type="application\/ld\+json">[\s\S]*?<\/script>\s*/gi,
      "",
    );
}

function outputPathForRoute(route) {
  if (route === "/") return path.join(distDir, "index.html");
  return path.join(distDir, route.slice(1), "index.html");
}

function splitHeadTags(renderedHtml) {
  const headTags = [];
  let html = renderedHtml;

  while (true) {
    const match = html.match(
      /^\s*(<title[\s\S]*?<\/title>|<meta\b[^>]*\/?>|<link\b[^>]*\/?>|<script\b[^>]*type="application\/ld\+json"[^>]*>[\s\S]*?<\/script>)/i,
    );

    if (!match) break;

    headTags.push(match[1]);
    html = html.slice(match[0].length);
  }

  return { head: headTags.join("\n"), html };
}

/** Decode HTML entities that React's renderToString double-encodes. */
function decodeEntities(str) {
  return str
    .replace(/&amp;/g, "&")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&quot;/g, '"')
    .replace(/&#x27;/g, "'");
}

for (const route of routes) {
  const rendered = render(route);
  const extracted = splitHeadTags(rendered.html);

  // Move any JSON-LD <script> tags from body into <head>
  const jsonLdRe = /<script\s+type="application\/ld\+json"[\s\S]*?<\/script>/gi;
  const jsonLdTags = extracted.html.match(jsonLdRe) || [];
  const bodyHtml = extracted.html.replace(jsonLdRe, "");

  const head = decodeEntities(
    [rendered.head, extracted.head, ...jsonLdTags].filter(Boolean).join("\n"),
  );
  const html = stripClientSeo(template)
    .replace("</head>", `${head}\n  </head>`)
    .replace(
      '<div id="root"></div>',
      `<div id="root">${bodyHtml}</div>`,
    );

  const outputPath = outputPathForRoute(route);
  await mkdir(path.dirname(outputPath), { recursive: true });
  await writeFile(outputPath, html);
  console.log(`prerendered ${route} -> ${path.relative(distDir, outputPath)}`);
}
