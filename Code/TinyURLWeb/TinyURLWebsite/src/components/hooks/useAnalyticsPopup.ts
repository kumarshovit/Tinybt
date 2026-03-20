import { useState } from "react";
type PopupData = {
  shortUrl: string;
  shortCode: string;
  clickCount: number;
};
export default function useAnalyticsPopup(){

const [openPopup,setOpenPopup] = useState(false);
const [popupTitle,setPopupTitle] = useState("");
const [popupData,setPopupData] = useState<PopupData[]>([]);

const openDataPopup = (title: string, data: PopupData[])=>{

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