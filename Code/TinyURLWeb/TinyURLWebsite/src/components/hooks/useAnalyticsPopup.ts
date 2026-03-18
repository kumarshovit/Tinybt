import { useState } from "react";

export default function useAnalyticsPopup(){

const [openPopup,setOpenPopup] = useState(false);
const [popupTitle,setPopupTitle] = useState("");
const [popupData,setPopupData] = useState([]);

const openDataPopup = (title: string, data: any)=>{

setPopupTitle(title);
setPopupData(data);
setOpenPopup(true);

};

return{
openPopup,
setOpenPopup,
popupTitle,
setPopupTitle,
popupData,
setPopupData,
openDataPopup
}

}