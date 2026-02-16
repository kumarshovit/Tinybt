import { useState } from "react";
import { createUrl, addTags } from "../api/urlService";

interface Props {
  reload: () => void;
}

export default function CreateLinkCard({ reload }: Props) {
  const [longUrl, setLongUrl] = useState("");
  const [alias, setAlias] = useState("");
  const [createdLink, setCreatedLink] = useState<any>(null);
  const [tagInput, setTagInput] = useState("");
  const [error, setError] = useState("");

  const handleCreate = async () => {
    try {
      const response = await createUrl(longUrl, alias);

      setCreatedLink(response);   // save created link
      setLongUrl("");
      setAlias("");
      setError("");
      reload();
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleAddTags = async () => {
    if (!createdLink) return;

    const tags = tagInput.split(",").map(t => t.trim());

    await addTags(createdLink.id, tags);

    setTagInput("");
    reload();
  };

  return (
    <div className="bg-white shadow-lg rounded-xl p-6 max-w-3xl mx-auto">
      <div className="flex flex-col md:flex-row gap-4">
        <input
          className="flex-1 border rounded-lg px-4 py-2"
          placeholder="Enter long URL"
          value={longUrl}
          onChange={(e) => setLongUrl(e.target.value)}
        />

        <input
          className="w-48 border rounded-lg px-4 py-2"
          placeholder="Custom alias"
          value={alias}
          onChange={(e) => setAlias(e.target.value)}
        />

        <button
          onClick={handleCreate}
          className="bg-blue-600 text-white px-6 py-2 rounded-lg"
        >
          Shorten
        </button>
      </div>

      {error && <p className="text-red-500 mt-2">{error}</p>}

      {/* SUCCESS SECTION */}
      {createdLink && (
        <div className="mt-6 p-4 bg-green-50 border border-green-200 rounded-lg">
          <p className="font-semibold text-green-700">
            Short URL is:
          </p>

          <a
            href={createdLink.shortUrl}
            target="_blank"
            className="text-blue-600 hover:underline"
          >
            {createdLink.shortUrl}
          </a>

          {/* TAG INPUT */}
          <div className="flex gap-2 mt-4">
            <input
              placeholder="Add tags (comma separated)"
              value={tagInput}
              onChange={(e) => setTagInput(e.target.value)}
              className="border rounded px-3 py-1 flex-1"
            />
            <button
              onClick={handleAddTags}
              className="bg-blue-600 text-white px-4 py-1 rounded"
            >
              Add Tags
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
