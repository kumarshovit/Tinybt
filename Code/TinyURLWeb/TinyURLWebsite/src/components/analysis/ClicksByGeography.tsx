// // import { useEffect, useState } from "react";
// // import {
// //   PieChart,
// //   Pie,
// //   Tooltip,
// //   ResponsiveContainer,
// //   Cell,
// //   Legend,
// // } from "recharts";
// // import { getClicksByCountry } from "../../api/analyticsService";

// // export default function ClicksByGeography() {
// //   const [data, setData] = useState<any[]>([]);

// //   useEffect(() => {
// //     loadData();
// //   }, []);

// //   const loadData = async () => {
// //     const res = await getClicksByCountry();
// //     setData(res.data);
// //   };

// //   return (
// //     <div>
// //       <h2 className="text-xl font-bold mb-6">
// //         Clicks by Geography
// //       </h2>

// //       <ResponsiveContainer width="100%" height={400}>
// //         <PieChart>
// //           <Pie
// //             data={data}
// //             dataKey="clicks"
// //             nameKey="country"
// //             outerRadius={150}
// //             label
// //           >
// //             {data.map((_, index) => (
// //               <Cell key={index} />
// //             ))}
// //           </Pie>
// //           <Tooltip />
// //           <Legend />
// //         </PieChart>
// //       </ResponsiveContainer>
// //     </div>
// //   );
// // }

// import { useEffect, useState } from "react";
// import {
//   PieChart,
//   Pie,
//   Tooltip,
//   ResponsiveContainer,
//   Cell,
//   Legend,
// } from "recharts";
// import { getClicksByCountry } from "../../api/analyticsService";

// interface GeoData {
//   country: string;
//   clicks: number;
// }

// interface Props {
//   startDate: string;
//   endDate: string;
// }

// const COLORS = [
//   "#2563EB",
//   "#16A34A",
//   "#F59E0B",
//   "#DC2626",
//   "#7C3AED",
//   "#0EA5E9",
//   "#F43F5E",
// ];

// export default function ClicksByGeography({ startDate, endDate }: Props) {
//   const [data, setData] = useState<GeoData[]>([]);

//   const loadData = async () => {
//     try {
//       const res = await getClicksByCountry(startDate, endDate);
//       setData(res.data);
//     } catch (error) {
//       console.error("Failed to load geography data:", error);
//     }
//   };

//   useEffect(() => {
//     loadData();
//   }, [startDate, endDate]);

//   return (
//     <div>
//       <h2 className="text-xl font-bold mb-6">
//         Clicks by Geography
//       </h2>

//       <ResponsiveContainer width="100%" height={400}>
//         <PieChart>
//           <Pie
//             data={data}
//             dataKey="clicks"
//             nameKey="country"
//             outerRadius={150}
//             label={({ name, percent }) =>
//               `${name} (${(percent * 100).toFixed(0)}%)`
//             }
//           >
//             {data.map((_, index) => (
//               <Cell
//                 key={index}
//                 fill={COLORS[index % COLORS.length]}
//               />
//             ))}
//           </Pie>

//           <Tooltip
//             formatter={(value: any, name: any, props: any) => [
//               value,
//               props.payload.country,
//             ]}
//           />

//           <Legend
//             formatter={(value: any, entry: any) =>
//               entry.payload.country
//             }
//           />
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
  Cell,
  Legend,
} from "recharts";
import { getClicksByCountry } from "../../api/analyticsService";

interface GeoData {
  country: string;
  clicks: number;
}

interface Props {
  startDate: string;
  endDate: string;
}

const COLORS = [
  "#6366F1",
  "#22C55E",
  "#F97316",
  "#EF4444",
  "#14B8A6",
  "#A855F7",
];

export default function ClicksByGeography({ startDate, endDate }: Props) {
  const [data, setData] = useState<GeoData[]>([]);
  const [selected, setSelected] = useState<GeoData | null>(null);

  const loadData = async () => {
    try {
      const res = await getClicksByCountry(startDate, endDate);
      setData(res.data);
      setSelected(null);
    } catch (error) {
      console.error("Failed to load geography data:", error);
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
        Clicks by Geography
      </h2>

      <div className="flex gap-8 items-start">

        {/* Chart */}
        <div className="flex-1">
          <ResponsiveContainer width="100%" height={350}>
            <PieChart>
              <Pie
                data={data}
                dataKey="clicks"
                nameKey="country"
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

              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Details Panel */}
        <div className="w-72">
          {selected && (
            <div className="bg-gray-100 p-5 rounded shadow">
              <h3 className="text-lg font-semibold mb-3">
                Country Details
              </h3>

              <p>
                <strong>Country:</strong> {selected.country}
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