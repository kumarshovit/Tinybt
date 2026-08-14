import { useState, useEffect } from "react";
import { useParams, useSearchParams } from "react-router-dom";
import { Lock, Eye, EyeOff, AlertCircle, Loader2 } from "lucide-react";
import SEO from "../components/SEO";
import { verifyPassword } from "../api/urlService";

const PasswordProtectedPage = () => {
    const { shortCode } = useParams<{ shortCode: string }>();
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token") ?? "";

    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const [tokenMissing] = useState(!token);

    // If no token in URL, the user navigated here directly (not via redirect).
    // Show a clear error — they must visit the original short URL.
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

            // Navigate to the access endpoint.
            // 'credentials: include' in the fetch already ensured the HttpOnly
            // cookie was set. Now the browser navigation sends it automatically.
            window.location.replace(result.redirectTo!);
            // Don't set loading to false — the page is navigating away.
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
            <div className="min-h-screen bg-gradient-to-br from-gray-100 to-blue-50 flex items-center justify-center px-6">
                <div className="max-w-md w-full bg-white rounded-3xl shadow-2xl border border-gray-200 p-10">

                    <div className="flex justify-center mb-6">
                        <div className="bg-blue-100 p-5 rounded-full">
                            <Lock className="text-blue-600" size={48} />
                        </div>
                    </div>

                    <h1 className="text-3xl font-bold text-gray-900 text-center mb-2">
                        Password Required
                    </h1>
                    <p className="text-gray-500 text-center mb-8 text-sm leading-6">
                        This link is protected. Enter the password to continue.
                    </p>

                    {tokenMissing ? (
                        <div className="bg-red-50 border border-red-200 text-red-700 rounded-xl p-4 text-sm text-center">
                            Access token missing. Please visit the original short link.
                        </div>
                    ) : (
                        <form onSubmit={handleSubmit} className="space-y-5" noValidate>
                            <div>
                                <label
                                    htmlFor="pwd-input"
                                    className="block text-sm font-medium text-gray-700 mb-1.5"
                                >
                                    Password
                                </label>
                                <div className="relative">
                                    <input
                                        id="pwd-input"
                                        type={showPassword ? "text" : "password"}
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                        placeholder="Enter password"
                                        className="w-full border border-gray-300 rounded-xl px-4 py-3 pr-11 text-gray-900 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition"
                                        disabled={loading}
                                        autoComplete="current-password"
                                    />
                                    <button
                                        type="button"
                                        onClick={() => setShowPassword(!showPassword)}
                                        className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition"
                                        aria-label={showPassword ? "Hide password" : "Show password"}
                                        tabIndex={-1}
                                    >
                                        {showPassword
                                            ? <EyeOff size={18} />
                                            : <Eye size={18} />}
                                    </button>
                                </div>
                            </div>

                            {error && (
                                <div className="flex items-start gap-2 bg-red-50 border border-red-200 text-red-700 rounded-xl p-3 text-sm">
                                    <AlertCircle size={16} className="shrink-0 mt-0.5" />
                                    <span>{error}</span>
                                </div>
                            )}

                            <button
                                type="submit"
                                disabled={loading}
                                className="w-full bg-blue-600 hover:bg-blue-700 disabled:opacity-60 disabled:cursor-not-allowed text-white font-semibold py-3 rounded-xl transition flex items-center justify-center gap-2"
                            >
                                {loading && <Loader2 size={18} className="animate-spin" />}
                                {loading ? "Verifying…" : "Continue"}
                            </button>
                        </form>
                    )}
                </div>
            </div>
        </>
    );
};

export default PasswordProtectedPage;
