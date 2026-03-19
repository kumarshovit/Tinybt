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
import { getUsersOverTime } from "../../api/analyticsService";

interface ClickData {
  period: string;
  users: number;
}

interface Props {
  startDate: string;
  endDate: string;
}

export default function UsersOverTime({ startDate, endDate }: Props) {

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

      const res = await getUsersOverTime(startDate, endDate, viewType);

      setData(res.data);

    } catch (err) {
      console.error("Error loading users data", err);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (value: string) => {

    if (viewType === "daily") {
      return new Date(value).toLocaleDateString("en-IN", {
        day: "numeric",
        month: "short",
      });
    }

    return value;
  };

  return (

    <div className="bg-white p-4 sm:p-6 rounded-lg shadow">

      {/* Header */}

      <div className="flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4 mb-6">

        <h2 className="text-lg sm:text-xl font-bold">
          Users Over Time
        </h2>

        <div className="flex gap-2">

          <button
            onClick={() => setViewType("daily")}
            className={`px-3 sm:px-4 py-1 rounded text-sm ${
              viewType === "daily"
                ? "bg-blue-600 text-white"
                : "bg-gray-200"
            }`}
          >
            Daily
          </button>

          <button
            onClick={() => setViewType("weekly")}
            className={`px-3 sm:px-4 py-1 rounded text-sm ${
              viewType === "weekly"
                ? "bg-blue-600 text-white"
                : "bg-gray-200"
            }`}
          >
            Weekly
          </button>

        </div>

      </div>

      {/* Chart */}

      {loading ? (

        <p className="text-gray-500">Loading chart...</p>

      ) : data.length === 0 ? (

        <p className="text-gray-500">No data available</p>

      ) : (

        <ResponsiveContainer width="100%" height={320}>

          <LineChart data={data}>

            <CartesianGrid strokeDasharray="3 3" />

            <XAxis
              dataKey="period"
              tickFormatter={(value) => formatDate(value)}
            />

            <YAxis allowDecimals={false} />

            <Tooltip
              labelFormatter={(label) => formatDate(label as string)}
            />

            <Line
              type="monotone"
              dataKey="users"
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