import {
PieChart,
Pie,
Cell,
Tooltip,
ResponsiveContainer,
Legend
} from "recharts";

const COLORS = [
"#7c3aed",
"#22c55e",
"#f97316",
"#06b6d4",
"#ef4444"
];

export default function ChartPie({title,data}:any){

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