import Navbar from "../components/Navbar";
import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div className="min-h-screen bg-gradient-to-r from-blue-600 to-purple-700 text-white">
      <Navbar />

      <div className="flex flex-col items-center justify-center text-center px-6 py-20">
        <h1 className="text-5xl font-bold mb-6">
          Shorten Links. Track Clicks. Grow Faster.
        </h1>

        <p className="text-lg max-w-2xl mb-8">
          TinyBT is a powerful and secure URL shortening platform with analytics,
          QR codes, and custom aliases.
        </p>

        <div className="bg-white rounded-xl p-6 flex gap-4 w-full max-w-2xl shadow-lg">
          <input
            type="text"
            placeholder="Paste your long URL here..."
            className="flex-1 p-3 rounded-lg border outline-none text-gray-800"
          />
          <button className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-semibold">
            Shorten
          </button>
        </div>

        <div className="mt-8 flex gap-4">
          <Link
            to="/register"
            className="bg-green-500 hover:bg-green-600 px-6 py-3 rounded-lg font-semibold"
          >
            Get Started
          </Link>

          <Link
            to="/login"
            className="bg-gray-900 hover:bg-black px-6 py-3 rounded-lg font-semibold"
          >
            Login
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Home;
