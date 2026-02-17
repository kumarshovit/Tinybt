import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
    updateAlias,
    updateDestination
} from "../api/urlService";

export default function LinkRow({ link, setLinks }: any) {

    const navigate = useNavigate();

    const [hovered, setHovered] = useState(false);
    const [editingAlias, setEditingAlias] = useState(false);
    const [editingDestination, setEditingDestination] = useState(false);
    const [newAlias, setNewAlias] = useState("");
    const [newDestination, setNewDestination] = useState("");

    const handleAliasUpdate = async () => {
    try {
        const response = await updateAlias(link.id, newAlias);

        setLinks((prev: any[]) =>
            prev.map(l =>
                l.id === link.id
                    ? { ...l, shortUrl: response.shortUrl }
                    : l
            )
        );

        setEditingAlias(false);
        setNewAlias("");

    } catch (err: any) {
        alert(err.message);
    }
};


    const handleDestinationUpdate = async () => {
        try {
    await updateDestination(link.id, newDestination);

    setLinks((prev: any[]) =>
      prev.map(l =>
        l.id === link.id
          ? { ...l, longUrl: newDestination }
          : l
      )
    );

    setEditingDestination(false);
    setNewDestination("");

        } catch (err: any) {
            alert(err.message);
        }
    };

    return (
        <div
            className="bg-white shadow rounded-lg p-5 transition hover:shadow-lg relative"
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
        >

            <div className="flex justify-between items-center">

                <div>
                    <a
                        href={link.shortUrl}
                        target="_blank"
                        className="text-blue-600 font-medium"
                    >
                        {link.shortUrl}
                    </a>
                    <p className="text-sm text-gray-500 mt-1 truncate max-w-md">
                        {link.longUrl}
                    </p>
                    <p className="text-sm text-gray-400">
                        {link.clickCount} clicks
                    </p>
                </div>

                <div className="flex gap-3">
                    <button
                        onClick={() =>
                            navigator.clipboard.writeText(link.shortUrl)
                        }
                        className="bg-gray-200 px-3 py-1 rounded text-sm"
                    >
                        Copy
                    </button>

                    <button
                        onClick={() => navigate(`/tags/${link.id}`)}
                        className="bg-blue-100 text-blue-600 px-3 py-1 rounded text-sm"
                    >
                        Manage Tags
                    </button>
                </div>
            </div>

            {/* Hover Buttons */}
            {hovered && !editingAlias && !editingDestination && (
                <div className="flex gap-4 mt-4 text-sm text-gray-500">
                    <button
                        onClick={() => setEditingAlias(true)}
                        className="hover:text-blue-600"
                    >
                        Edit Alias
                    </button>

                    <button
                        onClick={() => setEditingDestination(true)}
                        className="hover:text-blue-600"
                    >
                        Edit Destination
                    </button>
                </div>
            )}

            {/* Edit Alias */}
            {editingAlias && (
                <div className="flex gap-2 mt-4">
                    <input
                        placeholder="New alias"
                        value={newAlias}
                        onChange={(e) => setNewAlias(e.target.value)}
                        className="border px-3 py-1 rounded flex-1"
                    />
                    <button
                        onClick={handleAliasUpdate}
                        className="bg-blue-600 text-white px-3 py-1 rounded"
                    >
                        Save
                    </button>
                </div>
            )}

            {/* Edit Destination */}
            {editingDestination && (
                <div className="flex gap-2 mt-4">
                    <input
                        placeholder="New destination URL"
                        value={newDestination}
                        onChange={(e) => setNewDestination(e.target.value)}
                        className="border px-3 py-1 rounded flex-1"
                    />
                    <button
                        onClick={handleDestinationUpdate}
                        className="bg-blue-600 text-white px-3 py-1 rounded"
                    >
                        Save
                    </button>
                </div>
            )}

        </div>
    );
}
