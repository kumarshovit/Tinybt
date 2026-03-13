import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { addTags, removeTag, renameTag, getAllUrls } from "../api/urlService";

export default function TagManagement() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [tags, setTags] = useState<string[]>([]);
  const [allUserTags, setAllUserTags] = useState<string[]>([]);
  const [tagInput, setTagInput] = useState("");

  const [editingTag, setEditingTag] = useState<string | null>(null);
  const [newTag, setNewTag] = useState("");

  const linkId = Number(id);

  const loadTags = async () => {
    const result = await getAllUrls();

    if (!result.success) {
      alert(result.message);
      return;
    }

    const urls = result.data;

    const currentLink = urls.find((l: any) => l.id === linkId);

    if (currentLink) {
      setTags(currentLink.tags || []);
    }

    const tagSet = new Set<string>();

    urls.forEach((url: any) => {
      (url.tags || []).forEach((t: string) => {
        tagSet.add(t.toLowerCase());
      });
    });

    setAllUserTags(Array.from(tagSet));
  };

  useEffect(() => {
    loadTags();
  }, []);

  const handleAddTags = async () => {
    if (!tagInput.trim()) return;

    const tagList = tagInput
      .split(",")
      .map((t) => t.trim())
      .filter((t) => t);

    const result = await addTags(linkId, tagList);

    if (!result.success) {
      alert(result.message);
      return;
    }

    setTagInput("");
    loadTags();
  };

  const handleRemoveTag = async (tag: string) => {
    const result = await removeTag(linkId, tag);

    if (!result.success) {
      alert(result.message);
      return;
    }

    loadTags();
  };

  const handleRename = async (oldTag: string) => {
    if (!newTag.trim()) return;

    const renamed = newTag.trim().toLowerCase();

    const result = await renameTag(linkId, oldTag, renamed);

    if (!result.success) {
      alert(result.message);
      return;
    }

    setEditingTag(null);
    setNewTag("");

    loadTags();
  };

  const addSuggestedTag = (tag: string) => {
    if (!tagInput.includes(tag)) {
      if (tagInput === "") {
        setTagInput(tag);
      } else {
        setTagInput(tagInput + ", " + tag);
      }
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 px-4 sm:px-6 py-6 sm:py-10">
      <div className="max-w-3xl mx-auto bg-white shadow-md rounded-xl p-5 sm:p-8">
        {/* HEADER */}

        <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4 mb-8">
          <div className="flex items-center gap-3">
            <div className="bg-blue-100 text-blue-600 p-2 rounded-lg">🏷️</div>

            <h2 className="text-2xl font-semibold text-gray-800">
              Manage Tags
            </h2>
          </div>

          <button
            onClick={() => navigate("/dashboard")}
            className="flex items-center gap-2 text-sm font-medium text-blue-600 border border-blue-200 px-3 py-1.5 rounded-lg hover:bg-blue-50 transition w-fit"
          >
            ← Back to Dashboard
          </button>
        </div>

        {/* ADD TAG INPUT */}

        <div className="flex flex-col sm:flex-row gap-3 mb-6">
          <input
            value={tagInput}
            onChange={(e) => setTagInput(e.target.value)}
            placeholder="Add tags (comma separated)"
            className="flex-1 border rounded-lg px-4 py-2 focus:ring-2 focus:ring-blue-500 outline-none"
          />

          <button
            onClick={handleAddTags}
            className="bg-blue-600 text-white px-5 py-2 rounded-lg hover:bg-blue-700 transition w-full sm:w-auto"
          >
            Add
          </button>
        </div>

        {/* SUGGESTED TAG BOX */}

        {allUserTags.length > 0 && (
          <div className="mb-8">
            <h3 className="text-sm font-semibold text-gray-600 mb-3">
              Suggested Tags
            </h3>

            <div className="bg-gray-50 border rounded-lg p-4 flex flex-wrap gap-3">
              {allUserTags.map((tag, index) => (
                <button
                  key={index}
                  onClick={() => addSuggestedTag(tag)}
                  className="flex items-center gap-1 text-sm bg-white border px-3 py-1 rounded-full hover:bg-blue-50 hover:border-blue-300 transition"
                >
                  <span className="text-green-500 font-bold">+</span>
                  {tag}
                </button>
              ))}
            </div>
          </div>
        )}

        {/* CURRENT TAG LIST */}

        {tags.length === 0 ? (
          <p className="text-gray-400 text-sm text-center">
            No tags added yet.
          </p>
        ) : (
          <div className="flex flex-col gap-3">
            {tags.map((tag, index) => (
              <div
                key={`${tag}-${index}`}
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
