const StatCard = ({ title, value }: { title: string; value: string }) => {
  return (
    <div className="bg-white shadow-md rounded-xl p-6 w-full">
      <h3 className="text-gray-500 text-sm">{title}</h3>
      <p className="text-2xl font-bold text-gray-900 mt-2">{value}</p>
    </div>
  );
};

export default StatCard;
