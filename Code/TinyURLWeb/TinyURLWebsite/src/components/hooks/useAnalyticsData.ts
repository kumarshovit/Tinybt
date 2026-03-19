import { useState, useEffect } from "react";

const API_BASE = import.meta.env.VITE_API_URL;

export default function useAnalyticsData({
  from,
  to,
  selectedLink,
  selectedTag,
  userId
}: any) {

  /* ---------- DATA STATES ---------- */
  const [allLinks, setAllLinks] = useState<any[]>([]);
  const [allTags, setAllTags] = useState<string[]>([]);

  const [clicks, setClicks] = useState<any[]>([]);
  const [referrer, setReferrer] = useState<any[]>([]);
  const [country, setCountry] = useState<any[]>([]);
  const [device, setDevice] = useState<any[]>([]);
  const [os, setOs] = useState<any[]>([]);
  const [browser, setBrowser] = useState<any[]>([]);
  const [topLinks, setTopLinks] = useState<any[]>([]);
  const [language, setLanguage] = useState<any[]>([]);
  const [totalClicks, setTotalClicks] = useState(0);
  const [linkClicks, setLinkClicks] = useState<any[]>([]);
  const [heatmap, setHeatmap] = useState<any[]>([]);

  const token = localStorage.getItem("token");

  const headers = {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`
  };

  /* ---------- FETCH USER LINKS ---------- */
  const fetchUserLinks = async () => {

    const isAdminSelectingUser = userId !== undefined && userId !== null;

    const url = isAdminSelectingUser
      ? `${API_BASE}/api/admin/urls`
      : `${API_BASE}/api/urls`;

    const options: any = {
      method: isAdminSelectingUser ? "POST" : "GET",
      headers
    };

    if (isAdminSelectingUser) {
      options.body = JSON.stringify({ userId });
    }

    const res = await fetch(url, options);
    const data = await res.json();

    setAllLinks(data);

    const tagSet = new Set<string>();

    data.forEach((link: any) => {
      if (link.tags) {
        link.tags.forEach((tag: string) => tagSet.add(tag));
      }
    });

    setAllTags(Array.from(tagSet));
  };

  /* ---------- FETCH DASHBOARD ---------- */
  const fetchDashboard = async () => {

    const body = {
      from,
      to,
      link: selectedLink,
      tag: selectedTag,
      ...(userId && { userId })
    };

    try {

      /* ---------- CLICKS ---------- */
      const clicksRes = await fetch(`${API_BASE}/analytics/user/clicks-over-time`, {
        method: "POST",
        headers,
        body: JSON.stringify(body)
      });

      const clicksData = await clicksRes.json();
      setClicks(clicksData);

      let total = 0;
      clicksData.forEach((x: any) => total += x.count);
      setTotalClicks(total);

      /* ---------- BREAKDOWN ---------- */
      const breakdown = async (type: string) => {
        const res = await fetch(`${API_BASE}/analytics/breakdown`, {
          method: "POST",
          headers,
          body: JSON.stringify({
            from,
            to,
            type,
            link: selectedLink,
            tag: selectedTag,
            ...(userId && { userId })
          })
        });
        return res.json();
      };

      setReferrer(await breakdown("referrer"));
      setCountry(await breakdown("country"));
      setDevice(await breakdown("device"));
      setOs(await breakdown("os"));
      setBrowser(await breakdown("browser"));

      /* ---------- LANGUAGE ---------- */
      const langRes = await fetch(`${API_BASE}/analytics/user/device-language`, {
        method: "POST",
        headers,
        body: JSON.stringify(body)
      });

      setLanguage(await langRes.json());

      /* ---------- TOP LINKS ---------- */
      const linksRes = await fetch(`${API_BASE}/analytics/user/popular-links`, {
        method: "POST",
        headers,
        body: JSON.stringify(body)
      });

      const linksData = await linksRes.json();

      // ✅ IMPORTANT FIX
      setLinkClicks(linksData);

      const aliasClicks: Record<string, number> = {};
      linksData.forEach((x: any) => {
        aliasClicks[x.label] = x.count;
      });

      const tagCounts: Record<string, number> = {};

      allLinks.forEach((link: any) => {
        const clicks = aliasClicks[link.shortCode] || 0;

        if (link.tags) {
          link.tags.forEach((tag: string) => {
            if (!tagCounts[tag]) tagCounts[tag] = 0;
            tagCounts[tag] += clicks;
          });
        }
      });

      const tagData = Object.keys(tagCounts)
        .map(tag => ({
          label: tag,
          count: tagCounts[tag]
        }))
        .sort((a, b) => b.count - a.count);

      setTopLinks(tagData);

      /* ---------- 🔥 HEATMAP ---------- */
      const heatmapRes = await fetch(`${API_BASE}/analytics/heatmap`, {
        method: "POST",
        headers,
        body: JSON.stringify({
          from,
          to,
          ...(userId && { userId })
        })
      });

      const heatmapData = await heatmapRes.json();
      setHeatmap(heatmapData);

    } catch (err) {
      console.error("Analytics error", err);
    }
  };

  /* ---------- LOAD ---------- */
  useEffect(() => {
    fetchUserLinks();
  }, [userId]);

  useEffect(() => {
    if (allLinks.length) {
      fetchDashboard();
    }
  }, [from, to, selectedLink, selectedTag, allLinks, userId]);

  /* ---------- FILTERED LINKS ---------- */
  const filteredLinks = allLinks.filter((link: any) => {

    const hasClicks = linkClicks.some(
      (x: any) => x.label === link.shortCode
    );

    const linkMatch =
      !selectedLink || link.shortCode === selectedLink;

    const tagMatch =
      !selectedTag ||
      (link.tags && link.tags.includes(selectedTag));

    return hasClicks && linkMatch && tagMatch;
  });

  /* ---------- ALL LINKS POPUP ---------- */
  const openAllLinksPopup = async () => {

    const clickMap: Record<string, number> = {};

    linkClicks.forEach((x: any) => {
      clickMap[x.label] = x.count;
    });

    return filteredLinks.map((link: any) => ({
      shortUrl: `${API_BASE}/${link.shortCode}`,
      shortCode: link.shortCode,
      clickCount: clickMap[link.shortCode] || 0
    }));
  };

const openTagPopup = async (tag: string) => {

  const body = {
    from,
    to,
    tag,
    ...(userId && { userId })
  };

  const res = await fetch(
    `${API_BASE}/analytics/user/popular-links`,
    {
      method: "POST",
      headers,
      body: JSON.stringify(body)
    }
  );

  const data = await res.json();

  // ✅ FIX: ensure array
  if (!Array.isArray(data)) {
    console.error("Invalid popup data:", data);
    return [];
  }

  return data.map((x: any) => ({
    shortUrl: `${API_BASE}/${x.label}`,
    shortCode: x.label,
    clickCount: x.count
  }));
};
  /* ---------- RETURN ---------- */
  return {
    allLinks,
    allTags,
    clicks,
    referrer,
    country,
    device,
    os,
    browser,
    language,
    topLinks,
    totalClicks,
    linkClicks,
    totalUrls: filteredLinks.length,
    heatmap,
    openAllLinksPopup,
    openTagPopup
  };
}