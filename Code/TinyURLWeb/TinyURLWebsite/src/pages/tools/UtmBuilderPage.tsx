import { useState, useMemo } from "react";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { Copy, Check, ExternalLink, HelpCircle, Sparkles, RefreshCw } from "lucide-react";
import { buildBreadcrumbSchema, buildFAQSchema, buildSoftwareApplicationSchema } from "../../seo/schemaBuilders";

const FAQS = [
  {
    question: "What is a UTM Builder?",
    answer: "A UTM Builder is a marketing tool that attaches standard tracking parameters (utm_source, utm_medium, utm_campaign, utm_term, utm_content) to a destination URL so web analytics platforms like Google Analytics 4 can accurately identify traffic sources."
  },
  {
    question: "Which UTM parameters are required?",
    answer: "While only the destination URL and Campaign Source (utm_source) are technically mandatory for basic attribution, best practice strongly recommends including Campaign Medium (utm_medium) and Campaign Name (utm_campaign) for clear grouping in Google Analytics."
  },
  {
    question: "Are UTM parameters case-sensitive?",
    answer: "Yes. Google Analytics treats 'Email', 'email', and 'EMAIL' as three separate mediums. We recommend always using lowercase letters, hyphens instead of spaces, and consistent naming conventions."
  },
  {
    question: "Can I shorten a UTM-tagged URL with LinkBT?",
    answer: "Yes! In fact, long UTM strings are often unsightly and easily truncated. Shortening a UTM-tagged URL with LinkBT preserves all underlying tracking parameters upon 301 redirect while giving you a clean, branded short link."
  }
];

export default function UtmBuilderPage() {
  const [url, setUrl] = useState("");
  const [source, setSource] = useState("");
  const [medium, setMedium] = useState("");
  const [campaign, setCampaign] = useState("");
  const [term, setTerm] = useState("");
  const [content, setContent] = useState("");
  const [copied, setCopied] = useState(false);

  const generatedUrl = useMemo(() => {
    if (!url.trim()) return "";
    try {
      let base = url.trim();
      if (!/^https?:\/\//i.test(base)) {
        base = `https://${base}`;
      }
      const parsed = new URL(base);
      if (source.trim()) parsed.searchParams.set("utm_source", source.trim().toLowerCase());
      if (medium.trim()) parsed.searchParams.set("utm_medium", medium.trim().toLowerCase());
      if (campaign.trim()) parsed.searchParams.set("utm_campaign", campaign.trim().toLowerCase().replace(/\s+/g, "-"));
      if (term.trim()) parsed.searchParams.set("utm_term", term.trim().toLowerCase());
      if (content.trim()) parsed.searchParams.set("utm_content", content.trim().toLowerCase());
      return parsed.toString();
    } catch {
      return "";
    }
  }, [url, source, medium, campaign, term, content]);

  const handleCopy = () => {
    if (!generatedUrl) return;
    navigator.clipboard.writeText(generatedUrl);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleReset = () => {
    setUrl("");
    setSource("");
    setMedium("");
    setCampaign("");
    setTerm("");
    setContent("");
  };

  const breadcrumbs = [
    { label: "Tools", href: "/tools/" },
    { label: "UTM Campaign Builder", href: "/tools/utm-builder/" }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: "Tools", url: "/tools/" },
      { name: "UTM Campaign Builder", url: "/tools/utm-builder/" }
    ]),
    buildSoftwareApplicationSchema("LinkBT Free UTM Builder", "Free online UTM parameter campaign URL generator for Google Analytics 4."),
    buildFAQSchema(FAQS)
  ];

  return (
    <MarketingLayout
      seo={{
        title: "Free UTM Builder – Google Analytics 4 Campaign URL Generator | LinkBT",
        description: "Generate clean, standard UTM campaign tracking URLs for Google Analytics 4. Input source, medium, campaign, term, and content with instant copy and LinkBT shortening.",
        canonical: "/tools/utm-builder/",
        keywords: "utm builder, google analytics campaign url builder, utm code generator, ga4 utm tracking",
        jsonLd
      }}
      badge="Free Web Marketing Utility"
      breadcrumbs={breadcrumbs}
      title="Free UTM Campaign URL Builder"
      subtitle="Attach standardized Google Analytics 4 tracking parameters to any destination link to measure channel performance, ROI, and conversion attribution."
    >
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        {/* Interactive Tool Card */}
        <div className="lg:col-span-7 bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 sm:p-8 backdrop-blur-sm shadow-xl">
          <div className="flex items-center justify-between mb-6 pb-4 border-b border-slate-700">
            <h2 className="text-xl font-bold text-white flex items-center gap-2">
              <Sparkles className="w-5 h-5 text-blue-400" />
              <span>Configure Campaign Parameters</span>
            </h2>
            <button
              onClick={handleReset}
              className="text-xs text-slate-400 hover:text-slate-200 flex items-center gap-1 transition-colors"
              title="Reset fields"
            >
              <RefreshCw className="w-3.5 h-3.5" />
              <span>Reset</span>
            </button>
          </div>

          <div className="space-y-4">
            {/* Website URL */}
            <div>
              <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                Website URL <span className="text-red-400">*</span>
              </label>
              <input
                type="text"
                placeholder="https://example.com/product-launch"
                value={url}
                onChange={(e) => setUrl(e.target.value)}
                className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500 text-sm"
              />
              <p className="text-xs text-slate-400 mt-1">The full destination page URL you want visitors to reach.</p>
            </div>

            {/* Campaign Source */}
            <div>
              <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                Campaign Source (<code className="text-blue-400 font-mono text-xs">utm_source</code>) <span className="text-red-400">*</span>
              </label>
              <input
                type="text"
                placeholder="e.g. google, newsletter, twitter, linkedin"
                value={source}
                onChange={(e) => setSource(e.target.value)}
                className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm"
              />
              <p className="text-xs text-slate-400 mt-1">Identifies which site or platform sent the traffic.</p>
            </div>

            {/* Campaign Medium */}
            <div>
              <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                Campaign Medium (<code className="text-blue-400 font-mono text-xs">utm_medium</code>)
              </label>
              <input
                type="text"
                placeholder="e.g. cpc, email, social, referral, banner"
                value={medium}
                onChange={(e) => setMedium(e.target.value)}
                className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm"
              />
              <p className="text-xs text-slate-400 mt-1">Identifies the marketing medium (e.g. cpc, social, email).</p>
            </div>

            {/* Campaign Name */}
            <div>
              <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                Campaign Name (<code className="text-blue-400 font-mono text-xs">utm_campaign</code>)
              </label>
              <input
                type="text"
                placeholder="e.g. spring-sale, webinar-may, product-v2"
                value={campaign}
                onChange={(e) => setCampaign(e.target.value)}
                className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm"
              />
              <p className="text-xs text-slate-400 mt-1">Identifies the specific promotional campaign or promotion theme.</p>
            </div>

            {/* Campaign Term & Content */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                  Campaign Term (<code className="text-blue-400 font-mono text-xs">utm_term</code>)
                </label>
                <input
                  type="text"
                  placeholder="e.g. url-shortener"
                  value={term}
                  onChange={(e) => setTerm(e.target.value)}
                  className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm"
                />
              </div>

              <div>
                <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                  Campaign Content (<code className="text-blue-400 font-mono text-xs">utm_content</code>)
                </label>
                <input
                  type="text"
                  placeholder="e.g. top-button, banner-blue"
                  value={content}
                  onChange={(e) => setContent(e.target.value)}
                  className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm"
                />
              </div>
            </div>
          </div>
        </div>

        {/* Live Output Card & Quick Actions */}
        <div className="lg:col-span-5 space-y-6">
          <div className="bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 backdrop-blur-sm shadow-xl">
            <h3 className="text-base font-bold text-white mb-3">Generated Campaign URL</h3>

            {generatedUrl ? (
              <div>
                <div className="p-3.5 rounded-xl bg-slate-900/90 border border-slate-700 font-mono text-xs text-blue-300 break-all select-all max-h-36 overflow-y-auto mb-4 leading-relaxed">
                  {generatedUrl}
                </div>

                <div className="flex flex-col sm:flex-row gap-3">
                  <button
                    onClick={handleCopy}
                    className="flex-1 inline-flex items-center justify-center gap-2 px-4 py-2.5 rounded-xl bg-blue-600 hover:bg-blue-500 text-white font-semibold text-sm transition-all shadow-md active:scale-95"
                  >
                    {copied ? (
                      <>
                        <Check className="w-4 h-4 text-emerald-300" />
                        <span>Copied to Clipboard!</span>
                      </>
                    ) : (
                      <>
                        <Copy className="w-4 h-4" />
                        <span>Copy URL</span>
                      </>
                    )}
                  </button>

                  <a
                    href={`/?url=${encodeURIComponent(generatedUrl)}`}
                    className="inline-flex items-center justify-center gap-1.5 px-4 py-2.5 rounded-xl bg-slate-700 hover:bg-slate-600 text-slate-200 font-medium text-sm transition-colors"
                  >
                    <span>Shorten with LinkBT</span>
                    <ExternalLink className="w-3.5 h-3.5" />
                  </a>
                </div>
              </div>
            ) : (
              <div className="p-8 text-center rounded-xl bg-slate-900/50 border border-dashed border-slate-700 text-slate-400 text-sm">
                Enter your Website URL and at least a Campaign Source to preview your tracking link.
              </div>
            )}
          </div>

          {/* Quick Best Practice Tips */}
          <div className="bg-blue-950/30 border border-blue-900/40 rounded-2xl p-6 text-sm text-slate-300 space-y-3">
            <h4 className="font-semibold text-blue-300 flex items-center gap-1.5">
              <HelpCircle className="w-4 h-4 text-blue-400" />
              <span>UTM Best Practices</span>
            </h4>
            <ul className="space-y-2 text-xs text-slate-400 list-disc list-inside">
              <li>Always use <strong className="text-slate-200">lowercase</strong> to prevent split rows in GA4.</li>
              <li>Use <strong className="text-slate-200">hyphens</strong> instead of spaces or underscores.</li>
              <li>Keep parameters clean; avoid redundant or personally identifiable data.</li>
              <li>Shorten long UTM links with LinkBT to prevent wrapping on SMS or social media.</li>
            </ul>
          </div>
        </div>
      </div>

      {/* In-depth Educational Guide Section */}
      <section className="mt-16 pt-12 border-t border-slate-800">
        <div className="max-w-3xl">
          <h2 className="text-2xl font-bold text-white mb-4">
            How UTM Parameters Power Digital Marketing Attribution
          </h2>
          <div className="space-y-4 text-slate-300 text-base leading-relaxed">
            <p>
              UTM (Urchin Tracking Module) parameters are five standard URL query variables that marketers append to destination links. When a user clicks a UTM-tagged URL, web analytics tools like Google Analytics 4, Adobe Analytics, or Mixpanel parse these values and map the visitor to specific acquisition channels.
            </p>
            <p>
              Without UTM tracking, traffic from email newsletters, SMS alerts, PDF downloads, and social media ads often gets lumped into generic &quot;Direct&quot; or unspecified &quot;Referral&quot; buckets. Using LinkBT&apos;s free UTM Builder ensures that every conversion, lead, and sale is accurately credited to the right campaign.
            </p>
          </div>

          <h3 className="text-xl font-bold text-white mt-8 mb-3">
            Frequently Asked Questions
          </h3>
          <div className="space-y-4 mt-4">
            {FAQS.map((faq, idx) => (
              <div key={idx} className="p-5 rounded-xl bg-slate-800/60 border border-slate-700/60">
                <h4 className="font-semibold text-white text-base mb-2">{faq.question}</h4>
                <p className="text-slate-300 text-sm leading-relaxed">{faq.answer}</p>
              </div>
            ))}
          </div>
        </div>
      </section>
    </MarketingLayout>
  );
}

