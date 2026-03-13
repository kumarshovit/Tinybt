import React, { useEffect, useState } from "react";

const API_BASE = import.meta.env.VITE_API_URL;

type HeatmapItem = {
  day: number;
  hour: number;
  count: number;
};

const days = [
  { label: "Sun", index: 0 },
  { label: "Mon", index: 1 },
  { label: "Tue", index: 2 },
  { label: "Wed", index: 3 },
  { label: "Thu", index: 4 },
  { label: "Fri", index: 5 },
  { label: "Sat", index: 6 }
];

export default function HeatmapChart() {

  const [data, setData] = useState<HeatmapItem[]>([]);

useEffect(()=>{

  const token = localStorage.getItem("token");

  fetch(`${API_BASE}/analytics/heatmap`,{
    headers:{
      Authorization:`Bearer ${token}`
    }
  })
  .then(res=>res.json())
  .then(result=>{
    console.log("Heatmap:",result);
    setData(result);
  });

},[]);

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

  return (

    <div style={{
      marginTop: 40,
      padding: 20,
      background: "white",
      borderRadius: 12,
      boxShadow: "0 2px 8px rgba(0,0,0,0.08)"
    }}>

      <h3 style={{ marginBottom: 15 }}>
        🔥 Clicks by Popular Days & Times
      </h3>

      <div style={{ overflowX: "auto" }}>

        <div style={{
          display: "grid",
          gridTemplateColumns: "60px repeat(24,30px)",
          gap: "4px",
          alignItems: "center"
        }}>

          <div></div>

          {[...Array(24)].map((_, hour) => (
            <div key={hour} style={{
              fontSize: 10,
              textAlign: "center",
              color: "#6b7280"
            }}>
              {hour}
            </div>
          ))}

          {days.map(day => (
            <React.Fragment key={day.index}>

              <div style={{
                fontWeight: 600,
                fontSize: 13
              }}>
                {day.label}
              </div>

              {[...Array(24)].map((_, hour) => {

                const count = getCount(day.index, hour);

                return (
                  <div
                    key={hour}
                    title={`${day.label} ${hour}:00 → ${count} clicks`}
                    style={{
                      width: 28,
                      height: 28,
                      borderRadius: 4,
                      background: getColor(count),
                      cursor: "pointer"
                    }}
                  />
                );

              })}

            </React.Fragment>
          ))}

        </div>

      </div>

      {/* Legend */}

      <div style={{
        display: "flex",
        alignItems: "center",
        gap: 8,
        marginTop: 15,
        fontSize: 12
      }}>

        <span>Less</span>

        <div style={{ width: 16, height: 16, background: "#e5e7eb" }} />
        <div style={{ width: 16, height: 16, background: "#bbf7d0" }} />
        <div style={{ width: 16, height: 16, background: "#4ade80" }} />
        <div style={{ width: 16, height: 16, background: "#22c55e" }} />
        <div style={{ width: 16, height: 16, background: "#166534" }} />

        <span>More</span>

      </div>

    </div>
  );
}