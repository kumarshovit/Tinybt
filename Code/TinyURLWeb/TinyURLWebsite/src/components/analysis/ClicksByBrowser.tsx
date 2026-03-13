

// import { useEffect, useState } from "react";
// import {
//   PieChart,
//   Pie,
//   Tooltip,
//   ResponsiveContainer,
//   Legend,
//   Cell,
// } from "recharts";
// import { getClicksByBrowser } from "../../api/analyticsService";

// const COLORS = [
//   "#4285F4",
//   "#0078D7",
//   "#FF7139",
//   "#000000",
//   "#6B7280",
// ];

// interface Props {
//   startDate: string;
//   endDate: string;
// }

// export default function ClicksByBrowser({ startDate, endDate }: Props) {
//   const [data, setData] = useState<any[]>([]);

//   const loadData = async () => {
//     try {
//       const res = await getClicksByBrowser(startDate, endDate);
//       setData(res.data);
//     } catch (error) {
//       console.error("Failed to load browser analytics", error);
//     }
//   };

//   useEffect(() => {
//     loadData();
//   }, [startDate, endDate]);

//   return (
//     <div>
//       <h2 className="text-xl font-bold mb-6">
//         Clicks by Browser
//       </h2>

//       <ResponsiveContainer width="100%" height={400}>
//         <PieChart>
//           <Pie
//             data={data}
//             dataKey="clicks"
//             nameKey="browser"
//             outerRadius={150}
//             label={({ name }) => name}
//           >
//             {data.map((_, index) => (
//               <Cell
//                 key={index}
//                 fill={COLORS[index % COLORS.length]}
//               />
//             ))}
//           </Pie>

//           <Tooltip />
//           <Legend />
//         </PieChart>
//       </ResponsiveContainer>
//     </div>
//   );
// }

import { useEffect, useState } from "react";
import {
  PieChart,
  Pie,
  Tooltip,
  ResponsiveContainer,
  Legend,
  Cell,
} from "recharts";
import { getClicksByBrowser } from "../../api/analyticsService";

const COLORS = [
  "#6366F1",
  "#22C55E",
  "#F97316",
  "#EF4444",
  "#14B8A6",
  "#A855F7",
];

interface BrowserData {
  browser: string;
  clicks: number;
}

interface Props {
  startDate: string;
  endDate: string;
}

export default function ClicksByBrowser({ startDate, endDate }: Props) {
  const [data, setData] = useState<BrowserData[]>([]);
  const [selected, setSelected] = useState<BrowserData | null>(null);

  const loadData = async () => {
    try {
      const res = await getClicksByBrowser(startDate, endDate);

      const normalized = res.data.map((item: any) => {
        const ua = item.browser?.toLowerCase() || "";
        let browser = "Other";

        if (ua.includes("chrome") && !ua.includes("edg")) browser = "Chrome";
        else if (ua.includes("edg")) browser = "Edge";
        else if (ua.includes("firefox")) browser = "Firefox";
        else if (ua.includes("safari") && !ua.includes("chrome"))
          browser = "Safari";

        return {
          browser,
          clicks: item.clicks,
        };
      });

      const grouped = Object.values(
        normalized.reduce((acc: any, curr: any) => {
          if (!acc[curr.browser]) {
            acc[curr.browser] = { browser: curr.browser, clicks: 0 };
          }

          acc[curr.browser].clicks += curr.clicks;
          return acc;
        }, {})
      );

      setData(grouped as BrowserData[]);
      setSelected(null);
    } catch (error) {
      console.error("Failed to load browser analytics", error);
    }
  };

  useEffect(() => {
    loadData();
  }, [startDate, endDate]);

  const handleClick = (entry: any) => {
    setSelected(entry.payload);
  };

  return (
    <div>
      <h2 className="text-xl font-bold mb-6">
        Clicks by Browser
      </h2>

      <div className="flex gap-8 items-start">

        {/* Chart */}
        <div className="flex-1">
          <ResponsiveContainer width="100%" height={350}>
            <PieChart>
              <Pie
                data={data}
                dataKey="clicks"
                nameKey="browser"
                outerRadius={120}
                cursor="pointer"
                onClick={handleClick}
                activeShape={false}
                label={({ name, percent }) =>
                  `${name} (${(percent * 100).toFixed(0)}%)`
                }
              >
                {data.map((_, index) => (
                  <Cell
                    key={index}
                    fill={COLORS[index % COLORS.length]}
                  />
                ))}
              </Pie>

              <Tooltip
                formatter={(value: any, name: any, props: any) => [
                  value,
                  props.payload.browser,
                ]}
              />

              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Details Panel */}
        <div className="w-72">
          {selected && (
            <div className="bg-gray-100 p-5 rounded shadow">
              <h3 className="text-lg font-semibold mb-3">
                Browser Details
              </h3>

              <p>
                <strong>Browser:</strong> {selected.browser}
              </p>

              <p>
                <strong>Clicks:</strong> {selected.clicks}
              </p>
            </div>
          )}
        </div>

      </div>
    </div>
  );
}

// import { useEffect, useState } from "react";
// import {
//   PieChart,
//   Pie,
//   Tooltip,
//   ResponsiveContainer,
//   Legend,
//   Cell,
// } from "recharts";
// import { getClicksByBrowser } from "../../api/analyticsService";

// const COLORS = [
//   "#6366F1",
//   "#22C55E",
//   "#F97316",
//   "#EF4444",
//   "#14B8A6",
//   "#A855F7",
// ];

// interface BrowserData {
//   browser: string;
//   clicks: number;
// }

// interface Props {
//   startDate: string;
//   endDate: string;
// }

// export default function ClicksByBrowser({ startDate, endDate }: Props) {
//   const [data, setData] = useState<BrowserData[]>([]);
//   const [selected, setSelected] = useState<BrowserData | null>(null);

//   const loadData = async () => {
//     try {
//       const res = await getClicksByBrowser(startDate, endDate);

//       const normalized = res.data.map((item: any) => {
//         const ua = item.browser?.toLowerCase() || "";
//         let browser = "Other";

//         if (ua.includes("chrome") && !ua.includes("edg")) browser = "Chrome";
//         else if (ua.includes("edg")) browser = "Edge";
//         else if (ua.includes("firefox")) browser = "Firefox";
//         else if (ua.includes("safari") && !ua.includes("chrome"))
//           browser = "Safari";

//         return {
//           browser,
//           clicks: item.clicks,
//         };
//       });

//       const grouped = Object.values(
//         normalized.reduce((acc: any, curr: any) => {
//           if (!acc[curr.browser]) {
//             acc[curr.browser] = { browser: curr.browser, clicks: 0 };
//           }

//           acc[curr.browser].clicks += curr.clicks;
//           return acc;
//         }, {})
//       );

//       setData(grouped as BrowserData[]);
//       setSelected(null);
//     } catch (error) {
//       console.error("Failed to load browser analytics", error);
//     }
//   };

//   useEffect(() => {
//     loadData();
//   }, [startDate, endDate]);

//   const handleClick = (entry: any) => {
//     setSelected(entry.payload);
//   };

//   return (
//     <div>
//       <h2 className="text-xl font-bold mb-6">
//         Clicks by Browser
//       </h2>

//       <div className="flex gap-8 items-start">

//         {/* Chart */}
//         <div className="flex-1">
//           <ResponsiveContainer width="100%" height={350}>
//             <PieChart>

//               <Pie
//                 data={data}
//                 dataKey="clicks"
//                 nameKey="browser"
//                 outerRadius={120}
//                 stroke="none"
//                 cursor={false}
//                 activeShape={undefined}
//                 onClick={handleClick}
//                 label={({ name, percent }) =>
//                   `${name} (${(percent * 100).toFixed(0)}%)`
//                 }
//               >
//                 {data.map((_, index) => (
//                   <Cell
//                     key={index}
//                     fill={COLORS[index % COLORS.length]}
//                   />
//                 ))}
//               </Pie>

//               <Tooltip
//                 cursor={false}
//                 contentStyle={{
//                   border: "none",
//                   borderRadius: "8px",
//                   boxShadow: "0px 2px 8px rgba(0,0,0,0.15)",
//                 }}
//                 formatter={(value: any, name: any, props: any) => [
//                   value,
//                   props.payload.browser,
//                 ]}
//               />

//               <Legend />

//             </PieChart>
//           </ResponsiveContainer>
//         </div>

//         {/* Details Panel */}
//         <div className="w-72">
//           {selected && (
//             <div className="bg-gray-100 p-5 rounded shadow">
//               <h3 className="text-lg font-semibold mb-3">
//                 Browser Details
//               </h3>

//               <p>
//                 <strong>Browser:</strong> {selected.browser}
//               </p>

//               <p>
//                 <strong>Clicks:</strong> {selected.clicks}
//               </p>
//             </div>
//           )}
//         </div>

//       </div>
//     </div>
//   );
// }

// import { useEffect, useState } from "react";
// import {
//   PieChart,
//   Pie,
//   Tooltip,
//   ResponsiveContainer,
//   Legend,
//   Cell,
// } from "recharts";
// import { getClicksByBrowser } from "../../api/analyticsService";

// const COLORS = [
//   "#6366F1",
//   "#22C55E",
//   "#F97316",
//   "#EF4444",
//   "#14B8A6",
//   "#A855F7",
// ];

// interface BrowserData {
//   browser: string;
//   clicks: number;
// }

// interface Props {
//   startDate: string;
//   endDate: string;
// }

// export default function ClicksByBrowser({ startDate, endDate }: Props) {
//   const [data, setData] = useState<BrowserData[]>([]);
//   const [selected, setSelected] = useState<BrowserData | null>(null);
//   const [activeIndex, setActiveIndex] = useState<number | null>(null);

//   const loadData = async () => {
//     try {
//       const res = await getClicksByBrowser(startDate, endDate);

//       const normalized = res.data.map((item: any) => {
//         const ua = item.browser?.toLowerCase() || "";
//         let browser = "Other";

//         if (ua.includes("chrome") && !ua.includes("edg")) browser = "Chrome";
//         else if (ua.includes("edg")) browser = "Edge";
//         else if (ua.includes("firefox")) browser = "Firefox";
//         else if (ua.includes("safari") && !ua.includes("chrome"))
//           browser = "Safari";

//         return {
//           browser,
//           clicks: item.clicks,
//         };
//       });

//       const grouped = Object.values(
//         normalized.reduce((acc: any, curr: any) => {
//           if (!acc[curr.browser]) {
//             acc[curr.browser] = { browser: curr.browser, clicks: 0 };
//           }
//           acc[curr.browser].clicks += curr.clicks;
//           return acc;
//         }, {})
//       );

//       setData(grouped as BrowserData[]);
//       setSelected(null);
//     } catch (error) {
//       console.error("Failed to load browser analytics", error);
//     }
//   };

//   useEffect(() => {
//     loadData();
//   }, [startDate, endDate]);

//   const totalClicks = data.reduce((sum, d) => sum + d.clicks, 0);

//   const handleClick = (entry: any) => {
//     setSelected(entry.payload);
//   };

//   const onPieEnter = (_: any, index: number) => {
//     setActiveIndex(index);
//   };

//   return (
//     <div>
//       <h2 className="text-xl font-bold mb-6">Clicks by Browser</h2>

//       <div className="flex gap-8 items-start">

//         {/* Chart */}
//         <div className="flex-1 relative">

//           {/* Center Total Clicks */}
//           <div className="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
//             <p className="text-gray-500 text-sm">Total Clicks</p>
//             <h2 className="text-3xl font-bold">{totalClicks}</h2>
//           </div>

//           <ResponsiveContainer width="100%" height={350}>
//             <PieChart>

//               <Pie
//                 data={data}
//                 dataKey="clicks"
//                 nameKey="browser"
//                 innerRadius={70}
//                 outerRadius={120}
//                 stroke="none"
//                 activeIndex={activeIndex ?? undefined}
//                 activeOuterRadius={130}
//                 cursor="pointer"
//                 onMouseEnter={onPieEnter}
//                 onClick={handleClick}
//                 isAnimationActive={true}
//                 animationDuration={800}
//                 label={({ name, percent }) =>
//                   `${name} (${(percent * 100).toFixed(0)}%)`
//                 }
//               >
//                 {data.map((_, index) => (
//                   <Cell
//                     key={index}
//                     fill={COLORS[index % COLORS.length]}
//                   />
//                 ))}
//               </Pie>

//               <Tooltip
//                 cursor={false}
//                 contentStyle={{
//                   border: "none",
//                   borderRadius: "8px",
//                   boxShadow: "0px 3px 10px rgba(0,0,0,0.15)",
//                 }}
//                 formatter={(value: any, name: any, props: any) => [
//                   value,
//                   props.payload.browser,
//                 ]}
//               />

//               <Legend />

//             </PieChart>
//           </ResponsiveContainer>
//         </div>

//         {/* Details Panel */}
//         <div className="w-72">
//           {selected && (
//             <div className="bg-gray-100 p-5 rounded shadow">
//               <h3 className="text-lg font-semibold mb-3">
//                 Browser Details
//               </h3>

//               <p>
//                 <strong>Browser:</strong> {selected.browser}
//               </p>

//               <p>
//                 <strong>Clicks:</strong> {selected.clicks}
//               </p>
//             </div>
//           )}
//         </div>

//       </div>
//     </div>
//   );
// }