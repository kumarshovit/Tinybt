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
<p className="text-sm text-gray-500 mb-4">
💡 Tip: Click on any chart or card to view detailed data
</p>
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
  totalClicks={analytics.clicks}   
  totalLinks={analytics.linkClicks} 
  country={analytics.country}
  device={analytics.device}
  browser={analytics.browser}
  openDataPopup={popup.openDataPopup}
  openAllLinksPopup={analytics.openAllLinksPopup}
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