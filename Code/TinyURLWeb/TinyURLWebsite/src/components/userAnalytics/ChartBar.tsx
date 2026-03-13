import {
BarChart,
Bar,
XAxis,
YAxis,
Tooltip,
ResponsiveContainer,
Legend
} from "recharts";

export default function ChartBar({title,data}:any){

if(!data.length)
return(
<div className="bg-white rounded-xl shadow p-4 sm:p-6 text-center text-gray-400">
No data available
</div>
);

return(

<div className="bg-white rounded-xl shadow p-4 sm:p-6 hover:shadow-lg transition">

<h3 className="text-base sm:text-lg font-semibold mb-4">
{title}
</h3>

<ResponsiveContainer width="100%" height={240}>

<BarChart data={data}>

<XAxis
dataKey="label"
axisLine={false}
tickLine={false}
tick={{fontSize:12}}
interval="preserveStartEnd"
/>

<YAxis
axisLine={false}
tickLine={false}
/>

<Tooltip formatter={(v)=>`${v} clicks`} />

<Legend wrapperStyle={{fontSize:"12px"}}/>

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