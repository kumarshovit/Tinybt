import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  addTags,
  removeTag,
  renameTag,
  getAllUrls
} from "../api/urlService";

export default function TagManagement() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [tags, setTags] = useState<string[]>([]);
  const [tagInput, setTagInput] = useState("");
  const [editingTag, setEditingTag] = useState<string | null>(null);
  const [newTag, setNewTag] = useState("");

  const linkId = Number(id);

  // 🔹 Load tags from backend
  const loadTags = async () => {
    const allLinks = await getAllUrls();
    const currentLink = allLinks.find((l: any) => l.id === linkId);

    if (currentLink) {
      setTags(currentLink.tags || []);
    }
  };

  useEffect(() => {
    loadTags();
  }, []);

  // 🔹 Add Tags
  const handleAddTags = async () => {
    if (!tagInput.trim()) return;

    const tagList = tagInput
      .split(",")
      .map(t => t.trim())
      .filter(t => t);

    await addTags(linkId, tagList);
    setTagInput("");
    loadTags();
  };

  // 🔹 Remove Tag
  const handleRemoveTag = async (tag: string) => {
    await removeTag(linkId, tag);
    loadTags();
  };

  // 🔹 Rename Tag
const handleRename = async (oldTag: string) => {
  if (!newTag.trim()) return;

  try {
    const renamed = newTag.trim().toLowerCase();

    await renameTag(linkId, oldTag, renamed);

    setTags(prev =>
      prev.map(tag =>
        tag === oldTag ? renamed : tag
      )
    );

    setEditingTag(null);
    setNewTag("");

  } catch (err: any) {
    alert(err.message);
  }
};

  return (
    <div className="min-h-screen bg-gray-50 px-6 py-10">

      <div className="max-w-3xl mx-auto bg-white shadow-md rounded-xl p-8">

        {/* HEADER */}
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-semibold text-gray-800">
            Manage Tags
          </h2>

          <button
            onClick={() => navigate("/")}
            className="text-sm text-blue-600 hover:underline"
          >
            ← Back to Dashboard
          </button>
        </div>

        {/* ADD TAG SECTION */}
        <div className="flex gap-3 mb-8">
          <input
            value={tagInput}
            onChange={(e) => setTagInput(e.target.value)}
            placeholder="Add tags (comma separated)"
            className="flex-1 border rounded-lg px-4 py-2 focus:ring-2 focus:ring-blue-500 outline-none"
          />
          <button
            onClick={handleAddTags}
            className="bg-blue-600 text-white px-5 py-2 rounded-lg hover:bg-blue-700 transition"
          >
            Add
          </button>
        </div>

        {/* TAG LIST */}
        {tags.length === 0 ? (
          <p className="text-gray-400 text-sm text-center">
            No tags added yet.
          </p>
        ) : (
          <div className="flex flex-col gap-3">

            {tags.map((tag) => (
              <div
                key={tag}
                className="flex items-center justify-between bg-gray-50 border rounded-lg px-4 py-3 hover:shadow-sm transition"
              >
                {editingTag === tag ? (
                  <div className="flex gap-3 w-full">
                    <input
                      value={newTag}
                      onChange={(e) => setNewTag(e.target.value)}
                      className="flex-1 border rounded px-3 py-1 text-sm focus:ring-2 focus:ring-blue-500 outline-none"
                    />
                    <button
                      onClick={() => handleRename(tag)}
                      className="bg-blue-600 text-white px-3 py-1 rounded text-sm"
                    >
                      Save
                    </button>
                  </div>
                ) : (
                  <>
                    <span className="text-gray-700 font-medium text-sm">
                      {tag}
                    </span>

                    <div className="flex gap-4 text-sm">
                      <button
                        onClick={() => {
                          setEditingTag(tag);
                          setNewTag(tag);
                        }}
                        className="text-blue-600 hover:underline"
                      >
                        Edit
                      </button>

                      <button
                        onClick={() => handleRemoveTag(tag)}
                        className="text-red-500 hover:underline"
                      >
                        Delete
                      </button>
                    </div>
                  </>
                )}
              </div>
            ))}

          </div>
        )}

      </div>
    </div>
  );
}
