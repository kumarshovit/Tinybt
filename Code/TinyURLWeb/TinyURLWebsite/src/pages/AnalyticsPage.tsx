import { useState, useEffect } from "react";
import Navbar from "../components/Navbar";
import SEO from "../components/SEO";
import AnalyticsSidebar from "../components/AnalyticsSidebar";
import HeatmapChart from "../components/analysis/HeatmapChart";
import Filters from "../components/userAnalytics/Filters";
import SummarySection from "../components/userAnalytics/SummarySection";
import TopTagsTable from "../components/userAnalytics/TopTagsTable";
import ClicksChart from "../components/userAnalytics/ClicksChart";
import ChartsSection from "../components/userAnalytics/ChartsSection";
import DataPopup from "../components/userAnalytics/DataPopup";
import useAnalyticsFilters from "../components/hooks/useAnalyticsFilters";
import useAnalyticsData from "../components/hooks/useAnalyticsData";
import useAnalyticsPopup from "../components/hooks/useAnalyticsPopup";
import { getUserRole } from "../utils/auth";
import Footer from "../components/Footer";

// ---------------- USER DROPDOWN ----------------
interface User {
  id: number;
  email: string;
  role: "User" | "Admin";
}

const UserDropdown: React.FC<{ onUserSelect: (user: User | null) => void }> = ({ onUserSelect }) => {
  const [users, setUsers] = useState<User[]>([]);
  const [selectedUserId, setSelectedUserId] = useState<number | "">("");
  const API_BASE = import.meta.env.VITE_API_URL;

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const token = localStorage.getItem("token");
        if (!token) throw new Error("No token found");

        const res = await fetch(`${API_BASE}/api/admin/all-users`, {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        });

        const data: User[] = await res.json();
        setUsers(data.filter(u => u.role.toLowerCase() === "user"));
      } catch (err) {
        console.error(err);
      }
    };

    fetchUsers();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const id = e.target.value ? parseInt(e.target.value) : "";
    setSelectedUserId(id);
    const user = users.find(u => u.id === id) || null;
    onUserSelect(user);
  };

  return (
    <div className="mb-4 w-64 min-w-[200px]">
      <label htmlFor="user-select" className="block mb-1 font-medium">Select User</label>
      <select
        id="user-select"
        value={selectedUserId}
        onChange={handleChange}
        className="border border-gray-300 rounded px-3 py-2 w-full"
      >
        <option value="">All Users</option>
        {users.map(user => (
          <option key={user.id} value={user.id}>
            {user.email}
          </option>
        ))}
      </select>
    </div>
  );
};

// ---------------- MAIN PAGE ----------------
export default function AnalyticsPage() {

  const role = getUserRole(); // 🔥 role detect

  const filters = useAnalyticsFilters();
  const popup = useAnalyticsPopup();

  const [selectedUser, setSelectedUser] = useState<User | null>(null);

  const analytics = useAnalyticsData({
    from: filters.from,
    to: filters.to,
    selectedLink: filters.selectedLink,
    selectedTag: filters.selectedTag,
    userId: role === "Admin" ? selectedUser?.id : undefined
  });

  // ---------- CSV ----------
  const exportCSV = () => {
    let csv = "Date,Clicks\n";

    analytics.clicks.forEach((x: any) => {
      csv += `${x.label},${x.count}\n`;
    });

    csv += "\nTop Links\n";

    analytics.topLinks.forEach((x: any) => {
      csv += `${x.label},${x.count}\n`;
    });

    const blob = new Blob([csv], { type: "text/csv" });
    const url = window.URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = "analytics.csv";
    a.click();
  };

  return (
    <>
      <SEO
        title="Analytics – LinkBT"
        description="View detailed click analytics for your short links."
        noindex={true}
      />
      <Navbar />

      <div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">

        {/* Sidebar */}
        <div className="lg:w-64 w-full">
          <AnalyticsSidebar />
        </div>

        <div className="flex-1 p-4 sm:p-6 lg:p-8">
          <div className="max-w-7xl mx-auto w-full">

            {/* 🔥 Dynamic Heading */}
            <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6 lg:mb-8">
              📊 {role === "Admin" ? "Admin Analytics Dashboard" : "My Analytics Dashboard"}
            </h1>

            {/* USER TIP (only user) */}
            {role === "User" && (
              <p className="text-sm text-gray-500 mb-4">
                💡 Tip: Click on any chart or card to view detailed data
              </p>
            )}

            {/* 🔥 ADMIN ONLY */}
            {role === "Admin" && (
              <div className="flex flex-wrap items-center mb-4 gap-4">
                <UserDropdown onUserSelect={setSelectedUser} />
              </div>
            )}

            {/* FILTERS */}
            <Filters
              setLast7Days={filters.setLast7Days}
              setLast30Days={filters.setLast30Days}
              from={filters.from}
              to={filters.to}
              setFrom={filters.setFrom}
              setTo={filters.setTo}
              selectedLink={filters.selectedLink}
              setSelectedLink={filters.setSelectedLink}
              selectedTag={filters.selectedTag}
              setSelectedTag={filters.setSelectedTag}
              allLinks={analytics.allLinks}
              allTags={analytics.allTags}
              exportCSV={exportCSV}
            />

            {/* SUMMARY */}
            <SummarySection
              totalClicks={role === "Admin" ? analytics.totalClicks : analytics.clicks}
             clicks={analytics.clicks} 
              totalLinks={analytics.linkClicks}
              country={analytics.country}
              device={analytics.device}
              browser={analytics.browser}
              openDataPopup={popup.openDataPopup}
              openAllLinksPopup={analytics.openAllLinksPopup}
            />

            {/* TOP TAGS */}
            <TopTagsTable
              selectedLink={filters.selectedLink}
              selectedTag={filters.selectedTag}
              topLinks={analytics.topLinks}
              openTagPopup={async (tag: string) => {
                const data = await analytics.openTagPopup(tag);
                popup.setPopupTitle(`Links for tag: ${tag}`);
                popup.setPopupData(data);
                popup.setOpenPopup(true);
              }}
            />

            {/* CLICKS */}
            <ClicksChart
              clicks={analytics.clicks}
              openDataPopup={popup.openDataPopup}
            />

            {/* CHARTS */}
            <ChartsSection
              referrer={analytics.referrer}
              country={analytics.country}
              device={analytics.device}
              os={analytics.os}
              browser={analytics.browser}
              language={analytics.language}
              openDataPopup={popup.openDataPopup}
            />

            {/* HEATMAP */}
            <div className="mt-8 lg:mt-12 overflow-x-auto">
              <HeatmapChart data={analytics.heatmap} />
            </div>

          </div>
        </div>
      </div>

      {/* POPUP */}
      <DataPopup
        openPopup={popup.openPopup}
        popupTitle={popup.popupTitle}
        popupData={popup.popupData}
        setOpenPopup={popup.setOpenPopup}
      />

      <Footer/>
    </>
  );
}