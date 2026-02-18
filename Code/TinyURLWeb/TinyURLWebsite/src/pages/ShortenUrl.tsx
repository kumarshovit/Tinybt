import { useState } from "react";
import api from "../api/axios";

interface ShortenResponse {
  shortUrl: string;
}

const ShortenUrl = () => {
  const [longUrl, setLongUrl] = useState<string>("");
  const [shortUrl, setShortUrl] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [errorMsg, setErrorMsg] = useState<string>("");
  const [copied, setCopied] = useState<boolean>(false);

  const handleShorten = async (): Promise<void> => {
    try {
      setLoading(true);
      setErrorMsg("");
      setShortUrl("");
      setCopied(false);

      if (!longUrl.trim()) {
        setErrorMsg("Please enter a valid URL.");
        return;
      }

      const response = await api.post<ShortenResponse>("/api/url/shorten", {
        longUrl,
      });

      setShortUrl(response.data.shortUrl);
    } catch (err) {
      console.error(err);
      setErrorMsg("Backend not connected OR API not working.");
    } finally {
      setLoading(false);
    }
  };

  const handleCopy = () => {
    navigator.clipboard.writeText(shortUrl);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-100 to-purple-100 p-6">
      <div className="bg-white shadow-2xl rounded-2xl p-8 w-full max-w-xl transition-all">

        {/* Title */}
        <h1 className="text-3xl font-bold text-gray-800 mb-2">
          🔗 TinyBT URL Shortener
        </h1>
        <p className="text-gray-500 mb-6">
          Convert long links into clean, shareable short URLs instantly.
        </p>

        {/* Input */}
        <input
          type="text"
          placeholder="Paste your long URL here..."
          value={longUrl}
          onChange={(e) => setLongUrl(e.target.value)}
          className="w-full p-4 border border-gray-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-500 mb-4 text-sm"
        />

        {/* Button */}
        <button
          onClick={handleShorten}
          disabled={loading}
          className={`w-full py-3 rounded-xl font-semibold text-white transition-all ${
            loading
              ? "bg-gray-400 cursor-not-allowed"
              : "bg-indigo-600 hover:bg-indigo-700"
          }`}
        >
          {loading ? "Generating..." : "Shorten URL"}
        </button>

        {/* Error */}
        {errorMsg && (
          <div className="mt-4 text-red-500 text-sm font-medium">
            {errorMsg}
          </div>
        )}

        {/* Result */}
        {shortUrl && (
          <div className="mt-6 bg-gray-50 border rounded-xl p-4">
            <p className="text-sm text-gray-600 mb-2">
              ✅ Your Short URL
            </p>

            <div className="flex items-center justify-between bg-white border rounded-lg px-3 py-2">
              <a
                href={shortUrl}
                target="_blank"
                rel="noreferrer"
                className="text-indigo-600 font-semibold truncate"
              >
                {shortUrl}
              </a>

              <button
                onClick={handleCopy}
                className="ml-3 text-sm bg-indigo-100 text-indigo-700 px-3 py-1 rounded-lg hover:bg-indigo-200 transition"
              >
                {copied ? "Copied!" : "Copy"}
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ShortenUrl;
