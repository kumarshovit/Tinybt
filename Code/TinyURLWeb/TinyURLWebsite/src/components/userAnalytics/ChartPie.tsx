import {
PieChart,
Pie,
Cell,
Tooltip,
ResponsiveContainer,
Legend
} from "recharts";

const COLORS = [
"#2563eb",
"#22c55e",
"#f97316",
"#06b6d4",
"#ef4444"
];

export default function ChartPie({title,data}:any){

if(!data.length)
return(
<div className="bg-white rounded-xl shadow p-4 sm:p-6 text-center text-gray-400">
No data available
</div>
);

return(

<div className="bg-white rounded-xl shadow p-4 sm:p-6 overflow-hidden hover:shadow-lg transition">

<h3 className="text-base sm:text-lg font-semibold mb-4">
{title}
</h3>

<ResponsiveContainer width="100%" height={240}>

<PieChart>

<Tooltip formatter={(v)=>`${v} clicks`} />

<Legend
wrapperStyle={{fontSize:"12px"}}
/>

<Pie
data={data}
dataKey="count"
nameKey="label"
outerRadius={80}
stroke="none"
strokeWidth={0}
paddingAngle={1}
isAnimationActive={true}
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