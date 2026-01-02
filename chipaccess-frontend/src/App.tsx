import { Routes, Route, Navigate } from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import ProtectedRoute from "./components/ProtectedRoute/ProtectedRoute";

import LoginPage from "./pages/LoginPage/LoginPage";
import MyAccessesPage from "./pages/MyAccessesPage/MyAccessesPage";
import ArchivePage from "./pages/ArchivePage/ArchivePage";
import DashboardPage from "./pages/DashboardPage/DashboardPage";

export default function App() {
    return (
        <Routes>
            <Route path="/login" element={<LoginPage />} />

            <Route
                element={
                    <ProtectedRoute>
                        <AppLayout />
                    </ProtectedRoute>
                }
            >
                <Route path="/" element={<Navigate to="/accesses" />} />
                <Route path="/accesses" element={<MyAccessesPage />} />
                <Route path="/archive" element={<ArchivePage />} />
                <Route path="/dashboard" element={<DashboardPage />} />
            </Route>
        </Routes>
    );
}
