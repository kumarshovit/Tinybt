import { useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import useClickOutside from "../hooks/useClickOutside";
import {
    updateAlias,
    updateDestination,
    deleteUrl,
} from "../api/urlService";

interface Props {
    link: any;
    setLinks: React.Dispatch<React.SetStateAction<any[]>>;
    activeEdit: {
        id: string;
        type: "alias" | "destination";
    } | null;
    setActiveEdit: React.Dispatch<
        React.SetStateAction<{
            id: string;
            type: "alias" | "destination";
        } | null>
    >;
}

export default function LinkRow({
    link,
    setLinks,
    activeEdit,
    setActiveEdit,
}: Props) {
    const navigate = useNavigate();

    const [hovered, setHovered] = useState(false);
    const [newAlias, setNewAlias] = useState("");
    const [newDestination, setNewDestination] = useState("");
    const [error, setError] = useState("");
    const [copied, setCopied] = useState(false);

    const cardRef = useRef<HTMLDivElement>(null);

    const isEditingAlias =
        activeEdit?.id === link.id && activeEdit?.type === "alias";

    const isEditingDestination =
        activeEdit?.id === link.id && activeEdit?.type === "destination";

    // useClickOutside(cardRef, () => {
    //     setActiveEdit(null);
    // });

    useClickOutside(
        cardRef,
        () => setActiveEdit(null),
        isEditingAlias || isEditingDestination
    );
   const handleCopy = () => {
    const urlToCopy = link.shortUrl || `${window.location.origin}/${link.shortCode}`;
    navigator.clipboard.writeText(urlToCopy);
    setCopied(true);

    setTimeout(() => setCopied(false), 2000);
};

    const handleAliasUpdate = async () => {
        if (!newAlias.trim()) return;

        setError("");

        const result = await updateAlias(link.id, newAlias);

        if (!result.success) {
            setError("Alias already exists. Try another."); // ✅ simple message
            return;
        }

        setLinks((prev) =>
            prev.map((l) =>
                l.id === link.id
                    ? {
                        ...l,
                        shortUrl: `${window.location.origin}/${result.data.shortCode}`,
                    }
                    : l
            )
        );

        setActiveEdit(null);
    };

    const handleDestinationUpdate = async () => {
        if (!newDestination.trim()) return;

        const result = await updateDestination(link.id, newDestination);
        if (!result.success) {
            alert(result.message);
            return;
        }

        setLinks((prev: any[]) =>
            prev.map((l) =>
                l.id === link.id ? { ...l, longUrl: newDestination } : l
            )
        );

        setActiveEdit(null);
    };

    const handleDelete = async () => {
        const confirmDelete = window.confirm(
            "Are you sure you want to delete this link?"
        );
        if (!confirmDelete) return;

        const result = await deleteUrl(link.id);
        if (!result.success) {
            alert(result.message);
            return;
        }

        setLinks((prev: any[]) =>
            prev.filter((l) => l.id !== link.id)
        );
    };

    return (
        <div
            ref={cardRef}
            className="bg-white shadow rounded-lg p-5 m-4 relative"
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
        >
            <div className="flex flex-col md:flex-row md:justify-between md:items-center gap-3">                <div>
                <a
                    href={link.shortUrl}
                    target="_blank"
                    className="text-blue-600 font-medium"
                >
                    {link.shortUrl}
                </a>

                <p className="text-sm text-gray-500 mt-1 truncate max-w-[450px]">
                    {link.longUrl.length > 60
                        ? link.longUrl.slice(0, 60) + "..."
                        : link.longUrl}
                </p>
                <p className="text-sm text-gray-400">
                    {link.clickCount} clicks
                </p>

                {link.expirationDate && (
                    <p className="text-sm text-red-500">
                        Expires on: {new Date(link.expirationDate).toLocaleString()}
                    </p>
                )}
            </div>

                <div className="flex gap-3">
                    <button
                        onClick={handleCopy}
                        className="bg-gray-200 px-3 py-1 rounded text-sm"
                    >
                        {copied ? "Copied!" : "Copy"}
                    </button>

                    <button
                        onClick={() => navigate(`/tags/${link.id}`)}
                        className="bg-blue-100 text-blue-600 px-3 py-1 rounded text-sm"
                    >
                        Manage Tags
                    </button>

                    <button
                        onClick={handleDelete}
                        className="text-red-600"
                    >
                        Delete
                    </button>
                </div>

            </div>

            {/* EDIT BUTTONS */}
            {!isEditingAlias && !isEditingDestination && (
                <div
                    className={`flex gap-4 mt-4 text-sm text-gray-500 transition-all duration-300 ease-in-out ${hovered
                        ? "opacity-100 translate-y-0"
                        : "opacity-0 -translate-y-2 pointer-events-none"
                        }`}
                >

                    <button
                        onClick={() => {
                            setNewAlias(link.shortUrl.split("/").pop() || "");
                            setActiveEdit({ id: link.id, type: "alias" });
                        }}
                    >
                        Edit Alias
                    </button>

                    <button
                        onClick={() => {
                            setNewDestination(link.longUrl);
                            setActiveEdit({ id: link.id, type: "destination" });
                        }}
                    >
                        Edit Destination
                    </button>

                </div>
            )}

            {/* ALIAS EDIT */}
            {isEditingAlias && (
                <div className="flex gap-2 mt-4">


                    <input
                        autoFocus
                        value={newAlias}
                        onChange={(e) => setNewAlias(e.target.value)}
                        className="border px-3 py-1 rounded flex-1"
                    />
                    {error && (
                        <p style={{ color: "red", fontSize: "13px", marginTop: "5px" }}>
                            {error}
                        </p>
                    )}

                    <button
                        onClick={handleAliasUpdate}
                        className="bg-blue-600 text-white px-3 py-1 rounded"
                    >
                        Save
                    </button>

                    <button
                        onClick={() => setActiveEdit(null)}
                        className="bg-gray-300 px-3 py-1 rounded"
                    >
                        Cancel
                    </button>

                </div>
            )}

            {/* DESTINATION EDIT */}
            {isEditingDestination && (
                <div className="flex gap-2 mt-4">

                    <input
                        value={newDestination}
                        onChange={(e) =>
                            setNewDestination(e.target.value)
                        }
                        className="border px-3 py-1 rounded flex-1"
                    />

                    <button
                        onClick={handleDestinationUpdate}
                        className="bg-blue-600 text-white px-3 py-1 rounded"
                    >
                        Save
                    </button>

                    <button
                        onClick={() => setActiveEdit(null)}
                        className="bg-gray-300 px-3 py-1 rounded"
                    >
                        Cancel
                    </button>

                </div>
            )}

        </div>
    );
}