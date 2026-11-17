import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { DOCS_PAGES } from "../../data/docsData";
import { ROUTE_BY_PATH } from "../../seo/routeRegistry";
import { buildBreadcrumbSchema, buildFAQSchema, buildArticleSchema } from "../../seo/schemaBuilders";
import { Terminal, Shield, Key, Zap, AlertTriangle, Copy, Check, ChevronRight } from "lucide-react";

interface CodeSnippet {
  language: string;
  label: string;
  code: string;
}

const DOCS_NAV = [
  { path: "/docs/api-overview/", title: "API Overview", icon: Terminal },
  { path: "/docs/authentication/", title: "Authentication", icon: Key },
  { path: "/docs/create-short-link/", title: "Create Short Link", icon: Zap },
  { path: "/docs/rate-limits/", title: "Rate Limits", icon: Shield },
  { path: "/docs/error-handling/", title: "Error Handling", icon: AlertTriangle }
];

const CODE_EXAMPLES: Record<string, CodeSnippet[]> = {
  "/docs/api-overview/": [
    {
      language: "bash",
      label: "cURL",
      code: `curl -X GET "https://link.bt/api/developer/ping" \\
  -H "Authorization: Bearer YOUR_API_KEY" \\
  -H "Accept: application/json"`
    },
    {
      language: "javascript",
      label: "Node.js / Fetch",
      code: `const response = await fetch("https://link.bt/api/developer/ping", {
  headers: {
    "Authorization": "Bearer YOUR_API_KEY",
    "Accept": "application/json"
  }
});
const data = await response.json();
console.log(data);`
    }
  ],
  "/docs/authentication/": [
    {
      language: "bash",
      label: "cURL Header",
      code: `curl -H "Authorization: Bearer lb_live_9a8f27b4e1c0d5e8" \\
  https://link.bt/api/developer/urls`
    },
    {
      language: "csharp",
      label: "C# / .NET",
      code: `using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", "lb_live_9a8f27b4e1c0d5e8");
var response = await client.GetAsync("https://link.bt/api/developer/urls");`
    }
  ],
  "/docs/create-short-link/": [
    {
      language: "bash",
      label: "cURL",
      code: `curl -X POST "https://link.bt/api/developer/urls" \\
  -H "Authorization: Bearer YOUR_API_KEY" \\
  -H "Content-Type: application/json" \\
  -d '{
    "originalUrl": "https://example.com/products/summer-sale?utm_source=twitter",
    "customAlias": "summer26",
    "expiresAt": "2026-12-31T23:59:59Z"
  }'`
    },
    {
      language: "javascript",
      label: "TypeScript / Fetch",
      code: `interface CreateLinkResponse {
  shortCode: string;
  shortUrl: string;
  originalUrl: string;
  createdAt: string;
}

const res = await fetch("https://link.bt/api/developer/urls", {
  method: "POST",
  headers: {
    "Authorization": "Bearer " + process.env.LINKBT_API_KEY,
    "Content-Type": "application/json"
  },
  body: JSON.stringify({
    originalUrl: "https://example.com/landing",
    customAlias: "promo-2026"
  })
});
const link: CreateLinkResponse = await res.json();`
    },
    {
      language: "csharp",
      label: "C# HttpClient",
      code: `var payload = new {
    originalUrl = "https://example.com/deal",
    customAlias = "special-offer"
};
var response = await httpClient.PostAsJsonAsync(
    "https://link.bt/api/developer/urls", payload);
var result = await response.Content.ReadFromJsonAsync<CreateUrlResult>();`
    }
  ],
  "/docs/rate-limits/": [
    {
      language: "http",
      label: "HTTP Response Headers",
      code: `HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
X-RateLimit-Limit: 120
X-RateLimit-Remaining: 118
X-RateLimit-Reset: 1731715200`
    }
  ],
  "/docs/error-handling/": [
    {
      language: "json",
      label: "Error Response 422 Unprocessable",
      code: `{
  "type": "https://link.bt/errors/validation-failed",
  "title": "Validation Error",
  "status": 422,
  "detail": "The custom alias 'pricing' is a reserved system keyword.",
  "instance": "/api/developer/urls"
}`
    }
  ]
};

interface DocsPageProps {
  forcedSlug?: string;
}

export default function DocsPage({ forcedSlug }: DocsPageProps) {
  const location = useLocation();
  let rawPath = forcedSlug || location.pathname;
  if (!rawPath.endsWith("/")) {
    rawPath = `${rawPath}/`;
  }

  // Fallback to overview if path is just /docs/
  const activePath = rawPath === "/docs/" ? "/docs/api-overview/" : rawPath;
  const content = DOCS_PAGES[activePath] || DOCS_PAGES["/docs/api-overview/"];
  const routeMeta = ROUTE_BY_PATH[activePath];

  const snippets = CODE_EXAMPLES[activePath] || CODE_EXAMPLES["/docs/api-overview/"];
  const [selectedSnippetIdx, setSelectedSnippetIdx] = useState(0);
  const [copied, setCopied] = useState(false);

  const activeSnippet = snippets[selectedSnippetIdx] || snippets[0];

  const handleCopy = () => {
    if (!activeSnippet) return;
    navigator.clipboard.writeText(activeSnippet.code);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const breadcrumbs = [
    { label: "Docs", href: "/docs/" },
    { label: content.badge || content.title, href: content.slug }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: "Documentation", url: "/docs/" },
      { name: content.title, url: content.slug }
    ]),
    buildArticleSchema({
      headline: content.title,
      description: content.subtitle,
      url: content.slug
    }),
    buildFAQSchema(content.faqs)
  ];

  return (
    <MarketingLayout
      seo={{
        title: routeMeta?.title || content.title,
        description: routeMeta?.description || content.subtitle,
        canonical: content.slug,
        keywords: routeMeta?.keywords,
        ogType: "article",
        jsonLd
      }}
      badge={content.badge}
      breadcrumbs={breadcrumbs}
      title={content.title}
      subtitle={content.subtitle}
    >
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
        {/* Navigation Sidebar */}
        <aside className="lg:col-span-1 space-y-2">
          <div className="p-4 rounded-2xl bg-slate-800/80 border border-slate-700/60 sticky top-24">
            <div className="text-xs font-semibold uppercase tracking-wider text-slate-400 mb-3 px-2">
              API Documentation
            </div>
            <nav className="space-y-1">
              {DOCS_NAV.map((nav) => {
                const isActive = nav.path === activePath;
                const Icon = nav.icon;
                return (
                  <Link
                    key={nav.path}
                    to={nav.path}
                    className={`flex items-center gap-2.5 px-3 py-2 rounded-xl text-sm font-medium transition-colors ${
                      isActive
                        ? "bg-blue-600/20 text-blue-400 border border-blue-500/30"
                        : "text-slate-300 hover:bg-slate-700/50 hover:text-white"
                    }`}
                  >
                    <Icon className="w-4 h-4 shrink-0" />
                    <span>{nav.title}</span>
                  </Link>
                );
              })}
            </nav>
            <div className="mt-6 pt-4 border-t border-slate-700/60">
              <Link
                to="/developer"
                className="flex items-center justify-between px-3 py-2 text-xs font-medium text-emerald-400 hover:text-emerald-300"
              >
                <span>Get API Keys &rarr;</span>
                <ChevronRight className="w-3.5 h-3.5" />
              </Link>
            </div>
          </div>
        </aside>

        {/* Main Content Area */}
        <div className="lg:col-span-3 space-y-10">
          {/* Intro Paragraphs */}
          <section className="space-y-4">
            {content.introParagraphs.map((p, idx) => (
              <p key={idx} className="text-slate-300 text-base sm:text-lg leading-relaxed">
                {p}
              </p>
            ))}
          </section>

          {/* Interactive Code Window */}
          {snippets.length > 0 && (
            <section className="rounded-2xl overflow-hidden border border-slate-700/80 bg-slate-950 shadow-xl">
              <div className="flex items-center justify-between px-4 py-3 bg-slate-900 border-b border-slate-800">
                <div className="flex items-center gap-2">
                  <div className="flex gap-1.5 mr-2">
                    <span className="w-3 h-3 rounded-full bg-red-500/60 inline-block" />
                    <span className="w-3 h-3 rounded-full bg-amber-500/60 inline-block" />
                    <span className="w-3 h-3 rounded-full bg-emerald-500/60 inline-block" />
                  </div>
                  <div className="flex gap-1">
                    {snippets.map((snip, idx) => (
                      <button
                        key={idx}
                        onClick={() => setSelectedSnippetIdx(idx)}
                        className={`px-3 py-1 text-xs font-semibold rounded-lg transition-colors ${
                          selectedSnippetIdx === idx
                            ? "bg-slate-800 text-blue-400"
                            : "text-slate-400 hover:text-slate-200"
                        }`}
                      >
                        {snip.label}
                      </button>
                    ))}
                  </div>
                </div>
                <button
                  onClick={handleCopy}
                  className="flex items-center gap-1.5 px-2.5 py-1 text-xs rounded-lg bg-slate-800 text-slate-300 hover:text-white transition-colors"
                >
                  {copied ? <Check className="w-3.5 h-3.5 text-emerald-400" /> : <Copy className="w-3.5 h-3.5" />}
                  <span>{copied ? "Copied" : "Copy"}</span>
                </button>
              </div>
              <pre className="p-4 text-xs sm:text-sm text-slate-200 font-mono overflow-x-auto leading-relaxed">
                <code>{activeSnippet.code}</code>
              </pre>
            </section>
          )}

          {/* Key Specifications / Benefits */}
          <section className="space-y-4">
            <h2 className="text-xl font-bold text-white">Specifications & Requirements</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {content.keyBenefits.map((item, idx) => (
                <div
                  key={idx}
                  className="p-4 rounded-xl bg-slate-800/40 border border-slate-700/50 hover:border-blue-500/30 transition-colors"
                >
                  <h3 className="text-base font-semibold text-white mb-1.5">{item.title}</h3>
                  <p className="text-sm text-slate-300 leading-relaxed">{item.description}</p>
                </div>
              ))}
            </div>
          </section>

          {/* Deep-Dive Sections */}
          {content.deepDiveSections.map((sec, idx) => (
            <section key={idx} className="space-y-3">
              <h2 className="text-xl font-bold text-white">{sec.heading}</h2>
              <p className="text-slate-300 text-sm sm:text-base leading-relaxed">{sec.content}</p>
            </section>
          ))}

          {/* FAQs */}
          {content.faqs.length > 0 && (
            <section className="space-y-4 pt-6 border-t border-slate-800">
              <h2 className="text-xl font-bold text-white">Developer FAQ</h2>
              <div className="space-y-3">
                {content.faqs.map((faq, idx) => (
                  <div key={idx} className="p-4 rounded-xl bg-slate-800/40 border border-slate-700/50">
                    <h3 className="text-base font-semibold text-white mb-2">{faq.question}</h3>
                    <p className="text-sm text-slate-300 leading-relaxed">{faq.answer}</p>
                  </div>
                ))}
              </div>
            </section>
          )}
        </div>
      </div>
    </MarketingLayout>
  );
}

