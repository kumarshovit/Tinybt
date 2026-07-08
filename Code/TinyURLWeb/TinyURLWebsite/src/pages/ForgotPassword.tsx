// import { useState } from "react";
// import type { FormEvent } from "react";
// import api from "../utils/api";
// import { Link } from "react-router-dom";

// const ForgotPassword = () => {
//   const [email, setEmail] = useState<string>("");
//   const [message, setMessage] = useState<string>("");
//   const [loading, setLoading] = useState<boolean>(false);

//   const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
//   e.preventDefault();
//   setLoading(true);
//   setMessage("");

//   try {
//     const response = await api.post("/auth/forgot-password", {
//       email,
//     });

//     setMessage(response.data.message);
//   } catch {
//     setMessage("Something went wrong. Please try again.");
//   } finally {
//     setLoading(false);
//   }
// };


//   return (
//     <div className="min-h-screen flex items-center justify-center px-4 bg-gray-50">
//       <div className="bg-white w-full max-w-md p-8 rounded-2xl shadow-2xl">
//         <h2 className="text-3xl font-bold text-center text-gray-800">
//           Forgot Password 🔐
//         </h2>

//         <p className="text-center text-gray-500 mt-2 mb-6">
//           Enter your email to receive a password reset link
//         </p>

//         <form onSubmit={handleSubmit} className="space-y-4">
//           <div>
//             <label className="block text-sm font-medium text-gray-600 mb-1">
//               Email Address
//             </label>

//             <input
//               type="email"
//               placeholder="Enter your email"
//               className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none transition"
//               value={email}
//               onChange={(e) => setEmail(e.target.value)}
//               required
//             />
//           </div>

//           <button
//             type="submit"
//             disabled={loading}
//             className="w-full bg-blue-600 text-white py-2 rounded-lg font-semibold hover:bg-blue-700 transition disabled:opacity-60"
//           >
//             {loading ? "Sending..." : "Send Reset Link"}
//           </button>
//         </form>

//         {message && (
//           <p className="text-center text-sm mt-4 text-green-600">
//             {message}
//           </p>
//         )}

//         <p className="text-center text-sm text-gray-500 mt-6">
//           Remember your password?{" "}
//           <Link
//             to="/login"
//             className="text-indigo-600 font-semibold hover:underline"
//           >
//             Back to Login
//           </Link>
//         </p>
//       </div>
//     </div>
//   );
// };

// export default ForgotPassword;


import { useState } from "react";
import type { FormEvent } from "react";
import api from "../utils/api";
import { Link } from "react-router-dom";
import SEO from "../components/SEO";

const ForgotPassword = () => {
  const [email, setEmail] = useState<string>("");
  const [message, setMessage] = useState<string>("");
  const [error, setError] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setLoading(true);
    setMessage("");
    setError("");

    try {
      const response = await api.post("/auth/forgot-password", {
        email,
      });

      // ✅ Success
      setMessage(response.data.message);
    } catch (err: any) {
      console.log(err); // for debugging

      // ❌ Extract backend error message
      const backendMessage = err?.response?.data?.message;

      if (backendMessage) {
        setError(backendMessage);
      } else {
        setError("Something went wrong. Please try again.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center px-4 bg-gray-50">
      <SEO
        title="Forgot Password – LinkBT"
        description="Reset your LinkBT password. Enter your email and we'll send you a secure password reset link."
        canonical="/forgot-password"
        noindex
      />

      <div className="bg-white w-full max-w-md p-8 rounded-2xl shadow-2xl">
        <h2 className="text-3xl font-bold text-center text-gray-800">
          Forgot Password 🔐
        </h2>

        <p className="text-center text-gray-500 mt-2 mb-6">
          Enter your email to receive a password reset link
        </p>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-600 mb-1">
              Email Address
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

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-blue-600 text-white py-2 rounded-lg font-semibold hover:bg-blue-700 transition disabled:opacity-60"
          >
            {loading ? "Sending..." : "Send Reset Link"}
          </button>
        </form>

        {/* ✅ Success Message */}
        {message && (
          <p className="text-center text-sm mt-4 text-green-600">
            {message}
          </p>
        )}

        {/* ❌ Error Message */}
        {error && (
          <p className="text-center text-sm mt-4 text-red-600">
            {error}
          </p>
        )}

        <p className="text-center text-sm text-gray-500 mt-6">
          Remember your password?{" "}
          <Link
            to="/login"
            className="text-indigo-600 font-semibold hover:underline"
          >
            Back to Login
          </Link>
        </p>
      </div>
    </div>
  );
};

export default ForgotPassword;