import { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import { Eye, EyeOff, Loader2 } from "lucide-react";
import { loginUser } from "../services/authService";
import GoogleLogin from "../components/auth/GoogleLogin";
import SEO from "../components/SEO";
import logo from "../assets/logo.png";
import sideImage from "../assets/WhatsApp Image 2026-09-10 at 1.38.57 PM.jpeg";

const Login = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState("");
  const [isBlocked, setIsBlocked] = useState(false);
  const timerRef = useRef<number | null>(null);

  useEffect(() => {
    return () => {
      if (timerRef.current !== null) {
        clearInterval(timerRef.current);
      }
    };
  }, []);

  useEffect(() => {
    if (error) {
      setSuccess("");
    }
  }, [error]);

  // ✅ Show/Hide Password State
  const [showPassword, setShowPassword] = useState(false);

  const handleLogin = async (e?: React.FormEvent) => {
    if (e) {
      e.preventDefault();
    }

    if (isBlocked) {
      return;
    }
    setError("");
    setSuccess("");

    // ✅ Stop previous timer
    if (timerRef.current !== null) {
      clearInterval(timerRef.current);
    }

    setLoading(true);

    try {
      await loginUser(email, password);

      window.location.replace("/dashboard");
    } catch (err: any) {
      console.log(err.response);

      const data = err.response?.data;

      // ✅ If blockedUntil exists
      if (data?.blockedUntil) {
        setIsBlocked(true);
        setSuccess("");

        const blockedTime = new Date(data.blockedUntil);

        const updateCountdown = () => {
          const now = new Date();
          const diff = blockedTime.getTime() - now.getTime();

          // ✅ Unblocked
          if (diff <= 0) {
            clearInterval(timerRef.current!);
            setIsBlocked(false);
            setError("");
            setSuccess("You can login now.");
            return;
          }

          const minutes = Math.floor(diff / 60000);
          const seconds = Math.floor((diff % 60000) / 1000);

          setError(`Too many attempts. Try again in ${minutes}m ${seconds}s`);
        };

        updateCountdown();

        timerRef.current = setInterval(() => {
          const now = new Date();
          const diff = blockedTime.getTime() - now.getTime();

          if (diff <= 0) {
            clearInterval(timerRef.current!);
            setIsBlocked(false);
            setError("");
            setSuccess("You can login now.");
            return;
          }

          updateCountdown();
        }, 1000);
      }
      // ✅ Normal backend message
      else if (typeof data === "string") {
        setSuccess("");
        setError(data);
      }
      else if (data?.message) {
        setError(data.message);
      }
      // ✅ Fallback
      else {
        setError("Login failed. Please try again.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex bg-gray-50">
      <SEO
        title="Login – LinkBT"
        description="Log in to your LinkBT account to manage your short links, view analytics, and create new branded URLs."
        canonical="/login"
        noindex
      />

      {/* Left side: Login form */}
      <div className="w-full lg:w-1/2 flex flex-col justify-center items-center px-4 relative">

        {/* Logo at the top left */}
        <div className="absolute top-6 left-6 md:top-8 md:left-12">
          <Link to="/">
            <img src={logo} alt="LinkBT Logo" className="h-10 object-contain" />
          </Link>
        </div>

        <div className="bg-white w-full max-w-md p-8 sm:p-10 rounded-2xl shadow-xl mt-16 lg:mt-0">
          {/* Heading */}
          <h2 className="text-3xl font-bold text-center text-gray-800">
            Login to LinkBT Smart Links
          </h2>

          <p className="text-center text-gray-500 mt-2 mb-6">
            Don't have an account?{" "}
            <Link
              to="/register"
              className="text-blue-600 font-semibold hover:underline"
            >
              Sign Up
            </Link>
          </p>

          {/* Form */}
          <form onSubmit={handleLogin} className="space-y-4">

            {/* Email */}
            <div>
              <label className="block text-sm font-medium text-gray-600 mb-1">
                Email
              </label>
              <input
                type="email"
                placeholder="Enter Email Address"
                className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none transition"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>

            {/* Password */}
            <div>
              <label className="block text-sm font-medium text-gray-600 mb-1">
                Password
              </label>
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  placeholder="Enter Password"
                  className="w-full px-4 py-2 pr-12 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none transition"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />

                {/* Eye Button */}
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700"
                >
                  {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
            </div>

            {/* Keep me logged in & Forgot Password */}
            <div className="flex items-center justify-between mt-2">
              <label className="flex items-center text-sm text-gray-600">
                <input type="checkbox" className="mr-2 text-blue-600 rounded border-gray-300 focus:ring-blue-500" />
                Keep me logged in
              </label>
              <Link
                to="/forgot-password"
                className="text-sm text-blue-600 hover:underline"
              >
                Forgot password?
              </Link>
            </div>

            {/* Error Message */}
            {error && (
              <p className="text-red-500 text-sm text-center">
                {error}
              </p>
            )}
            {success && (
              <p className="text-green-600 text-sm text-center">
                {success}
              </p>
            )}

            {/* Login Button */}
            <button
              type="submit"
              disabled={loading || isBlocked}
              className="w-full bg-gray-600 text-white py-2.5 mt-4 rounded-lg font-semibold hover:bg-gray-700 transition disabled:opacity-60 flex items-center justify-center gap-2"
            >
              {loading ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin" />
                  <span>Logging in...</span>
                </>
              ) : (
                "Log In"
              )}
            </button>
          </form>

          {/* Divider */}
          <div className="flex items-center my-6">
            <hr className="flex-grow border-gray-300" />
            <span className="mx-4 text-gray-500 text-sm">continue with</span>
            <hr className="flex-grow border-gray-300" />
          </div>

          {/* Social Logins */}
          <div className="flex justify-center space-x-4">
            {/* Instead of just rendering `<GoogleLogin />`, which is a custom block, maybe it fits the current style */}
            <GoogleLogin />
          </div>

        </div>
      </div>

      {/* Right side: Image */}
      <div className="hidden lg:block lg:w-1/2 relative bg-[#111827]">
        {/* Use object-contain or object-cover based on image aspect ratio, object-cover usually fills */}
        <img
          src={sideImage}
          alt="Welcome to LinkBT"
          className="absolute inset-0 w-full h-full object-cover"
        />
      </div>

    </div>
  );
};

export default Login;