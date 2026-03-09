import { useState } from "react";

export default function AnalyticsSidebar(){

const [active,setActive] = useState("clicks");

const scrollTo = (id:string)=>{

const element = document.getElementById(id);

if(element){
element.scrollIntoView({
behavior:"smooth",
block:"start"
});

setActive(id);
}

};

const itemClass = (id:string)=>
`cursor-pointer px-3 py-2 rounded-lg transition ${
active===id
? "bg-purple-100 text-purple-600 font-semibold"
: "hover:bg-gray-100"
}`;

return(

<div className="w-64 bg-white shadow h-screen sticky top-0 p-6">

<h2 className="text-xl font-bold text-purple-600 mb-6">
Analytics
</h2>

<ul className="space-y-2 text-gray-700">

<li
className={itemClass("links")}
onClick={()=>scrollTo("links")}
>
Top Links
</li>

<li
className={itemClass("clicks")}
onClick={()=>scrollTo("clicks")}
>
Clicks Over Time
</li>

<li
className={itemClass("traffic")}
onClick={()=>scrollTo("traffic")}
>
Traffic Source
</li>

<li
className={itemClass("country")}
onClick={()=>scrollTo("country")}
>
Country
</li>

<li
className={itemClass("device")}
onClick={()=>scrollTo("device")}
>
Device
</li>

<li
className={itemClass("os")}
onClick={()=>scrollTo("os")}
>
Operating System
</li>

<li
className={itemClass("browser")}
onClick={()=>scrollTo("browser")}
>
Browser
</li>
<li
className={itemClass("heatmap")}
onClick={()=>scrollTo("heatmap")}
>
Popular Days & Times
</li>
</ul>

</div>

);

}