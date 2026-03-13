export default function DataPopup({
openPopup,
popupTitle,
popupData,
setOpenPopup
}:any){

if(!openPopup) return null;

return(

<div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">

<div className="bg-white rounded-xl p-8 w-[600px]">

<h2 className="text-xl font-bold mb-6">
{popupTitle}
</h2>

<table className="w-full text-left">

<thead>

<tr className="border-b text-gray-500">

{popupData[0]?.shortCode ? (
<>
<th className="py-2">Short URL</th>
<th className="py-2">Short Alias</th>
<th className="py-2">Clicks</th>
</>
) : (
<>
<th className="py-2">Label</th>
<th className="py-2">Clicks</th>
</>
)}

</tr>

</thead>

<tbody>

{popupData.map((item:any,i:number)=>{

if(item.shortCode){
return(

<tr key={i} className="border-b">

<td className="py-2 text-purple-600">

<a
href={item.shortUrl}
target="_blank"
rel="noopener noreferrer"
className="underline"
>

{item.shortUrl}

</a>

</td>

<td className="py-2">
{item.shortCode}
</td>

<td className="py-2 font-semibold">
{item.clickCount}
</td>

</tr>

)
}

return(

<tr key={i} className="border-b">

<td className="py-2">
{item.label}
</td>

<td className="py-2 font-semibold">
{item.count}
</td>

<td></td>

</tr>

)

})}

</tbody>

</table>

<button
onClick={()=>setOpenPopup(false)}
className="mt-6 bg-purple-600 text-white px-4 py-2 rounded"
>

Close

</button>

</div>

</div>

)

}