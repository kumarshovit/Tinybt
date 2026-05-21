import { useState, useEffect, useRef } from "react";
import { useNavigate, Link } from "react-router-dom";
import { Eye, EyeOff } from "lucide-react";
import { loginUser } from "../services/authService";
import GoogleLogin from "../components/auth/GoogleLogin";

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

  const navigate = useNavigate();

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

      navigate("/dashboard");
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

          setError(
            `Too many attempts. Try again in ${minutes}m ${seconds}s`
          );
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
    }
    finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center px-4">
      <div className="bg-white w-full max-w-md p-8 rounded-2xl shadow-2xl">

        {/* Heading */}
        <h2 className="text-3xl font-bold text-center text-gray-800">
          Welcome Back 👋
        </h2>

        <p className="text-center text-gray-500 mt-2 mb-6">
          Login to manage your short links
        </p>

        {/* Form */}
        <form className="space-y-4">

          {/* Email */}
          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Email
            </label>

            <input
              type="email"
              placeholder="Enter your email"
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
                placeholder="Enter your password"
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
                {showPassword ? (
                  <EyeOff size={20} />
                ) : (
                  <Eye size={20} />
                )}
              </button>
            </div>
          </div>

          {/* Forgot Password */}
          <div className="text-right">
            <Link
              to="/forgot-password"
              className="text-sm text-blue-600 hover:underline"
            >
              Forgot Password?
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
            type="button"
            onClick={handleLogin}
            disabled={loading || isBlocked}
            className="w-full bg-blue-600 text-white py-2 rounded-lg font-semibold hover:bg-blue-700 transition disabled:opacity-60"
          >
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>

        {/* Register */}
        <p className="text-center text-sm text-gray-500 mt-6">
          Don't have an account?{" "}
          <Link
            to="/register"
            className="text-blue-600 font-semibold hover:underline"
          >
            Register
          </Link>
        </p>

        {/* Divider */}
        <div className="flex items-center my-4">
          <hr className="flex-grow border-gray-300" />
          <span className="mx-2 text-gray-400 text-sm">OR</span>
          <hr className="flex-grow border-gray-300" />
        </div>

        {/* Google Login */}
        <div className="flex justify-center">
          <GoogleLogin />
        </div>

      </div>
    </div>
  );
};

export default Login;