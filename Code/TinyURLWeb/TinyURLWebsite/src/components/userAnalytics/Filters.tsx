export default function Filters({
from,
to,
setFrom,
setTo,
setLast7Days,
setLast30Days,
selectedLink,
setSelectedLink,
selectedTag,
setSelectedTag,
allLinks,
allTags,
exportCSV
}:any){

return(

<div className="bg-white rounded-xl shadow p-6 mb-10 flex flex-wrap gap-4 items-center">

<button
onClick={setLast7Days}
className="bg-purple-100 text-purple-700 px-4 py-2 rounded-lg hover:bg-purple-200"
>
Last 7 Days
</button>

<button
onClick={setLast30Days}
className="bg-purple-100 text-purple-700 px-4 py-2 rounded-lg hover:bg-purple-200"
>
Last 30 Days
</button>

<input
type="date"
value={from}
onChange={e=>setFrom(e.target.value)}
className="border p-2 rounded-lg"
/>

<input
type="date"
value={to}
onChange={e=>setTo(e.target.value)}
className="border p-2 rounded-lg"
/>

<select
value={selectedLink}
onChange={(e)=>{setSelectedLink(e.target.value); setSelectedTag("");}}
className="border p-2 rounded-lg"
>
<option value="">All Alias</option>

{allLinks.map((link:any)=>(

<option key={link.id} value={link.shortCode}>
{link.shortCode}
</option>

))}

</select>

<select
value={selectedTag}
onChange={(e)=>{setSelectedTag(e.target.value);setSelectedLink("");}}
className="border p-2 rounded-lg"
>

<option value="">All Tags</option>

{allTags.map((tag)=>(

<option key={tag} value={tag}>
{tag}
</option>

))}

</select>

<button
onClick={exportCSV}
className="bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded-lg"
>
Export CSV
</button>

</div>

)

}