import { useState } from "react";
import { addTags, updateTags, removeTag } from "../api/urlService";

interface Props {
  link: any;
  links: any[];
  setLinks: React.Dispatch<React.SetStateAction<any[]>>;
}

export default function LinkCard({ link, links, setLinks }: Props) {
  const [tagInput, setTagInput] = useState("");
  const [editing, setEditing] = useState(false);

const handleAddTags = async () => {
  const tags = tagInput.split(",").map(t => t.trim());

  await addTags(link.id, tags);

  const updatedLinks = links.map(l =>
    l.id === link.id
      ? { ...l, tags: [...l.tags, ...tags] }
      : l
  );

  setLinks(updatedLinks);
  setTagInput("");
};

const handleRemoveTag = async (tag: string) => {
  try {
    await removeTag(link.id, tag);

    // Update state without reloading whole list
    const updatedLinks = links.map((l: any) =>
      l.id === link.id
        ? { ...l, tags: l.tags.filter((t: string) => t !== tag) }
        : l
    );

    setLinks(updatedLinks);

  } catch (error) {
    console.error("Error removing tag:", error);
  }
};


const handleEditTags = async () => {
  const tags = tagInput.split(",").map(t => t.trim());

  await updateTags(link.id, tags);

  const updatedLinks = links.map(l =>
    l.id === link.id
      ? { ...l, tags }
      : l
  );

  setLinks(updatedLinks);
  setEditing(false);
  setTagInput("");
};


  return (
    <div className="bg-white shadow-md rounded-xl p-6 flex flex-col gap-3">

      {/* Short URL */}
      <a
        href={link.shortUrl}
        target="_blank"
        className="text-blue-600 font-semibold text-lg hover:underline"
      >
        {link.shortUrl}
      </a>

      <p className="text-sm text-gray-500">
        {link.clickCount} clicks
      </p>

      {/* Tags Section */}
     <div className="flex flex-wrap gap-2">
  {link.tags?.length > 0 ? (
    link.tags.map((tag: string, index: number) => (
      <span
        key={index}
        className="bg-blue-100 text-blue-600 px-3 py-1 rounded-full text-xs flex items-center gap-2"
      >
        {tag}
        <button
          onClick={() => handleRemoveTag(tag)}
          className="text-red-500 hover:text-red-700 font-bold"
        >
          ×
        </button>
      </span>
    ))
  ) : (
    <span className="text-gray-400 text-sm">No tags</span>
  )}
</div>


      {/* Add or Edit Tags */}
      {editing ? (
        <div className="flex gap-2 mt-2">
          <input
            placeholder="tag1, tag2"
            value={tagInput}
            onChange={(e) => setTagInput(e.target.value)}
            className="border rounded px-2 py-1 flex-1 text-sm"
          />
          <button
            onClick={handleEditTags}
            className="bg-blue-600 text-white px-3 py-1 rounded text-sm"
          >
            Save
          </button>
        </div>
      ) : (
        <div className="flex gap-2 mt-2">
          <input
            placeholder="Add tags"
            value={tagInput}
            onChange={(e) => setTagInput(e.target.value)}
            className="border rounded px-2 py-1 flex-1 text-sm"
          />
          <button
            onClick={handleAddTags}
            className="bg-blue-600 text-white px-3 py-1 rounded text-sm"
          >
            Add
          </button>
          <button
            onClick={() => setEditing(true)}
            className="text-gray-500 text-sm"
          >
            Edit
          </button>
        </div>
      )}
    </div>
  );
}
