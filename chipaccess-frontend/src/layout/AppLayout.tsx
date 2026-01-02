import { useAtom } from "jotai";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { logoutAtom, userAtom } from "../atoms/authAtoms";
import "./AppLayout.css";
import UserMenu from "../components/UserMenu/UserMenu";
import { useEffect, useState } from "react";
import { getMyPendingReassignments } from "../api/reassignmentApi";
import PendingReassignmentsModal from "../components/PendingReassignmentsModal/PendingReassignmentsModal";

export default function AppLayout() {
    const [user] = useAtom(userAtom);
    const [, logout] = useAtom(logoutAtom);
    const navigate = useNavigate();

    const [pendingReassignments, setPendingReassignments] = useState<any[]>([]);
    const [showPendingModal, setShowPendingModal] = useState(false);
    const [checkedPending, setCheckedPending] = useState(false);

    useEffect(() => {
        if (!user || checkedPending) return;

        getMyPendingReassignments()
            .then(items => {
                if (items.length > 0) {
                    setPendingReassignments(items);
                    setShowPendingModal(true);
                }
            })
            .finally(() => setCheckedPending(true));
    }, [user, checkedPending]);

    function handleLogout() {
        logout();
        navigate("/login", { replace: true });
    }

    const role = (user?.role ?? "").toLowerCase();

    return (
        <div className="layout-container">
            <nav className="layout-nav">
                <div className="layout-left">
                    <div className="layout-title">ChipAccessForm</div>

                    {user && (
                        <div className="layout-links">
                            <NavLink
                                to="/accesses"
                                className={({ isActive }) =>
                                    isActive ? "nav-link active" : "nav-link"
                                }
                            >
                                Active Requests
                            </NavLink>

                            {(role.includes("admin") || role.includes("it")) && (
                                <NavLink
                                    to="/archive"
                                    className={({ isActive }) =>
                                        isActive ? "nav-link active" : "nav-link"
                                    }
                                >
                                    Archive
                                </NavLink>
                            )}
                        </div>
                    )}
                </div>

                <div className="layout-user">
                    {user && (
                        <UserMenu
                            bamId={user.bamId}
                            role={user.role}
                            onLogout={handleLogout}
                        />
                    )}
                </div>
            </nav>

            <main className="layout-content">
                <Outlet />
            </main>

            {showPendingModal && (
                <PendingReassignmentsModal
                    items={pendingReassignments}
                    onDone={() => {
                        setShowPendingModal(false);
                        setPendingReassignments([]);
                    }}
                />
            )}
        </div>
    );
}
