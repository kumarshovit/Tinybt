export default function TopTagsTable({
selectedLink,
selectedTag,
topLinks,
openTagPopup
}:any){

return(

<div id="links" className="bg-white rounded-xl shadow p-4 sm:p-6 mb-10">

<h2 className="text-lg sm:text-xl font-semibold mb-4">
{selectedLink
  ? `Tags for alias: ${selectedLink}`
  : selectedTag
  ? `Analytics for tag: ${selectedTag}`
  : "Top Performing Tags"}
</h2>

<div className="overflow-x-auto">

<table className="w-full text-left min-w-[400px]">

<thead>
<tr className="border-b text-gray-500">
<th className="py-2">Tag</th>
<th className="py-2">Clicks</th>
</tr>
</thead>

<tbody>

{topLinks.slice(0,5).map((tag:any,i:number)=>(

<tr
key={i}
className="border-b hover:bg-gray-50 cursor-pointer transition"
onClick={()=>openTagPopup(tag.label)}
>

<td className="py-2 text-purple-600 font-medium">
{tag.label}
</td>

<td className="py-2 font-semibold">
{tag.count}
</td>

</tr>

))}

</tbody>

</table>

</div>

</div>

)

}