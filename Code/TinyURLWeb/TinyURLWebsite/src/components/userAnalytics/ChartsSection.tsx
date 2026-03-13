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

<div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6 mb-10 border-none">

<div id="traffic" onClick={()=>openDataPopup("Traffic Source",referrer)}>
<ChartPie title="Traffic Source" data={referrer}/>
</div>

<div id="country" onClick={()=>openDataPopup("Country Distribution",country)}>
<ChartPie title="Country Distribution" data={country}/>
</div>

<div id="device" onClick={()=>openDataPopup("Device Types",device)}>
<ChartBar title="Device Types" data={device}/>
</div>

<div id="os" onClick={()=>openDataPopup("Operating Systems",os)}>
<ChartBar title="Operating Systems" data={os}/>
</div>

<div id="browser" onClick={()=>openDataPopup("Browsers",browser)}>
<ChartBar title="Browsers" data={browser}/>
</div>

<div id="language" onClick={()=>openDataPopup("Device Language",language)}>
<ChartPie title="Device Language" data={language}/>
</div>

</div>

)

}