import { useEffect, useState } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  ResponsiveContainer,
} from "recharts";
import { getClicksOverTime } from "../../api/analyticsService";

interface ClickData {
  period: string;
  clicks: number;
}

interface Props {
  startDate: string;
  endDate: string;
}

export default function ClicksOverTime({ startDate, endDate }: Props) {
  const [data, setData] = useState<ClickData[]>([]);
  const [viewType, setViewType] = useState<"daily" | "weekly">("daily");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (startDate && endDate) {
      loadData();
    }
  }, [startDate, endDate, viewType]);

  const loadData = async () => {
    try {
      setLoading(true);

      const res = await getClicksOverTime(startDate, endDate, viewType);

      setData(res.data);
    } catch (err) {
      console.error("Error loading clicks data", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-xl font-bold">Clicks Over Time</h2>

        <div className="flex gap-2">
          <button
            onClick={() => setViewType("daily")}
            className={`px-4 py-1 rounded ${
              viewType === "daily"
                ? "bg-blue-600 text-white"
                : "bg-gray-200"
            }`}
          >
            Daily
          </button>

          <button
            onClick={() => setViewType("weekly")}
            className={`px-4 py-1 rounded ${
              viewType === "weekly"
                ? "bg-blue-600 text-white"
                : "bg-gray-200"
            }`}
          >
            Weekly
          </button>
        </div>
      </div>

      {loading ? (
        <p className="text-gray-500">Loading chart...</p>
      ) : data.length === 0 ? (
        <p className="text-gray-500">No data available</p>
      ) : (
        <ResponsiveContainer width="100%" height={400}>
          <LineChart data={data}>
            <CartesianGrid strokeDasharray="3 3" />

            <XAxis
              dataKey="period"
              tickFormatter={(date) =>
                new Date(date).toLocaleDateString("en-IN", {
                  day: "numeric",
                  month: "short",
                })
              }
            />

            <YAxis allowDecimals={false} />

            <Tooltip
              labelFormatter={(label) =>
                new Date(label).toLocaleDateString()
              }
            />

            <Line
              type="monotone"
              dataKey="clicks"
              stroke="#2563eb"
              strokeWidth={3}
              dot={{ r: 4 }}
              activeDot={{ r: 6 }}
            />
          </LineChart>
        </ResponsiveContainer>
      )}
    </div>
  );
}