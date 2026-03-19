// // // // // import { useEffect, useState } from "react";
// // // // // import { getDashboardOverview } from "../api/dashboardService";
// // // // // import type { DashboardOverview } from "../api/dashboardService";

// // // // // import AnalysisSidebar from "../components/analysis/AnalysisSidebar";
// // // // // import ClicksByGeography from "../components/analysis/ClicksByGeography";
// // // // // import ClicksByLanguage from "../components/analysis/ClicksByLanguage";
// // // // // import PopularDaysTimes from "../components/analysis/PopularDaysTimes";
// // // // // import ClicksByDevice from "../components/analysis/ClicksByDevice";
// // // // // import ClicksByOS from "../components/analysis/ClicksByOS";
// // // // // import ClicksByBrowser from "../components/analysis/ClicksByBrowser";
// // // // // import Navbar from "../components/Navbar";
// // // // // import UsersOverTime from "../components/analysis/UsersOverTime";

// // // // // const AnalysisPage = () => {

// // // // //   const today = new Date();
// // // // //   const last7Days = new Date();
// // // // //   last7Days.setDate(today.getDate() - 7);

// // // // //   const [data, setData] = useState<DashboardOverview | null>(null);
// // // // //   const [loading, setLoading] = useState(true);

// // // // //   const [startDate, setStartDate] = useState(
// // // // //     last7Days.toISOString().split("T")[0]
// // // // //   );

// // // // //   const [endDate, setEndDate] = useState(
// // // // //     today.toISOString().split("T")[0]
// // // // //   );

// // // // //   const [active, setActive] = useState("time");

// // // // //   const loadDashboard = async () => {
// // // // //     try {
// // // // //       setLoading(true);

// // // // //       const result = await getDashboardOverview(startDate, endDate);
// // // // //       setData(result);

// // // // //     } catch (error) {
// // // // //       console.error("Failed to load dashboard", error);
// // // // //     } finally {
// // // // //       setLoading(false);
// // // // //     }
// // // // //   };

// // // // //   useEffect(() => {
// // // // //     loadDashboard();
// // // // //   }, [startDate, endDate]);

// // // // //   return (
// // // // //     <div className="min-h-screen bg-gray-50 flex flex-col">

// // // // //       <Navbar />

// // // // //       <div className="flex flex-col lg:flex-row flex-1">

// // // // //         <AnalysisSidebar active={active} setActive={setActive} />

// // // // //         <div className="flex-1 p-4 sm:p-6 lg:p-8">

// // // // //           <h1 className="text-xl sm:text-2xl font-bold mb-6 ">
// // // // //             Link Performance Overview
// // // // //           </h1>

// // // // //           {/* Date Filters */}

// // // // //           <div className="flex flex-col sm:flex-row gap-3 mb-6">

// // // // //             <input
// // // // //               type="date"
// // // // //               value={startDate}
// // // // //               onChange={(e) => setStartDate(e.target.value)}
// // // // //               className="border p-2 rounded w-full sm:w-auto"
// // // // //             />

// // // // //             <input
// // // // //               type="date"
// // // // //               value={endDate}
// // // // //               onChange={(e) => setEndDate(e.target.value)}
// // // // //               className="border p-2 rounded w-full sm:w-auto"
// // // // //             />

// // // // //             <button
// // // // //               onClick={loadDashboard}
// // // // //               className="bg-blue-600 text-white px-4 py-2 rounded w-full sm:w-auto hover:bg-blue-700 transition"
// // // // //             >
// // // // //               Apply
// // // // //             </button>

// // // // //           </div>

// // // // //           {/* Loading Skeleton */}

// // // // //           {loading && (
// // // // //             <>
// // // // //               <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
// // // // //                 {[1, 2, 3, 4].map((i) => (
// // // // //                   <div
// // // // //                     key={i}
// // // // //                     className="bg-gray-200 h-24 rounded animate-pulse"
// // // // //                   />
// // // // //                 ))}
// // // // //               </div>

// // // // //               <div className="bg-gray-200 h-96 rounded animate-pulse"></div>
// // // // //             </>
// // // // //           )}

// // // // //           {/* Dashboard Data */}

// // // // //           {!loading && data && (
// // // // //             <>

// // // // //               {/* Summary Cards */}

// // // // //               <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">

// // // // //                 <div className="bg-white shadow rounded p-6">
// // // // //                   <h2 className="text-gray-500 text-sm">Total Clicks</h2>
// // // // //                   <p className="text-2xl font-bold">{data.totalClicks}</p>
// // // // //                 </div>

// // // // //                 <div className="bg-white shadow rounded p-6">
// // // // //                   <h2 className="text-gray-500 text-sm">Total Links</h2>
// // // // //                   <p className="text-2xl font-bold">{data.totalUrls}</p>
// // // // //                 </div>

// // // // //                 <div className="bg-white shadow rounded p-6">
// // // // //                   <h2 className="text-gray-500 text-sm">Active Links</h2>
// // // // //                   <p className="text-2xl font-bold">{data.activeLinks}</p>
// // // // //                 </div>

// // // // //                 <div className="bg-white shadow rounded p-6">
// // // // //                   <h2 className="text-gray-500 text-sm">Expired Links</h2>
// // // // //                   <p className="text-2xl font-bold">{data.expiredLinks}</p>
// // // // //                 </div>

// // // // //               </div>

// // // // //               {/* Analytics Section */}

// // // // //               <div id="chart-section" className="bg-white rounded shadow p-4 sm:p-6">

// // // // //                 {active === "time" && (
// // // // //                   <UsersOverTime startDate={startDate} endDate={endDate} />
// // // // //                 )}

// // // // //                 {active === "geo" && (
// // // // //                   <ClicksByGeography startDate={startDate} endDate={endDate} />
// // // // //                 )}

// // // // //                 {active === "lang" && (
// // // // //                   <ClicksByLanguage startDate={startDate} endDate={endDate} />
// // // // //                 )}

// // // // //                 {active === "popular" && (
// // // // //                   <PopularDaysTimes startDate={startDate} endDate={endDate} />
// // // // //                 )}

// // // // //                 {active === "device" && (
// // // // //                   <ClicksByDevice startDate={startDate} endDate={endDate} />
// // // // //                 )}

// // // // //                 {active === "os" && (
// // // // //                   <ClicksByOS startDate={startDate} endDate={endDate} />
// // // // //                 )}

// // // // //                 {active === "browser" && (
// // // // //                   <ClicksByBrowser startDate={startDate} endDate={endDate} />
// // // // //                 )}

// // // // //               </div>

// // // // //             </>
// // // // //           )}

// // // // //         </div>

// // // // //       </div>

// // // // //     </div>
// // // // //   );
// // // // // };

// // // // // export default AnalysisPage;

// // // // // import Navbar from "../components/Navbar";
// // // // // import AnalyticsSidebar from "../components/AnalyticsSidebar";
// // // // // import HeatmapChart from "../components/analysis/HeatmapChart";
// // // // // import Filters from "../components/userAnalytics/Filters";
// // // // // import SummarySection from "../components/userAnalytics/SummarySection";
// // // // // import TopTagsTable from "../components/userAnalytics/TopTagsTable";
// // // // // import ClicksChart from "../components/userAnalytics/ClicksChart";
// // // // // import ChartsSection from "../components/userAnalytics/ChartsSection";
// // // // // import DataPopup from "../components/userAnalytics/DataPopup";
// // // // // import useAnalyticsFilters from "../components/hooks/useAnalyticsFilters";
// // // // // import useAnalyticsData from "../components/hooks/useAnalyticsData";
// // // // // import useAnalyticsPopup from "../components/hooks/useAnalyticsPopup";

// // // // // export default function MyAnalytics(){

// // // // // /* ---------- HOOKS ---------- */

// // // // // const filters = useAnalyticsFilters();

// // // // // const analytics = useAnalyticsData({
// // // // // from:filters.from,
// // // // // to:filters.to,
// // // // // selectedLink:filters.selectedLink,
// // // // // selectedTag:filters.selectedTag
// // // // // });

// // // // // const popup = useAnalyticsPopup();

// // // // // /* ---------- CSV ---------- */

// // // // // const exportCSV = ()=>{

// // // // // let csv = "Date,Clicks\n";

// // // // // analytics.clicks.forEach((x:any)=>{
// // // // // csv += `${x.label},${x.count}\n`;
// // // // // });

// // // // // csv += "\nTop Links\n";

// // // // // analytics.topLinks.forEach((x:any)=>{
// // // // // csv += `${x.label},${x.count}\n`;
// // // // // });

// // // // // const blob = new Blob([csv],{type:"text/csv"});
// // // // // const url = window.URL.createObjectURL(blob);

// // // // // const a = document.createElement("a");
// // // // // a.href = url;
// // // // // a.download = "analytics.csv";
// // // // // a.click();

// // // // // };

// // // // // return(

// // // // // <>
// // // // // <Navbar/>
// // // // // <div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">
// // // // // <div className="lg:w-64 w-full">
// // // // // <AnalyticsSidebar/>
// // // // // </div>
// // // // // <div className="flex-1 p-4 sm:p-6 lg:p-8">
// // // // // <div className="max-w-7xl mx-auto w-full">
// // // // // <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6 lg:mb-8">
// // // // // 📊 My Analytics Dashboard
// // // // // </h1>

// // // // // {/* ---------- FILTERS ---------- */}

// // // // // <Filters
// // // // // setLast7Days={filters.setLast7Days}
// // // // // setLast30Days={filters.setLast30Days}
// // // // // from={filters.from}
// // // // // to={filters.to}
// // // // // setFrom={filters.setFrom}
// // // // // setTo={filters.setTo}
// // // // // selectedLink={filters.selectedLink}
// // // // // setSelectedLink={filters.setSelectedLink}
// // // // // selectedTag={filters.selectedTag}
// // // // // setSelectedTag={filters.setSelectedTag}
// // // // // allLinks={analytics.allLinks}
// // // // // allTags={analytics.allTags}
// // // // // exportCSV={exportCSV}
// // // // // />

// // // // // {/* ---------- SUMMARY ---------- */}

// // // // // <SummarySection
// // // // // totalClicks={analytics.totalClicks}
// // // // // country={analytics.country}
// // // // // device={analytics.device}
// // // // // browser={analytics.browser}
// // // // // />

// // // // // {/* ---------- TOP TAGS ---------- */}

// // // // // <TopTagsTable
// // // // // selectedLink={filters.selectedLink}
// // // // // selectedTag={filters.selectedTag}
// // // // // topLinks={analytics.topLinks}
// // // // // openTagPopup={async(tag:string)=>{

// // // // // const data = await analytics.openTagPopup(tag)

// // // // // popup.setPopupTitle(`Links for tag: ${tag}`)
// // // // // popup.setPopupData(data)
// // // // // popup.setOpenPopup(true)

// // // // // }}
// // // // // />

// // // // // {/* ---------- CLICKS CHART ---------- */}

// // // // // <ClicksChart
// // // // // clicks={analytics.clicks}
// // // // // openDataPopup={popup.openDataPopup}
// // // // // />

// // // // // {/* ---------- CHARTS ---------- */}

// // // // // <ChartsSection
// // // // // referrer={analytics.referrer}
// // // // // country={analytics.country}
// // // // // device={analytics.device}
// // // // // os={analytics.os}
// // // // // browser={analytics.browser}
// // // // // language={analytics.language}
// // // // // openDataPopup={popup.openDataPopup}
// // // // // />

// // // // // {/* ---------- HEATMAP ---------- */}

// // // // // <div id="heatmap" className="mt-8 lg:mt-12 overflow-x-auto">
// // // // // <HeatmapChart/>
// // // // // </div>

// // // // // </div>

// // // // // </div>

// // // // // </div>

// // // // // {/* ---------- POPUP ---------- */}

// // // // // <DataPopup
// // // // // openPopup={popup.openPopup}
// // // // // popupTitle={popup.popupTitle}
// // // // // popupData={popup.popupData}
// // // // // setOpenPopup={popup.setOpenPopup}
// // // // // />

// // // // // </>

// // // // // );

// // // // // }


// // // // import { useState, useEffect } from "react";
// // // // import Navbar from "../components/Navbar";
// // // // import AnalyticsSidebar from "../components/AnalyticsSidebar";
// // // // import HeatmapChart from "../components/analysis/HeatmapChart";
// // // // import Filters from "../components/userAnalytics/Filters";
// // // // import SummarySection from "../components/userAnalytics/SummarySection";
// // // // import TopTagsTable from "../components/userAnalytics/TopTagsTable";
// // // // import ClicksChart from "../components/userAnalytics/ClicksChart";
// // // // import ChartsSection from "../components/userAnalytics/ChartsSection";
// // // // import DataPopup from "../components/userAnalytics/DataPopup";
// // // // import useAnalyticsFilters from "../components/hooks/useAnalyticsFilters";
// // // // import useAnalyticsData from "../components/hooks/useAnalyticsData";
// // // // import useAnalyticsPopup from "../components/hooks/useAnalyticsPopup";

// // // // // ---------------- USER DROPDOWN COMPONENT ----------------
// // // // interface User {
// // // //   id: number;
// // // //   email: string;
// // // //   role: "User" | "Admin";
// // // // }

// // // // const UserDropdown: React.FC<{ onUserSelect: (user: User | null) => void }> = ({ onUserSelect }) => {
// // // //   const [users, setUsers] = useState<User[]>([]);
// // // //   const [selectedUserId, setSelectedUserId] = useState<number | "">("");

// // // //   // Fetch all users
// // // //   useEffect(() => {
// // // //     const fetchUsers = async () => {
// // // //       try {
// // // //         const res = await fetch("/api/admin/all-users", {
// // // //           headers: { "Content-Type": "application/json" },
// // // //         });
// // // //         if (!res.ok) throw new Error("Failed to fetch users");

// // // //         const data: User[] = await res.json();

// // // //         // Only include users with role "User"
// // // //         const onlyUsers = data.filter((u) => u.role === "User");
// // // //         setUsers(onlyUsers);
// // // //       } catch (err) {
// // // //         console.error("Failed to fetch users:", err);
// // // //       }
// // // //     };
// // // //     fetchUsers();
// // // //   }, []);

// // // //   const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
// // // //     const id = e.target.value ? parseInt(e.target.value) : "";
// // // //     setSelectedUserId(id);
// // // //     const user = users.find((u) => u.id === id) || null;
// // // //     onUserSelect(user);
// // // //   };

// // // //   return (
// // // //     <div className="mb-4 w-full max-w-sm">
// // // //       <label className="block mb-1 font-medium">Select User</label>
// // // //       <select
// // // //         value={selectedUserId}
// // // //         onChange={handleChange}
// // // //         className="border border-gray-300 rounded px-3 py-2 w-full"
// // // //       >
// // // //         <option value="">All / Default</option>
// // // //         {users.map((user) => (
// // // //           <option key={user.id} value={user.id}>
// // // //             {user.email}
// // // //           </option>
// // // //         ))}
// // // //       </select>
// // // //     </div>
// // // //   );
// // // // };

// // // // // ---------------- MAIN PAGE ----------------
// // // // export default function MyAnalytics() {
// // // //   /* ---------- HOOKS ---------- */
// // // //   const filters = useAnalyticsFilters();
// // // //   const popup = useAnalyticsPopup();
// // // //   const [selectedUser, setSelectedUser] = useState<User | null>(null);

// // // //   // Fetch analytics based on filters + selected user
// // // //   const analytics = useAnalyticsData({
// // // //     from: filters.from,
// // // //     to: filters.to,
// // // //     selectedLink: filters.selectedLink,
// // // //     selectedTag: filters.selectedTag,
// // // //     userId: selectedUser?.id,
// // // //   });

// // // //   /* ---------- CSV EXPORT ---------- */
// // // //   const exportCSV = () => {
// // // //     let csv = "Date,Clicks\n";
// // // //     analytics.clicks.forEach((x: any) => {
// // // //       csv += `${x.label},${x.count}\n`;
// // // //     });

// // // //     csv += "\nTop Links\n";
// // // //     analytics.topLinks.forEach((x: any) => {
// // // //       csv += `${x.label},${x.count}\n`;
// // // //     });

// // // //     const blob = new Blob([csv], { type: "text/csv" });
// // // //     const url = window.URL.createObjectURL(blob);
// // // //     const a = document.createElement("a");
// // // //     a.href = url;
// // // //     a.download = "analytics.csv";
// // // //     a.click();
// // // //   };

// // // //   /* ---------- RENDER ---------- */
// // // //   return (
// // // //     <>
// // // //       <Navbar />
// // // //       <div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">
// // // //         <div className="lg:w-64 w-full">
// // // //           <AnalyticsSidebar />
// // // //         </div>
// // // //         <div className="flex-1 p-4 sm:p-6 lg:p-8">
// // // //           <div className="max-w-7xl mx-auto w-full">
// // // //             <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6 lg:mb-8">
// // // //               📊 My Analytics Dashboard
// // // //             </h1>

// // // //             {/* ---------- USER DROPDOWN ---------- */}
// // // //             <UserDropdown onUserSelect={setSelectedUser} />

// // // //             {/* ---------- FILTERS ---------- */}
// // // //             <Filters
// // // //               setLast7Days={filters.setLast7Days}
// // // //               setLast30Days={filters.setLast30Days}
// // // //               from={filters.from}
// // // //               to={filters.to}
// // // //               setFrom={filters.setFrom}
// // // //               setTo={filters.setTo}
// // // //               selectedLink={filters.selectedLink}
// // // //               setSelectedLink={filters.setSelectedLink}
// // // //               selectedTag={filters.selectedTag}
// // // //               setSelectedTag={filters.setSelectedTag}
// // // //               allLinks={analytics.allLinks}
// // // //               allTags={analytics.allTags}
// // // //               exportCSV={exportCSV}
// // // //             />

// // // //             {/* ---------- SUMMARY ---------- */}
// // // //             <SummarySection
// // // //               totalClicks={analytics.totalClicks}
// // // //               country={analytics.country}
// // // //               device={analytics.device}
// // // //               browser={analytics.browser}
// // // //             />

// // // //             {/* ---------- TOP TAGS ---------- */}
// // // //             <TopTagsTable
// // // //               selectedLink={filters.selectedLink}
// // // //               selectedTag={filters.selectedTag}
// // // //               topLinks={analytics.topLinks}
// // // //               openTagPopup={async (tag: string) => {
// // // //                 const data = await analytics.openTagPopup(tag);
// // // //                 popup.setPopupTitle(`Links for tag: ${tag}`);
// // // //                 popup.setPopupData(data);
// // // //                 popup.setOpenPopup(true);
// // // //               }}
// // // //             />

// // // //             {/* ---------- CLICKS CHART ---------- */}
// // // //             <ClicksChart clicks={analytics.clicks} openDataPopup={popup.openDataPopup} />

// // // //             {/* ---------- CHARTS ---------- */}
// // // //             <ChartsSection
// // // //               referrer={analytics.referrer}
// // // //               country={analytics.country}
// // // //               device={analytics.device}
// // // //               os={analytics.os}
// // // //               browser={analytics.browser}
// // // //               language={analytics.language}
// // // //               openDataPopup={popup.openDataPopup}
// // // //             />

// // // //             {/* ---------- HEATMAP ---------- */}
// // // //             <div id="heatmap" className="mt-8 lg:mt-12 overflow-x-auto">
// // // //               <HeatmapChart />
// // // //             </div>
// // // //           </div>
// // // //         </div>
// // // //       </div>

// // // //       {/* ---------- POPUP ---------- */}
// // // //       <DataPopup
// // // //         openPopup={popup.openPopup}
// // // //         popupTitle={popup.popupTitle}
// // // //         popupData={popup.popupData}
// // // //         setOpenPopup={popup.setOpenPopup}
// // // //       />
// // // //     </>
// // // //   );
// // // // }


// // // import { useState } from "react";
// // // import Navbar from "../components/Navbar";
// // // import AnalyticsSidebar from "../components/AnalyticsSidebar";
// // // import HeatmapChart from "../components/analysis/HeatmapChart";
// // // import Filters from "../components/userAnalytics/Filters";
// // // import SummarySection from "../components/userAnalytics/SummarySection";
// // // import TopTagsTable from "../components/userAnalytics/TopTagsTable";
// // // import ClicksChart from "../components/userAnalytics/ClicksChart";
// // // import ChartsSection from "../components/userAnalytics/ChartsSection";
// // // import DataPopup from "../components/userAnalytics/DataPopup";
// // // import useAnalyticsFilters from "../components/hooks/useAnalyticsFilters";
// // // import useAnalyticsData from "../components/hooks/useAnalyticsData";
// // // import useAnalyticsPopup from "../components/hooks/useAnalyticsPopup";

// // // // ---------------- USER DROPDOWN ----------------
// // // interface User {
// // //   id: number;
// // //   email: string;
// // //   role: "User" | "Admin";
// // // }

// // // const UserDropdown: React.FC<{ onUserSelect: (user: User | null) => void }> = ({ onUserSelect }) => {
// // //   const [users, setUsers] = useState<User[]>([]);
// // //   const [selectedUserId, setSelectedUserId] = useState<number | "">("");
// // //   const API_BASE = import.meta.env.VITE_API_URL;

// // //   // Fetch all users on mount
// // //   useState(() => {
// // //     const fetchUsers = async () => {
// // //       try {
// // //         const res = await fetch(`${API_BASE}/api/admin/all-users`, {
// // //           headers: { "Content-Type": "application/json" },
// // //         });
// // //         if (!res.ok) throw new Error("Failed to fetch users");
// // //         const data: User[] = await res.json();
// // //         console.log("Fetched users:", data); // check console
// // //         setUsers(data); // ✅ include all users
// // //       } catch (err) {
// // //         console.error("Error fetching users:", err);
// // //       }
// // //     };
// // //     fetchUsers();
// // //   });

// // //   const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
// // //     const id = e.target.value ? parseInt(e.target.value) : "";
// // //     setSelectedUserId(id);
// // //     const user = users.find((u) => u.id === id) || null;
// // //     onUserSelect(user);
// // //   };

// // //   return (
// // //     <div className="mr-4 mb-4 w-64 min-w-[200px]">
// // //       <label className="block mb-1 font-medium">Select User</label>
// // //       <select
// // //         value={selectedUserId}
// // //         onChange={handleChange}
// // //         className="border border-gray-300 rounded px-3 py-2 w-full"
// // //       >
// // //         <option value="">All / Default</option>
// // //         {users.map((user) => (
// // //           <option key={user.id} value={user.id}>
// // //             {user.email} ({user.role})
// // //           </option>
// // //         ))}
// // //       </select>
// // //     </div>
// // //   );
// // // };

// // // // ---------------- MAIN PAGE ----------------
// // // export default function MyAnalytics() {
// // //   const filters = useAnalyticsFilters();
// // //   const popup = useAnalyticsPopup();
// // //   const [selectedUser, setSelectedUser] = useState<User | null>(null);

// // //   // Fetch analytics dynamically based on selected user
// // //   const analytics = useAnalyticsData({
// // //     from: filters.from,
// // //     to: filters.to,
// // //     selectedLink: filters.selectedLink,
// // //     selectedTag: filters.selectedTag,
// // //     userId: selectedUser?.id, // 🔹 important for per-user analytics
// // //   });

// // //   // CSV Export
// // //   const exportCSV = () => {
// // //     let csv = "Date,Clicks\n";
// // //     analytics.clicks.forEach((x: any) => {
// // //       csv += `${x.label},${x.count}\n`;
// // //     });
// // //     csv += "\nTop Links\n";
// // //     analytics.topLinks.forEach((x: any) => {
// // //       csv += `${x.label},${x.count}\n`;
// // //     });
// // //     const blob = new Blob([csv], { type: "text/csv" });
// // //     const url = window.URL.createObjectURL(blob);
// // //     const a = document.createElement("a");
// // //     a.href = url;
// // //     a.download = "analytics.csv";
// // //     a.click();
// // //   };

// // //   return (
// // //     <>
// // //       <Navbar />
// // //       <div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">
// // //         <div className="lg:w-64 w-full">
// // //           <AnalyticsSidebar />
// // //         </div>
// // //         <div className="flex-1 p-4 sm:p-6 lg:p-8">
// // //           <div className="max-w-7xl mx-auto w-full">
// // //             <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6 lg:mb-8">
// // //               📊 My Analytics Dashboard
// // //             </h1>

// // //             ---------- DROPDOWN + CSV BUTTON ----------
// // //             {/* <div className="flex flex-wrap items-end mb-4">
// // //               <UserDropdown onUserSelect={setSelectedUser} />
// // //               <button
// // //                 onClick={exportCSV}
// // //                 className="mb-2 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
// // //               >
// // //                 Export CSV
// // //               </button>
// // //             </div> */}

// // //             {/* ---------- FILTERS ---------- */}
// // //             <Filters
// // //               setLast7Days={filters.setLast7Days}
// // //               setLast30Days={filters.setLast30Days}
// // //               from={filters.from}
// // //               to={filters.to}
// // //               setFrom={filters.setFrom}
// // //               setTo={filters.setTo}
// // //               selectedLink={filters.selectedLink}
// // //               setSelectedLink={filters.setSelectedLink}
// // //               selectedTag={filters.selectedTag}
// // //               setSelectedTag={filters.setSelectedTag}
// // //               allLinks={analytics.allLinks}
// // //               allTags={analytics.allTags}
// // //               userDropdown={<UserDropdown onUserSelect={setSelectedUser} />}
// // //             />

// // //             {/* ---------- SUMMARY ---------- */}
// // //             <SummarySection
// // //               totalClicks={analytics.totalClicks}
// // //               country={analytics.country}
// // //               device={analytics.device}
// // //               browser={analytics.browser}
// // //             />

// // //             {/* ---------- TOP TAGS ---------- */}
// // //             <TopTagsTable
// // //               selectedLink={filters.selectedLink}
// // //               selectedTag={filters.selectedTag}
// // //               topLinks={analytics.topLinks}
// // //               openTagPopup={async (tag: string) => {
// // //                 const data = await analytics.openTagPopup(tag);
// // //                 popup.setPopupTitle(`Links for tag: ${tag}`);
// // //                 popup.setPopupData(data);
// // //                 popup.setOpenPopup(true);
// // //               }}
// // //             />

// // //             {/* ---------- CLICKS CHART ---------- */}
// // //             <ClicksChart clicks={analytics.clicks} openDataPopup={popup.openDataPopup} />

// // //             {/* ---------- CHARTS ---------- */}
// // //             <ChartsSection
// // //               referrer={analytics.referrer}
// // //               country={analytics.country}
// // //               device={analytics.device}
// // //               os={analytics.os}
// // //               browser={analytics.browser}
// // //               language={analytics.language}
// // //               openDataPopup={popup.openDataPopup}
// // //             />

// // //             {/* ---------- HEATMAP ---------- */}
// // //             <div id="heatmap" className="mt-8 lg:mt-12 overflow-x-auto">
// // //               <HeatmapChart />
// // //             </div>
// // //           </div>
// // //         </div>
// // //       </div>

// // //       {/* ---------- POPUP ---------- */}
// // //       <DataPopup
// // //         openPopup={popup.openPopup}
// // //         popupTitle={popup.popupTitle}
// // //         popupData={popup.popupData}
// // //         setOpenPopup={popup.setOpenPopup}
// // //       />
// // //     </>
// // //   );
// // // }


// // import { useState, useEffect } from "react";
// // import Navbar from "../components/Navbar";
// // import AnalyticsSidebar from "../components/AnalyticsSidebar";
// // import HeatmapChart from "../components/analysis/HeatmapChart";
// // import Filters from "../components/userAnalytics/Filters";
// // import SummarySection from "../components/userAnalytics/SummarySection";
// // import TopTagsTable from "../components/userAnalytics/TopTagsTable";
// // import ClicksChart from "../components/userAnalytics/ClicksChart";
// // import ChartsSection from "../components/userAnalytics/ChartsSection";
// // import DataPopup from "../components/userAnalytics/DataPopup";
// // import useAnalyticsFilters from "../components/hooks/useAnalyticsFilters";
// // import useAnalyticsData from "../components/hooks/useAnalyticsData";
// // import useAnalyticsPopup from "../components/hooks/useAnalyticsPopup";

// // // ---------------- USER DROPDOWN ----------------
// // interface User {
// //   id: number;
// //   email: string;
// //   role: "User" | "Admin";
// // }

// // const UserDropdown: React.FC<{ onUserSelect: (user: User | null) => void }> = ({ onUserSelect }) => {
// //   const [users, setUsers] = useState<User[]>([]);
// //   const [selectedUserId, setSelectedUserId] = useState<number | "">("");
// //   const API_BASE = import.meta.env.VITE_API_URL;

// //   // ✅ useEffect to fetch users on mount
// //   useEffect(() => {
// //     const fetchUsers = async () => {
// //       try {
// //         const res = await fetch(`${API_BASE}/api/admin/all-users`);
// //         if (!res.ok) throw new Error("Failed to fetch users");
// //         const data: User[] = await res.json();
// //         setUsers(data.filter(u => u.role === "User")); // Only normal users
// //       } catch (err) {
// //         console.error("Error fetching users:", err);
// //       }
// //     };
// //     fetchUsers();
// //   }, []);

// //   const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
// //     const id = e.target.value ? parseInt(e.target.value) : "";
// //     setSelectedUserId(id);
// //     const user = users.find(u => u.id === id) || null;
// //     onUserSelect(user);
// //   };

// //   return (
// //     <div className="mb-4 w-64 min-w-[200px]">
// //       <label className="block mb-1 font-medium">Select User</label>
// //       <select
// //         value={selectedUserId}
// //         onChange={handleChange}
// //         className="border border-gray-300 rounded px-3 py-2 w-full"
// //       >
// //         <option value="">All / Default</option>
// //         {users.map(user => (
// //           <option key={user.id} value={user.id}>
// //             {user.email}
// //           </option>
// //         ))}
// //       </select>
// //     </div>
// //   );
// // };

// // // ---------------- MAIN PAGE ----------------
// // export default function AdminAnalytics() {
// //   const filters = useAnalyticsFilters();
// //   const popup = useAnalyticsPopup();
// //   const [selectedUser, setSelectedUser] = useState<User | null>(null);

// //   // ✅ Fetch analytics with optional userId
// //   const analytics = useAnalyticsData({
// //     from: filters.from,
// //     to: filters.to,
// //     selectedLink: filters.selectedLink,
// //     selectedTag: filters.selectedTag,
// //     userId: selectedUser?.id,
// //   });

// //   // CSV Export
// //   const exportCSV = () => {
// //     let csv = "Date,Clicks\n";
// //     analytics.clicks.forEach((x: any) => {
// //       csv += `${x.label},${x.count}\n`;
// //     });
// //     csv += "\nTop Links\n";
// //     analytics.topLinks.forEach((x: any) => {
// //       csv += `${x.label},${x.count}\n`;
// //     });
// //     const blob = new Blob([csv], { type: "text/csv" });
// //     const url = window.URL.createObjectURL(blob);
// //     const a = document.createElement("a");
// //     a.href = url;
// //     a.download = "analytics.csv";
// //     a.click();
// //   };

// //   return (
// //     <>
// //       <Navbar />
// //       <div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">
// //         <div className="lg:w-64 w-full">
// //           <AnalyticsSidebar />
// //         </div>
// //         <div className="flex-1 p-4 sm:p-6 lg:p-8">
// //           <div className="max-w-7xl mx-auto w-full">
// //             <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6 lg:mb-8">
// //               📊 Admin Analytics Dashboard
// //             </h1>

// //             {/* ---------- USER DROPDOWN + CSV BUTTON ---------- */}
// //             <div className="flex flex-wrap items-center mb-4 gap-4">
// //               <UserDropdown onUserSelect={setSelectedUser} />
// //               <button
// //                 onClick={exportCSV}
// //                 className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
// //               >
// //                 Export CSV
// //               </button>
// //             </div>

// //             {/* ---------- FILTERS ---------- */}
// //             <Filters
// //               setLast7Days={filters.setLast7Days}
// //               setLast30Days={filters.setLast30Days}
// //               from={filters.from}
// //               to={filters.to}
// //               setFrom={filters.setFrom}
// //               setTo={filters.setTo}
// //               selectedLink={filters.selectedLink}
// //               setSelectedLink={filters.setSelectedLink}
// //               selectedTag={filters.selectedTag}
// //               setSelectedTag={filters.setSelectedTag}
// //               allLinks={analytics.allLinks}
// //               allTags={analytics.allTags}
// //             />

// //             {/* ---------- SUMMARY ---------- */}
// //             <SummarySection
// //               totalClicks={analytics.totalClicks}
// //               country={analytics.country}
// //               device={analytics.device}
// //               browser={analytics.browser}
// //             />

// //             {/* ---------- TOP TAGS ---------- */}
// //             <TopTagsTable
// //               selectedLink={filters.selectedLink}
// //               selectedTag={filters.selectedTag}
// //               topLinks={analytics.topLinks}
// //               openTagPopup={async (tag: string) => {
// //                 const data = await analytics.openTagPopup(tag);
// //                 popup.setPopupTitle(`Links for tag: ${tag}`);
// //                 popup.setPopupData(data);
// //                 popup.setOpenPopup(true);
// //               }}
// //             />

// //             {/* ---------- CLICKS CHART ---------- */}
// //             <ClicksChart clicks={analytics.clicks} openDataPopup={popup.openDataPopup} />

// //             {/* ---------- CHARTS ---------- */}
// //             <ChartsSection
// //               referrer={analytics.referrer}
// //               country={analytics.country}
// //               device={analytics.device}
// //               os={analytics.os}
// //               browser={analytics.browser}
// //               language={analytics.language}
// //               openDataPopup={popup.openDataPopup}
// //             />

// //             {/* ---------- HEATMAP ---------- */}
// //             <div id="heatmap" className="mt-8 lg:mt-12 overflow-x-auto">
// //               <HeatmapChart />
// //             </div>
// //           </div>
// //         </div>
// //       </div>

// //       {/* ---------- POPUP ---------- */}
// //       <DataPopup
// //         openPopup={popup.openPopup}
// //         popupTitle={popup.popupTitle}
// //         popupData={popup.popupData}
// //         setOpenPopup={popup.setOpenPopup}
// //       />
// //     </>
// //   );
// // }

// import { useState, useEffect } from "react";
// import Navbar from "../components/Navbar";
// import AnalyticsSidebar from "../components/AnalyticsSidebar";
// import HeatmapChart from "../components/analysis/HeatmapChart";
// import Filters from "../components/userAnalytics/Filters";
// import SummarySection from "../components/userAnalytics/SummarySection";
// import TopTagsTable from "../components/userAnalytics/TopTagsTable";
// import ClicksChart from "../components/userAnalytics/ClicksChart";
// import ChartsSection from "../components/userAnalytics/ChartsSection";
// import DataPopup from "../components/userAnalytics/DataPopup";
// import useAnalyticsFilters from "../components/hooks/useAnalyticsFilters";
// import useAnalyticsData from "../components/hooks/useAnalyticsData";
// import useAnalyticsPopup from "../components/hooks/useAnalyticsPopup";

// // ---------------- USER DROPDOWN ----------------
// interface User {
//   id: number;
//   email: string;
//   role: "User" | "Admin";
// }

// const UserDropdown: React.FC<{ onUserSelect: (user: User | null) => void }> = ({ onUserSelect }) => {
//   const [users, setUsers] = useState<User[]>([]);
//   const [selectedUserId, setSelectedUserId] = useState<number | "">("");
//   const API_BASE = import.meta.env.VITE_API_URL;

//    useEffect(() => {
//     const fetchUsers = async () => {
//       try {
//         const token = localStorage.getItem("token"); // ✅ get token
//         if (!token) throw new Error("No token found, please login.");

//         const res = await fetch(`${API_BASE}/api/admin/all-users`, {
//           headers: {
//             "Content-Type": "application/json",
//             "Authorization": `Bearer ${token}`, // ✅ add token here
//           },
//         });

//         if (!res.ok) throw new Error("Failed to fetch users");

//         const data: User[] = await res.json();
//         setUsers(data.filter(u => u.role.toLowerCase() === "user"));
//       } catch (err) {
//         console.error("Error fetching users:", err);
//       }
//     };
//     fetchUsers();
//   }, []);


//   const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
//     const id = e.target.value ? parseInt(e.target.value) : "";
//     setSelectedUserId(id);
//     const user = users.find(u => u.id === id) || null;
//     onUserSelect(user);
//   };

//   return (
//     <div className="mb-4 w-64 min-w-[200px]">
//       <label className="block mb-1 font-medium">Select User</label>
//       <select
//         value={selectedUserId}
//         onChange={handleChange}
//         className="border border-gray-300 rounded px-3 py-2 w-full"
//       >
//         <option value="">All / Default</option>
//         {users.map(user => (
//           <option key={user.id} value={user.id}>
//             {user.email}
//           </option>
//         ))}
//       </select>
//     </div>
//   );
// };

// // ---------------- MAIN PAGE ----------------
// export default function AdminAnalytics() {
//   const filters = useAnalyticsFilters();
//   const popup = useAnalyticsPopup();
//   const [selectedUser, setSelectedUser] = useState<User | null>(null);

//   const analytics = useAnalyticsData({
//     from: filters.from,
//     to: filters.to,
//     selectedLink: filters.selectedLink,
//     selectedTag: filters.selectedTag,
//     userId: selectedUser?.id, // per-user analytics
//   });

//   const exportCSV = () => {
//     let csv = "Date,Clicks\n";
//     analytics.clicks.forEach((x: any) => {
//       csv += `${x.label},${x.count}\n`;
//     });
//     csv += "\nTop Links\n";
//     analytics.topLinks.forEach((x: any) => {
//       csv += `${x.label},${x.count}\n`;
//     });
//     const blob = new Blob([csv], { type: "text/csv" });
//     const url = window.URL.createObjectURL(blob);
//     const a = document.createElement("a");
//     a.href = url;
//     a.download = "analytics.csv";
//     a.click();
//   };

//   return (
//     <>
//       <Navbar />
//       <div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">
//         <div className="lg:w-64 w-full">
//           <AnalyticsSidebar />
//         </div>
//         <div className="flex-1 p-4 sm:p-6 lg:p-8">
//           <div className="max-w-7xl mx-auto w-full">
//             <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6 lg:mb-8">
//               📊 Admin Analytics Dashboard
//             </h1>

//             {/* ---------- USER DROPDOWN + CSV BUTTON ---------- */}
//             <div className="flex flex-wrap items-center mb-4 gap-4">
//               <UserDropdown onUserSelect={setSelectedUser} />
//               <button
//                 onClick={exportCSV}
//                 className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
//               >
//                 Export CSV
//               </button>
//             </div>

//             {/* ---------- FILTERS ---------- */}
//             <Filters
//               setLast7Days={filters.setLast7Days}
//               setLast30Days={filters.setLast30Days}
//               from={filters.from}
//               to={filters.to}
//               setFrom={filters.setFrom}
//               setTo={filters.setTo}
//               selectedLink={filters.selectedLink}
//               setSelectedLink={filters.setSelectedLink}
//               selectedTag={filters.selectedTag}
//               setSelectedTag={filters.setSelectedTag}
//               allLinks={analytics.allLinks}
//               allTags={analytics.allTags}
//             />

//             {/* ---------- SUMMARY ---------- */}
//             <SummarySection
//               totalClicks={analytics.totalClicks}
//               country={analytics.country}
//               device={analytics.device}
//               browser={analytics.browser}
//             />

//             {/* ---------- TOP TAGS ---------- */}
//             <TopTagsTable
//               selectedLink={filters.selectedLink}
//               selectedTag={filters.selectedTag}
//               topLinks={analytics.topLinks}
//               openTagPopup={async (tag: string) => {
//                 const data = await analytics.openTagPopup(tag);
//                 popup.setPopupTitle(`Links for tag: ${tag}`);
//                 popup.setPopupData(data);
//                 popup.setOpenPopup(true);
//               }}
//             />

//             {/* ---------- CLICKS CHART ---------- */}
//             <ClicksChart clicks={analytics.clicks} openDataPopup={popup.openDataPopup} />

//             {/* ---------- CHARTS ---------- */}
//             <ChartsSection
//               referrer={analytics.referrer}
//               country={analytics.country}
//               device={analytics.device}
//               os={analytics.os}
//               browser={analytics.browser}
//               language={analytics.language}
//               openDataPopup={popup.openDataPopup}
//             />

//             {/* ---------- HEATMAP ---------- */}
//             <div id="heatmap" className="mt-8 lg:mt-12 overflow-x-auto">
//               <HeatmapChart data={analytics.heatmap} />
//             </div>
//           </div>
//         </div>
//       </div>

//       {/* ---------- POPUP ---------- */}
//       <DataPopup
//         openPopup={popup.openPopup}
//         popupTitle={popup.popupTitle}
//         popupData={popup.popupData}
//         setOpenPopup={popup.setOpenPopup}
//       />
//     </>
//   );
// }


// import { useState, useEffect } from "react";
// import Navbar from "../components/Navbar";
// import AnalyticsSidebar from "../components/AnalyticsSidebar";
// import HeatmapChart from "../components/analysis/HeatmapChart";
// import Filters from "../components/userAnalytics/Filters";
// import SummarySection from "../components/userAnalytics/SummarySection";
// import TopTagsTable from "../components/userAnalytics/TopTagsTable";
// import ClicksChart from "../components/userAnalytics/ClicksChart";
// import ChartsSection from "../components/userAnalytics/ChartsSection";
// import DataPopup from "../components/userAnalytics/DataPopup";

// import useAnalyticsFilters from "../components/hooks/useAnalyticsFilters";
// import useAnalyticsData from "../components/hooks/useAnalyticsData";
// import useAnalyticsPopup from "../components/hooks/useAnalyticsPopup";

// import { getUserRole } from "../utils/auth";

// // ---------------- USER DROPDOWN ----------------
// interface User {
//   id: number;
//   email: string;
//   role: "User" | "Admin";
// }

// const UserDropdown = ({ onUserSelect }: any) => {
//   const [users, setUsers] = useState<User[]>([]);
//   const [selectedUserId, setSelectedUserId] = useState<number | "">("");
//   const [loading, setLoading] = useState(false);

//   const API_BASE = import.meta.env.VITE_API_URL;

//   useEffect(() => {
//     const fetchUsers = async () => {
//       try {
//         setLoading(true);

//         const token = localStorage.getItem("token");

//         const res = await fetch(`${API_BASE}/api/admin/all-users`, {
//           headers: {
//             Authorization: `Bearer ${token}`,
//           },
//         });

//         if (!res.ok) {
//           console.error("Failed to fetch users");
//           return;
//         }

//         const data = await res.json();
//         setUsers(data); // ✅ ALL users
//       } catch (err) {
//         console.error(err);
//       } finally {
//         setLoading(false);
//       }
//     };

//     fetchUsers();
//   }, []);

//   const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
//     const id = e.target.value ? parseInt(e.target.value) : "";
//     setSelectedUserId(id);

//     const user = users.find((u) => u.id === id) || null;
//     onUserSelect(user);
//   };

//   return (
//     <div className="mb-4">
//       <label className="block mb-1 font-medium">Select User</label>

//       <select
//         value={selectedUserId}
//         onChange={handleChange}
//         className="border p-2 rounded-lg w-64"
//       >
//         <option value="">All Users</option>

//         {loading && <option>Loading...</option>}

//         {!loading &&
//           users.map((u) => (
//             <option key={u.id} value={u.id}>
//               {u.email} ({u.role})
//             </option>
//           ))}
//       </select>
//     </div>
//   );
// };

// // ---------------- MAIN PAGE ----------------
// export default function AnalysisPage() {
//   const role = getUserRole();
//   const isAdmin = role?.toLowerCase() === "admin";

//   const filters = useAnalyticsFilters();
//   const popup = useAnalyticsPopup();

//   const [selectedUser, setSelectedUser] = useState<User | null>(null);

//   const analytics = useAnalyticsData({
//     from: filters.from,
//     to: filters.to,
//     selectedLink: filters.selectedLink,
//     selectedTag: filters.selectedTag,
//     userId: isAdmin ? selectedUser?.id : undefined,
//   });

//   const exportCSV = () => {
//     let csv = "Date,Clicks\n";

//     analytics.clicks.forEach((x: any) => {
//       csv += `${x.label},${x.count}\n`;
//     });

//     csv += "\nTop Links\n";

//     analytics.topLinks.forEach((x: any) => {
//       csv += `${x.label},${x.count}\n`;
//     });

//     const blob = new Blob([csv], { type: "text/csv" });
//     const url = window.URL.createObjectURL(blob);

//     const a = document.createElement("a");
//     a.href = url;
//     a.download = "analytics.csv";
//     a.click();
//   };

//   return (
//     <>
//       <Navbar />

//       <div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">
//         <div className="lg:w-64 w-full">
//           <AnalyticsSidebar />
//         </div>

//         <div className="flex-1 p-4 sm:p-6 lg:p-8">
//           <div className="max-w-7xl mx-auto w-full">

//             <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6">
//               📊 {isAdmin ? "Admin" : "My"} Analytics Dashboard
//             </h1>

//             {/* ADMIN ONLY */}
//             {isAdmin && (
//               <UserDropdown onUserSelect={setSelectedUser} />
//             )}

//             {/* FILTERS */}
//             <Filters
//               {...filters}
//               allLinks={analytics.allLinks}
//               allTags={analytics.allTags}
//               exportCSV={exportCSV}
//             />

//             {/* SUMMARY */}
//             <SummarySection
//               totalClicks={analytics.totalClicks}
//               country={analytics.country}
//               device={analytics.device}
//               browser={analytics.browser}
//             />

//             {/* TOP TAGS */}
//             <TopTagsTable
//               selectedLink={filters.selectedLink}
//               selectedTag={filters.selectedTag}
//               topLinks={analytics.topLinks}
//               openTagPopup={async (tag: string) => {
//                 const data = await analytics.openTagPopup(tag);
//                 popup.setPopupTitle(`Links for tag: ${tag}`);
//                 popup.setPopupData(data);
//                 popup.setOpenPopup(true);
//               }}
//             />

//             {/* CLICKS */}
//             <ClicksChart
//               clicks={analytics.clicks}
//               openDataPopup={popup.openDataPopup}
//             />

//             {/* CHARTS */}
//             <ChartsSection
//               referrer={analytics.referrer}
//               country={analytics.country}
//               device={analytics.device}
//               os={analytics.os}
//               browser={analytics.browser}
//               language={analytics.language}
//               openDataPopup={popup.openDataPopup}
//             />

//             {/* HEATMAP */}
//             <div className="mt-8">
//               <HeatmapChart data={analytics.heatmap} />
//             </div>

//           </div>
//         </div>
//       </div>

//       {/* POPUP */}
//       <DataPopup
//         openPopup={popup.openPopup}
//         popupTitle={popup.popupTitle}
//         popupData={popup.popupData}
//         setOpenPopup={popup.setOpenPopup}
//       />
//     </>
//   );
// }

import { useState, useEffect } from "react";
import Navbar from "../components/Navbar";
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
        const token = localStorage.getItem("token"); // ✅ get token
        if (!token) throw new Error("No token found, please login.");

        const res = await fetch(`${API_BASE}/api/admin/all-users`, {
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`, // ✅ add token here
          },
        });

        if (!res.ok) throw new Error("Failed to fetch users");

        const data: User[] = await res.json();
        setUsers(data.filter(u => u.role.toLowerCase() === "user"));
      } catch (err) {
        console.error("Error fetching users:", err);
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
      <label className="block mb-1 font-medium">Select User</label>
      <select
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
export default function AdminAnalytics() {
  const filters = useAnalyticsFilters();
  const popup = useAnalyticsPopup();
  const [selectedUser, setSelectedUser] = useState<User | null>(null);

  const analytics = useAnalyticsData({
    from: filters.from,
    to: filters.to,
    selectedLink: filters.selectedLink,
    selectedTag: filters.selectedTag,
    userId: selectedUser?.id, // per-user analytics
  });

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
      <Navbar />
      <div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">
        <div className="lg:w-64 w-full">
          <AnalyticsSidebar />
        </div>
        <div className="flex-1 p-4 sm:p-6 lg:p-8">
          <div className="max-w-7xl mx-auto w-full">
            <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6 lg:mb-8">
              📊 Admin Analytics Dashboard
            </h1>

            {/* ---------- USER DROPDOWN + CSV BUTTON ---------- */}
            <div className="flex flex-wrap items-center mb-4 gap-4">
              <UserDropdown onUserSelect={setSelectedUser} />
              {/* <button
                onClick={exportCSV}
                className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
              >
                Export CSV
              </button> */}
            </div>

            {/* ---------- FILTERS ---------- */}
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
            />

            {/* ---------- SUMMARY ---------- */}
            <SummarySection
              totalClicks={analytics.totalClicks}
              country={analytics.country}
              device={analytics.device}
              browser={analytics.browser}
            />

            {/* ---------- TOP TAGS ---------- */}
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

            {/* ---------- CLICKS CHART ---------- */}
            <ClicksChart clicks={analytics.clicks} openDataPopup={popup.openDataPopup} />

            {/* ---------- CHARTS ---------- */}
            <ChartsSection
              referrer={analytics.referrer}
              country={analytics.country}
              device={analytics.device}
              os={analytics.os}
              browser={analytics.browser}
              language={analytics.language}
              openDataPopup={popup.openDataPopup}
            />

            {/* ---------- HEATMAP ---------- */}
            <div id="heatmap" className="mt-8 lg:mt-12 overflow-x-auto">
              <HeatmapChart data={analytics.heatmap} />
            </div>
          </div>
        </div>
      </div>

      {/* ---------- POPUP ---------- */}
      <DataPopup
        openPopup={popup.openPopup}
        popupTitle={popup.popupTitle}
        popupData={popup.popupData}
        setOpenPopup={popup.setOpenPopup}
      />
    </>
  );
} 