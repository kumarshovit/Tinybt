import { useState } from "react";
import { Link } from "react-router-dom";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { Layers, Copy, Check, AlertCircle, CheckCircle2, Loader2, ArrowRight, ShieldAlert } from "lucide-react";
import { createUrl } from "../../api/urlService";
import { isTokenValid } from "../../utils/auth";
import { buildBreadcrumbSchema, buildFAQSchema, buildSoftwareApplicationSchema } from "../../seo/schemaBuilders";

const FAQS = [
  {
    question: "How does the LinkBT Bulk Shortener work?",
    answer: "The Bulk Shortener processes up to 5 destination URLs sequentially with a conservative 2-second throttle between requests. This protects API rate limits and prevents spam triggers while ensuring 100% reliable short code generation."
  },
  {
    question: "Why is the batch size limited to 5 URLs?",
    answer: "To ensure enterprise-grade security and prevent automated abuse, batch requests are capped at 5 URLs with sequential execution. For higher throughput programmatic requirements, developers can use the authenticated LinkBT REST API."
  },
  {
    question: "What happens if one URL in the batch fails?",
    answer: "LinkBT features partial failure recovery. If one URL is invalid or blocked by our 3-engine security scanner, the remaining valid URLs will continue to be processed and shortened successfully."
  },
  {
    question: "Do bulk-shortened links support click tracking and analytics?",
    answer: "Yes! Every link generated through the bulk shortener includes standard LinkBT click tracking, geographic insights, and dynamic QR code generation."
  }
];

interface BulkItemResult {
  originalUrl: string;
  shortUrl?: string;
  status: "pending" | "processing" | "success" | "error";
  errorMessage?: string;
}

export default function BulkShortenerToolPage() {
  const [inputText, setInputText] = useState("");
  const [isProcessing, setIsProcessing] = useState(false);
  const [results, setResults] = useState<BulkItemResult[]>([]);
  const [copiedIndex, setCopiedIndex] = useState<number | null>(null);
  const [copiedAll, setCopiedAll] = useState(false);

  const isAuthenticated = typeof window !== "undefined" ? isTokenValid() : false;

  const handleProcess = async () => {
    const rawLines = inputText
      .split("\n")
      .map(l => l.trim())
      .filter(l => l.length > 0);

    if (rawLines.length === 0) return;

    if (!isAuthenticated) {
      alert("Please sign in to your LinkBT account to shorten link batches.");
      return;
    }

    if (rawLines.length > 5) {
      alert("Maximum batch size is 5 URLs. Please remove excess lines before proceeding.");
      return;
    }

    const initialItems: BulkItemResult[] = rawLines.map(url => ({
      originalUrl: url,
      status: "pending"
    }));

    setResults(initialItems);
    setIsProcessing(true);

    for (let i = 0; i < rawLines.length; i++) {
      const url = rawLines[i];

      // Update current item to processing
      setResults(prev => prev.map((item, idx) => idx === i ? { ...item, status: "processing" } : item));

      try {
        const response = await createUrl(url);

        if (response.success && response.data) {
          setResults(prev => prev.map((item, idx) => idx === i ? {
            ...item,
            status: "success",
            shortUrl: response.data.shortUrl
          } : item));
        } else {
          setResults(prev => prev.map((item, idx) => idx === i ? {
            ...item,
            status: "error",
            errorMessage: response.message || "Failed to shorten URL."
          } : item));
        }
      } catch (err: any) {
        setResults(prev => prev.map((item, idx) => idx === i ? {
          ...item,
          status: "error",
          errorMessage: err.message || "Network error occurred."
        } : item));
      }

      // Conservative rate-limit throttle: 2 seconds between sequential requests
      if (i < rawLines.length - 1) {
        await new Promise(r => setTimeout(r, 2000));
      }
    }

    setIsProcessing(false);
  };

  const handleCopyOne = (text: string, index: number) => {
    navigator.clipboard.writeText(text);
    setCopiedIndex(index);
    setTimeout(() => setCopiedIndex(null), 2000);
  };

  const handleCopyAll = () => {
    const allShorts = results
      .filter(r => r.status === "success" && r.shortUrl)
      .map(r => r.shortUrl)
      .join("\n");

    if (!allShorts) return;
    navigator.clipboard.writeText(allShorts);
    setCopiedAll(true);
    setTimeout(() => setCopiedAll(false), 2000);
  };

  const breadcrumbs = [
    { label: "Tools", href: "/tools/" },
    { label: "Bulk URL Shortener", href: "/tools/bulk-url-shortener/" }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: "Tools", url: "/tools/" },
      { name: "Bulk URL Shortener", url: "/tools/bulk-url-shortener/" }
    ]),
    buildSoftwareApplicationSchema("LinkBT Bulk URL Shortener", "Free online bulk link shortener with rate limiting and sequential batch processing."),
    buildFAQSchema(FAQS)
  ];

  return (
    <MarketingLayout
      seo={{
        title: "Free Bulk URL Shortener – Shorten Multiple Links Simultaneously | LinkBT",
        description: "Shorten up to 5 links in sequential batches with automated rate-limit protection, error recovery, and instant copy-all functionality. Free on LinkBT.",
        canonical: "/tools/bulk-url-shortener/",
        keywords: "bulk url shortener, batch link shortener, shorten multiple links, sequential bulk shortener",
        jsonLd
      }}
      badge="Batch Link Processing"
      breadcrumbs={breadcrumbs}
      title="Free Bulk URL Shortener"
      subtitle="Shorten multiple destination links in a single operation. Sequential batch processing with built-in rate-limit protection and instant copy-all export."
    >
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        {/* Input Card */}
        <div className="lg:col-span-6 bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 sm:p-8 backdrop-blur-sm shadow-xl">
          <div className="flex items-center justify-between mb-4 pb-3 border-b border-slate-700">
            <h2 className="text-xl font-bold text-white flex items-center gap-2">
              <Layers className="w-5 h-5 text-blue-400" />
              <span>Enter URLs (Max 5 per batch)</span>
            </h2>
            <span className="text-xs text-slate-400">One URL per line</span>
          </div>

          {!isAuthenticated && (
            <div className="mb-4 p-4 rounded-xl bg-amber-500/10 border border-amber-500/20 text-amber-200 text-xs flex items-start gap-2.5">
              <ShieldAlert className="w-4 h-4 text-amber-400 shrink-0 mt-0.5" />
              <div>
                <strong className="font-semibold text-white">Authentication Required:</strong> Bulk shortening requires an authenticated session to manage rate limits and link security. Please <Link to="/login" className="underline font-bold text-amber-300 hover:text-white">Sign In</Link> or <Link to="/register" className="underline font-bold text-amber-300 hover:text-white">Create an Account</Link> to process batches.
              </div>
            </div>
          )}

          <textarea
            rows={7}
            placeholder={`https://example.com/page-1\nhttps://example.com/page-2\nhttps://example.com/page-3\nhttps://example.com/page-4\nhttps://example.com/page-5`}
            value={inputText}
            onChange={(e) => setInputText(e.target.value)}
            disabled={isProcessing}
            className="w-full px-4 py-3 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 font-mono text-xs sm:text-sm leading-relaxed mb-4 disabled:opacity-50"
          />

          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-400">
              {inputText.split("\n").filter(l => l.trim().length > 0).length} / 5 URLs
            </span>

            {isAuthenticated ? (
              <button
                onClick={handleProcess}
                disabled={isProcessing || !inputText.trim()}
                className="inline-flex items-center gap-2 px-6 py-2.5 rounded-xl bg-blue-600 hover:bg-blue-500 text-white font-semibold text-sm transition-all shadow-md active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isProcessing ? (
                  <>
                    <Loader2 className="w-4 h-4 animate-spin" />
                    <span>Processing Batch...</span>
                  </>
                ) : (
                  <>
                    <span>Shorten Batch</span>
                    <ArrowRight className="w-4 h-4" />
                  </>
                )}
              </button>
            ) : (
              <Link
                to="/login"
                className="inline-flex items-center gap-2 px-6 py-2.5 rounded-xl bg-blue-600 hover:bg-blue-500 text-white font-semibold text-sm transition-all shadow-md active:scale-95"
              >
                <span>Sign In to Shorten Batch</span>
                <ArrowRight className="w-4 h-4" />
              </Link>
            )}
          </div>
        </div>

        {/* Results Card */}
        <div className="lg:col-span-6 bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 sm:p-8 backdrop-blur-sm shadow-xl">
          <div className="flex items-center justify-between mb-4 pb-3 border-b border-slate-700">
            <h3 className="text-lg font-bold text-white">Batch Results</h3>
            {results.some(r => r.status === "success") && (
              <button
                onClick={handleCopyAll}
                className="text-xs inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-slate-700 hover:bg-slate-600 text-slate-200 transition-colors"
              >
                {copiedAll ? <Check className="w-3.5 h-3.5 text-emerald-400" /> : <Copy className="w-3.5 h-3.5" />}
                <span>{copiedAll ? "All Copied!" : "Copy All"}</span>
              </button>
            )}
          </div>

          {results.length > 0 ? (
            <div className="space-y-3">
              {results.map((item, idx) => (
                <div key={idx} className="p-3.5 rounded-xl bg-slate-900 border border-slate-700/80 text-xs space-y-1.5">
                  <div className="flex items-center justify-between">
                    <span className="font-mono text-slate-400 truncate max-w-[240px]">
                      {item.originalUrl}
                    </span>
                    {item.status === "pending" && (
                      <span className="text-slate-500">Waiting...</span>
                    )}
                    {item.status === "processing" && (
                      <span className="text-blue-400 inline-flex items-center gap-1">
                        <Loader2 className="w-3 h-3 animate-spin" />
                        <span>Shortening...</span>
                      </span>
                    )}
                    {item.status === "success" && (
                      <span className="text-emerald-400 inline-flex items-center gap-1 font-semibold">
                        <CheckCircle2 className="w-3.5 h-3.5" />
                        <span>Success</span>
                      </span>
                    )}
                    {item.status === "error" && (
                      <span className="text-red-400 inline-flex items-center gap-1 font-semibold">
                        <AlertCircle className="w-3.5 h-3.5" />
                        <span>Error</span>
                      </span>
                    )}
                  </div>

                  {item.shortUrl && (
                    <div className="flex items-center justify-between pt-1 border-t border-slate-800">
                      <a
                        href={item.shortUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="font-mono text-blue-400 hover:underline font-semibold"
                      >
                        {item.shortUrl}
                      </a>
                      <button
                        onClick={() => handleCopyOne(item.shortUrl!, idx)}
                        className="p-1 text-slate-400 hover:text-white transition-colors"
                        title="Copy short link"
                      >
                        {copiedIndex === idx ? <Check className="w-3.5 h-3.5 text-emerald-400" /> : <Copy className="w-3.5 h-3.5" />}
                      </button>
                    </div>
                  )}

                  {item.errorMessage && (
                    <p className="text-red-400 text-[11px] pt-1 border-t border-slate-800">
                      {item.errorMessage}
                    </p>
                  )}
                </div>
              ))}
            </div>
          ) : (
            <div className="p-8 text-center rounded-xl bg-slate-900/50 border border-dashed border-slate-700 text-slate-400 text-sm">
              Enter your list of URLs on the left and click Shorten Batch to view live results.
            </div>
          )}
        </div>
      </div>

      {/* FAQs */}
      <section className="mt-16 pt-12 border-t border-slate-800">
        <div className="max-w-3xl">
          <h2 className="text-2xl font-bold text-white mb-4">
            Safe, Sequential Batch Link Processing
          </h2>
          <div className="space-y-4 text-slate-300 text-base leading-relaxed">
            <p>
              High-volume link generation without rate limits often results in IP blocks or server strain. LinkBT enforces a sequential 2-second interval between batch requests, verifying each destination with our 3-engine security scanner before generating a Base62 short code.
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

