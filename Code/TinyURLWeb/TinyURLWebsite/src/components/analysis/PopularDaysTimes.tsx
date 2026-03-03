import { useEffect, useState } from "react";
import { getPopularTimes } from "../../api/analyticsService";

export default function PopularDaysTimes() {
  const [data, setData] = useState<any[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    const res = await getPopularTimes();
    setData(res.data);
  };

  const getColor = (clicks: number) => {
    if (clicks > 50) return "bg-blue-800 text-white";
    if (clicks > 30) return "bg-blue-600 text-white";
    if (clicks > 15) return "bg-blue-400";
    if (clicks > 5) return "bg-blue-200";
    return "bg-gray-100";
  };

  return (
    <div>
      <h2 className="text-xl font-bold mb-6">
        Clicks by Popular Days & Times
      </h2>

      <div className="grid grid-cols-6 gap-4">
        {data.map((item: any, index: number) => (
          <div
            key={index}
            className={`p-4 rounded shadow text-center ${getColor(
              item.clicks
            )}`}
          >
            <p className="font-semibold">
              {item.dayOfWeek}
            </p>
            <p className="text-sm">
              Hour: {item.hour}
            </p>
            <p className="text-lg font-bold">
              {item.clicks}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
}