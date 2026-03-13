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

)

}