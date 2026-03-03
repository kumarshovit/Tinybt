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

const COLORS = [
  "#2563EB", // Blue
  "#16A34A", // Green
  "#DC2626", // Red
  "#F59E0B", // Orange
  "#7C3AED", // Purple
  "#0EA5E9", // Sky
  "#F43F5E", // Pink
];

interface LanguageData {
  deviceLanguage: string;
  clicks: number;
}

export default function ClicksByLanguage() {
  const [data, setData] = useState<LanguageData[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const res = await getClicksByLanguage();

      // Normalize language → take only first part (en-US from full header)
      const normalized = res.data.map((item: any) => {
        const cleanedLanguage =
          item.deviceLanguage?.split(",")[0] || "Unknown";

        return {
          deviceLanguage: cleanedLanguage,
          clicks: item.clicks,
        };
      });

      // Merge duplicate languages
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
    } catch (error) {
      console.error("Failed to load language data:", error);
    }
  };

  return (
    <div>
      <h2 className="text-xl font-bold mb-6">
        Clicks by Language
      </h2>

      <ResponsiveContainer width="100%" height={400}>
        <PieChart>
          <Pie
            data={data}
            dataKey="clicks"
            nameKey="deviceLanguage"
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
              props.payload.deviceLanguage,
            ]}
          />

          <Legend
            formatter={(value: any, entry: any) =>
              entry.payload.deviceLanguage
            }
          />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}