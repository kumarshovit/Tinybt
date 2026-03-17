import CountUp from "react-countup";

export default function SummaryCard({title,value}:any){

return(

<div className="bg-white p-4 sm:p-6 rounded-xl shadow hover:shadow-lg transition hover:scale-[1.02]">

<p className="text-gray-500 text-xs sm:text-sm mb-2">
{title}
</p>

<h2 className="text-2xl sm:text-3xl font-bold text-blue-600">
<CountUp end={value} duration={1.5}/>
</h2>

</div>

);

}