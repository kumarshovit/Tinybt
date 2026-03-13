// import axios from "axios";

// const API_URL = "https://localhost:57679/api/analytics/dashboard"; // your backend URL

// export interface DashboardOverview {
//   totalClicks: number;
//   totalUrls: number;
//   activeLinks: number;
//   expiredLinks: number;
// }

// export const getDashboardOverview = async (
//   startDate?: string,
//   endDate?: string
// ): Promise<DashboardOverview> => {
//   const response = await axios.get(API_URL, {
//     params: { startDate, endDate },
//     withCredentials: true,
//   });

//   return response.data;
// };

import axios from "axios";

const API_URL = "https://localhost:57679/api/analytics/dashboard";

export interface DashboardOverview {
  totalClicks: number;
  totalUrls: number;
  activeLinks: number;
  expiredLinks: number;
}

export const getDashboardOverview = async (
  startDate?: string,
  endDate?: string
): Promise<DashboardOverview> => {

  const response = await axios.get(API_URL, {
    params: {
      from: startDate,
      to: endDate
    },
    withCredentials: true
  });

  return response.data;
};