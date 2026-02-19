import Navbar from "../components/Navbar";
import { Link } from "react-router-dom";
import { useState } from "react";
import api from "../api/axios";

interface ShortenResponse {
  shortUrl: string;
}

const Home = () => {
  const [longUrl, setLongUrl] = useState<string>("");
  const [shortUrl, setShortUrl] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [errorMsg, setErrorMsg] = useState<string>("");

  const handleShorten = async (): Promise<void> => {
    try {
      setLoading(true);
      setErrorMsg("");
      setShortUrl("");

      if (!longUrl.trim()) {
        setErrorMsg("Please enter a valid URL.");
        return;
      }

      const response = await api.post<ShortenResponse>(
        "/api/url/shorten",
        { longUrl }
      );

      setShortUrl(response.data.shortUrl);
    } catch (err) {
      console.error(err);
      setErrorMsg("Backend not connected OR API not working.");
    } finally {
      setLoading(false);
    }
  };

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

        {/* Shortener Box */}
        <div className="bg-white rounded-xl p-6 flex gap-4 w-full max-w-2xl shadow-lg flex-col">
          
          <div className="flex gap-4">
            <input
              type="text"
              placeholder="Paste your long URL here..."
              value={longUrl}
              onChange={(e) => setLongUrl(e.target.value)}
              className="flex-1 p-3 rounded-lg border outline-none text-gray-800"
            />

            <button
              onClick={handleShorten}
              disabled={loading}
              className={`px-6 py-3 rounded-lg font-semibold text-white ${
                loading
                  ? "bg-gray-400 cursor-not-allowed"
                  : "bg-blue-600 hover:bg-blue-700"
              }`}
            >
              {loading ? "Generating..." : "Shorten"}
            </button>
          </div>

          {/* Error */}
          {errorMsg && (
            <div className="text-red-500 text-sm mt-2">
              {errorMsg}
            </div>
          )}

          {/* Result */}
          {shortUrl && (
            <div className="mt-4 text-gray-800 text-sm">
              <span className="font-semibold">Short URL: </span>
              <a
                href={shortUrl}
                target="_blank"
                rel="noreferrer"
                className="text-blue-600 underline"
              >
                {shortUrl}
              </a>
            </div>
          )}
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
