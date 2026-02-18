import { Link } from "react-router-dom";

const VerifyEmail = () => {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-r from-blue-600 to-purple-700">
      <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-md text-center">
        <h2 className="text-2xl font-bold text-gray-800 mb-4">
          Verify Your Email
        </h2>

        <p className="text-gray-600 mb-6">
          A verification link has been sent to your email. Please verify your
          email before logging in.
        </p>

        <Link
          to="/login"
          className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-semibold inline-block"
        >
          Go to Login
        </Link>
      </div>
    </div>
  );
};

export default VerifyEmail;
