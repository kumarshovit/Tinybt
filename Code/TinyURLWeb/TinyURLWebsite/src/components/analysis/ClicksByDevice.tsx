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

      <h2 className="text-lg sm:text-xl font-bold mb-6">
        Clicks by Device Type
      </h2>

      {/* Responsive Layout */}

      <div className="flex flex-col lg:flex-row gap-6 lg:gap-8 items-start">

        {/* Chart */}

        <div className="flex-1 w-full">

          <ResponsiveContainer width="100%" height={320}>

            <PieChart>

              <Pie
                data={data}
                dataKey="clicks"
                nameKey="deviceType"
                outerRadius={110}
                cursor="pointer"
                onClick={handleClick}
                label={({ name, percent }) =>
                  `${name} (${((percent ?? 0) * 100).toFixed(0)}%)`
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

        <div className="w-full lg:w-72">

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