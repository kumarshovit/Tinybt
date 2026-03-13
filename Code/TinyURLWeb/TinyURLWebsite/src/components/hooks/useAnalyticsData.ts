import { useState, useEffect } from "react";

const API_BASE = import.meta.env.VITE_API_URL;

export default function useAnalyticsData({
  from,
  to,
  selectedLink,
  selectedTag
}:any){

/* ---------- DATA STATES ---------- */

const [allLinks,setAllLinks] = useState<any[]>([]);
const [allTags,setAllTags] = useState<string[]>([]);

const [clicks,setClicks] = useState<any[]>([]);
const [referrer,setReferrer] = useState<any[]>([]);
const [country,setCountry] = useState<any[]>([]);
const [device,setDevice] = useState<any[]>([]);
const [os,setOs] = useState<any[]>([]);
const [browser,setBrowser] = useState<any[]>([]);
const [topLinks,setTopLinks] = useState<any[]>([]);
const [language,setLanguage] = useState<any[]>([]);
const [totalClicks,setTotalClicks] = useState(0);

const token = localStorage.getItem("token");

const headers = {
"Content-Type":"application/json",
Authorization:`Bearer ${token}`
};

/* ---------- FETCH USER LINKS ---------- */

const fetchUserLinks = async()=>{

const res = await fetch(`${API_BASE}/api/urls`,{
headers
});

const data = await res.json();

setAllLinks(data);

const tagSet = new Set<string>();

data.forEach((link:any)=>{
if(link.tags){
link.tags.forEach((tag:string)=>{
tagSet.add(tag);
});
}
});

setAllTags(Array.from(tagSet));

};

/* ---------- FETCH DASHBOARD ---------- */

const fetchDashboard = async()=>{

const body = {
from,
to,
link:selectedLink,
tag:selectedTag
};

try{

/* clicks over time */

const clicksRes = await fetch(
`${API_BASE}/analytics/user/clicks-over-time`,
{
method:"POST",
headers,
body:JSON.stringify(body)
}
);

const clicksData = await clicksRes.json();

setClicks(clicksData);

let total = 0;

clicksData.forEach((x:any)=>{
total += x.count;
});

setTotalClicks(total);

/* breakdown */

const breakdown = async(type:string)=>{

const res = await fetch(
`${API_BASE}/analytics/breakdown`,
{
method:"POST",
headers,
body:JSON.stringify({
from,
to,
type,
link:selectedLink,
tag:selectedTag
})
}
);

return res.json();

};

setReferrer(await breakdown("referrer"));
setCountry(await breakdown("country"));
setDevice(await breakdown("device"));
setOs(await breakdown("os"));
setBrowser(await breakdown("browser"));

/* language */

const langRes = await fetch(
`${API_BASE}/analytics/user/device-language`,
{
method:"POST",
headers,
body:JSON.stringify(body)
}
);

setLanguage(await langRes.json());

/* top links */

const linksRes = await fetch(
`${API_BASE}/analytics/user/popular-links`,
{
method:"POST",
headers,
body:JSON.stringify(body)
}
);

const linksData = await linksRes.json();

/* alias → clicks */

const aliasClicks: Record<string, number> = {};

linksData.forEach((x:any)=>{
aliasClicks[x.label] = x.count;
});

/* tag → clicks */

const tagCounts: Record<string, number> = {};

allLinks.forEach((link:any)=>{

const clicks = aliasClicks[link.shortCode] || 0;

if(link.tags){

link.tags.forEach((tag:string)=>{

if(!tagCounts[tag]) tagCounts[tag] = 0;

tagCounts[tag] += clicks;

});

}

});

const tagData = Object.keys(tagCounts).map(tag=>({

label: tag,
count: tagCounts[tag]

}))
.sort((a,b)=>b.count-a.count);

setTopLinks(tagData);

}
catch(err){
console.error("Analytics error",err);
}

};

/* ---------- TAG POPUP LINKS ---------- */

const openTagPopup = async(tag:string)=>{

const body = {
from,
to,
tag
};

const res = await fetch(
`${API_BASE}/analytics/user/popular-links`,
{
method:"POST",
headers,
body:JSON.stringify(body)
}
);

const data = await res.json();

return data.map((x:any)=>({

shortUrl: `${API_BASE}/${x.label}`,
shortCode: x.label,
clickCount: x.count

}));

};

/* ---------- LOAD DEFAULT ---------- */

useEffect(()=>{
fetchUserLinks();
},[]);

useEffect(()=>{
if(allLinks.length){
fetchDashboard();
}
},[from,to,selectedLink,selectedTag,allLinks]);

/* ---------- RETURN ---------- */

return{

allLinks,
allTags,

clicks,
referrer,
country,
device,
os,
browser,
language,
topLinks,
totalClicks,

openTagPopup

};

}