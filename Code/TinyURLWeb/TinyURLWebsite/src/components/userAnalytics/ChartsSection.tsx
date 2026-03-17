import ChartPie from "./ChartPie";
import ChartBar from "./ChartBar";

const CardWrapper = ({children,onClick,id}:any)=>(
<div
id={id}   
onClick={onClick}
className="relative group cursor-pointer min-h-[320px] bg-white rounded-xl shadow p-4 hover:shadow-xl hover:scale-[1.02] transition duration-200"
>
{children}

{/* 🔥 Overlay */}
<div className="absolute inset-0 bg-black/0  flex items-center justify-center opacity-0 group-hover:opacity-100 transition rounded-xl">
<span className="text-blue-400 text-sm font-semibold">
View details
</span>
</div>

</div>
);

export default function ChartsSection({
referrer,
country,
device,
os,
browser,
language,
openDataPopup
}:any){

return(

<div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4 sm:gap-6 mb-10">

<CardWrapper id="traffic" onClick={()=>openDataPopup("Traffic Source",referrer)}>
<ChartPie title="Traffic Source" data={referrer}/>
</CardWrapper>

<CardWrapper id="country" onClick={()=>openDataPopup("Country Distribution",country)}>
<ChartPie title="Country Distribution" data={country}/>
</CardWrapper>

<CardWrapper id="device" onClick={()=>openDataPopup("Device Types",device)}>
<ChartBar title="Device Types" data={device}/>
</CardWrapper>

<CardWrapper id="os" onClick={()=>openDataPopup("Operating Systems",os)}>
<ChartBar title="Operating Systems" data={os}/>
</CardWrapper>

<CardWrapper id="browser" onClick={()=>openDataPopup("Browsers",browser)}>
<ChartBar title="Browsers" data={browser}/>
</CardWrapper>

<CardWrapper id="language" onClick={()=>openDataPopup("Device Language",language)}>
<ChartPie title="Device Language" data={language}/>
</CardWrapper>

</div>
)
}