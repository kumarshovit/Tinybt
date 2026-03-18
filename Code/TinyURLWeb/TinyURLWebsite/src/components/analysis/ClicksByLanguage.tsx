import { useEffect, useState } from "react";
import {
  PieChart,
  Pie,
  Tooltip,
  ResponsiveContainer,
  Legend,
  Cell,
} from "recharts";
import { getClicksByLanguage } from "../../api/analyticsService";

interface LanguageData {
  deviceLanguage: string;
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
  "#F59E0B",
];

export default function ClicksByLanguage({ startDate, endDate }: Props) {

  const [data, setData] = useState<LanguageData[]>([]);
  const [selected, setSelected] = useState<LanguageData | null>(null);

  const loadData = async () => {
    try {
      const res = await getClicksByLanguage(startDate, endDate);

      const normalized = res.data.map((item: any) => {
        const cleanedLanguage =
          item.deviceLanguage?.split(",")[0] || "Unknown";

        return {
          deviceLanguage: cleanedLanguage,
          clicks: item.clicks,
        };
      });

      const grouped = Object.values(
        normalized.reduce((acc: any, curr: any) => {
          if (!acc[curr.deviceLanguage]) {
            acc[curr.deviceLanguage] = {
              deviceLanguage: curr.deviceLanguage,
              clicks: 0,
            };
          }

          acc[curr.deviceLanguage].clicks += curr.clicks;
          return acc;

        }, {})
      );

      setData(grouped as LanguageData[]);
      setSelected(null);

    } catch (error) {
      console.error("Failed to load language data:", error);
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
        Clicks by Language
      </h2>

      {/* Responsive Layout */}

      <div className="flex flex-col lg:flex-row gap-6 lg:gap-8 items-start">

        {/* Chart */}

        <div className="flex-1 w-full">

          <ResponsiveContainer width="100%" height={320}>

            <PieChart margin={{ top: 20, right: 30, left: 30, bottom: 20 }}>

              <Pie
                data={data}
                dataKey="clicks"
                nameKey="deviceLanguage"
                outerRadius={110}
                stroke="none"
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

              <Tooltip
                cursor={false}
                contentStyle={{
                  border: "none",
                  borderRadius: "6px",
                  boxShadow: "0px 2px 6px rgba(0,0,0,0.15)",
                }}
                formatter={(value: any, props: any) => [
                  value,
                  props.payload.deviceLanguage,
                ]}
              />

              <Legend
                formatter={(entry: any) =>
                  entry.payload.deviceLanguage
                }
              />

            </PieChart>

          </ResponsiveContainer>

        </div>

        {/* Details Panel */}

        <div className="w-full lg:w-72">

          {selected && (

            <div className="bg-gray-100 p-5 rounded shadow">

              <h3 className="text-lg font-semibold mb-3">
                Language Details
              </h3>

              <p>
                <strong>Language:</strong> {selected.deviceLanguage}
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