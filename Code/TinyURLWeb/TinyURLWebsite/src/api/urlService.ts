const BASE_URL = "https://localhost:57679/api/urls";

export const getAllUrls = async () => {
  const res = await fetch(BASE_URL);
   if (!res.ok) throw new Error("Failed to fetch URLs");
  return res.json();
};

export const createUrl = async (
  longUrl: string,
  customAlias?: string,
  expirationDate?: string
) => {
  const res = await fetch(BASE_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      longUrl,
      customAlias,
      expirationDate
    })
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
  const res = await fetch(`${BASE_URL}/by-tag/${tag}`);
  return res.json();
};

export const updateTags = async (id: number, tags: string[]) => {
  await fetch(`${BASE_URL}/${id}/tags`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ tags })
  });
};

export const removeTag = async (id: number, tagName: string) => {
  await fetch(`${BASE_URL}/${id}/tags/${tagName}`, {
    method: "DELETE"
  });
};

export const updateAlias = async (id: number, newAlias: string) => {
  const res = await fetch(
    `${BASE_URL}/${id}/alias`,
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
    `${BASE_URL}/${id}/destination`,
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
    `${BASE_URL}/${id}/tags/${oldTag}`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ newTag })  
    }
  );

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }
};

 


export const deleteUrl = async (id: number) => {
  const res = await fetch(`${BASE_URL}/${id}`,{
    method: "DELETE"
  });

  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg);
  }
};
