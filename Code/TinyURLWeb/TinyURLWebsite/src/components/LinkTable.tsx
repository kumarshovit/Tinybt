type LinkItem = {
  shortUrl: string;
  longUrl: string;
  clicks: number;
};

const LinkTable = ({ links }: { links: LinkItem[] }) => {
  return (
    <div className="bg-white shadow-md rounded-xl p-6 mt-6">
      <h2 className="text-xl font-bold mb-4">Recent Links</h2>

      <table className="w-full border-collapse">
        <thead>
          <tr className="text-left border-b">
            <th className="py-2">Short URL</th>
            <th className="py-2">Long URL</th>
            <th className="py-2">Clicks</th>
          </tr>
        </thead>

        <tbody>
          {links.map((link, index) => (
            <tr key={index} className="border-b hover:bg-gray-100">
              <td className="py-2 text-blue-600 font-medium">
                {link.shortUrl}
              </td>
              <td className="py-2 text-gray-600 truncate max-w-[300px]">
                {link.longUrl}
              </td>
              <td className="py-2 font-semibold">{link.clicks}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default LinkTable;
