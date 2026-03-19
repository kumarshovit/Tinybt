import React from "react";

export default function Filters({
    from,
    to,
    setFrom,
    setTo,
    setLast7Days,
    setLast30Days,
    selectedLink,
    setSelectedLink,
    selectedTag,
    setSelectedTag,
    allLinks,
    allTags,
    exportCSV,
    userDropdown,
}: any) {
    return (
        <div className="bg-white rounded-xl shadow p-4 sm:p-6 mb-10 flex flex-col sm:flex-row flex-wrap gap-3 sm:gap-4 items-start sm:items-center">

            {/* ---------- Date Filters ---------- */}
            <button
                onClick={setLast7Days}
                className="bg-blue-100 text-blue-700 px-4 py-2 rounded-lg hover:bg-blue-200 w-full sm:w-auto"
            >
                Last 7 Days
            </button>

            <button
                onClick={setLast30Days}
                className="bg-blue-100 text-blue-700 px-4 py-2 rounded-lg hover:bg-blue-200 w-full sm:w-auto"
            >
                Last 30 Days
            </button>

            <input
                type="date"
                value={to}
                onChange={(e) => setTo(e.target.value)}
                className="border p-2 rounded-lg w-full sm:w-auto"
            />

            {/* ---------- Alias Dropdown ---------- */}
            <select
                value={selectedLink}
                onChange={(e) => {
                    setSelectedLink(e.target.value);
                    setSelectedTag(""); // reset tag if alias changes
                }}
                className="border p-2 rounded-lg w-full sm:w-auto"
            >
                <option value="">All Alias</option>
                {allLinks.map((link: any) => (
                    <option key={link.id} value={link.shortCode}>
                        {link.shortCode}
                    </option>
                ))}
            </select>

            {/* ---------- Tag Dropdown ---------- */}
            <select
                value={selectedTag}
                onChange={(e) => {
                    setSelectedTag(e.target.value);
                    setSelectedLink(""); // reset alias if tag changes
                }}
                className="border p-2 rounded-lg w-full sm:w-auto"
            >
                <option value="">All Tags</option>
                {allTags.map((tag: any) => (
                    <option key={tag} value={tag}>
                        {tag}
                    </option>
                ))}
            </select>

            {/* ---------- User Dropdown ---------- */}
            {userDropdown && <div className="min-w-[200px]">{userDropdown}</div>}

            {/* ---------- CSV Export ---------- */}
            <button
                onClick={exportCSV}
                className="bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded-lg w-full sm:w-auto"
            >
                Export CSV
            </button>
        </div>
    );
}