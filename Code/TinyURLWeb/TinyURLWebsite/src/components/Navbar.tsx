// export default function Navbar() {
//   return (
//     <div className="bg-white shadow-sm px-8 py-4 flex justify-between items-center">
//       <h1 className="text-2xl font-bold text-blue-600">TinyBT</h1>
//       <span className="text-gray-500">Link Management Dashboard</span>
//     </div>
//   );
// }

export default function Navbar() {
  return (
    <div className="bg-white shadow-sm border-b">
      <div className="max-w-7xl mx-auto px-6 py-4 flex justify-between items-center">
        <h1 className="text-xl font-bold text-blue-600">
          TinyBT
        </h1>
        <div className="text-gray-500 text-sm">
          Link Management
        </div>
      </div>
    </div>
  );
}
