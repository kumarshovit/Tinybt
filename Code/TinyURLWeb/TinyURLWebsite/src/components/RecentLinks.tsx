import LinkRow from "./LinkRow";

interface Props {
    links: any[];
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

    searchTag: string;
    setSearchTag: React.Dispatch<React.SetStateAction<string>>;
    handleSearch: () => void;
    handleClear: () => void;

    loading: boolean;

}

export default function RecentLinks({
    links,
    setLinks,
    activeEdit,
    setActiveEdit,
    searchTag,
    setSearchTag,
    handleSearch,
    handleClear,
    loading
}: Props) {

    return (
        <section className="max-w-6xl mx-auto mt-16 px-6">

            {/* HEADER */}
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-6">

                {/* LEFT SIDE */}
                <div>
                    <h2 className="text-2xl font-semibold text-gray-800">
                        Recent Links
                    </h2>
                    <p className="text-sm text-gray-500">
                        Manage and track your branded short URLs
                    </p>
                </div>

                {/* SEARCH SECTION */}
                <div className="flex flex-col sm:flex-row gap-2 w-full md:w-auto">

                    <input
                        aria-label="Search links by tag"
                        value={searchTag}
                        onChange={(e) => setSearchTag(e.target.value)}
                        onKeyDown={(e) => {
                            if (e.key === "Enter" && searchTag.trim()) {
                                handleSearch();
                            }
                        }}
                        placeholder="Search by tag..."
                        className="h-9 px-3 text-sm border border-gray-300 rounded-md w-full sm:w-56 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                    />

                    <div className="flex gap-2">

                        <button
                            onClick={handleSearch}
                            className="h-9 px-4 text-sm bg-blue-600 text-white rounded-md hover:bg-blue-700 transition"
                        >
                            Search
                        </button>

                        <button
                            onClick={handleClear}
                            className="h-9 px-4 text-sm bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300 transition"
                        >
                            Clear
                        </button>

                    </div>
                </div>

            </div>

            {/* LINKS LIST */}
            <div className="bg-white border border-gray-200 rounded-xl shadow-sm divide-y">

                {/* LOADING SKELETON */}
                {loading &&
                    Array.from({ length: 3 }).map((_, i) => (
                        <div
                            key={i}
                            className="p-6 animate-pulse space-y-3"
                        >
                            <div className="h-4 bg-gray-200 rounded w-64"></div>
                            <div className="h-3 bg-gray-200 rounded w-96"></div>
                            <div className="h-3 bg-gray-200 rounded w-24"></div>
                        </div>
                    ))
                }

                {/* REAL LINKS */}
                {!loading && links.length === 0 ? (
                    <div className="p-8 text-center text-gray-400">
                        No links found.
                    </div>
                ) : (
                    !loading &&
                    links.map((link: any) => (
                        <LinkRow
                            key={link.id}
                            link={link}
                            setLinks={setLinks}
                            activeEdit={activeEdit}
                            setActiveEdit={setActiveEdit}
                        />
                    ))
                )}

            </div>
        </section>
    );
}
