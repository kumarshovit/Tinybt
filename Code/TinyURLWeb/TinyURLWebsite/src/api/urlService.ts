const BASE_URL = `${import.meta.env.VITE_API_URL}/api/urls`;

/* ============================= */
/* 🔐 AUTH HEADER HELPER         */
/* ============================= */
const getAuthHeaders = () => {
  const token = localStorage.getItem("token");

  if (!token) {
    throw new Error("User not authenticated");
  }

  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
};

/* ============================= */
/* 🔓 OPTIONAL AUTH HEADER       */
/* ============================= */
const getOptionalAuthHeaders = () => {
  const token = localStorage.getItem("token");

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  return headers;
};

/* ============================= */
/* 📌 GET ALL TAGS              */
/* ============================= */

export const getAllTags = async () => {

  const res = await fetch(`${BASE_URL}`, {
    headers: getAuthHeaders(),
  });

  if (!res.ok) {
    return { success: false };
  }

  const urls = await res.json();

  const tagSet = new Set<string>();

  urls.forEach((url: any) => {
    if (url.tags) {
      url.tags.forEach((t: string) => tagSet.add(t));
    }
  });

  return { success: true, data: Array.from(tagSet) };
};



/* ============================= */
/* 📌 GET ALL URLS              */
/* ============================= */
export const getAllUrls = async () => {
  const res = await fetch(BASE_URL, {
    headers: getAuthHeaders(),
  });

  if (!res.ok) {
    const msg = await res.text();
    return { success: false, message: msg };
  }

    const data = await res.json();
  return { success: true, data };
};

/* ➕ CREATE URL                 */
/* ============================= */
export const createUrl = async (
  longUrl: string,
  customAlias?: string,
  expirationDate?: string,
  captchaToken?: string
) => {
  const res = await fetch(BASE_URL, {
    method: "POST",
    headers: getOptionalAuthHeaders(),
    body: JSON.stringify({
      longUrl,
      customAlias,
      expirationDate,
      captchaToken
    }),
  });

  if (!res.ok) {
    const err = await res.json();
 return { success: false, message: err.message };
  }

const data = await res.json();
  return { success: true, data };};

/* ============================= */
/* 🏷 ADD TAGS                   */
/* ============================= */
export const addTags = async (id: number, tags: string[]) => {
  const res = await fetch(`${BASE_URL}/${id}/tags`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify({ tags }),
  });

  if (!res.ok) {
    const msg = await res.text();
    return { success: false, message: msg };
  }

  return { success: true };
};

/* ============================= */
/* 🔎 SEARCH BY TAG              */
/* ============================= */
export const searchByTag = async (tag: string) => {
  const res = await fetch(`${BASE_URL}/by-tag/${tag}`, {
    headers: getAuthHeaders(),
  });

 
  if (!res.ok) {
    const msg = await res.text();
    return { success: false, message: msg };
  }

  const data = await res.json();
  return { success: true, data };
};

/* ============================= */
/* ✏ UPDATE TAGS                */
/* ============================= */
export const updateTags = async (id: number, tags: string[]) => {
  const res = await fetch(`${BASE_URL}/${id}/tags`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify({ tags }),
  });

 if (!res.ok) {
    const msg = await res.text();
    return { success: false, message: msg };
  }

  const data = await res.json();
  return { success: true, data };
};

/* ============================= */
/* ❌ REMOVE TAG                 */
/* ============================= */
export const removeTag = async (id: number, tagName: string) => {
  const res = await fetch(`${BASE_URL}/${id}/tags/${tagName}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
  });

   if (!res.ok) {
    const msg = await res.text();
    return { success: false, message: msg };
  }

  return { success: true };
};

/* ============================= */
/* 🔁 UPDATE ALIAS               */
/* ============================= */
export const updateAlias = async (id: number, newAlias: string) => {
  const res = await fetch(`${BASE_URL}/${id}/alias`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify({ newAlias }),
  });

  if (!res.ok) {
    let message = "Something went wrong";

    try {
      const errorData = await res.json();
      message = errorData.message || message;
    } catch {
      message = await res.text();
    }

    return { success: false, message };
  }

  const data = await res.json();
  return { success: true, data };
};

/* ============================= */
/* 🔁 UPDATE DESTINATION         */
/* ============================= */
export const updateDestination = async (
  id: number,
  newLongUrl: string
) => {
  const res = await fetch(`${BASE_URL}/${id}/destination`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify({ newLongUrl }),
  });

   if (!res.ok) {
    const msg = await res.text();
    return { success: false, message: msg };
  }

  const data = await res.json();
  return { success: true, data };
};

/* ============================= */
/* ✏ RENAME TAG                 */
/* ============================= */
export const renameTag = async (
  id: number,
  oldTag: string,
  newTag: string
) => {
  const res = await fetch(`${BASE_URL}/${id}/tags/${oldTag}`, {
    method: "PUT",
    headers: getAuthHeaders(),
    body: JSON.stringify({ newTag }),
  });

   if (!res.ok) {
    const msg = await res.text();
    return { success: false, message: msg };
  }

  return { success: true };
};

/* ============================= */
/* 🗑 DELETE URL                 */
/* ============================= */
export const deleteUrl = async (id: number) => {
  const res = await fetch(`${BASE_URL}/${id}`, {
    method: "DELETE",
    headers: getAuthHeaders(),
  });

   if (!res.ok) {
    const msg = await res.text();
    return { success: false, message: msg };
  }

  return { success: true };
};