const BASE_URL = "https://localhost:57679/api/urls";

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
/* 📌 GET ALL URLS              */
/* ============================= */
export const getAllUrls = async () => {
  const res = await fetch(BASE_URL, {
    headers: getAuthHeaders(),
  });

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }

  return res.json();
};

/* ============================= */
/* ➕ CREATE URL                 */
/* ============================= */
export const createUrl = async (
  longUrl: string,
  customAlias?: string,
  expirationDate?: string
) => {
  const res = await fetch(BASE_URL, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify({
      longUrl,
      customAlias,
      expirationDate,
    }),
  });

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }

  return res.json();
};

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
    throw new Error(msg);
  }
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
    throw new Error(msg);
  }

  return res.json();
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
    throw new Error(msg);
  }
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
    throw new Error(msg);
  }
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
    const msg = await res.text();
    throw new Error(msg);
  }

  return res.json();
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
    throw new Error(msg);
  }

  return res.json();
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
    throw new Error(msg);
  }
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
    throw new Error(msg);
  }
};