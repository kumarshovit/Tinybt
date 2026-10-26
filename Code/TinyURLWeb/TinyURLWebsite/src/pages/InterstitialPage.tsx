import { useState, useEffect } from "react";
import { useParams, useSearchParams, useNavigate, Link } from "react-router-dom";
import {
  ShieldCheck,
  ShieldAlert,
  ExternalLink,
  ArrowRight,
  XCircle,
  Copy,
  Check,
  Loader2,
  AlertTriangle,
  Globe,
  Clock
} from "lucide-react";
import SEO from "../components/SEO";
import { getPreview, continueRedirect, type UrlPreviewData } from "../api/urlService";
import logo from "../assets/logo.png";

const InterstitialPage = () => {
  const { shortCode } = useParams<{ shortCode: string }>();
  const [searchParams] = useSearchParams();
  const token = searchParams.get("t") ?? "";
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [continuing, setContinuing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isBlocked, setIsBlocked] = useState(false);
  const [previewData, setPreviewData] = useState<UrlPreviewData | null>(null);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    if (!shortCode) {
      setError("Short code is missing.");
      setLoading(false);
      return;
    }

    if (!token) {
      setError("Preview token is missing or expired. Please visit the original short link.");
      setLoading(false);
      return;
    }

    let isMounted = true;

    const fetchPreview = async () => {
      setLoading(true);
      setError(null);

      const res = await getPreview(shortCode, token);

      if (!isMounted) return;

      if (!res.success) {
        if (res.isBlocked) {
          setIsBlocked(true);
          setError(res.message || "This destination has been blocked because it was flagged as malicious or harmful.");
        } else if (res.isExpired) {
          navigate("/expired-link", { replace: true });
          return;
        } else if (res.isNotFound) {
          navigate("/not-found", { replace: true });
          return;
        } else {
          setError(res.message || "Unable to load destination preview.");
        }
        setLoading(false);
        return;
      }

      setPreviewData(res.data!);
      setLoading(false);
    };

    fetchPreview();

    return () => {
      isMounted = false;
    };
  }, [shortCode, token, navigate]);

  const handleContinue = async () => {
    if (!shortCode || !token) return;

    setContinuing(true);
    setError(null);

    const res = await continueRedirect(shortCode, token);

    if (!res.success) {
      if (res.isBlocked) {
        setIsBlocked(true);
        setError("This destination was flagged as unsafe and cannot be opened.");
      } else {
        setError(res.message || "Redirection failed. Please try again.");
      }
      setContinuing(false);
      return;
    }

    // Authoritative destination redirect: browser navigates to target URL
    window.location.replace(res.redirectTo!);
  };

  const handleCancel = () => {
    // Visitor opted out. No continue request is sent, so no click is logged.
    navigate("/", { replace: true });
  };

  const handleCopyUrl = () => {
    if (previewData?.destinationUrl) {
      navigator.clipboard.writeText(previewData.destinationUrl);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  return (
    <>
      <SEO
        title="Destination Preview – LinkBT"
        description="Verify your destination before navigating safely."
        canonical={`/preview/${shortCode}`}
        noindex
      />

      <div className="min-h-screen relative flex items-center justify-center overflow-hidden bg-slate-950 font-sans px-4 py-12 text-slate-100">
        {/* Ambient Gradient Background */}
        <div className="absolute inset-0 w-full h-full pointer-events-none">
          <div className="absolute top-[-10%] left-[-10%] w-[45vw] h-[45vw] bg-blue-600/15 rounded-full blur-[140px] mix-blend-screen" />
          <div className="absolute bottom-[-10%] right-[-10%] w-[45vw] h-[45vw] bg-indigo-600/15 rounded-full blur-[140px] mix-blend-screen" />
          <div className="absolute top-[40%] right-[30%] w-[35vw] h-[35vw] bg-emerald-600/10 rounded-full blur-[120px] mix-blend-screen" />
        </div>

        <div className="relative z-10 w-full max-w-[540px] flex flex-col items-center">
          {/* Logo Header */}
          <div className="mb-6">
            <Link to="/">
              <img src={logo} alt="LinkBT Logo" className="h-10 object-contain" />
            </Link>
          </div>

          {/* Main Glass Card */}
          <div className="w-full bg-slate-900/70 backdrop-blur-2xl border border-slate-700/60 rounded-[2rem] p-7 sm:p-9 shadow-[0_12px_50px_-12px_rgba(0,0,0,0.6)] shadow-blue-900/20 transition-all duration-300">
            {/* Loading State */}
            {loading && (
              <div className="flex flex-col items-center justify-center py-12 space-y-4 text-center">
                <div className="relative flex items-center justify-center w-16 h-16 rounded-full bg-blue-500/10 border border-blue-500/20">
                  <div className="absolute inset-0 bg-blue-500/20 rounded-full blur-[8px] animate-pulse" />
                  <Loader2 className="w-8 h-8 text-blue-400 animate-spin" />
                </div>
                <p className="text-slate-300 font-medium text-base">Verifying link safety...</p>
                <p className="text-slate-500 text-xs">Inspecting destination threat status and security certificates</p>
              </div>
            )}

            {/* Blocked / Malicious State (Per requirement: no destination preview page is created) */}
            {!loading && isBlocked && (
              <div className="text-center py-4">
                <div className="flex justify-center mb-5">
                  <div className="relative flex items-center justify-center w-20 h-20 rounded-full bg-red-500/15 border border-red-500/30">
                    <div className="absolute inset-0 bg-red-500/20 rounded-full blur-[10px] animate-pulse" />
                    <ShieldAlert className="text-red-400 relative z-10" size={38} />
                  </div>
                </div>

                <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold uppercase tracking-wider bg-red-500/15 text-red-400 border border-red-500/30 mb-3">
                  Dangerous Link Blocked
                </span>

                <h1 className="text-2xl font-bold text-white mb-3">Destination Blocked</h1>
                <p className="text-slate-300 text-sm leading-relaxed mb-6">
                  {error || "Our automated safety checks identified this destination as malicious or harmful. Access is strictly blocked for your security."}
                </p>

                <div className="bg-red-950/40 border border-red-500/20 rounded-xl p-4 text-xs text-red-300/90 text-left mb-6">
                  Preview and continuation have been disabled. No visit or click was recorded.
                </div>

                <div className="flex justify-center">
                  <button
                    onClick={handleCancel}
                    className="w-full sm:w-auto px-6 py-3 bg-slate-800 hover:bg-slate-700 text-white rounded-xl font-medium text-sm transition"
                  >
                    Return to Safety
                  </button>
                </div>
              </div>
            )}

            {/* Generic Error / Token Missing State */}
            {!loading && !isBlocked && error && !previewData && (
              <div className="text-center py-4">
                <div className="flex justify-center mb-5">
                  <div className="w-16 h-16 rounded-full bg-amber-500/15 border border-amber-500/30 flex items-center justify-center">
                    <AlertTriangle className="text-amber-400" size={32} />
                  </div>
                </div>
                <h2 className="text-xl font-bold text-white mb-2">Unable to Preview Link</h2>
                <p className="text-slate-300 text-sm mb-6">{error}</p>
                <button
                  onClick={handleCancel}
                  className="px-6 py-3 bg-blue-600 hover:bg-blue-500 text-white rounded-xl font-medium text-sm transition"
                >
                  Go to Home
                </button>
              </div>
            )}

            {/* Safe Destination Preview Card */}
            {!loading && !isBlocked && previewData && (
              <div>
                {/* Safety Status Pill */}
                <div className="flex items-center justify-between mb-6">
                  <div className="inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full text-xs font-semibold bg-emerald-500/15 text-emerald-400 border border-emerald-500/30">
                    <ShieldCheck size={16} className="text-emerald-400" />
                    <span>Verified Safe Destination</span>
                  </div>

                  {previewData.expirationDate && (
                    <div className="inline-flex items-center gap-1.5 text-xs text-slate-400">
                      <Clock size={13} />
                      <span>Expires {new Date(previewData.expirationDate).toLocaleDateString()}</span>
                    </div>
                  )}
                </div>

                {/* Title */}
                <h1 className="text-2xl sm:text-3xl font-extrabold text-white mb-2 tracking-tight">
                  You are heading to
                </h1>
                <p className="text-slate-400 text-sm leading-relaxed mb-6">
                  LinkBT preview gives you full transparency before redirecting to external sites.
                </p>

                {/* Destination Display Box */}
                <div className="bg-slate-950/70 border border-slate-700/80 rounded-2xl p-5 mb-6 space-y-3 shadow-inner">
                  {/* Prominent Domain Display */}
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-xl bg-blue-500/10 border border-blue-500/20 flex items-center justify-center shrink-0">
                      <Globe className="w-5 h-5 text-blue-400" />
                    </div>
                    <div className="overflow-hidden">
                      <span className="text-xs uppercase tracking-wider text-slate-400 font-semibold block">
                        Target Domain
                      </span>
                      <span className="text-lg font-bold text-white truncate block">
                        {previewData.destinationDomain}
                      </span>
                    </div>
                  </div>

                  {/* Full Destination URL with Copy Action */}
                  <div className="pt-2 border-t border-slate-800/80 flex items-center justify-between gap-3">
                    <div className="overflow-hidden font-mono text-xs text-slate-300 break-all select-all">
                      {previewData.destinationUrl}
                    </div>
                    <button
                      type="button"
                      onClick={handleCopyUrl}
                      title="Copy URL"
                      aria-label="Copy destination URL"
                      className="p-2 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-400 hover:text-white transition shrink-0"
                    >
                      {copied ? <Check size={14} className="text-emerald-400" /> : <Copy size={14} />}
                    </button>
                  </div>
                </div>

                {/* Error Banner (if continue fails) */}
                {error && (
                  <div className="mb-5 bg-red-500/10 border border-red-500/30 text-red-300 p-3.5 rounded-xl text-xs flex items-center gap-2">
                    <AlertTriangle size={16} className="shrink-0 text-red-400" />
                    <span>{error}</span>
                  </div>
                )}

                {/* Action Buttons */}
                <div className="flex flex-col sm:flex-row gap-3 pt-2">
                  <button
                    type="button"
                    onClick={handleContinue}
                    disabled={continuing}
                    className="flex-1 bg-gradient-to-r from-blue-600 via-indigo-600 to-blue-500 hover:from-blue-500 hover:to-indigo-500 disabled:opacity-60 text-white font-semibold py-4 px-5 rounded-2xl shadow-lg shadow-blue-500/25 transition-all duration-200 flex items-center justify-center gap-2 group cursor-pointer"
                  >
                    {continuing ? (
                      <>
                        <Loader2 size={18} className="animate-spin text-white" />
                        <span>Redirecting...</span>
                      </>
                    ) : (
                      <>
                        <span>Continue to Site</span>
                        <ArrowRight size={18} className="group-hover:translate-x-0.5 transition-transform" />
                      </>
                    )}
                  </button>

                  <button
                    type="button"
                    onClick={handleCancel}
                    disabled={continuing}
                    className="sm:w-32 border border-slate-700/80 hover:border-slate-600 bg-slate-800/40 hover:bg-slate-800 text-slate-300 hover:text-white font-semibold py-4 px-4 rounded-2xl transition-all duration-200 flex items-center justify-center gap-1.5 cursor-pointer"
                  >
                    <XCircle size={17} />
                    <span>Cancel</span>
                  </button>
                </div>

                {/* Security Note */}
                <div className="mt-6 pt-5 border-t border-slate-800/60 text-center">
                  <p className="text-[11px] text-slate-500 flex items-center justify-center gap-1.5">
                    <ExternalLink size={12} />
                    Link count will register only when you click "Continue to Site".
                  </p>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
};

export default InterstitialPage;
