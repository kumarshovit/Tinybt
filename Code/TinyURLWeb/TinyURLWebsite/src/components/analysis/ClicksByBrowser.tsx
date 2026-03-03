import { useEffect, useState } from "react";
import {
  PieChart,
  Pie,
  Tooltip,
  ResponsiveContainer,
  Legend,
  Cell,
} from "recharts";
import { getClicksByBrowser } from "../../api/analyticsService";

const COLORS = [
  "#4285F4", // Chrome
  "#0078D7", // Edge
  "#FF7139", // Firefox
  "#000000", // Safari
  "#6B7280",
];

export default function ClicksByBrowser() {
  const [data, setData] = useState<any[]>([]);

  useEffect(() => {
    loadData();
  }, []);

 const loadData = async () => {
  const today = new Date();
  const past = new Date();
  past.setDate(today.getDate() - 30);

  const res = await getClicksByBrowser(
    past.toISOString(),
    today.toISOString()
  );

  setData(res.data);
};

  return (
    <div>
      <h2 className="text-xl font-bold mb-6">
        Clicks by Browser
      </h2>

      <ResponsiveContainer width="100%" height={400}>
        <PieChart>
          <Pie
            data={data}
            dataKey="clicks"
            nameKey="browser"
            outerRadius={150}
            label={({ name }) => name}
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
  );
}