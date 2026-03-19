import axios from "axios";

const BASE_URL = `${import.meta.env.VITE_API_URL}/api/analytics`;
// const BASE_URLS = `${import.meta.env.VITE_API_URL}/api/admin/analytics`;

export const getUsersOverTime = (
  start: string,
  end: string,
  viewType: string
) =>
  axios.get(`${BASE_URL}/users-over-time`, {
    params: { start, end, viewType },
    withCredentials: true,
  });


export const getClicksByCountry = (
  startDate: string,
  endDate: string
) =>
  axios.get(`${BASE_URL}/clicks-by-country`, {
    params: { startDate, endDate },
    withCredentials: true,
  });

export const getClicksByLanguage = (
  startDate: string,
  endDate: string
) =>
  axios.get(`${BASE_URL}/clicks-by-language`, {
    params: { startDate, endDate },
    withCredentials: true,
  });

export const getPopularTimes = (
  startDate: string,
  endDate: string
) =>
  axios.get(`${BASE_URL}/popular-times`, {
    params: { startDate, endDate },
    withCredentials: true,
  });

export const getClicksByDevice = (
  startDate: string,
  endDate: string
) =>
  axios.get(`${BASE_URL}/device-type`, {
    params: { startDate, endDate },
    withCredentials: true,
  });

export const getClicksByOS = (
  startDate: string,
  endDate: string
) =>
  axios.get(`${BASE_URL}/clicks-by-os`, {
    params: { startDate, endDate },
    withCredentials: true,
  });

export const getClicksByBrowser = async (
  startDate?: string,
  endDate?: string
) => {
  return await axios.get(
    `${import.meta.env.VITE_API_URL}/api/admin/analytics/users-by-browser`,
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
   

