import { useState } from "react";
import { createUrl } from "../api/urlService";

export default function ShortenCard({ onUrlCreated }: any) {

  const [longUrl, setLongUrl] = useState("");
  const [alias, setAlias] = useState("");
  const [result, setResult] = useState<any>(null);
  const [error, setError] = useState("");
  const [expirationDate, setExpirationDate] = useState("");

  const handleCreate = async () => {
    setError("");
    setResult(null);

    try {
      const response = await createUrl(
        longUrl,
        alias || undefined,
        expirationDate
          ? new Date(expirationDate).toISOString()
          : undefined
      );

      // ✅ handle backend response format
      if (!response.success) {
        setError(response.message || "Something went wrong");
        return;
      }

      const res = response.data;

      const newLink = {
        id: res.id,
        shortUrl: res.shortUrl,
        clickCount: 0,
        tags: []
      };

      await onUrlCreated();

      setResult(newLink);
      setLongUrl("");
      setAlias("");
      setExpirationDate("");

    } catch (err: any) {
      setError("Something went wrong");
    }
  };

  return (
    <div className="bg-white shadow-xl rounded-2xl p-6 w-full">

      <input
        placeholder="Enter long URL"
        value={longUrl}
        onChange={(e) => setLongUrl(e.target.value)}
        className="w-full border rounded-lg px-4 py-2 mb-4"
      />

      <input
        placeholder="Custom alias (optional)"
        value={alias}
        onChange={(e) => setAlias(e.target.value)}
        className="w-full border rounded-lg px-4 py-2 mb-4"
      />

      <input
        type="datetime-local"
        value={expirationDate}
        onChange={(e) => setExpirationDate(e.target.value)}
        min={new Date().toISOString().slice(0, 16)}
        className="w-full border rounded-lg px-4 py-2 mb-4"
      />

      <button
        onClick={handleCreate}
        className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg"
      >
        Generate Short Link
      </button>

      {/* ✅ Error Message */}
      {error && (
        <p className="text-red-500 mt-3 text-sm">
          {error}
        </p>
      )}

      {/* ✅ Success Result */}
      {result && (
        <div className="mt-6 bg-green-50 border border-green-200 p-4 rounded-lg">
          <p className="font-semibold text-green-700 mb-2">
            Short URL Generated:
          </p>

          <a
            href={result.shortUrl}
            target="_blank"
            rel="noreferrer"
            className="text-blue-600 hover:underline"
          >
            {result.shortUrl}
          </a>

          <div className="flex gap-2 mt-4">
            <button
              onClick={() =>
                navigator.clipboard.writeText(result.shortUrl)
              }
              className="bg-gray-200 px-4 py-1 rounded text-sm"
            >
              Copy
            </button>

            <button
              onClick={() => setResult(null)}
              className="bg-gray-100 px-4 py-1 rounded text-sm"
            >
              Shorten Another
            </button>
          </div>
        </div>
      )}

    </div>
  );
}