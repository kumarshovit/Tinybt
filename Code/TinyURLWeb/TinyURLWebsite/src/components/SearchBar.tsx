import { useState } from "react";
import { searchByTag } from "../api/urlService";

interface Props {
  setLinks: (links: any[]) => void;
}

export default function SearchBar({ setLinks }: Props) {
  const [tag, setTag] = useState("");

  const handleSearch = async () => {
    const data = await searchByTag(tag);
    setLinks(data);
  };

  return (
    <div className="max-w-3xl mx-auto mt-8 flex gap-4">
      <input
        placeholder="Search by tag"
        value={tag}
        onChange={(e) => setTag(e.target.value)}
        className="flex-1 border rounded-lg px-4 py-2"
      />
      <button
        onClick={handleSearch}
        className="bg-blue-600 text-white px-6 py-2 rounded-lg"
      >
        Search
      </button>
    </div>
  );
}
