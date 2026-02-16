import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import CreateLinkCard from "../components/CreateLinkCard";
import SearchBar from "../components/SearchBar";
import LinkCard from "../components/LinkCard";
import { getAllUrls } from "../api/urlService";

export default function Dashboard() {
  const [links, setLinks] = useState<any[]>([]);

  const loadLinks = async () => {
    try {
      const data = await getAllUrls();
      setLinks(data);
    } catch (error) {
      console.error("Error loading links:", error);
    }
  };

  useEffect(() => {
   
  }, []);

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-gray-100">

      {/* NAVBAR */}
      <Navbar />

      {/* HERO SECTION */}
      <div className="text-center py-16 px-6">
        <h2 className="text-4xl font-bold text-gray-800 mb-4">
          Shorten & Manage Your Links
        </h2>
        <p className="text-gray-500 text-lg">
          Custom aliases. Smart tagging. Click tracking.
        </p>
      </div>

      {/* CREATE LINK */}
      <div className="px-6">
        <CreateLinkCard reload={loadLinks} />
      </div>

      {/* SEARCH */}
      <SearchBar setLinks={setLinks} />

      {/* LINKS GRID */}
    <div className="grid md:grid-cols-2 gap-6 max-w-6xl mx-auto mt-12 pb-20 px-6">

  {links.length === 0 ? (
    <div className="col-span-2 text-center text-gray-400 text-lg">
      No links to display.
      <br />
      Create a link or search by tag.
    </div>
  ) : (
    links.map((link: any) => (
      <LinkCard
        key={link.id}
        link={link}
        links={links}
        setLinks={setLinks}
      />
    ))
  )}

</div>


    </div>
  );
}
