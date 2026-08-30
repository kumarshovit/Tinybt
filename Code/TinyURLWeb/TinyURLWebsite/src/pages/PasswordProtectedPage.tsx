import { useState, useEffect } from "react";
import { useParams, useSearchParams, Link } from "react-router-dom";
import { Lock, Eye, EyeOff, AlertCircle, Loader2 } from "lucide-react";
import SEO from "../components/SEO";
import { verifyPassword } from "../api/urlService";
import logo from "../assets/logo.png";

const PasswordProtectedPage = () => {
    const { shortCode } = useParams<{ shortCode: string }>();
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token") ?? "";

    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const [tokenMissing] = useState(!token);

    useEffect(() => {
        if (!token) {
            setError("Access token missing. Please visit the original short link.");
        }
    }, [token]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");

        if (!password.trim()) {
            setError("Please enter the password.");
            return;
        }

        if (!token) {
            setError("Access token missing. Please visit the original short link.");
            return;
        }

        setLoading(true);
        try {
            const result = await verifyPassword(shortCode!, password, token);

            if (!result.success) {
                setError(result.message || "Incorrect password. Please try again.");
                setLoading(false);
                return;
            }

            window.location.replace(result.redirectTo!);
        } catch {
            setError("Something went wrong. Please try again.");
            setLoading(false);
        }
    };

    return (
        <>
            <SEO
                title="Password Required – LinkBT"
                description="This link is password-protected."
                canonical={`/protected/${shortCode}`}
                noindex
            />

            <div className="min-h-screen relative flex items-center justify-center overflow-hidden bg-slate-950 font-sans">
                {/* Ambient Background Glows */}
                <div className="absolute inset-0 w-full h-full pointer-events-none">
                    <div className="absolute top-[-10%] left-[-10%] w-[40vw] h-[40vw] bg-blue-600/20 rounded-full blur-[120px] mix-blend-screen" />
                    <div className="absolute bottom-[-10%] right-[-10%] w-[40vw] h-[40vw] bg-indigo-600/20 rounded-full blur-[120px] mix-blend-screen" />
                    <div className="absolute top-[40%] left-[60%] w-[30vw] h-[30vw] bg-purple-600/15 rounded-full blur-[100px] mix-blend-screen" />
                </div>

                {/* Main Card */}
                <div className="relative z-10 w-full max-w-[420px] mx-6 flex flex-col items-center">
                    {/* Logo */}
                    <div className="mb-8">
                        <Link to="/">
                            <img src={logo} alt="LinkBT Logo" className="h-10 object-contain" />
                        </Link>
                    </div>

                    <div className="w-full bg-slate-900/60 backdrop-blur-2xl border border-slate-700/50 rounded-[2rem] p-8 sm:p-10 shadow-[0_8px_40px_-12px_rgba(0,0,0,0.5)] shadow-blue-900/20 transition-all duration-500">

                        {/* Lock Icon */}
                        <div className="flex justify-center mb-8">
                            <div className="relative flex items-center justify-center w-20 h-20 rounded-full bg-slate-800/80 border border-slate-700/50 shadow-inner">
                                <div className="absolute inset-0 bg-blue-500/20 rounded-full blur-[10px] animate-pulse" />
                                <Lock className="text-blue-400 relative z-10" size={32} strokeWidth={1.5} />
                            </div>
                        </div>

                        {/* Text */}
                        <h1 className="text-2xl sm:text-3xl font-extrabold text-white text-center mb-3 tracking-tight">
                            Protected Link
                        </h1>
                        <p className="text-slate-400 text-center mb-8 text-sm leading-relaxed px-2">
                            This destination is secured. Please enter the password to unlock access.
                        </p>

                        {/* Form */}
                        {tokenMissing ? (
                            <div className="bg-red-500/10 border border-red-500/20 text-red-400 rounded-2xl p-4 text-sm text-center font-medium backdrop-blur-sm">
                                Access token missing. Please visit the original short link.
                            </div>
                        ) : (
                            <form onSubmit={handleSubmit} className="space-y-5" noValidate>

                                {/* Password Input */}
                                <div className="space-y-1.5">
                                    <label htmlFor="pwd-input" className="block text-xs font-semibold text-slate-300 uppercase tracking-wider ml-1">
                                        Password
                                    </label>
                                    <div className="relative group">
                                        <input
                                            id="pwd-input"
                                            type={showPassword ? "text" : "password"}
                                            value={password}
                                            onChange={(e) => setPassword(e.target.value)}
                                            placeholder="Enter password..."
                                            className="w-full bg-slate-950/50 border border-slate-700 rounded-2xl px-5 py-4 pr-12 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-all duration-300"
                                            disabled={loading}
                                            autoComplete="current-password"
                                        />
                                        <button
                                            type="button"
                                            onClick={() => setShowPassword(!showPassword)}
                                            className="absolute right-4 top-1/2 -translate-y-1/2 text-slate-500 hover:text-white transition-colors duration-200"
                                            aria-label={showPassword ? "Hide password" : "Show password"}
                                            tabIndex={-1}
                                        >
                                            {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                                        </button>
                                    </div>
                                </div>

                                {/* Error Alert */}
                                <div className={`transition-all duration-300 overflow-hidden ${error ? "max-h-24 opacity-100" : "max-h-0 opacity-0"}`}>
                                    <div className="flex items-start gap-2.5 bg-red-500/10 border border-red-500/20 text-red-400 rounded-xl p-3.5 text-sm backdrop-blur-sm">
                                        <AlertCircle size={18} className="shrink-0 mt-0.5 text-red-400" />
                                        <span className="font-medium">{error}</span>
                                    </div>
                                </div>

                                {/* Submit Button */}
                                <button
                                    type="submit"
                                    disabled={loading}
                                    className="w-full mt-2 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-500 hover:to-indigo-500 disabled:from-slate-600 disabled:to-slate-700 text-white font-semibold py-4 rounded-2xl shadow-lg hover:shadow-blue-500/25 disabled:shadow-none disabled:opacity-60 disabled:cursor-not-allowed transition-all duration-300 flex items-center justify-center gap-2"
                                >
                                    {loading && <Loader2 size={20} className="animate-spin text-white/70" />}
                                    <span>{loading ? "Verifying..." : "Unlock Link"}</span>
                                </button>
                            </form>
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};

export default PasswordProtectedPage;
