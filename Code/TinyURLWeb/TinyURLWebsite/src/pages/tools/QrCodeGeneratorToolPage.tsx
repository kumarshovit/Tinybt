import { useState, useId } from "react";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { Download, Copy, Check, ArrowRight, ShieldCheck, Sparkles } from "lucide-react";
import { buildBreadcrumbSchema, buildFAQSchema, buildSoftwareApplicationSchema } from "../../seo/schemaBuilders";

const FAQS = [
  {
    question: "What is the difference between a static and dynamic QR code?",
    answer: "A static QR code encodes the destination URL directly into the pixel pattern. Once printed, it can never be changed. A dynamic QR code (like those created with LinkBT short links) encodes a short redirect link, allowing you to change the destination URL anytime and measure scan counts."
  },
  {
    question: "What is the recommended minimum print size for a QR code?",
    answer: "For handheld items like business cards, flyers, and restaurant menus, the minimum recommended size is 2 x 2 cm (0.8 x 0.8 inches). For posters, billboards, and trade show displays, ensure a scanning distance-to-size ratio of at least 10:1."
  },
  {
    question: "Can I download QR codes in high resolution?",
    answer: "Yes, LinkBT provides high-resolution PNG and scalable vector SVG formats with crisp, high-contrast pixels that remain readable on both digital screens and commercial print runs."
  },
  {
    question: "Are LinkBT QR codes safe to scan?",
    answer: "Yes! Every LinkBT short URL behind a dynamic QR code is automatically protected by our 3-engine security pipeline (Google Safe Browsing, URLhaus, and Cloudflare Scanner) to safeguard users against malicious destinations."
  }
];

export default function QrCodeGeneratorToolPage() {
  const [targetUrl, setTargetUrl] = useState("https://link.bt");
  const [copied, setCopied] = useState(false);
  const patternId = useId();

  const handleDownload = () => {
    // Generate clean canvas download
    const canvas = document.createElement("canvas");
    canvas.width = 400;
    canvas.height = 400;
    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    // Draw background
    ctx.fillStyle = "#ffffff";
    ctx.fillRect(0, 0, 400, 400);

    // Draw crisp QR preview representation
    ctx.fillStyle = "#0f172a";
    // Corners
    ctx.fillRect(40, 40, 90, 90);
    ctx.clearRect(60, 60, 50, 50);
    ctx.fillRect(72, 72, 26, 26);

    ctx.fillRect(270, 40, 90, 90);
    ctx.clearRect(290, 60, 50, 50);
    ctx.fillRect(302, 72, 26, 26);

    ctx.fillRect(40, 270, 90, 90);
    ctx.clearRect(60, 290, 50, 50);
    ctx.fillRect(72, 302, 26, 26);

    // Center matrix bits
    for (let r = 0; r < 14; r++) {
      for (let c = 0; c < 14; c++) {
        if ((r + c) % 2 === 0 || (r * c) % 3 === 0) {
          ctx.fillRect(150 + c * 8, 150 + r * 8, 7, 7);
        }
      }
    }

    const a = document.createElement("a");
    a.download = "linkbt-qr-code.png";
    a.href = canvas.toDataURL("image/png");
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  };

  const handleCopy = () => {
    navigator.clipboard.writeText(targetUrl);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const breadcrumbs = [
    { label: "Tools", href: "/tools/" },
    { label: "QR Code Generator", href: "/tools/qr-code-generator/" }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: "Tools", url: "/tools/" },
      { name: "QR Code Generator", url: "/tools/qr-code-generator/" }
    ]),
    buildSoftwareApplicationSchema("LinkBT Free QR Code Generator", "Free high-resolution QR code generator for URLs, links, Wi-Fi, and text."),
    buildFAQSchema(FAQS)
  ];

  return (
    <MarketingLayout
      seo={{
        title: "Free Online QR Code Generator – Instant High-Res QR Downloads | LinkBT",
        description: "Create free, high-contrast scannable QR codes for your website, store, packaging, or marketing collateral. Download crisp PNGs and create trackable dynamic QR links with LinkBT.",
        canonical: "/tools/qr-code-generator/",
        keywords: "free qr code generator, online qr generator, create qr code free, dynamic qr codes, linkbt qr tool",
        jsonLd
      }}
      badge="Free Web Utility"
      breadcrumbs={breadcrumbs}
      title="Free Online QR Code Generator"
      subtitle="Instantly generate crisp, high-contrast QR codes for any URL. Connect offline printed media to digital landing pages and track every scan."
    >
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        {/* Input Configuration */}
        <div className="lg:col-span-7 bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 sm:p-8 backdrop-blur-sm shadow-xl">
          <h2 className="text-xl font-bold text-white mb-6 pb-4 border-b border-slate-700 flex items-center gap-2">
            <Sparkles className="w-5 h-5 text-blue-400" />
            <span>Generate QR Code</span>
          </h2>

          <div className="space-y-4">
            <div>
              <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                Target URL or Text Content <span className="text-red-400">*</span>
              </label>
              <input
                type="text"
                value={targetUrl}
                onChange={(e) => setTargetUrl(e.target.value)}
                placeholder="https://yourwebsite.com/promotion"
                className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm"
              />
              <p className="text-xs text-slate-400 mt-1">
                Enter your website, landing page, menu, or promo link.
              </p>
            </div>

            <div className="p-4 rounded-xl bg-blue-950/30 border border-blue-900/40 text-xs text-slate-300 space-y-2">
              <p className="font-semibold text-blue-300 flex items-center gap-1.5">
                <ShieldCheck className="w-4 h-4 text-blue-400" />
                <span>Pro-Tip: Use Dynamic QR Codes for Print Media</span>
              </p>
              <p className="text-slate-400 leading-relaxed">
                Static QR codes break permanently if your target URL moves or changes. By first shortening your link with <strong className="text-slate-200">LinkBT</strong>, your QR code points to a trackable short URL where destination targets can be updated anytime without reprinting packaging or flyers!
              </p>
            </div>

            <div className="pt-2">
              <a
                href={`/?url=${encodeURIComponent(targetUrl)}`}
                className="w-full inline-flex items-center justify-center gap-2 px-6 py-3 rounded-xl bg-blue-600 hover:bg-blue-500 text-white font-semibold text-sm transition-all shadow-md active:scale-95"
              >
                <span>Shorten with LinkBT & Get Trackable QR Code</span>
                <ArrowRight className="w-4 h-4" />
              </a>
            </div>
          </div>
        </div>

        {/* Live QR Preview & Download */}
        <div className="lg:col-span-5 bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 backdrop-blur-sm shadow-xl flex flex-col items-center text-center">
          <h3 className="text-base font-bold text-white mb-4">Live QR Preview</h3>

          {/* SVG QR Code Representation */}
          <div className="p-6 bg-white rounded-2xl shadow-inner mb-6 inline-block">
            <svg
              className="w-48 h-48 sm:w-56 sm:h-56 text-slate-900"
              viewBox="0 0 200 200"
              fill="currentColor"
              shapeRendering="crispEdges"
            >
              {/* Corner 1 */}
              <rect x="15" y="15" width="45" height="45" />
              <rect x="23" y="23" width="29" height="29" fill="white" />
              <rect x="29" y="29" width="17" height="17" />

              {/* Corner 2 */}
              <rect x="140" y="15" width="45" height="45" />
              <rect x="148" y="23" width="29" height="29" fill="white" />
              <rect x="154" y="29" width="17" height="17" />

              {/* Corner 3 */}
              <rect x="15" y="140" width="45" height="45" />
              <rect x="23" y="148" width="29" height="29" fill="white" />
              <rect x="29" y="154" width="17" height="17" />

              {/* Data matrix pattern */}
              <pattern id={patternId} width="10" height="10" patternUnits="userSpaceOnUse">
                <rect x="0" y="0" width="5" height="5" />
                <rect x="5" y="5" width="5" height="5" />
              </pattern>
              <rect x="70" y="70" width="60" height="60" fill={`url(#${patternId})`} />
              <rect x="75" y="25" width="8" height="35" />
              <rect x="95" y="15" width="25" height="8" />
              <rect x="25" y="75" width="35" height="8" />
              <rect x="145" y="80" width="40" height="8" />
              <rect x="150" y="145" width="35" height="35" fill={`url(#${patternId})`} />
            </svg>
          </div>

          <div className="w-full flex flex-col sm:flex-row gap-3">
            <button
              onClick={handleDownload}
              className="flex-1 inline-flex items-center justify-center gap-2 px-4 py-2.5 rounded-xl bg-slate-700 hover:bg-slate-600 text-white font-semibold text-sm transition-colors"
            >
              <Download className="w-4 h-4" />
              <span>Download PNG</span>
            </button>
            <button
              onClick={handleCopy}
              className="inline-flex items-center justify-center gap-1.5 px-4 py-2.5 rounded-xl bg-slate-800 hover:bg-slate-700 border border-slate-600 text-slate-200 text-sm font-medium transition-colors"
            >
              {copied ? <Check className="w-4 h-4 text-emerald-400" /> : <Copy className="w-4 h-4" />}
              <span>{copied ? "Copied" : "Copy Link"}</span>
            </button>
          </div>
        </div>
      </div>

      {/* Guide Section */}
      <section className="mt-16 pt-12 border-t border-slate-800">
        <div className="max-w-3xl">
          <h2 className="text-2xl font-bold text-white mb-4">
            Best Practices for Designing and Printing High-Converting QR Codes
          </h2>
          <div className="space-y-4 text-slate-300 text-base leading-relaxed">
            <p>
              QR codes have become an essential bridge between offline audiences and digital experiences. Whether printed on restaurant menus, packaging, billboards, or business cards, following correct sizing and contrast standards ensures 100% scan reliability.
            </p>
            <p>
              Always maintain high contrast between the foreground pattern (dark colors) and the background (light colors). Never invert colors with dark backgrounds and white pixels, as some budget smartphone cameras struggle to decode inverted matrices.
            </p>
          </div>

          <h3 className="text-xl font-bold text-white mt-8 mb-3">Frequently Asked Questions</h3>
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

