import { useState, useRef } from "react";
import { Link, useLocation } from "react-router-dom";
import { Loader2, Zap, Sparkles, BarChart3, QrCode } from "lucide-react";
import { Turnstile } from "@marsidev/react-turnstile";
import GoogleLogin from "../components/auth/GoogleLogin";
import MicrosoftLogin from "../components/auth/MicrosoftLogin";
import SEO from "../components/SEO";
import logo from "../assets/logo.png";
import sideImage from "../assets/WhatsApp Image 2026-09-10 at 1.38.57 PM.jpeg";

const Login = () => {
  const location = useLocation();
  const [error, setError] = useState(location.state?.message || "");
  const [loading, setLoading] = useState(false);

  // 🛡️ Cloudflare Turnstile State
  const [captchaToken, setCaptchaToken] = useState("");
  const turnstileRef = useRef<any>(null);
  const siteKey = import.meta.env.VITE_TURNSTILE_SITEKEY || "";

  return (
    <div className="min-h-screen flex bg-[#0B1120] text-gray-100 font-sans selection:bg-blue-500/30 overflow-hidden relative">
      <SEO
        title="Sign In – LinkBT"
        description="Sign in to your LinkBT account with Google or Microsoft to manage short links, analytics, and dynamic QR codes."
        canonical="/login"
        noindex
      />

      {/* Left side: Premium Dark Glassmorphism Auth Card */}
      <div className="w-full lg:w-1/2 flex flex-col justify-center items-center px-4 sm:px-6 relative py-12 z-10">
        
        {/* Ambient Glows */}
        <div className="absolute -top-24 -left-24 w-96 h-96 bg-blue-600/15 rounded-full blur-[120px] pointer-events-none" />
        <div className="absolute -bottom-24 left-1/3 w-80 h-80 bg-indigo-600/10 rounded-full blur-[100px] pointer-events-none" />

        {/* Logo at Top Left */}
        <div className="absolute top-6 left-6 md:top-8 md:left-12 z-20">
          <Link to="/" className="flex items-center gap-2 group">
            <div className="bg-white/95 backdrop-blur-md px-3.5 py-1.5 rounded-xl shadow-lg shadow-black/20 border border-white/20 transition-all duration-300 group-hover:scale-105 group-hover:shadow-blue-500/20">
              <img src={logo} alt="LinkBT Logo" className="h-8 md:h-9 object-contain" />
            </div>
          </Link>
        </div>

        {/* Glass Auth Card */}
        <div className="w-full max-w-md bg-[#0f172a]/85 backdrop-blur-xl p-8 sm:p-10 rounded-3xl border border-slate-700/60 shadow-2xl shadow-black/60 mt-14 lg:mt-0 relative overflow-hidden before:absolute before:inset-x-0 before:-top-px before:h-px before:bg-gradient-to-r before:from-transparent before:via-blue-500/70 before:to-transparent">
          
          {/* Badge */}
          <div className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium bg-blue-500/10 text-blue-400 border border-blue-500/20 mb-5">
            <Sparkles className="w-3.5 h-3.5 text-blue-400" />
            <span>Fast & Seamless Access</span>
          </div>

          {/* Heading */}
          <div className="mb-6">
            <h1 className="text-3xl sm:text-4xl font-extrabold tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-white via-slate-100 to-blue-200">
              Welcome to LinkBT
            </h1>
            <p className="text-slate-400 text-sm mt-2 font-normal leading-relaxed">
              Sign in with Google or Microsoft to manage links, track analytics, and generate QR codes.
            </p>
          </div>

          {/* Feature Badges */}
          <div className="space-y-3 mb-6 bg-slate-900/70 p-4 rounded-2xl border border-slate-800/90 backdrop-blur-sm">
            <div className="flex items-center text-xs text-slate-300">
              <div className="p-1.5 rounded-lg bg-blue-500/15 text-blue-400 mr-3 shrink-0">
                <Zap className="w-4 h-4" />
              </div>
              <span className="font-medium">1-Click instant sign-in with Google or Microsoft</span>
            </div>
            
            <div className="flex items-center text-xs text-slate-300">
              <div className="p-1.5 rounded-lg bg-emerald-500/15 text-emerald-400 mr-3 shrink-0">
                <BarChart3 className="w-4 h-4" />
              </div>
              <span className="font-medium">Real-time click tracking, geolocation & referrer analytics</span>
            </div>
            
            <div className="flex items-center text-xs text-slate-300">
              <div className="p-1.5 rounded-lg bg-indigo-500/15 text-indigo-400 mr-3 shrink-0">
                <QrCode className="w-4 h-4" />
              </div>
              <span className="font-medium">Custom branded aliases, dynamic QR codes & tags</span>
            </div>
          </div>

          {/* Error Message Alert */}
          {error && (
            <div className="mb-5 p-3.5 bg-red-500/10 border border-red-500/30 rounded-xl text-red-300 text-xs text-center leading-relaxed">
              {error}
            </div>
          )}

          {/* Loading Indicator */}
          {loading && (
            <div className="flex items-center justify-center gap-2.5 py-2.5 mb-4 text-sm text-blue-400 font-medium">
              <Loader2 className="w-4 h-4 animate-spin text-blue-400" />
              <span>Authenticating securely...</span>
            </div>
          )}

          {/* 🛡️ Cloudflare Turnstile Human Verification */}
          {siteKey && (
            <div className="mb-5 flex flex-col items-center justify-center">
              <Turnstile
                ref={turnstileRef}
                siteKey={siteKey}
                onSuccess={(token) => {
                  setCaptchaToken(token);
                  setError("");
                }}
                onError={() => {
                  setError("Security verification failed. Please refresh and try again.");
                  setCaptchaToken("");
                }}
                onExpire={() => {
                  setCaptchaToken("");
                }}
              />
            </div>
          )}

          {/* Social Sign-In Buttons (Protected by Turnstile) */}
          <div className={`flex flex-col items-center justify-center gap-3 py-3 px-2 rounded-2xl bg-slate-900/50 border border-slate-800/80 shadow-inner transition-all duration-300 ${
            siteKey && !captchaToken ? "opacity-40 pointer-events-none select-none grayscale" : "opacity-100"
          }`}>
            <GoogleLogin
              theme="filled_blue"
              onError={(msg) => setError(msg)}
              onLoading={(val) => setLoading(val)}
            />
            <MicrosoftLogin
              onError={(msg) => setError(msg)}
              onLoading={(val) => setLoading(val)}
            />
          </div>

          {siteKey && !captchaToken && (
            <p className="text-center text-[11px] text-slate-400 mt-2.5">
              Please complete the security check above to sign in.
            </p>
          )}

          {/* Legal / Policy Disclaimer */}
          <p className="text-center text-xs text-slate-400 mt-7 leading-relaxed">
            By signing in, you agree to LinkBT's{" "}
            <Link to="/terms" className="text-blue-400 hover:text-blue-300 underline underline-offset-2 transition-colors">
              Terms of Service
            </Link>{" "}
            and{" "}
            <Link to="/privacy-policy" className="text-blue-400 hover:text-blue-300 underline underline-offset-2 transition-colors">
              Privacy Policy
            </Link>
            .
          </p>

        </div>
      </div>

      {/* Right side: Visual Hero Graphic */}
      <div className="hidden lg:block lg:w-1/2 relative bg-[#0B1120]">
        <img
          src={sideImage}
          alt="Welcome to LinkBT"
          className="absolute inset-0 w-full h-full object-cover"
        />
        {/* Soft edge blend overlay */}
        <div className="absolute inset-y-0 left-0 w-24 bg-gradient-to-r from-[#0B1120] to-transparent pointer-events-none" />
      </div>

    </div>
  );
};

export default Login;
