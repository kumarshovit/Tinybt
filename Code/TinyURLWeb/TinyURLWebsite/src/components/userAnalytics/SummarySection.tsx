import SummaryCard from "./SummaryCard";

export default function SummarySection({totalClicks,country,device,browser}:any){

return(

<div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6 mb-10">

<SummaryCard title="Total Clicks 📈" value={totalClicks}/>
<SummaryCard title="Countries 🌍" value={country.length}/>
<SummaryCard title="Devices 💻" value={device.length}/>
<SummaryCard title="Browsers 🌐" value={browser.length}/>

</div>

)

}