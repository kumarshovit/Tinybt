import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import HeroSection from "../components/HeroSection";
import RecentLinks from "../components/RecentLinks";
import { searchByTag, getAllUrls } from "../api/urlService";

export default function Dashboard() {

  const [links, setLinks] = useState<any[]>([]);
  const [searchTag, setSearchTag] = useState("");

  useEffect(() => {
    loadLinks();
  }, []);

  useEffect(() => {
    const handleVisibility = () => {
      if (document.visibilityState === "visible") {
        loadLinks();
      }
    };
    document.addEventListener("visibilitychange", handleVisibility);

    return () => {
      document.removeEventListener("visibilitychange", handleVisibility);
    };
  }, []);

  const loadLinks = async () => {
    const data = await getAllUrls();
    setLinks(data);
  };
  const handleSearch = async () => {
    if (!searchTag.trim()) return;

    const result = await searchByTag(searchTag);
    setLinks(result);
  };

  const handleClear = async () => {
    setSearchTag("");
    const data = await getAllUrls();
    setLinks(data);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <HeroSection onUrlCreated={loadLinks}  />
      <RecentLinks links={links} searchTag={searchTag}
        setSearchTag={setSearchTag}
        handleSearch={handleSearch}
        handleClear={handleClear}
         setLinks={setLinks} />
    </div>
  );
}
