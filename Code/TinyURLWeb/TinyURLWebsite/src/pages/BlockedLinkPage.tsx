import { Link } from "react-router-dom";
import { ShieldAlert, ArrowLeft, LifeBuoy, ShieldCheck } from "lucide-react";
import SEO from "../components/SEO";
import logo from "../assets/logo.png";

const BlockedLinkPage = () => {
  return (
    <>
      <SEO
        title="Link Blocked – LinkBT Security"
        description="This link has been blocked by LinkBT security scanners to protect you from malicious or harmful destinations."
        canonical="/blocked-link"
        noindex
      />

      <div className="min-h-screen relative flex items-center justify-center overflow-hidden bg-slate-950 font-sans px-6 py-12">
        {/* Ambient Background Glows */}
        <div className="absolute inset-0 w-full h-full pointer-events-none">
          <div className="absolute top-[-10%] left-[-10%] w-[40vw] h-[40vw] bg-red-600/15 rounded-full blur-[130px] mix-blend-screen" />
          <div className="absolute bottom-[-10%] right-[-10%] w-[40vw] h-[40vw] bg-orange-600/10 rounded-full blur-[130px] mix-blend-screen" />
        </div>

        <div className="relative z-10 w-full max-w-[500px] flex flex-col items-center">
          {/* Logo */}
          <div className="mb-8">
            <Link to="/">
              <img src={logo} alt="LinkBT Logo" className="h-10 object-contain" />
            </Link>
          </div>

          <div className="w-full bg-slate-900/60 backdrop-blur-2xl border border-red-500/30 rounded-[2rem] p-8 sm:p-10 shadow-[0_8px_40px_-12px_rgba(239,68,68,0.2)] transition-all duration-500 text-center">
            {/* Warning Shield Icon */}
            <div className="flex justify-center mb-6">
              <div className="relative flex items-center justify-center w-20 h-20 rounded-full bg-red-500/10 border border-red-500/30 shadow-inner">
                <div className="absolute inset-0 bg-red-500/20 rounded-full blur-[12px] animate-pulse" />
                <ShieldAlert className="text-red-400 relative z-10" size={38} strokeWidth={1.8} />
              </div>
            </div>

            <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold uppercase tracking-wider bg-red-500/15 text-red-400 border border-red-500/30 mb-4">
              Threat Detected
            </span>

            <h1 className="text-2xl sm:text-3xl font-extrabold text-white mb-3 tracking-tight">
              Link Blocked for Your Safety
            </h1>

            <p className="text-slate-300 text-sm leading-relaxed mb-6">
              Our automated threat intelligence systems (Google Safe Browsing, URLhaus, and Cloudflare Scanner) identified this destination as potentially dangerous, malicious, or involved in phishing and malware distribution.
            </p>

            <div className="bg-red-950/40 border border-red-500/20 rounded-xl p-4 text-xs text-red-300/90 text-left mb-8 flex items-start gap-2.5">
              <ShieldCheck className="w-4 h-4 text-red-400 shrink-0 mt-0.5" />
              <span>
                To ensure your device and personal data remain protected, access to this destination has been disabled. No preview or redirection is available.
              </span>
            </div>

            {/* Action buttons */}
            <div className="flex flex-col sm:flex-row gap-3">
              <Link
                to="/"
                className="flex-1 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-500 hover:to-indigo-500 text-white py-3.5 px-4 rounded-xl font-semibold text-sm transition-all shadow-lg shadow-blue-500/20 flex items-center justify-center gap-2"
              >
                <ArrowLeft size={16} />
                <span>Go to Home</span>
              </Link>

              <Link
                to="/contact"
                className="flex-1 border border-slate-700 hover:border-slate-600 bg-slate-800/50 hover:bg-slate-800 text-slate-300 hover:text-white py-3.5 px-4 rounded-xl font-semibold text-sm transition-all flex items-center justify-center gap-2"
              >
                <LifeBuoy size={16} />
                <span>Support</span>
              </Link>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default BlockedLinkPage;
