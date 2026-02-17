const BASE_URL = "https://localhost:7025/api/url";

export const getAllUrls = async () => {
  const res = await fetch(`${BASE_URL}/all`);
  return res.json();
};

export const createUrl = async (longUrl: string, customAlias?: string) => {
  const res = await fetch(`${BASE_URL}/shorten`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ longUrl, customAlias })
  });

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }

  return res.json();
};

export const addTags = async (id: number, tags: string[]) => {
  await fetch(`${BASE_URL}/${id}/tags`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ tags })
  });
};

export const searchByTag = async (tag: string) => {
  const res = await fetch(`${BASE_URL}/search?tag=${tag}`);
  return res.json();
};

export const updateTags = async (id: number, tags: string[]) => {
  await fetch(`https://localhost:7025/api/url/${id}/tags`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ tags })
  });
};

export const removeTag = async (id: number, tagName: string) => {
  await fetch(`https://localhost:7025/api/url/${id}/tags/${tagName}`, {
    method: "DELETE"
  });
};

export const updateAlias = async (id: number, newAlias: string) => {
  const res = await fetch(
    `https://localhost:7025/api/url/${id}/alias`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ newAlias })
    }
  );

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }

  return res.json();
};

export const updateDestination = async (id: number, newLongUrl: string) => {
  const res = await fetch(
    `https://localhost:7025/api/url/${id}/destination`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ newLongUrl })
    }
  );

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }

  return res.json();
};

export const renameTag = async (
  id: number,
  oldTag: string,
  newTag: string
) => {
  const res = await fetch(
    `https://localhost:7025/api/url/${id}/tags/${oldTag}`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(newTag)
    }
  );

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }
};


export const deleteUrl = async (id: number) => {
  const res = await fetch(`${BASE_URL}/${id}`, {
    method: "DELETE"
  });

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }
};
