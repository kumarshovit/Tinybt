import { useEffect, useState } from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  ResponsiveContainer,
} from "recharts";
import { getClicksByOS } from "../../api/analyticsService";

export default function ClicksByOS() {
  const [data, setData] = useState<any[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    const res = await getClicksByOS();
    setData(res.data);
  };

  return (
    <div>
      <h2 className="text-xl font-bold mb-6">
        Clicks by Operating System
      </h2>

      <ResponsiveContainer width="100%" height={400}>
        <BarChart data={data}>
          <CartesianGrid stroke="#ccc" />
          <XAxis dataKey="os" />
          <YAxis />
          <Tooltip />
          <Bar dataKey="clicks" fill="#2563eb" />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}