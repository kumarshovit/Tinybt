// import axios from "axios";

// const BASE_URL = "https://localhost:57679/api/analytics";

// export const getClicksOverTime = (
//   startDate: string,
//   endDate: string,
//   viewType: string
// ) =>
//   axios.get(`${BASE_URL}/clicks-over-time`, {
//     params: { startDate, endDate, viewType },
//     withCredentials: true,
//   });

// export const getClicksByCountry = () =>
//   axios.get(`${BASE_URL}/clicks-by-country`, { withCredentials: true });

// export const getClicksByLanguage = () =>
//   axios.get(`${BASE_URL}/clicks-by-language`, { withCredentials: true });

// export const getPopularTimes = () =>
//   axios.get(`${BASE_URL}/popular-times`, { withCredentials: true });

// export const getClicksByDevice = () =>
//   axios.get(`${BASE_URL}/device-type`, { withCredentials: true });

// export const getClicksByOS = () =>
//   axios.get(`${BASE_URL}/clicks-by-os`, { withCredentials: true });

// export const getClicksByBrowser = () =>
//   axios.get(`${BASE_URL}/clicks-by-browser`, { withCredentials: true });

// // export const getClicksByBrowser = async (
// //   startDate?: string,
// //   endDate?: string
// // ) => {
// //   return await axios.get(
// //     "https://localhost:57679/api/analytics/clicks-by-browser",
// //     {
// //       params: {
// //         startDate,
// //         endDate,
// //       },
// //       withCredentials: true,
// //     }
// //   );
// // };

import axios from "axios";

const BASE_URL = "https://localhost:57679/api/analytics";

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

export const getClicksByBrowser = (
  startDate: string,
  endDate: string
) =>
  axios.get(`${BASE_URL}/clicks-by-browser`, {
    params: { startDate, endDate },
    withCredentials: true,
  });