

import type { Activity } from "../types/activity";
import api from "../utils/api";
export const getUserActivity = async (userId: number): Promise<Activity[]> => {
  const res = await api.get(`/analytics/users/${userId}/activity`);
  return res.data;
};