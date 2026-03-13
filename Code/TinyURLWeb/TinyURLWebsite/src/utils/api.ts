import axios from "axios";


const api = axios.create({
    baseURL: `${import.meta.env.VITE_API_URL}/api`,
});

// ==========================
// REQUEST INTERCEPTOR
// ==========================
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");

  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// ==========================
// RESPONSE INTERCEPTOR
// ==========================
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token invalid or expired
      localStorage.removeItem("token");
      localStorage.removeItem("refreshToken");
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);

export default api;
