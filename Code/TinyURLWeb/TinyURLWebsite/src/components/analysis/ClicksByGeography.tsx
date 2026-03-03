import { useEffect, useState } from "react";
import {
  PieChart,
  Pie,
  Tooltip,
  ResponsiveContainer,
  Cell,
  Legend,
} from "recharts";
import { getClicksByCountry } from "../../api/analyticsService";

export default function ClicksByGeography() {
  const [data, setData] = useState<any[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    const res = await getClicksByCountry();
    setData(res.data);
  };

  return (
    <div>
      <h2 className="text-xl font-bold mb-6">
        Clicks by Geography
      </h2>

      <ResponsiveContainer width="100%" height={400}>
        <PieChart>
          <Pie
            data={data}
            dataKey="clicks"
            nameKey="country"
            outerRadius={150}
            label
          >
            {data.map((_, index) => (
              <Cell key={index} />
            ))}
          </Pie>
          <Tooltip />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}