import axios from "axios";

const BASE_URL = `${import.meta.env.VITE_API_URL}/api/analytics`;

export const getClicksOverTime = (
  startDate: string,
  endDate: string,
  viewType: string
) =>
  axios.get(`${BASE_URL}/clicks-over-time`, {
    params: { startDate, endDate, viewType },
    withCredentials: true,
  });

export const getClicksByCountry = () =>
  axios.get(`${BASE_URL}/clicks-by-country`, { withCredentials: true });

export const getClicksByLanguage = () =>
  axios.get(`${BASE_URL}/clicks-by-language`, { withCredentials: true });

export const getPopularTimes = () =>
  axios.get(`${BASE_URL}/popular-times`, { withCredentials: true });

export const getClicksByDevice = () =>
  axios.get(`${BASE_URL}/device-type`, { withCredentials: true });

export const getClicksByOS = () =>
  axios.get(`${BASE_URL}/clicks-by-os`, { withCredentials: true });

// export const getClicksByBrowser = () =>
//   axios.get(`${BASE_URL}/clicks-by-browser`, { withCredentials: true });

export const getClicksByBrowser = async (
  startDate?: string,
  endDate?: string
) => {
  return await axios.get(
    `${import.meta.env.VITE_API_URL}/api/analytics/clicks-by-browser`,
    {
      params: {
        startDate,
        endDate,
      },
      withCredentials: true,
    }
  );
};

export const getHeatmap = async () => {
  const res = await fetch("/analytics/heatmap");
  return res.json();
};