export default function DataPopup({
  openPopup,
  popupTitle,
  popupData,
  setOpenPopup
}: any) {

  if (!openPopup) return null;

  const BASE_URL = import.meta.env.VITE_API_URL;

  return (

    <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50 p-4">

      <div className="bg-white rounded-xl p-4 sm:p-6 lg:p-8 w-full max-w-2xl max-h-[90vh] overflow-y-auto">

        <h2 className="text-lg sm:text-xl font-bold mb-4 sm:mb-6">
          {popupTitle}
        </h2>

        <div className="overflow-x-auto">

          <table className="w-full text-left min-w-[400px]">

            <thead>
              <tr className="border-b text-gray-500">

                {popupData[0]?.shortCode ? (
                  <>
                    <th className="py-2">Short URL</th>
                    <th className="py-2">Short Alias</th>
                    <th className="py-2">Clicks</th>
                  </>
                ) : (
                  <>
                    <th className="py-2">Label</th>
                    <th className="py-2">Clicks</th>
                  </>
                )}

              </tr>
            </thead>

            <tbody>

              {popupData.map((item: any, i: number) => {

                if (item.shortCode) {

                  const url = `${BASE_URL}/${item.shortCode}`;

                  return (
                    <tr key={i} className="border-b">

                      <td className="py-2 text-blue-600">

                        <a
                          href={url}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="underline break-all hover:text-blue-800"
                        >
                          {url}
                        </a>

                      </td>

                      <td className="py-2">
                        {item.shortCode}
                      </td>

                      <td className="py-2 font-semibold">
                        {item.clickCount}
                      </td>

                    </tr>
                  );
                }

                return (
                  <tr key={i} className="border-b">

                    <td className="py-2">
                      {item.label}
                    </td>

                    <td className="py-2 font-semibold">
                      {item.count}
                    </td>

                  </tr>
                );

              })}

            </tbody>

          </table>

        </div>

        <button
          onClick={() => setOpenPopup(false)}
          className="mt-6 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 w-full sm:w-auto"
        >
          Close
        </button>

      </div>

    </div>

  );

}