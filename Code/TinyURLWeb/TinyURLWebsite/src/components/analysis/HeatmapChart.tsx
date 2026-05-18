// import React, { useEffect, useState } from "react";

// const API_BASE = import.meta.env.VITE_API_URL;

// type HeatmapItem = {
//   day: number;
//   hour: number;
//   count: number;
// };

// const days = [
//   { label: "Sun", index: 0 },
//   { label: "Mon", index: 1 },
//   { label: "Tue", index: 2 },
//   { label: "Wed", index: 3 },
//   { label: "Thu", index: 4 },
//   { label: "Fri", index: 5 },
//   { label: "Sat", index: 6 }
// ];

// export default function HeatmapChart() {

// const [data,setData] = useState<HeatmapItem[]>([]);

// useEffect(()=>{

// const token = localStorage.getItem("token");

// fetch(`${API_BASE}/analytics/heatmap`,{
// headers:{
// Authorization:`Bearer ${token}`
// }
// })
// .then(res=>res.json())
// .then(result=>{
// console.log("Heatmap:",result);
// setData(result);
// });

// },[]);

// const getCount = (day:number,hour:number)=>{
// const item = data.find(x=>x.day===day && x.hour===hour);
// return item ? item.count : 0;
// };

// const getColor = (count:number)=>{

// if(count===0) return "#e5e7eb";
// if(count<3) return "#bbf7d0";
// if(count<6) return "#4ade80";
// if(count<10) return "#22c55e";

// return "#166534";
// };

// return(

// <div className="mt-10 bg-white rounded-xl shadow p-4 sm:p-6">

// <h3 className="text-base sm:text-lg font-semibold mb-4">
// 🔥 Clicks by Popular Days & Times
// </h3>

// <div className="overflow-x-auto">

// <div
// className="grid gap-1 items-center"
// style={{
// gridTemplateColumns:"60px repeat(24,28px)"
// }}
// >

// <div></div>

// {[...Array(24)].map((_,hour)=>(

// <div
// key={hour}
// className="text-[10px] text-center text-gray-500"
// >
// {hour}
// </div>

// ))}

// {days.map(day=>(

// <React.Fragment key={day.index}>

// <div className="font-semibold text-xs sm:text-sm">
// {day.label}
// </div>

// {[...Array(24)].map((_,hour)=>{

// const count = getCount(day.index,hour);

// return(

// <div
// key={hour}
// title={`${day.label} ${hour}:00 → ${count} clicks`}
// className="w-6 h-6 sm:w-7 sm:h-7 rounded cursor-pointer"
// style={{
// background:getColor(count)
// }}
// />

// );

// })}

// </React.Fragment>

// ))}

// </div>

// </div>

// {/* Legend */}

// <div className="flex items-center gap-2 mt-4 text-xs sm:text-sm">

// <span>Less</span>

// <div className="w-4 h-4 bg-gray-200"/>
// <div className="w-4 h-4 bg-green-200"/>
// <div className="w-4 h-4 bg-green-400"/>
// <div className="w-4 h-4 bg-green-500"/>
// <div className="w-4 h-4 bg-green-900"/>

// <span>More</span>

// </div>

// </div>

// );

// }


import React from "react";

type HeatmapItem = {
  day: number;
  hour: number;
  count: number;
};

interface Props {
  data: HeatmapItem[];
}

const days = [
  { label: "Sun", index: 0 },
  { label: "Mon", index: 1 },
  { label: "Tue", index: 2 },
  { label: "Wed", index: 3 },
  { label: "Thu", index: 4 },
  { label: "Fri", index: 5 },
  { label: "Sat", index: 6 }
];

export default function HeatmapChart({ data }: Props) {

  /* ---------- HELPERS ---------- */

  const getCount = (day: number, hour: number) => {
    const item = data.find(x => x.day === day && x.hour === hour);
    return item ? item.count : 0;
  };

  const getColor = (count: number) => {
    if (count === 0) return "#e5e7eb";
    if (count < 3) return "#bbf7d0";
    if (count < 6) return "#4ade80";
    if (count < 10) return "#22c55e";
    return "#166534";
  };

  /* ---------- EMPTY STATE ---------- */

  if (!data || data.length === 0) {
    return (
      <div className="mt-10 bg-white rounded-xl shadow p-4 sm:p-6">
        <h3 className="text-base sm:text-lg font-semibold mb-4">
          🔥 Clicks by Popular Days & Times
        </h3>
        <p className="text-gray-500 text-sm">No heatmap data available</p>
      </div>
    );
  }

  /* ---------- UI ---------- */

  return (
    <div id="heatmap" className="mt-10 bg-white rounded-xl shadow p-4 sm:p-6">

      <h3 className="text-base sm:text-lg font-semibold mb-4">
        🔥 Clicks by Popular Days & Times
      </h3>

      <div className="overflow-x-auto">

        <div
          className="grid gap-1 items-center"
          style={{
            gridTemplateColumns: "60px repeat(24,28px)"
          }}
        >

          {/* EMPTY CORNER */}
          <div></div>

          {/* HOURS */}
          {[...Array(24)].map((_, hour) => (
            <div
              key={hour}
              className="text-[10px] text-center text-gray-500"
            >
              {hour}
            </div>
          ))}

          {/* DAYS + CELLS */}
          {days.map(day => (
            <React.Fragment key={day.index}>

              <div className="font-semibold text-xs sm:text-sm">
                {day.label}
              </div>

              {[...Array(24)].map((_, hour) => {
                const count = getCount(day.index, hour);

                return (
                  <div
                    key={hour}
                    title={`${day.label} ${hour}:00 → ${count} clicks`}
                    className="w-6 h-6 sm:w-7 sm:h-7 rounded cursor-pointer transition hover:scale-110"
                    style={{
                      background: getColor(count)
                    }}
                  />
                );
              })}

            </React.Fragment>
          ))}

        </div>
      </div>

      {/* LEGEND */}
      <div className="flex items-center gap-2 mt-4 text-xs sm:text-sm">
        <span>Less</span>

        <div className="w-4 h-4 bg-gray-200" />
        <div className="w-4 h-4 bg-green-200" />
        <div className="w-4 h-4 bg-green-400" />
        <div className="w-4 h-4 bg-green-500" />
        <div className="w-4 h-4 bg-green-900" />

        <span>More</span>
      </div>

    </div>
  );
}