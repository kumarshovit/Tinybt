import Sidebar from "../components/Sidebar";
import LinkTable from "../components/LinkTable";

const MyLinks = () => {
  const links = [
    {
      shortUrl: "tinybt.io/abc123",
      longUrl: "https://example.com/very-long-link",
      clicks: 23,
    },
  ];

  return (
    <div className="flex">
      <Sidebar />

      <div className="flex-1 p-8 bg-gray-100 min-h-screen">
        <h1 className="text-3xl font-bold mb-6">My Links</h1>
        <LinkTable links={links} />
      </div>
    </div>
  );
};

export default MyLinks;
