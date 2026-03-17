// import { useEffect, useState } from "react";
// import { getDashboardOverview } from "../api/dashboardService";
// import type { DashboardOverview } from "../api/dashboardService";

// import AnalysisSidebar from "../components/analysis/AnalysisSidebar";
// import ClicksByGeography from "../components/analysis/ClicksByGeography";
// import ClicksByLanguage from "../components/analysis/ClicksByLanguage";
// import PopularDaysTimes from "../components/analysis/PopularDaysTimes";
// import ClicksByDevice from "../components/analysis/ClicksByDevice";
// import ClicksByOS from "../components/analysis/ClicksByOS";
// import ClicksByBrowser from "../components/analysis/ClicksByBrowser";
// import Navbar from "../components/Navbar";
// import UsersOverTime from "../components/analysis/UsersOverTime";

// const AnalysisPage = () => {

//   const today = new Date();
//   const last7Days = new Date();
//   last7Days.setDate(today.getDate() - 7);

//   const [data, setData] = useState<DashboardOverview | null>(null);
//   const [loading, setLoading] = useState(true);

//   const [startDate, setStartDate] = useState(
//     last7Days.toISOString().split("T")[0]
//   );

//   const [endDate, setEndDate] = useState(
//     today.toISOString().split("T")[0]
//   );

//   const [active, setActive] = useState("time");

//   const loadDashboard = async () => {
//     try {
//       setLoading(true);

//       const result = await getDashboardOverview(startDate, endDate);
//       setData(result);

//     } catch (error) {
//       console.error("Failed to load dashboard", error);
//     } finally {
//       setLoading(false);
//     }
//   };

//   useEffect(() => {
//     loadDashboard();
//   }, [startDate, endDate]);

//   return (
//     <div className="min-h-screen bg-gray-50 flex flex-col">

//       <Navbar />

//       <div className="flex flex-col lg:flex-row flex-1">

//         <AnalysisSidebar active={active} setActive={setActive} />

//         <div className="flex-1 p-4 sm:p-6 lg:p-8">

//           <h1 className="text-xl sm:text-2xl font-bold mb-6 ">
//             Link Performance Overview
//           </h1>

//           {/* Date Filters */}

//           <div className="flex flex-col sm:flex-row gap-3 mb-6">

//             <input
//               type="date"
//               value={startDate}
//               onChange={(e) => setStartDate(e.target.value)}
//               className="border p-2 rounded w-full sm:w-auto"
//             />

//             <input
//               type="date"
//               value={endDate}
//               onChange={(e) => setEndDate(e.target.value)}
//               className="border p-2 rounded w-full sm:w-auto"
//             />

//             <button
//               onClick={loadDashboard}
//               className="bg-blue-600 text-white px-4 py-2 rounded w-full sm:w-auto hover:bg-blue-700 transition"
//             >
//               Apply
//             </button>

//           </div>

//           {/* Loading Skeleton */}

//           {loading && (
//             <>
//               <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
//                 {[1, 2, 3, 4].map((i) => (
//                   <div
//                     key={i}
//                     className="bg-gray-200 h-24 rounded animate-pulse"
//                   />
//                 ))}
//               </div>

//               <div className="bg-gray-200 h-96 rounded animate-pulse"></div>
//             </>
//           )}

//           {/* Dashboard Data */}

//           {!loading && data && (
//             <>

//               {/* Summary Cards */}

//               <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">

//                 <div className="bg-white shadow rounded p-6">
//                   <h2 className="text-gray-500 text-sm">Total Clicks</h2>
//                   <p className="text-2xl font-bold">{data.totalClicks}</p>
//                 </div>

//                 <div className="bg-white shadow rounded p-6">
//                   <h2 className="text-gray-500 text-sm">Total Links</h2>
//                   <p className="text-2xl font-bold">{data.totalUrls}</p>
//                 </div>

//                 <div className="bg-white shadow rounded p-6">
//                   <h2 className="text-gray-500 text-sm">Active Links</h2>
//                   <p className="text-2xl font-bold">{data.activeLinks}</p>
//                 </div>

//                 <div className="bg-white shadow rounded p-6">
//                   <h2 className="text-gray-500 text-sm">Expired Links</h2>
//                   <p className="text-2xl font-bold">{data.expiredLinks}</p>
//                 </div>

//               </div>

//               {/* Analytics Section */}

//               <div id="chart-section" className="bg-white rounded shadow p-4 sm:p-6">

//                 {active === "time" && (
//                   <UsersOverTime startDate={startDate} endDate={endDate} />
//                 )}

//                 {active === "geo" && (
//                   <ClicksByGeography startDate={startDate} endDate={endDate} />
//                 )}

//                 {active === "lang" && (
//                   <ClicksByLanguage startDate={startDate} endDate={endDate} />
//                 )}

//                 {active === "popular" && (
//                   <PopularDaysTimes startDate={startDate} endDate={endDate} />
//                 )}

//                 {active === "device" && (
//                   <ClicksByDevice startDate={startDate} endDate={endDate} />
//                 )}

//                 {active === "os" && (
//                   <ClicksByOS startDate={startDate} endDate={endDate} />
//                 )}

//                 {active === "browser" && (
//                   <ClicksByBrowser startDate={startDate} endDate={endDate} />
//                 )}

//               </div>

//             </>
//           )}

//         </div>

//       </div>

//     </div>
//   );
// };

// export default AnalysisPage;

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

export default function MyAnalytics(){

/* ---------- HOOKS ---------- */

const filters = useAnalyticsFilters();

const analytics = useAnalyticsData({
from:filters.from,
to:filters.to,
selectedLink:filters.selectedLink,
selectedTag:filters.selectedTag
});

const popup = useAnalyticsPopup();

/* ---------- CSV ---------- */

const exportCSV = ()=>{

let csv = "Date,Clicks\n";

analytics.clicks.forEach((x:any)=>{
csv += `${x.label},${x.count}\n`;
});

csv += "\nTop Links\n";

analytics.topLinks.forEach((x:any)=>{
csv += `${x.label},${x.count}\n`;
});

const blob = new Blob([csv],{type:"text/csv"});
const url = window.URL.createObjectURL(blob);

const a = document.createElement("a");
a.href = url;
a.download = "analytics.csv";
a.click();

};

return(

<>
<Navbar/>
<div className="flex flex-col lg:flex-row bg-gray-100 min-h-screen">
<div className="lg:w-64 w-full">
<AnalyticsSidebar/>
</div>
<div className="flex-1 p-4 sm:p-6 lg:p-8">
<div className="max-w-7xl mx-auto w-full">
<h1 className="text-xl sm:text-2xl lg:text-3xl font-bold mb-6 lg:mb-8">
📊 My Analytics Dashboard
</h1>

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
exportCSV={exportCSV}
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
openTagPopup={async(tag:string)=>{

const data = await analytics.openTagPopup(tag)

popup.setPopupTitle(`Links for tag: ${tag}`)
popup.setPopupData(data)
popup.setOpenPopup(true)

}}
/>

{/* ---------- CLICKS CHART ---------- */}

<ClicksChart
clicks={analytics.clicks}
openDataPopup={popup.openDataPopup}
/>

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
<HeatmapChart/>
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