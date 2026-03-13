import axios from "axios";
const API_BASE = import.meta.env.VITE_API_URL;

const API_URL = `${API_BASE}/api/analytics/dashboard`;

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
    params: { startDate, endDate },
    withCredentials: true,
  });

  return response.data;
};