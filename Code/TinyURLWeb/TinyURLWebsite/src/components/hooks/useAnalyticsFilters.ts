import { useState } from "react";

export default function useAnalyticsFilters(){

const today = new Date();
const lastWeek = new Date();
lastWeek.setDate(today.getDate()-6);

const [from,setFrom] = useState(lastWeek.toISOString().split("T")[0]);
const [to,setTo] = useState(today.toISOString().split("T")[0]);

const [selectedLink,setSelectedLink] = useState("");
const [selectedTag,setSelectedTag] = useState("");

const setToday = () => {
const today = new Date().toISOString().split("T")[0];
setFrom(today);
setTo(today);
};

const setLast7Days = () => {
const today = new Date();
const lastWeek = new Date();
lastWeek.setDate(today.getDate() - 6);

setFrom(lastWeek.toISOString().split("T")[0]);
setTo(today.toISOString().split("T")[0]);
};

const setLast30Days = () => {
const today = new Date();
const lastMonth = new Date();
lastMonth.setDate(today.getDate() - 29);

setFrom(lastMonth.toISOString().split("T")[0]);
setTo(today.toISOString().split("T")[0]);
};

return{
from,
to,
setFrom,
setTo,
selectedLink,
setSelectedLink,
selectedTag,
setSelectedTag,
setToday,
setLast7Days,
setLast30Days
}

}