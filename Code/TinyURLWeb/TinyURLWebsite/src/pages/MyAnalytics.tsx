import { useState, useEffect } from "react";
import Navbar from "../components/Navbar";
import AnalyticsSidebar from "../components/AnalyticsSidebar";
import HeatmapChart from "../components/analysis/HeatmapChart";

import {
LineChart,
Line,
XAxis,
YAxis,
Tooltip,
ResponsiveContainer,
PieChart,
Pie,
Cell,
BarChart,
Bar,
Legend
} from "recharts";

import CountUp from "react-countup";

const API_BASE = import.meta.env.VITE_API_URL;

const COLORS = [
"#7c3aed",
"#22c55e",
"#f97316",
"#06b6d4",
"#ef4444"
];

export default function MyAnalytics(){

/* ---------- DEFAULT LAST 7 DAYS ---------- */

const today = new Date();
const lastWeek = new Date();
lastWeek.setDate(today.getDate()-7);

const [from,setFrom] = useState(lastWeek.toISOString().split("T")[0]);
const [to,setTo] = useState(today.toISOString().split("T")[0]);

/* ---------- FILTER STATES ---------- */

const [allLinks,setAllLinks] = useState<any[]>([]);
const [selectedLink,setSelectedLink] = useState("");

const [allTags,setAllTags] = useState<string[]>([]);
const [selectedTag,setSelectedTag] = useState("");

/* ---------- DATA STATES ---------- */

const [clicks,setClicks] = useState<any[]>([]);
const [referrer,setReferrer] = useState<any[]>([]);
const [country,setCountry] = useState<any[]>([]);
const [device,setDevice] = useState<any[]>([]);
const [os,setOs] = useState<any[]>([]);
const [browser,setBrowser] = useState<any[]>([]);
const [topLinks,setTopLinks] = useState<any[]>([]);
const [language,setLanguage] = useState<any[]>([]);
const [totalClicks,setTotalClicks] = useState(0);

/* ---------- POPUP STATES ---------- */

const [openPopup,setOpenPopup] = useState(false);
const [popupTitle,setPopupTitle] = useState("");
const [popupData,setPopupData] = useState<any[]>([]);

const token = localStorage.getItem("token");


// ---------- QUICK DATE FILTERS ----------

const setToday = () => {
  const today = new Date().toISOString().split("T")[0];
  setFrom(today);
  setTo(today);
};

const setLast7Days = () => {
  const today = new Date();
  const lastWeek = new Date();
  lastWeek.setDate(today.getDate() - 7);

  setFrom(lastWeek.toISOString().split("T")[0]);
  setTo(today.toISOString().split("T")[0]);
};

const setLast30Days = () => {
  const today = new Date();
  const lastMonth = new Date();
  lastMonth.setDate(today.getDate() - 30);

  setFrom(lastMonth.toISOString().split("T")[0]);
  setTo(today.toISOString().split("T")[0]);
};
const headers = {
"Content-Type":"application/json",
Authorization:`Bearer ${token}`
};

/* ---------- FETCH USER LINKS (for filters + tags) ---------- */

const fetchUserLinks = async()=>{

const res = await fetch(`${API_BASE}/api/urls`,{
headers
});

const data = await res.json();

setAllLinks(data);

/* extract tags */

const tagSet = new Set<string>();

data.forEach((link:any)=>{
if(link.tags){
link.tags.forEach((tag:string)=>{
tagSet.add(tag);
});
}
});

setAllTags(Array.from(tagSet));

};

/* ---------- FETCH DASHBOARD ---------- */

const fetchDashboard = async()=>{

const body = {
from,
to,
link:selectedLink,
tag:selectedTag
};

try{

/* clicks over time */

const clicksRes = await fetch(
`${API_BASE}/analytics/user/clicks-over-time`,
{
method:"POST",
headers,
body:JSON.stringify(body)
}
);

const clicksData = await clicksRes.json();

setClicks(clicksData);

let total = 0;

clicksData.forEach((x:any)=>{
total += x.count;
});

setTotalClicks(total);

/* breakdown */

const breakdown = async(type:string)=>{

const res = await fetch(
`${API_BASE}/analytics/breakdown`,
{
method:"POST",
headers,
body:JSON.stringify({
from,
to,
type,
link:selectedLink,
tag:selectedTag
})
}
);

return res.json();

};

setReferrer(await breakdown("referrer"));
setCountry(await breakdown("country"));
setDevice(await breakdown("device"));
setOs(await breakdown("os"));
setBrowser(await breakdown("browser"));

/* language */

const langRes = await fetch(
`${API_BASE}/analytics/user/device-language`,
{
method:"POST",
headers,
body:JSON.stringify(body)
}
);

setLanguage(await langRes.json());

/* top links */

const linksRes = await fetch(
`${API_BASE}/analytics/user/popular-links`,
{
method:"POST",
headers,
body:JSON.stringify(body)
}
);

const linksData = await linksRes.json();

// alias -> clicks map
const aliasClicks: Record<string, number> = {};
linksData.forEach((x:any)=>{
  aliasClicks[x.label] = x.count;
});

// tag -> clicks
const tagCounts: Record<string, number> = {};

allLinks.forEach((link:any)=>{
  const clicks = aliasClicks[link.shortCode] || 0;

  if(link.tags){
    link.tags.forEach((tag:string)=>{
      if(!tagCounts[tag]) tagCounts[tag] = 0;
      tagCounts[tag] += clicks;
    });
  }
});

const tagData = Object.keys(tagCounts).map(tag=>({
  label: tag,
  count: tagCounts[tag]
}))
.sort((a,b)=>b.count-a.count);

setTopLinks(tagData);


}
catch(err){
console.error("Analytics error",err);
}

};

/* ---------- LOAD DEFAULT ---------- */

useEffect(()=>{
fetchUserLinks();
},[]);

useEffect(()=>{
if(allLinks.length){
fetchDashboard();
}
},[from,to,selectedLink,selectedTag,allLinks]);

/* ---------- POPUP ---------- */

const openDataPopup = (title:string,data:any[])=>{
setPopupTitle(title);
setPopupData(data);
setOpenPopup(true);
};

/* ---------- CSV ---------- */

const exportCSV = ()=>{

let csv = "Date,Clicks\n";

clicks.forEach((x:any)=>{
csv += `${x.label},${x.count}\n`;
});

csv += "\nTop Links\n";

topLinks.forEach((x:any)=>{
csv += `${x.label},${x.count}\n`;
});

const blob = new Blob([csv],{type:"text/csv"});
const url = window.URL.createObjectURL(blob);

const a = document.createElement("a");
a.href = url;
a.download = "analytics.csv";
a.click();

};
const openTagPopup = async(tag)=>{

const body = {
  from,
  to,
  tag
};

const res = await fetch(
`${API_BASE}/analytics/user/popular-links`,
{
  method:"POST",
  headers,
  body:JSON.stringify(body)
}
);

const data = await res.json();

const popupLinks = data.map((x:any)=>({
  shortUrl: `${API_BASE}/${x.label}`,
  shortCode: x.label,
  clickCount: x.count
}));

setPopupTitle(`Links for tag: ${tag}`);
setPopupData(popupLinks);
setOpenPopup(true);

};
return(

<>
<Navbar/>

<div className="flex bg-gray-100 min-h-screen">

<AnalyticsSidebar/>

<div className="flex-1 p-8">

<div className="max-w-7xl mx-auto">

<h1 className="text-3xl font-bold mb-8">
📊 My Analytics Dashboard
</h1>

{/* ---------- FILTERS ---------- */}

<div className="bg-white rounded-xl shadow p-6 mb-10 flex flex-wrap gap-4 items-center">
{/* Quick Date Buttons */}

<button
onClick={setLast7Days}
className="bg-purple-100 text-purple-700 px-4 py-2 rounded-lg hover:bg-purple-200"
>
Last 7 Days
</button>

<button
onClick={setLast30Days}
className="bg-purple-100 text-purple-700 px-4 py-2 rounded-lg hover:bg-purple-200"
>
Last 30 Days
</button>
<input
type="date"
value={from}
onChange={e=>setFrom(e.target.value)}
className="border p-2 rounded-lg"
/>

<input
type="date"
value={to}
onChange={e=>setTo(e.target.value)}
className="border p-2 rounded-lg"
/>

<select
value={selectedLink}
onChange={(e)=>{setSelectedLink(e.target.value); setSelectedTag("");}}
className="border p-2 rounded-lg"
>
<option value="">All Alias</option>

{allLinks.map((link:any)=>(
<option key={link.id} value={link.shortCode}>
{link.shortCode}
</option>
))}

</select>

<select
value={selectedTag}
onChange={(e)=>{setSelectedTag(e.target.value);setSelectedLink("");
}}
className="border p-2 rounded-lg"
>

<option value="">All Tags</option>

{allTags.map((tag)=>(
<option key={tag} value={tag}>
{tag}
</option>
))}

</select>

<button
onClick={exportCSV}
className="bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded-lg"
>
Export CSV
</button>

</div>

{/* ---------- SUMMARY ---------- */}

<div className="grid grid-cols-2 md:grid-cols-4 gap-6 mb-10">

<SummaryCard title="Total Clicks 📈" value={totalClicks}/>
<SummaryCard title="Countries 🌍" value={country.length}/>
<SummaryCard title="Devices 💻" value={device.length}/>
<SummaryCard title="Browsers 🌐" value={browser.length}/>

</div>

{/* ---------- TOP LINKS ---------- */}

<div id="links" className="bg-white rounded-xl shadow p-6 mb-10">

<h2 className="text-xl font-semibold mb-4">
{selectedLink
  ? `Tags for alias: ${selectedLink}`
  : selectedTag
  ? `Analytics for tag: ${selectedTag}`
  : "Top Performing Tags"}
</h2>

<table className="w-full text-left">

<thead>
<tr className="border-b text-gray-500">
<th className="py-2">Tag</th>
<th className="py-2">Clicks</th>
</tr>
</thead>

<tbody>

{topLinks.slice(0,5).map((tag:any,i:number)=>(

<tr
key={i}
className="border-b hover:bg-gray-50 cursor-pointer"
onClick={()=>openTagPopup(tag.label)}
>

<td className="py-2 text-purple-600 font-medium">
{tag.label}
</td>

<td className="py-2 font-semibold">
{tag.count}
</td>

</tr>

))}

</tbody>

</table>

</div>

{/* ---------- CLICKS OVER TIME ---------- */}

<div
id="clicks"
onClick={()=>openDataPopup("Clicks Over Time",clicks)}
className="bg-white rounded-xl shadow p-6 mb-10 cursor-pointer"
>

<h2 className="text-xl font-semibold mb-4">
Clicks Over Time
</h2>

<ResponsiveContainer width="100%" height={320}>

<LineChart data={clicks}>

<XAxis dataKey="label"/>
<YAxis/>

<Tooltip formatter={(v)=>`${v} clicks`} />

<Legend/>

<Line
type="monotone"
dataKey="count"
stroke="#7c3aed"
strokeWidth={3}
/>

</LineChart>

</ResponsiveContainer>

</div>

{/* ---------- CHARTS ---------- */}

<div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6 mb-10 border-none">

<div id="traffic" onClick={()=>openDataPopup("Traffic Source",referrer)}>
<ChartPie title="Traffic Source" data={referrer}/>
</div>

<div id="country" onClick={()=>openDataPopup("Country Distribution",country)}>
<ChartPie title="Country Distribution" data={country}/>
</div>

<div id="device" onClick={()=>openDataPopup("Device Types",device)}>
<ChartBar title="Device Types" data={device}/>
</div>

<div id="os" onClick={()=>openDataPopup("Operating Systems",os)}>
<ChartBar title="Operating Systems" data={os}/>
</div>

<div id="browser" onClick={()=>openDataPopup("Browsers",browser)}>
<ChartBar title="Browsers" data={browser}/>
</div>

<div id="language" onClick={()=>openDataPopup("Device Language",language)}>
<ChartPie title="Device Language" data={language}/>
</div>

</div>

<div id="heatmap" className="mt-12">
<HeatmapChart/>
</div>

</div>

</div>

</div>

{/* ---------- DATA POPUP ---------- */}

{openPopup &&(

<div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">

<div className="bg-white rounded-xl p-8 w-[600px]">

<h2 className="text-xl font-bold mb-6">
{popupTitle}
</h2>

<table className="w-full text-left">

<thead>
<tr className="border-b text-gray-500">

{popupData[0]?.shortCode ? (
<>
<th className="py-2">Short URL</th>
<th className="py-2">Short Alias</th>
<th className="py-2">Clicks</th>
</>
) : (
<>
<th className="py-2">Label</th>
<th className="py-2">Clicks</th>
</>
)}

</tr>
</thead>

<tbody>
{popupData.map((item:any,i:number)=>{

// CASE 1 → link popup
if(item.shortCode){
return(
<tr key={i} className="border-b">

<td className="py-2 text-purple-600">
<a
href={item.shortUrl}
target="_blank"
rel="noopener noreferrer"
className="underline"
>
{item.shortUrl}
</a>
</td>

<td className="py-2">
{item.shortCode}
</td>

<td className="py-2 font-semibold">
{item.clickCount}
</td>

</tr>
)
}

// CASE 2 → analytics popup
return(
<tr key={i} className="border-b">

<td className="py-2">
{item.label}
</td>

<td className="py-2 font-semibold">
{item.count}
</td>

<td></td>

</tr>
)

})}
</tbody>

</table>

<button
onClick={()=>setOpenPopup(false)}
className="mt-6 bg-purple-600 text-white px-4 py-2 rounded"
>
Close
</button>

</div>

</div>

)}

</>

);

}

/* ---------- COMPONENTS ---------- */

function SummaryCard({title,value}:any){

return(

<div className="bg-white p-6 rounded-xl shadow hover:shadow-lg transition">

<p className="text-gray-500 text-sm mb-2">
{title}
</p>

<h2 className="text-3xl font-bold text-purple-600">
<CountUp end={value} duration={1.5}/>
</h2>

</div>

);

}

function ChartPie({title,data}:any){

if(!data.length)
return(
<div className="bg-white rounded-xl shadow p-6 text-center text-gray-400">
No data available
</div>
);

return(

<div className="bg-white rounded-xl shadow p-6 border-0 overflow-hidden">

<h3 className="text-lg font-semibold mb-4">
{title}
</h3>

<ResponsiveContainer width="100%" height={250} style={{outline:"none"}}>

<PieChart style={{border:"none", outline:"none"}}>

<Tooltip formatter={(v)=>`${v} clicks`} />

<Legend/>

<Pie
data={data}
dataKey="count"
nameKey="label"
outerRadius={80}
stroke="none"
strokeWidth={0}
innerRadius={0}
paddingAngle={0}
stroke="none"
isAnimationActive={false}
>

{data.map((_:any,index:number)=>(

<Cell
key={index}
fill={COLORS[index % COLORS.length]}
stroke="none"
/>

))}

</Pie>

</PieChart>

</ResponsiveContainer>

</div>

);

}

function ChartBar({title,data}:any){

if(!data.length)
return(
<div className="bg-white rounded-xl shadow p-6 text-center text-gray-400">
No data available
</div>
);

return(

<div className="bg-white rounded-xl shadow p-6 border-0">

<h3 className="text-lg font-semibold mb-4">
{title}
</h3>

<ResponsiveContainer width="100%" height={250}>

<BarChart data={data}>

<XAxis dataKey="label" axisLine={false} tickLine={false}/>
<YAxis axisLine={false} tickLine={false}/>

<Tooltip formatter={(v)=>`${v} clicks`} />

<Legend/>

<Bar
dataKey="count"
fill="#7c3aed"
radius={[4,4,0,0]}
/>

</BarChart>

</ResponsiveContainer>

</div>

);

}