import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { logoutUser } from "../services/authService";
import { shortenUrl } from "../services/urlService";
import Navbar from "../components/Navbar";

const Dashboard = () => {
  const navigate = useNavigate();

  const [longUrl, setLongUrl] = useState("");
  const [shortUrl, setShortUrl] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [copied, setCopied] = useState(false);

  const handleGenerate = async () => {
    setError("");
    setShortUrl("");
    setCopied(false);

    if (!longUrl.trim()) {
      setError("Please enter a URL");
      return;
    }

    setLoading(true);

    try {
      const data = await shortenUrl(longUrl.trim());
      setShortUrl(data.shortUrl);
    } catch (err: any) {
      setError(err.response?.data || "Failed to generate URL");
    } finally {
      setLoading(false);
    }
  };

  const handleCopy = async () => {
    await navigator.clipboard.writeText(shortUrl);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <>
     <Navbar/>
    
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-100 px-4">
      <h2 className="text-3xl font-bold mb-6">Welcome to Dashboard 🎉</h2>

      <div className="bg-white p-6 rounded-xl shadow-md w-full max-w-md">
        <input
          type="text"
          placeholder="Enter your long URL"
          className="w-full px-4 py-2 border rounded-lg mb-3 focus:outline-none focus:ring-2 focus:ring-indigo-500"
          value={longUrl}
          onChange={(e) => setLongUrl(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleGenerate()}
        />

        <button
          onClick={handleGenerate}
          disabled={loading}
          className="w-full bg-indigo-600 text-white py-2 rounded-lg hover:bg-indigo-700 transition disabled:opacity-60"
        >
          {loading ? "Generating..." : "Generate Short URL"}
        </button>

        {error && (
          <p className="text-red-500 text-sm mt-3 text-center">{error}</p>
        )}

        {shortUrl && (
          <div className="mt-4 p-3 bg-green-100 rounded-lg">
            <p className="text-sm mb-1">Short URL:</p>

            <div className="flex items-center gap-2">
              <a
                href={shortUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="text-indigo-600 font-semibold break-all flex-1"
              >
                {shortUrl}
              </a>

              <button
                onClick={handleCopy}
                className="bg-indigo-600 text-white px-3 py-1 rounded text-sm hover:bg-indigo-700"
              >
                Copy
              </button>
            </div>

            {copied && (
              <p className="text-green-600 text-sm mt-2">
                Copied to clipboard!
              </p>
            )}
          </div>
        )}
      </div>
    </div>
    </>
  );
};

export default Dashboard;
