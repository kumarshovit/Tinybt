import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import HeroSection from "../components/HeroSection";
import RecentLinks from "../components/RecentLinks";
import { searchByTag, getAllUrls } from "../api/urlService";
import api from "../utils/api";
import Footer from "../components/Footer";
import SEO from "../components/SEO";
export default function Dashboard() {

  const [links, setLinks] = useState<any[]>([]);
  const [searchTag, setSearchTag] = useState("");
  const [loading, setLoading] = useState(true);

  const [activeEdit, setActiveEdit] = useState<{
    id: string;
    type: "alias" | "destination";
  } | null>(null);

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

  // const loadLinks = async () => {
  //   const result = await getAllUrls();

  //   if (!result.success) {
  //     alert(result.message);
  //     return;
  //   }

  //   setLinks(result.data);
  // };

  const loadLinks = async () => {
    setLoading(true);

    try {
      const result = await api.get("/urls");
      setLinks(result.data);
    } catch (err) {
      console.error("Failed to load links", err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async () => {
    if (!searchTag.trim()) return;

    const result = await searchByTag(searchTag);

    if (!result.success) {
      alert(result.message);
      return;
    }

    setLinks(result.data);
  };

  const handleClear = async () => {
    setSearchTag("");

    const result = await getAllUrls();

    if (!result.success) {
      alert(result.message);
      return;
    }

    setLinks(result.data);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <SEO
        title="Dashboard – LinkBT"
        description="Manage your short links from the LinkBT dashboard."
        noindex={true}
      />
      <Navbar />
      <HeroSection onUrlCreated={loadLinks} />
      <RecentLinks
        links={links}
        searchTag={searchTag}
        setSearchTag={setSearchTag}
        handleSearch={handleSearch}
        handleClear={handleClear}
        setLinks={setLinks}
        activeEdit={activeEdit}
        setActiveEdit={setActiveEdit}
        loading={loading}
      />

      <Footer/>

    </div>
  );
}
