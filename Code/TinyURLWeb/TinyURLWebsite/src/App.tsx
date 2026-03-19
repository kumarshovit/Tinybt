import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import Register from "./pages/Register";
import Login from "./pages/Login";
import VerifyEmail from "./pages/Verify";
import Dashboard from "./pages/Dashboard";
import ForgotPassword from "./pages/ForgotPassword";
import ResetPassword from "./pages/ResetPassword";
import AdminPage from "./pages/AdminPage";
import ProfilePage from "./pages/ProfilePage";
import TagManagement from "./pages/TagManagement";
import MyAnalytics from "./pages/MyAnalytics";
import AnalysisPage from "./pages/AnalysisPage";
import LandingPage from "./pages/LandingPage";

import ProtectedRoute from "./routes/ProtectedRoute";
import AdminRoute from "./components/auth/AdminRoute";

export default function App() {

  const token = localStorage.getItem("token");

  return (
    <BrowserRouter>

      <Routes>

        {/* Landing */}
        <Route path="/" element={<LandingPage />} />

        {/* Auth Pages */}
        <Route
          path="/login"
          element={token ? <Navigate to="/dashboard" /> : <Login />}
        />

        <Route
          path="/register"
          element={token ? <Navigate to="/dashboard" /> : <Register />}
        />

        <Route path="/verify-email" element={<VerifyEmail />} />

        <Route path="/forgot-password" element={<ForgotPassword />} />

        <Route path="/reset-password" element={<ResetPassword />} />


        {/* Protected Routes */}
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />

        <Route
          path="/analysis"
          element={
            <ProtectedRoute>
              <AnalysisPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <ProfilePage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/tags/:id"
          element={
            <ProtectedRoute>
              <TagManagement />
            </ProtectedRoute>
          }
        />

        <Route
          path="/my-analytics"
          element={
            <ProtectedRoute>
              <MyAnalytics />
            </ProtectedRoute>
          }
        />

        {/* Admin */}
        <Route
          path="/admin"
          element={
            <AdminRoute>
              <AdminPage />
            </AdminRoute>
          }
        />

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/" />} />

      </Routes>

    </BrowserRouter>
  );
}

// import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
// import Register from "./pages/Register";
// import Login from "./pages/Login";
// import VerifyEmail from "./pages/Verify";
// import ProtectedRoute from "./routes/ProtectedRoute";
// import Dashboard from "./pages/Dashboard";
// import ForgotPassword from "./pages/ForgotPassword";
// import ResetPassword from "./pages/ResetPassword";
// import AdminRoute from "./components/auth/AdminRoute";
// import AdminPage from "./pages/AdminPage";
// import ProfilePage from "./pages/ProfilePage";
// import TagManagement from "./pages/TagManagement";
// import AnalysisPage from "./pages/AnalysisPage";

// export default function App() {
//   return (
//     <BrowserRouter>
//       <Routes>

//         {/* Public */}
//         <Route path="/" element={<Navigate to="/login" />} />
//         <Route path="/register" element={<Register />} />
//         <Route path="/login" element={<Login />} />
//         <Route path="/verify-email" element={<VerifyEmail />} />
//         <Route path="/forgot-password" element={<ForgotPassword />} />
//         <Route path="/reset-password" element={<ResetPassword />} />

//         {/* Protected */}
//         <Route
//           path="/dashboard"
//           element={
//             <ProtectedRoute>
//               <Dashboard />
//             </ProtectedRoute>
//           }
//         />

//         <Route
//           path="/analysis"
//           element={
//             <ProtectedRoute>
//               <AnalysisPage />
//             </ProtectedRoute>
//           }
//         />

//         <Route
//           path="/my-analytics"
//           element={
//             <ProtectedRoute>
//               <AnalysisPage />
//             </ProtectedRoute>
//           }
//         />

//         <Route
//           path="/profile"
//           element={
//             <ProtectedRoute>
//               <ProfilePage />
//             </ProtectedRoute>
//           }
//         />

//         <Route
//           path="/tags/:id"
//           element={
//             <ProtectedRoute>
//               <TagManagement />
//             </ProtectedRoute>
//           }
//         />

//         {/* Admin */}
//         <Route
//           path="/admin"
//           element={
//             <AdminRoute>
//               <AdminPage />
//             </AdminRoute>
//           }
//         />

//         {/* Fallback */}
//         <Route path="*" element={<Navigate to="/login" />} />

//       </Routes>
//     </BrowserRouter>
//   );
// }