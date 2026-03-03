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

interface Props {
  startDate: string;
  endDate: string;
}

export default function ClicksOverTime({
  startDate,
  endDate,
}: Props) {
  const [data, setData] = useState<any[]>([]);
  const [viewType, setViewType] = useState<"daily" | "weekly">(
    "daily"
  );

  useEffect(() => {
    if (startDate && endDate) {
      loadData();
    }
  }, [startDate, endDate, viewType]);

  const loadData = async () => {
    const res = await getClicksOverTime(
      startDate,
      endDate,
      viewType
    );
    setData(res.data);
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-xl font-bold">
          Clicks Over Time
        </h2>

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

      <ResponsiveContainer width="100%" height={400}>
        <LineChart data={data}>
          <CartesianGrid stroke="#ccc" />
          <XAxis dataKey="date" />
          <YAxis />
          <Tooltip />
          <Line
            type="monotone"
            dataKey="clicks"
            stroke="#2563eb"
            strokeWidth={3}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}