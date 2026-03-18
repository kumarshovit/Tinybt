import { useEffect, useState } from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  ResponsiveContainer,
  Cell,
} from "recharts";
import { getClicksByOS } from "../../api/analyticsService";

interface OSData {
  os: string;
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

export default function ClicksByOS({ startDate, endDate }: Props) {

  const [data, setData] = useState<OSData[]>([]);
  const [selected, setSelected] = useState<OSData | null>(null);

  const loadData = async () => {
    try {
      const res = await getClicksByOS(startDate, endDate);
      setData(res.data);
      setSelected(null);
    } catch (error) {
      console.error("Failed to load OS data:", error);
    }
  };

  useEffect(() => {
    loadData();
  }, [startDate, endDate]);

  const handleClick = (entry: any) => {
    setSelected(entry);
  };

  return (
    <div>

      <h2 className="text-lg sm:text-xl font-bold mb-6">
        Clicks by Operating System
      </h2>

      {/* Responsive Layout */}

      <div className="flex flex-col lg:flex-row gap-6 lg:gap-8 items-start">

        {/* Chart */}

        <div className="flex-1 w-full">

          <ResponsiveContainer width="100%" height={320}>

            <BarChart data={data}>

              <CartesianGrid stroke="#e5e7eb" />

              <XAxis dataKey="os" />

              <YAxis allowDecimals={false} />

              <Tooltip cursor={{ fill: "rgba(0,0,0,0.05)" }} />

              <Bar
                dataKey="clicks"
                cursor="pointer"
                onClick={handleClick}
              >
                {data.map((_, index) => (
                  <Cell
                    key={index}
                    fill={COLORS[index % COLORS.length]}
                  />
                ))}
              </Bar>

            </BarChart>

          </ResponsiveContainer>

        </div>

        {/* Details Panel */}

        <div className="w-full lg:w-72">

          {selected && (

            <div className="bg-gray-100 p-5 rounded shadow">

              <h3 className="text-lg font-semibold mb-3">
                OS Details
              </h3>

              <p>
                <strong>Operating System:</strong> {selected.os}
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