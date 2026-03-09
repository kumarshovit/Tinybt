import { useState } from "react";
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

const [from,setFrom] = useState("");
const [to,setTo] = useState("");

const [clicks,setClicks] = useState<any[]>([]);
const [referrer,setReferrer] = useState<any[]>([]);
const [country,setCountry] = useState<any[]>([]);
const [device,setDevice] = useState<any[]>([]);
const [os,setOs] = useState<any[]>([]);
const [browser,setBrowser] = useState<any[]>([]);
const [topLinks,setTopLinks] = useState<any[]>([]);

const [totalClicks,setTotalClicks] = useState(0);

const token = localStorage.getItem("token");

const headers = {
"Content-Type":"application/json",
Authorization:`Bearer ${token}`
};

const fetchDashboard = async()=>{

const body = { from,to };

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

const breakdown = async(type:string)=>{

const res = await fetch(
`${API_BASE}/analytics/breakdown`,
{
method:"POST",
headers,
body:JSON.stringify({
from,
to,
type
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

const linksRes = await fetch(
`${API_BASE}/analytics/user/popular-links`,
{
method:"POST",
headers,
body:JSON.stringify(body)
}
);

setTopLinks(await linksRes.json());

};

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

{/* Filters */}

<div className="bg-white rounded-xl shadow p-6 mb-10 flex flex-wrap gap-4 items-center">

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

<button
onClick={fetchDashboard}
className="bg-purple-600 hover:bg-purple-700 text-white px-6 py-2 rounded-lg"
>
Apply
</button>

<button
onClick={exportCSV}
className="bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded-lg"
>
Export CSV
</button>

</div>

{/* Summary Cards */}

<div className="grid grid-cols-2 md:grid-cols-4 gap-6 mb-10">

<SummaryCard title="Total Clicks 📈" value={totalClicks}/>
<SummaryCard title="Countries 🌍" value={country.length}/>
<SummaryCard title="Devices 💻" value={device.length}/>
<SummaryCard title="Browsers 🌐" value={browser.length}/>

</div>

{/* Top Links */}

<div id="links" className="bg-white rounded-xl shadow p-6 mb-10">

<h2 className="text-xl font-semibold mb-4">
Top Performing Links
</h2>

<table className="w-full text-left">

<thead>
<tr className="border-b text-gray-500">
<th className="py-2">Short Link</th>
<th className="py-2">Clicks</th>
</tr>
</thead>

<tbody>

{topLinks.map((link:any,i:number)=>(

<tr key={i} className="border-b hover:bg-gray-50">

<td className="py-2 text-purple-600 font-medium">
{link.label}
</td>

<td className="py-2 font-semibold">
{link.count}
</td>

</tr>

))}

</tbody>

</table>

</div>

{/* Clicks Over Time */}

<div id="clicks" className="bg-white rounded-xl shadow p-6 mb-10">

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

{/* Charts */}

<div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6 mb-10">

<div id="traffic">
<ChartPie title="Traffic Source" data={referrer}/>
</div>

<div id="country">
<ChartPie title="Country Distribution" data={country}/>
</div>

<div id="device">
<ChartBar title="Device Types" data={device}/>
</div>

<div id="os">
<ChartBar title="Operating Systems" data={os}/>
</div>

<div id="browser">
<ChartBar title="Browsers" data={browser}/>
</div>

</div>

{/* Heatmap LAST */}

<div id="heatmap" className="mt-12">

<HeatmapChart/>

</div>

</div>

</div>

</div>

</>

);

}

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

<div className="bg-white rounded-xl shadow p-6">

<h3 className="text-lg font-semibold mb-4">
{title}
</h3>

<ResponsiveContainer width="100%" height={250}>

<PieChart>

<Tooltip formatter={(v)=>`${v} clicks`} />

<Legend/>

<Pie
data={data}
dataKey="count"
nameKey="label"
outerRadius={80}
label={({name,value}:any)=>`${name}: ${value}`}
>

{data.map((_:any,index:number)=>(

<Cell
key={index}
fill={COLORS[index % COLORS.length]}
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

<div className="bg-white rounded-xl shadow p-6">

<h3 className="text-lg font-semibold mb-4">
{title}
</h3>

<ResponsiveContainer width="100%" height={250}>

<BarChart data={data}>

<XAxis dataKey="label"/>

<YAxis/>

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