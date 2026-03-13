// // import { useEffect, useState } from "react";
// // import {
// //   PieChart,
// //   Pie,
// //   Tooltip,
// //   ResponsiveContainer,
// //   Legend,
// //   Cell,
// // } from "recharts";
// // import { getClicksByDevice } from "../../api/analyticsService";

// // const COLORS = [
// //   "#2563EB", // Desktop - Blue
// //   "#16A34A", // Mobile - Green
// //   "#F59E0B", // Tablet - Orange
// //   "#DC2626", // Other - Red
// //   "#7C3AED", // Purple
// // ];

// // interface DeviceData {
// //   deviceType: string;
// //   clicks: number;
// // }

// // export default function ClicksByDevice() {
// //   const [data, setData] = useState<DeviceData[]>([]);

// //   useEffect(() => {
// //     loadData();
// //   }, []);

// //   const loadData = async () => {
// //     try {
// //       const res = await getClicksByDevice();
// //       setData(res.data);
// //     } catch (error) {
// //       console.error("Failed to load device data:", error);
// //     }
// //   };

// //   return (
// //     <div>
// //       <h2 className="text-xl font-bold mb-6">
// //         Clicks by Device Type
// //       </h2>

// //       <ResponsiveContainer width="100%" height={400}>
// //         <PieChart>
// //           <Pie
// //             data={data}
// //             dataKey="clicks"
// //             nameKey="deviceType"
// //             outerRadius={150}
// //             label={({ name, percent }) =>
// //               `${name} (${(percent * 100).toFixed(0)}%)`
// //             }
// //           >
// //             {data.map((_, index) => (
// //               <Cell
// //                 key={`cell-${index}`}
// //                 fill={COLORS[index % COLORS.length]}
// //               />
// //             ))}
// //           </Pie>

// //           <Tooltip
// //             formatter={(value: any, name: any, props: any) => [
// //               value,
// //               props.payload.deviceType,
// //             ]}
// //           />

// //           <Legend
// //             formatter={(value: any, entry: any) =>
// //               entry.payload.deviceType
// //             }
// //           />
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
//   Legend,
//   Cell,
// } from "recharts";
// import { getClicksByDevice } from "../../api/analyticsService";

// const COLORS = [
//   "#2563EB",
//   "#16A34A",
//   "#F59E0B",
//   "#DC2626",
//   "#7C3AED",
// ];

// interface DeviceData {
//   deviceType: string;
//   clicks: number;
// }

// interface Props {
//   startDate: string;
//   endDate: string;
// }

// export default function ClicksByDevice({ startDate, endDate }: Props) {
//   const [data, setData] = useState<DeviceData[]>([]);

//   const loadData = async () => {
//     try {
//       const res = await getClicksByDevice(startDate, endDate);
//       setData(res.data);
//     } catch (error) {
//       console.error("Failed to load device data:", error);
//     }
//   };

//   useEffect(() => {
//     loadData();
//   }, [startDate, endDate]);

//   return (
//     <div>
//       <h2 className="text-xl font-bold mb-6">
//         Clicks by Device Type
//       </h2>

//       <ResponsiveContainer width="100%" height={400}>
//         <PieChart>
//           <Pie
//             data={data}
//             dataKey="clicks"
//             nameKey="deviceType"
//             outerRadius={150}
//             label={({ name, percent }) =>
//               `${name} (${(percent * 100).toFixed(0)}%)`
//             }
//           >
//             {data.map((_, index) => (
//               <Cell
//                 key={`cell-${index}`}
//                 fill={COLORS[index % COLORS.length]}
//               />
//             ))}
//           </Pie>

//           <Tooltip
//             formatter={(value: any, name: any, props: any) => [
//               value,
//               props.payload.deviceType,
//             ]}
//           />

//           <Legend
//             formatter={(value: any, entry: any) =>
//               entry.payload.deviceType
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
  Legend,
  Cell,
} from "recharts";
import { getClicksByDevice } from "../../api/analyticsService";

interface DeviceData {
  deviceType: string;
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

export default function ClicksByDevice({ startDate, endDate }: Props) {
  const [data, setData] = useState<DeviceData[]>([]);
  const [selected, setSelected] = useState<DeviceData | null>(null);

  const loadData = async () => {
    try {
      const res = await getClicksByDevice(startDate, endDate);
      setData(res.data);
      setSelected(null);
    } catch (error) {
      console.error("Failed to load device data:", error);
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
        Clicks by Device Type
      </h2>

      <div className="flex gap-8 items-start">

        {/* Chart */}
        <div className="flex-1">
          <ResponsiveContainer width="100%" height={350}>
            <PieChart>
              <Pie
                data={data}
                dataKey="clicks"
                nameKey="deviceType"
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
                Device Details
              </h3>

              <p>
                <strong>Device Type:</strong> {selected.deviceType}
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
