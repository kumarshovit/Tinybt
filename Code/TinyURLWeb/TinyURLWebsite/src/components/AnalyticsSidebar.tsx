import { useState, useEffect } from "react";
import {
PanelLeft,
X,
BarChart3,
Globe,
Monitor,
Clock
} from "lucide-react";

export default function AnalyticsSidebar(){

const [active,setActive] = useState("clicks");
const [open,setOpen] = useState(false);

/* Lock body scroll when sidebar open */

useEffect(()=>{
document.body.style.overflow = open ? "hidden" : "auto";
},[open]);

const scrollTo = (id:string)=>{

const element = document.getElementById(id);

if(element){
element.scrollIntoView({
behavior:"smooth",
block:"start"
});

setActive(id);
setOpen(false);
}

};

const itemClass = (id:string)=>
`flex items-center gap-3 cursor-pointer px-3 py-2 rounded-lg transition
${active===id
? "bg-blue-100 text-blue-600 font-semibold"
: "text-gray-600 hover:bg-gray-100"}`;

return(

<>

{/* Overlay */}

{open && (
<div
className="fixed inset-0 bg-black/40 z-30 lg:hidden"
onClick={()=>setOpen(false)}
></div>
)}

{/* Mobile Menu Button */}

<button
onClick={()=>setOpen(!open)}
className="lg:hidden mt-1 ml-1 top-16 left-4 z-50 bg-blue-600 text-white p-1 rounded-lg shadow hover:bg-purple-700 transition"
>
<PanelLeft size={20}/>
</button>

{/* Sidebar */}

<div
className={`bg-white shadow-xl p-6 fixed lg:static top-0 left-0 h-full w-64 max-w-[80%]
transform transition-transform duration-300 z-40
${open ? "translate-x-0" : "-translate-x-full"}
lg:translate-x-0`}
>

{/* Close button (mobile) */}

<button
onClick={()=>setOpen(false)}
className="absolute top-4 right-4 text-gray-500 lg:hidden"
>
<X size={22}/>
</button>

{/* Sidebar Title */}

<h2 className="text-xl font-bold text-blue-600 mb-6 flex items-center gap-2">
<BarChart3 size={20}/>
Analytics
</h2>

<ul className="space-y-2 text-sm">

<li
className={itemClass("links")}
onClick={()=>scrollTo("links")}
>
<BarChart3 size={18}/>
Top Links
</li>

<li
className={itemClass("clicks")}
onClick={()=>scrollTo("clicks")}
>
<Clock size={18}/>
Clicks Over Time
</li>

<li
className={itemClass("traffic")}
onClick={()=>scrollTo("traffic")}
>
<Globe size={18}/>
Traffic Source
</li>

<li
className={itemClass("country")}
onClick={()=>scrollTo("country")}
>
<Globe size={18}/>
Country
</li>

<li
className={itemClass("device")}
onClick={()=>scrollTo("device")}
>
<Monitor size={18}/>
Device
</li>

<li
className={itemClass("os")}
onClick={()=>scrollTo("os")}
>
<Monitor size={18}/>
Operating System
</li>

<li
className={itemClass("language")}
onClick={()=>scrollTo("language")}
>
<Globe size={18}/>
Device Language
</li>

<li
className={itemClass("browser")}
onClick={()=>scrollTo("browser")}
>
<Monitor size={18}/>
Browser
</li>

<li
className={itemClass("heatmap")}
onClick={()=>scrollTo("heatmap")}
>
<BarChart3 size={18}/>
Popular Days & Times
</li>

</ul>

</div>

</>

);

}