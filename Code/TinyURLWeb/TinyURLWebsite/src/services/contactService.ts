import apiClient from "./authService";

// 🔹 Contact Payload Type
export interface ContactPayload {
  name: string;
  email: string;
  subject: string;
  message: string;
}

// 🔹 Send Contact Message
export const createContactMessage = async (
  data: ContactPayload
) => {

  const response = await apiClient.post(
    `${import.meta.env.VITE_API_URL}/api/contact`,
    data
  );

  return response.data;
};