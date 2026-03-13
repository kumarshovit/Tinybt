import ChartPie from "./ChartPie";
import ChartBar from "./ChartBar";

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

<div
id="traffic"
onClick={()=>openDataPopup("Traffic Source",referrer)}
className="cursor-pointer min-h-[320px]"
>
<ChartPie title="Traffic Source" data={referrer}/>
</div>

<div
id="country"
onClick={()=>openDataPopup("Country Distribution",country)}
className="cursor-pointer min-h-[320px]"
>
<ChartPie title="Country Distribution" data={country}/>
</div>

<div
id="device"
onClick={()=>openDataPopup("Device Types",device)}
className="cursor-pointer min-h-[320px]"
>
<ChartBar title="Device Types" data={device}/>
</div>

<div
id="os"
onClick={()=>openDataPopup("Operating Systems",os)}
className="cursor-pointer min-h-[320px]"
>
<ChartBar title="Operating Systems" data={os}/>
</div>

<div
id="browser"
onClick={()=>openDataPopup("Browsers",browser)}
className="cursor-pointer min-h-[320px]"
>
<ChartBar title="Browsers" data={browser}/>
</div>

<div
id="language"
onClick={()=>openDataPopup("Device Language",language)}
className="cursor-pointer min-h-[320px]"
>
<ChartPie title="Device Language" data={language}/>
</div>

</div>

)

}