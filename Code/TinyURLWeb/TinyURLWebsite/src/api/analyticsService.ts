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

export const getClicksByBrowser = (
  startDate: string,
  endDate: string
) =>
  axios.get(`${BASE_URL}/clicks-by-browser`, {
    params: { startDate, endDate },
    withCredentials: true,
  });

