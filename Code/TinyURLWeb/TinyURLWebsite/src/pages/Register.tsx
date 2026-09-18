import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { Eye, EyeOff, Loader2 } from "lucide-react";
import { registerUser } from "../services/authService";
import { validateEmail, validatePassword } from "../utils/validators";
import GoogleLogin from "../components/auth/GoogleLogin";
import SEO from "../components/SEO";
import logo from "../assets/logo.png";
import sideImage from "../assets/WhatsApp Image 2026-09-18 at 4.55.27 PM.jpeg";

const Register = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  // ✅ Show/Hide Password
  const [showPassword, setShowPassword] = useState(false);

  const navigate = useNavigate();

  const handleSubmit = async (e: any) => {
    e.preventDefault();

    setError("");
    setMessage("");

    if (!validateEmail(email)) {
      return setError("Invalid email format");
    }

    if (!validatePassword(password)) {
      return setError("Password must be 8+ chars, 1 uppercase & 1 number");
    }

    if (password !== confirmPassword) {
      return setError("Passwords do not match");
    }

    try {
      setLoading(true);

      const response = await registerUser(email, password);

      setMessage(response.message);

      setEmail("");
      setPassword("");

      setTimeout(() => {
        navigate("/login");
      }, 2000);
    } catch (err: any) {
      setError(err.response?.data?.message || "Registration failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex bg-gray-50">
      <SEO
        title="Create Account – LinkBT"
        description="Sign up for a free LinkBT account and start shortening URLs, creating custom branded links, and tracking click analytics in seconds."
        canonical="/register"
        noindex
      />

      {/* Left side: Register form */}
      <div className="w-full lg:w-1/2 flex flex-col justify-center items-center px-4 relative">
        {/* Logo at the top left */}
        <div className="absolute top-6 left-6 md:top-8 md:left-12">
          <Link to="/">
            <img src={logo} alt="LinkBT Logo" className="h-10 object-contain" />
          </Link>
        </div>

        <div className="bg-white w-full max-w-md p-8 sm:p-10 rounded-xl shadow-xl mt-16 lg:mt-0">
          {/* Heading */}
          <h2 className="text-[26px] font-bold text-center text-[#1E293B] mb-2 leading-tight">
            Create Your LinkBT Account
          </h2>

          <p className="text-center text-[13px] text-gray-700 mb-6 font-medium">
            Already have an account?{" "}
            <Link
              to="/login"
              className="text-[#02629b] font-semibold hover:underline"
            >
              Login
            </Link>
          </p>

          {/* Form */}
          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Email */}
            <div>
              <label className="block text-xs font-bold text-slate-800 mb-1">
                Email Address <span className="text-red-500">*</span>
              </label>

              <input
                type="email"
                placeholder="Email Address"
                className="w-full px-4 py-2 border border-slate-400 rounded focus:ring-1 focus:ring-slate-500 focus:outline-none transition text-sm"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>

            {/* Password */}
            <div>
              <label className="block text-xs font-bold text-slate-800 mb-1">
                Create Password <span className="text-red-500">*</span>
              </label>

              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  placeholder="Create Password"
                  className="w-full px-4 py-2 pr-12 border border-slate-400 rounded focus:ring-1 focus:ring-slate-500 focus:outline-none transition text-sm"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />

                {/* Eye Toggle */}
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-600 hover:text-slate-800"
                >
                  {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              </div>
            </div>

            {/* Confirm Password */}
            <div>
              <label className="block text-xs font-bold text-slate-800 mb-1">
                Confirm Password <span className="text-red-500">*</span>
              </label>

              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  placeholder="Confirm Password"
                  className="w-full px-4 py-2 pr-12 border border-slate-400 rounded focus:ring-1 focus:ring-slate-500 focus:outline-none transition text-sm"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                />

                {/* Eye Toggle */}
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-600 hover:text-slate-800"
                >
                  {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              </div>
            </div>

            {/* Error */}
            {error && <p className="text-red-500 text-sm text-center font-medium">{error}</p>}

            {/* Success */}
            {message && (
              <p className="text-green-500 text-sm text-center font-medium">{message}</p>
            )}

            {/* Button */}
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-[#465466] text-white py-[10px] rounded font-semibold hover:bg-[#334155] transition disabled:opacity-60 flex items-center justify-center gap-2 text-sm mt-2"
            >
              {loading ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin" />
                  <span>Registering...</span>
                </>
              ) : (
                "Register Now"
              )}
            </button>
          </form>

          {/* Divider */}
          <div className="flex items-center my-6">
            <hr className="flex-grow border-slate-200" />
            <span className="mx-3 text-slate-600 text-xs font-medium">continue with:</span>
            <hr className="flex-grow border-slate-200" />
          </div>

          {/* Google Login */}
          <div className="flex justify-center space-x-4 mb-2">
            <GoogleLogin />
          </div>
        </div>
      </div>

      {/* Right side: Image */}
      <div className="hidden lg:block lg:w-1/2 relative bg-[#111827]">
        <img
          src={sideImage}
          alt="Join LinkBT"
          className="absolute inset-0 w-full h-full object-cover"
        />
      </div>

    </div>
  );
};

export default Register;
