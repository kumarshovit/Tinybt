import type { Activity } from "../types/activity";

interface Props {
  activities: Activity[];
  onClose: () => void;
}

const ActivityModal = ({ activities, onClose }: Props) => {
  return (
    <div className="fixed inset-0 bg-black/40 flex justify-center items-center">

      <div className="bg-white p-6 rounded-lg w-[800px] max-h-[80vh] overflow-y-auto">

        <div className="flex justify-between mb-4">
          <h2 className="text-xl font-semibold">User Activity</h2>

          <button
            onClick={onClose}
            className="text-red-500 font-bold"
          >
            Close
          </button>
        </div>

        {activities.length === 0 ? (
          <p>No activity found.</p>
        ) : (
          <table className="w-full border">

            <thead className="bg-gray-200">
              <tr>
                <th className="border p-2">Activity</th>
                <th className="border p-2">Short Code</th>
                <th className="border p-2">URL</th>
                <th className="border p-2">Time</th>
              </tr>
            </thead>

            <tbody>
              {activities.map((a, i) => (
                <tr key={i}>

                  <td className="border p-2">{a.activityType}</td>

                  <td className="border p-2">{a.shortCode}</td>

                  <td className="border p-2 truncate max-w-[250px]">
                    {a.longUrl}
                  </td>

                  <td className="border p-2">
                    {new Date(a.activityTime).toLocaleString()}
                  </td>

                </tr>
              ))}
            </tbody>

          </table>
        )}

      </div>
    </div>
  );
};

export default ActivityModal;