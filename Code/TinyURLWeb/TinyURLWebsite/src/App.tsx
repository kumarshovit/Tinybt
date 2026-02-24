import { BrowserRouter, Routes, Route } from "react-router-dom";
import Dashboard from "./pages/Dashboard";
import TagManagement from "./pages/TagManagement";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/tags/:id" element={<TagManagement />} />
      </Routes>
    </BrowserRouter>
  );
}
