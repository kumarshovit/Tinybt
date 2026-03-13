import {
LineChart,
Line,
XAxis,
YAxis,
Tooltip,
ResponsiveContainer,
Legend
} from "recharts";

export default function ClicksChart({clicks,openDataPopup}:any){

return(

<div
id="clicks"
onClick={()=>openDataPopup("Clicks Over Time",clicks)}
className="bg-white rounded-xl shadow p-4 sm:p-6 mb-10 cursor-pointer hover:shadow-lg transition"
>

<h2 className="text-lg sm:text-xl font-semibold mb-4">
Clicks Over Time
</h2>

<ResponsiveContainer width="100%" height={280}>

<LineChart data={clicks}>

<XAxis
dataKey="label"
tick={{fontSize:12}}
interval="preserveStartEnd"
/>

<YAxis/>

<Tooltip formatter={(v)=>`${v} clicks`} />

<Legend/>

<Line
type="monotone"
dataKey="count"
stroke="#7c3aed"
strokeWidth={3}
dot={{r:3}}
activeDot={{r:6}}
/>

</LineChart>

</ResponsiveContainer>

</div>

)

}