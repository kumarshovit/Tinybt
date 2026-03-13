import CountUp from "react-countup";

export default function SummaryCard({title,value}:any){

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