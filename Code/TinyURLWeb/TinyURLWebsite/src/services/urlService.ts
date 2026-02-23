import axios from "axios";

const BASE_URL = "https://localhost:7025/api/url";

export const shortenUrl = async (longUrl: string) => {
  const token = localStorage.getItem("token");

  const response = await axios.post(
    `${BASE_URL}/shorten`,
    { longUrl },
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  return response.data;
};
