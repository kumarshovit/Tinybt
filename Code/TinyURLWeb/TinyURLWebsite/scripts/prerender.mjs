import { mkdir, readFile, writeFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { render, getPrerenderRoutes, getSitemapRoutes } from "../dist-ssr/entry-prerender.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const rootDir = path.resolve(__dirname, "..");
const distDir = path.join(rootDir, "dist");
const template = await readFile(path.join(distDir, "index.html"), "utf8");

// Save clean SPA template as shell.html before prerendering overwrites index.html.
// IIS uses /shell.html for SPA fallback routes (/dashboard, /preview/*, /:shortCode)
// so dynamic routes never flash the prerendered landing page before React hydrates.
await writeFile(path.join(distDir, "shell.html"), template);
console.log("created SPA fallback shell -> shell.html");

const routes = getPrerenderRoutes();
console.log(`Discovered ${routes.length} routes to prerender from route registry.`);

function stripClientSeo(html) {
  return html
    .replace(/<title>[\s\S]*?<\/title>\s*/i, "")
    .replace(
      /\s*<meta\s+name="(?:description|robots|keywords|author|twitter:[^"]+)"[^>]*>\s*/gi,
      ""
    )
    .replace(/\s*<meta\s+property="og:[^"]+"[^>]*>\s*/gi, "")
    .replace(/\s*<link\s+rel="canonical"[^>]*>\s*/gi, "")
    .replace(/\s*<link\s+rel="alternate"\s+hreflang="[^"]+"\s+href="[^"]*"\s*\/?>\s*/gi, "")
    .replace(
      /\s*<script\s+type="application\/ld\+json">[\s\S]*?<\/script>\s*/gi,
      ""
    );
}

function outputPathForRoute(route) {
  if (route === "/") return path.join(distDir, "index.html");
  const clean = route.replace(/^\/+|\/+$/g, "");
  return path.join(distDir, clean, "index.html");
}

function splitHeadTags(renderedHtml) {
  const headTags = [];
  let html = renderedHtml;

  while (true) {
    const match = html.match(
      /^\s*(<title[\s\S]*?<\/title>|<meta\b[^>]*\/?>|<link\b[^>]*\/?>|<script\b[^>]*type="application\/ld\+json"[^>]*>[\s\S]*?<\/script>)/i
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

let prerenderSuccessCount = 0;
let prerenderErrorCount = 0;

for (const route of routes) {
  try {
    const rendered = render(route);
    const extracted = splitHeadTags(rendered.html);

    // Move any JSON-LD <script> tags from body into <head>
    const jsonLdRe = /<script\s+type="application\/ld\+json"[\s\S]*?<\/script>/gi;
    const jsonLdTags = extracted.html.match(jsonLdRe) || [];
    const bodyHtml = extracted.html.replace(jsonLdRe, "");

    const head = decodeEntities(
      [rendered.head, extracted.head, ...jsonLdTags].filter(Boolean).join("\n")
    );
    const html = stripClientSeo(template)
      .replace("</head>", `${head}\n  </head>`)
      .replace(
        '<div id="root"></div>',
        `<div id="root">${bodyHtml}</div>`
      );

    const outputPath = outputPathForRoute(route);
    await mkdir(path.dirname(outputPath), { recursive: true });
    await writeFile(outputPath, html);
    prerenderSuccessCount++;
    console.log(`[${prerenderSuccessCount}/${routes.length}] prerendered ${route} -> ${path.relative(distDir, outputPath)}`);
  } catch (err) {
    prerenderErrorCount++;
    console.error(`ERROR prerendering ${route}:`, err);
  }
}

console.log(`\nPrerender finished: ${prerenderSuccessCount} succeeded, ${prerenderErrorCount} failed.`);

if (prerenderErrorCount > 0) {
  throw new Error(`Prerender failed for ${prerenderErrorCount} routes!`);
}

// ==========================================
// Build-time Sitemap Generation
// Strictly write to dist/sitemap.xml (do NOT overwrite public/sitemap.xml)
// ==========================================
const sitemapRoutes = getSitemapRoutes();
const today = new Date().toISOString().split("T")[0];

const sitemapEntries = sitemapRoutes.map((r) => {
  const isHome = r.path === "/";
  const priority = isHome ? "1.0" : r.category === "core" ? "0.9" : r.category === "tools" ? "0.9" : "0.8";
  const changefreq = isHome ? "daily" : "weekly";
  return `  <url>
    <loc>${r.canonical}</loc>
    <lastmod>${today}</lastmod>
    <changefreq>${changefreq}</changefreq>
    <priority>${priority}</priority>
  </url>`;
});

const sitemapXml = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
${sitemapEntries.join("\n")}
</urlset>
`;

const distSitemapPath = path.join(distDir, "sitemap.xml");
await writeFile(distSitemapPath, sitemapXml, "utf8");
console.log(`\nGenerated dist/sitemap.xml with ${sitemapRoutes.length} verified indexable URLs.`);
console.log(`Note: public/sitemap.xml was preserved unmutated per strict guardrails.`);
