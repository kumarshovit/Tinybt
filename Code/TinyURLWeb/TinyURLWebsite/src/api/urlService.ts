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
  captchaToken?: string,
  password?: string
) => {
  const res = await fetch(BASE_URL, {
    method: "POST",
    headers: getOptionalAuthHeaders(),
    body: JSON.stringify({
      longUrl,
      customAlias,
      expirationDate,
      captchaToken,
      password
    }),
  });

  if (!res.ok) {
    const err = await res.json();
    return { success: false, message: err.message };
  }

  const data = await res.json();
  return { success: true, data };
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

/* ============================= */
/* 🔐 VERIFY PASSWORD             */
/* ============================= */
/**
 * Submits the visitor-entered password and the server-issued access token.
 * credentials: "include" is required so the browser stores the HttpOnly cookie
 * that the API sets in the response.
 * Returns { success, message, redirectTo } — never the destination URL.
 */
export const verifyPassword = async (
  shortCode: string,
  password: string,
  token: string
) => {
  const res = await fetch(
    `${import.meta.env.VITE_API_URL}/api/urls/${shortCode}/verify-password`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",   // required to receive the HttpOnly cookie
      body: JSON.stringify({ password, token }),
    }
  );

  if (!res.ok) {
    try {
      const err = await res.json();
      return { success: false, message: err.message as string, redirectTo: undefined };
    } catch {
      return { success: false, message: "Something went wrong.", redirectTo: undefined };
    }
  }

  const data = await res.json();
  return { success: true, message: undefined, redirectTo: data.redirectTo as string };
};

/* ============================= */
/* 🛡️ DESTINATION PREVIEW        */
/* ============================= */

export interface UrlPreviewData {
  shortCode: string;
  destinationDomain: string;
  destinationUrl: string;
  isSafe: boolean;
  expirationDate?: string;
}

export interface UrlPreviewResult {
  success: boolean;
  isBlocked?: boolean;
  isExpired?: boolean;
  isNotFound?: boolean;
  message?: string;
  data?: UrlPreviewData;
}

export const getPreview = async (
  shortCode: string,
  token: string
): Promise<UrlPreviewResult> => {
  try {
    const res = await fetch(
      `${import.meta.env.VITE_API_URL}/api/urls/${shortCode}/preview?t=${encodeURIComponent(token)}`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!res.ok) {
      try {
        const err = await res.json();
        return {
          success: false,
          isBlocked: res.status === 451 || !!err.isBlocked,
          isExpired: res.status === 410,
          isNotFound: res.status === 404,
          message: err.message || "Failed to load destination preview.",
        };
      } catch {
        return {
          success: false,
          isBlocked: res.status === 451,
          isExpired: res.status === 410,
          isNotFound: res.status === 404,
          message: "Failed to load destination preview.",
        };
      }
    }

    const data: UrlPreviewData = await res.json();
    return {
      success: true,
      data,
    };
  } catch {
    return {
      success: false,
      message: "Network error. Please check your connection and try again.",
    };
  }
};

/* ============================= */
/* 🚀 CONTINUE REDIRECT          */
/* ============================= */

export interface ContinueRedirectResult {
  success: boolean;
  isBlocked?: boolean;
  message?: string;
  redirectTo?: string;
}

export const continueRedirect = async (
  shortCode: string,
  token: string
): Promise<ContinueRedirectResult> => {
  try {
    const res = await fetch(
      `${import.meta.env.VITE_API_URL}/api/urls/${shortCode}/continue`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ token }),
      }
    );

    if (!res.ok) {
      try {
        const err = await res.json();
        return {
          success: false,
          isBlocked: res.status === 451,
          message: err.message || "Redirection failed. Please try again.",
        };
      } catch {
        return {
          success: false,
          isBlocked: res.status === 451,
          message: "Redirection failed. Please try again.",
        };
      }
    }

    const data = await res.json();
    return {
      success: true,
      redirectTo: data.redirectTo as string,
    };
  } catch {
    return {
      success: false,
      message: "Network error. Please check your connection and try again.",
    };
  }
};