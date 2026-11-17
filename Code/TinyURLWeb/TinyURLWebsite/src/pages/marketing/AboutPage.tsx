import MarketingLayout from "../../components/marketing/MarketingLayout";
import { ShieldCheck, Zap, Lock, Globe2, Server, CheckCircle2 } from "lucide-react";
import { buildBreadcrumbSchema, buildOrganizationSchema } from "../../seo/schemaBuilders";

export default function AboutPage() {
  const breadcrumbs = [
    { label: "About Us", href: "/about/" }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: "About", url: "/about/" }
    ]),
    buildOrganizationSchema()
  ];

  return (
    <MarketingLayout
      seo={{
        title: "About LinkBT – Enterprise URL Shortening & Security Mission",
        description: "Learn about LinkBT: our commitment to link privacy, multi-engine security scanning, high-uptime global infrastructure, and transparent tools.",
        canonical: "/about/",
        keywords: "about linkbt, linkbt team, url shortener company, linkbt mission, link security infrastructure",
        jsonLd
      }}
      badge="About LinkBT"
      breadcrumbs={breadcrumbs}
      title="Building a Safer, Faster & More Transparent Web"
      subtitle="LinkBT is a modern URL shortening and link intelligence platform engineered to eliminate suspicious redirects, protect user privacy, and provide actionable analytics."
    >
      <div className="space-y-16">
        {/* Our Mission */}
        <section className="grid grid-cols-1 md:grid-cols-2 gap-10 items-center">
          <div className="space-y-4">
            <h2 className="text-2xl sm:text-3xl font-bold text-white">
              Why We Built LinkBT
            </h2>
            <p className="text-slate-300 text-base leading-relaxed">
              Traditional URL shorteners were built in an era when character counts were the only constraint. Today, short links are ubiquitous, but they are also frequently abused for phishing, malware distribution, and deceptive redirects.
            </p>
            <p className="text-slate-300 text-base leading-relaxed">
              LinkBT was created to solve this fundamental trust deficit. We engineered a platform that pairs high-speed URL compression with proactive, multi-engine security scanning. Every link shortened through LinkBT is verified before it can ever redirect a visitor.
            </p>
          </div>

          <div className="p-8 rounded-2xl bg-gradient-to-br from-blue-900/30 via-slate-800/60 to-slate-800 border border-blue-500/20 shadow-xl space-y-4">
            <h3 className="text-xl font-bold text-white flex items-center gap-2">
              <ShieldCheck className="w-5 h-5 text-blue-400" />
              <span>Our Core Principles</span>
            </h3>
            <ul className="space-y-3 text-sm text-slate-300">
              <li className="flex items-start gap-2.5">
                <CheckCircle2 className="w-4 h-4 text-emerald-400 shrink-0 mt-0.5" />
                <span><strong className="text-white">Security First:</strong> Zero tolerance for malicious links, phishing, or malware.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <CheckCircle2 className="w-4 h-4 text-emerald-400 shrink-0 mt-0.5" />
                <span><strong className="text-white">Visitor Privacy:</strong> No creepy ad-retargeting pixels or sale of visitor browsing habits.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <CheckCircle2 className="w-4 h-4 text-emerald-400 shrink-0 mt-0.5" />
                <span><strong className="text-white">Sub-50ms Redirects:</strong> High-performance architecture ensuring immediate page loads.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <CheckCircle2 className="w-4 h-4 text-emerald-400 shrink-0 mt-0.5" />
                <span><strong className="text-white">Developer Friendly:</strong> Clean, well-documented REST APIs with robust rate limiting.</span>
              </li>
            </ul>
          </div>
        </section>

        {/* Technical Architecture */}
        <section className="p-8 sm:p-10 rounded-2xl bg-slate-800/60 border border-slate-700/60 space-y-6">
          <h2 className="text-2xl font-bold text-white flex items-center gap-2">
            <Server className="w-6 h-6 text-blue-400" />
            <span>High-Reliability Architecture</span>
          </h2>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
            <div className="p-5 rounded-xl bg-slate-900 border border-slate-800">
              <div className="w-10 h-10 rounded-lg bg-blue-500/10 text-blue-400 flex items-center justify-center mb-3">
                <Zap className="w-5 h-5" />
              </div>
              <h3 className="font-semibold text-white mb-1">.NET 10 Core Engine</h3>
              <p className="text-xs text-slate-400 leading-relaxed">
                Powered by modern C# 13 and .NET 10 with asynchronous I/O and bounded collision retry handling.
              </p>
            </div>

            <div className="p-5 rounded-xl bg-slate-900 border border-slate-800">
              <div className="w-10 h-10 rounded-lg bg-emerald-500/10 text-emerald-400 flex items-center justify-center mb-3">
                <Lock className="w-5 h-5" />
              </div>
              <h3 className="font-semibold text-white mb-1">3-Engine Threat Pipeline</h3>
              <p className="text-xs text-slate-400 leading-relaxed">
                Automated scanning across Google Safe Browsing, URLhaus intelligence, and Cloudflare Scanner API.
              </p>
            </div>

            <div className="p-5 rounded-xl bg-slate-900 border border-slate-800">
              <div className="w-10 h-10 rounded-lg bg-violet-500/10 text-violet-400 flex items-center justify-center mb-3">
                <Globe2 className="w-5 h-5" />
              </div>
              <h3 className="font-semibold text-white mb-1">Global Edge Caching</h3>
              <p className="text-xs text-slate-400 leading-relaxed">
                Fast HTTP 301 responses served from geographically distributed points of presence worldwide.
              </p>
            </div>
          </div>
        </section>
      </div>
    </MarketingLayout>
  );
}

