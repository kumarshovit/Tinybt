import { useNavigate } from "react-router-dom";
import LinkRow from "./LinkRow";

interface Props {
    links: any[];
    setLinks: React.Dispatch<React.SetStateAction<any[]>>;
    searchTag: string;
    setSearchTag: React.Dispatch<React.SetStateAction<string>>;
    handleSearch: () => void;
    handleClear: () => void;
}


export default function RecentLinks({
    links,
    setLinks,
    searchTag,
    setSearchTag,
    handleSearch,
    handleClear
}: Props) {

    const navigate = useNavigate();

    return (
        <section className="max-w-6xl mx-auto mt-16 px-6">

            <div className="flex items-center justify-between mb-6">
                <div>
                    <h2 className="text-2xl font-semibold text-gray-800">
                        Recent Links
                    </h2>
                    <p className="text-sm text-gray-500">
                        Manage and track your branded short URLs
                    </p>
                </div>

                <div className="flex items-center gap-2">
                    <input
                        value={searchTag}
                        onChange={(e) => setSearchTag(e.target.value)}
                        onKeyDown={(e) => {
                            if (e.key === "Enter" && searchTag.trim()) {
                                handleSearch();
                            }
                        }}
                        placeholder="Search by tag..."
                        className="h-9 px-3 text-sm border border-gray-300 rounded-md w-56 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                    />

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

            <div className="bg-white border border-gray-200 rounded-xl shadow-sm divide-y">

                {links.length === 0 ? (
                    <div className="p-8 text-center text-gray-400">
                        No links found.
                    </div>
                ) : (
                    links.map((link: any) => (
                        <LinkRow
                            key={link.id}
                            link={link}
                            setLinks={setLinks}
                        />
                    ))

                )}
            </div>
        </section>
    );
}
