import { useEffect, useState } from "react";
import { getPopularTimes } from "../../api/analyticsService";

interface Props {
  startDate: string;
  endDate: string;
}

interface HeatmapItem {
  day: string;
  hour: number;
  clicks: number;
}

const days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];
const hours = Array.from({ length: 24 }, (_, i) => i);

const dayMap: any = {
  Monday: "Mon",
  Tuesday: "Tue",
  Wednesday: "Wed",
  Thursday: "Thu",
  Friday: "Fri",
  Saturday: "Sat",
  Sunday: "Sun",
};

export default function PopularDaysTimes({ startDate, endDate }: Props) {

  const [data, setData] = useState<HeatmapItem[]>([]);

  const loadData = async () => {
    try {

      const res = await getPopularTimes(startDate, endDate);

      const normalized = res.data.map((item: any) => ({
        day: dayMap[item.day] || item.day,
        hour: Number(item.hour),
        clicks: item.clicks,
      }));

      setData(normalized);

    } catch (error) {
      console.error("Failed to load popular times:", error);
    }
  };

  useEffect(() => {
    loadData();
  }, [startDate, endDate]);

  const dataMap = new Map<string, number>();

  data.forEach((item) => {
    const key = `${item.day}-${item.hour}`;
    dataMap.set(key, item.clicks);
  });

  const getColor = (clicks: number) => {
    if (clicks >= 10) return "bg-green-600";
    if (clicks >= 5) return "bg-green-400";
    if (clicks >= 1) return "bg-green-200";
    return "bg-gray-200";
  };

  return (

    <div className="w-full">

      <h2 className="text-lg sm:text-xl font-bold mb-6">
        Clicks by Popular Days & Times
      </h2>

      <div className="overflow-x-auto">

        {/* Hour labels */}

        <div className="flex ml-16 sm:ml-20 mb-3 text-xs text-gray-600 gap-1 sm:gap-2">

          {hours.map((hour) => (

            <div key={hour} className="w-6 sm:w-8 text-center">
              {hour}
            </div>

          ))}

        </div>

        {/* Heatmap rows */}

        {days.map((day) => (

          <div key={day} className="flex items-center mb-2">

            {/* Day label */}

            <div className="w-16 sm:w-20 font-medium text-sm">
              {day}
            </div>

            {/* Cells */}

            <div className="flex gap-1 sm:gap-2">

              {hours.map((hour) => {

                const clicks = dataMap.get(`${day}-${hour}`) || 0;

                return (
                  <div
                    key={hour}
                    className={`w-6 h-6 sm:w-8 sm:h-8 rounded ${getColor(clicks)} hover:scale-110 transition`}
                    title={`${day} ${hour}:00 → ${clicks} clicks`}
                  />
                );

              })}

            </div>

          </div>

        ))}

      </div>

      {/* Legend */}

      <div className="flex flex-wrap items-center gap-3 mt-6 text-sm text-gray-600">

        <span>Less</span>

        <div className="w-4 h-4 sm:w-5 sm:h-5 bg-gray-200 rounded"></div>
        <div className="w-4 h-4 sm:w-5 sm:h-5 bg-green-200 rounded"></div>
        <div className="w-4 h-4 sm:w-5 sm:h-5 bg-green-400 rounded"></div>
        <div className="w-4 h-4 sm:w-5 sm:h-5 bg-green-600 rounded"></div>

        <span>More</span>

      </div>

    </div>
  );
}