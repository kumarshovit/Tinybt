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

    const cardRef = useRef<HTMLDivElement>(null);

    const isEditingAlias =
        activeEdit?.id === link.id && activeEdit?.type === "alias";

    const isEditingDestination =
        activeEdit?.id === link.id && activeEdit?.type === "destination";

    useClickOutside(cardRef, () => {
        setActiveEdit(null);
    });

    const handleAliasUpdate = async () => {
        if (!newAlias.trim()) return;

        const result = await updateAlias(link.id, newAlias);
        if (!result.success) {
            alert(result.message);
            return;
        }

        setLinks((prev: any[]) =>
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
            <div className="flex justify-between items-center">
                <div>
                    <a
                        href={link.shortUrl}
                        target="_blank"
                        className="text-blue-600 font-medium"
                    >
                        {link.shortUrl}
                    </a>

                    <p className="text-sm text-gray-500 mt-1">
                        {link.longUrl}
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

                    <button
                        onClick={handleDelete}
                        className="text-red-600"
                    >
                        Delete
                    </button>
                </div>
            </div>

            {/* {hovered && !isEditingAlias && !isEditingDestination && (
                <div className="flex gap-4 mt-4 text-sm text-gray-500">
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
            )} */}

            <div
                className={`
        flex gap-4 mt-4 text-sm text-gray-500
        transition-all duration-300 ease-in-out
        ${hovered && !isEditingAlias && !isEditingDestination
                        ? "opacity-100 translate-y-0"
                        : "opacity-0 -translate-y-2 pointer-events-none"}
    `}
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


            {isEditingAlias && (
                <div className="flex gap-2 mt-4">
                    <input
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
                </div>
            )}
        </div>
    );
}