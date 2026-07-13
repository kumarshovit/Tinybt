import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import CreatableSelect from "react-select/creatable";

import {
  createUrl,
  addTags,
  getAllTags
} from "../api/urlService";

let memoryGuestUrlResult: any = null;

export default function ShortenCard({ onUrlCreated }: any) {
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  const isAuthenticated = typeof window !== "undefined" && mounted ? !!localStorage.getItem("token") : false;

  const [longUrl, setLongUrl] = useState("");
  const [alias, setAlias] = useState("");
  const [expirationDate, setExpirationDate] = useState("");

  const [tags, setTags] = useState<string[]>([]);
  const [tagOptions, setTagOptions] = useState<any[]>([]);

  const [result, setResult] = useState<any>(memoryGuestUrlResult);
  const [error, setError] = useState("");

  /* ============================= */
  /* LOAD EXISTING TAGS            */
  /* ============================= */

  useEffect(() => {
    const loadTags = async () => {

      const response = await getAllTags();

      if (response.success) {

        const options = (response.data as string[]).map((t) => ({
          value: t,
          label: t
        }));

        setTagOptions(options);
      }
    };

    if (isAuthenticated) {
      loadTags();
    }

  }, [isAuthenticated]);

  /* ============================= */
  /* CREATE URL                    */
  /* ============================= */

  const handleCreate = async () => {

    setError("");
    setResult(null);

    if (!longUrl.trim()) {
      setError("Please enter a URL.");
      return;
    }

    try {
      new URL(longUrl);
    } catch {
      setError("Please enter a valid URL (e.g. https://example.com).");
      return;
    }

    try {

      const response = await createUrl(
        longUrl,
        alias || undefined,
        expirationDate
          ? new Date(expirationDate).toISOString()
          : undefined
      );

      if (!response.success) {
        setError(response.message || "Something went wrong");
        return;
      }

      const res = response.data;

      /* add tags */

      if (tags.length > 0) {
        await addTags(res.id, tags);
      }

      const newLink = {
        id: res.id,
        shortUrl: res.shortUrl,
        clickCount: 0,
        tags
      };

      if (!isAuthenticated && typeof window !== "undefined") {
        memoryGuestUrlResult = newLink;
      }

      if (onUrlCreated) {
        await onUrlCreated();
      }

      setResult(newLink);

      setLongUrl("");
      setAlias("");
      setExpirationDate("");
      setTags([]);

    } catch {

      setError("Something went wrong");

    }
  };

  return (

    <div className="bg-white shadow-xl rounded-2xl p-8 w-full max-w-md">

      {/* URL */}

      <input
        aria-label="Long URL"
        placeholder="Enter long URL"
        value={longUrl}
        onChange={(e) => setLongUrl(e.target.value)}
        className="w-full border rounded-lg px-4 py-2 mb-4"
      />

      {/* Alias */}

      <input
        aria-label="Custom alias (optional)"
        placeholder="Custom alias (optional)"
        value={alias}
        onChange={(e) => setAlias(e.target.value)}
        className="w-full border rounded-lg px-4 py-2 mb-4"
      />


      {/* TAG DROPDOWN */}
      {isAuthenticated && (
        <CreatableSelect
          isMulti
          options={tagOptions}
          value={tagOptions.filter((opt) => tags.includes(opt.value))}
          placeholder="Search or add tags..."
          className="mb-4"

          onChange={(selected) => {
            const values = selected
              ? selected.map((t: any) => t.value)
              : [];
            setTags(values);
          }}

          onCreateOption={(inputValue) => {

            const newOption = {
              value: inputValue,
              label: inputValue
            };

            setTagOptions([...tagOptions, newOption]);
            setTags([...tags, inputValue]);

          }}
        />
      )}

      {/* Expiration */}

      <div className="flex items-start gap-2 bg-blue-50 border border-blue-200 rounded-md px-3 py-2 mt-2 mb-2">

        <span className="text-blue-500 text-sm mt-[2px]">ℹ️</span>

        <div className="text-xs text-blue-700 mb">
          <p>Select an expiration date if needed.</p>
          <p className="text-blue-600 font-medium">
            Default: 10 days
          </p>
        </div>

      </div>
      <input
        type="datetime-local"
        aria-label="Expiration date (optional)"
        value={expirationDate}
        placeholder="Select expiration date (optional)"
        onChange={(e) => setExpirationDate(e.target.value)}
        min={new Date().toISOString().slice(0, 16)}
        className="w-full border rounded-lg px-4 py-2 mb-4"
      />


      {/* BUTTON */}

      <button
        onClick={handleCreate}
        className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg"
      >
        Generate Short Link
      </button>
        
         <p className="text-sm text-gray-500 text-center mt-3">
              Sign in to save and manage your links
            </p>

      {error && (
        <p className="text-red-500 mt-3 text-sm">
          {error}
        </p>
      )}
      {result && (

        <div className="mt-6 bg-green-50 border border-green-200 p-4 rounded-lg">

          <p className="font-semibold text-green-700 mb-2">
            Short URL Generated:
          </p>

          <div className="flex items-center justify-between mt-2">
            <a
              href={result.shortUrl}
              target="_blank"
              rel="noreferrer"
              className="text-blue-600 hover:underline break-all"
            >
              {result.shortUrl}
            </a>
            
            {!isAuthenticated && (
              <Link 
                to="/register" 
                className="ml-4 bg-white border border-gray-300 text-gray-700 hover:bg-gray-50 px-3 py-1 rounded text-sm whitespace-nowrap"
              >
                View Analytics
              </Link>
            )}
          </div>
          
          
        </div>

      )}

    </div>
  );
}