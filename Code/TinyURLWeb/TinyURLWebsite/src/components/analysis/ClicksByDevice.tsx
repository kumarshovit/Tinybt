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

const COLORS = [
  "#2563EB", // Desktop - Blue
  "#16A34A", // Mobile - Green
  "#F59E0B", // Tablet - Orange
  "#DC2626", // Other - Red
  "#7C3AED", // Purple
];

interface DeviceData {
  deviceType: string;
  clicks: number;
}

export default function ClicksByDevice() {
  const [data, setData] = useState<DeviceData[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const res = await getClicksByDevice();
      setData(res.data);
    } catch (error) {
      console.error("Failed to load device data:", error);
    }
  };

  return (
    <div>
      <h2 className="text-xl font-bold mb-6">
        Clicks by Device Type
      </h2>

      <ResponsiveContainer width="100%" height={400}>
        <PieChart>
          <Pie
            data={data}
            dataKey="clicks"
            nameKey="deviceType"
            outerRadius={150}
            label={({ name, percent }) =>
              `${name} (${(percent * 100).toFixed(0)}%)`
            }
          >
            {data.map((_, index) => (
              <Cell
                key={`cell-${index}`}
                fill={COLORS[index % COLORS.length]}
              />
            ))}
          </Pie>

          <Tooltip
            formatter={(value: any, name: any, props: any) => [
              value,
              props.payload.deviceType,
            ]}
          />

          <Legend
            formatter={(value: any, entry: any) =>
              entry.payload.deviceType
            }
          />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}