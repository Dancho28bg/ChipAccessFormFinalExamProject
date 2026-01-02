import { useState } from "react";
import ReassignAccessModal from "../ReassignAccessModal/ReassignAccessModal";
import AuditLogExportModal from "../AuditLogExportModal/AuditLogExportModal";
import "./UserMenu.css";

interface Props {
    bamId: string;
    role: string;
    onLogout: () => void;
}

export default function UserMenu({ bamId, role, onLogout }: Props) {
    const [open, setOpen] = useState(false);
    const [showReassignModal, setShowReassignModal] = useState(false);
    const [showAuditModal, setShowAuditModal] = useState(false);

    const isAdmin = role.toLowerCase().includes("admin");

    function closeMenu() {
        setOpen(false);
    }

    return (
        <div className="user-menu">
            <button
                className="user-menu-trigger"
                onClick={() => setOpen(prev => !prev)}
                aria-haspopup="menu"
            >
                <div className="user-avatar">
                    {bamId.charAt(0).toUpperCase()}
                </div>

                <div className="user-text">
                    <div className="user-name">{bamId}</div>
                    <div className={`user-role role-${role.toLowerCase()}`}>
                        {role}
                    </div>
                </div>

                <span className={`chevron ${open ? "open" : ""}`}>▾</span>
            </button>

            {open && (
                <div className="menu-dropdown">
                    {isAdmin && (
                        <>
                            <button
                                onClick={() => {
                                    setShowReassignModal(true);
                                    closeMenu();
                                }}
                            >
                                Reassign Access
                            </button>

                            <button
                                onClick={() => {
                                    setShowAuditModal(true);
                                    closeMenu();
                                }}
                            >
                                Export Audit Log
                            </button>
                        </>
                    )}

                    <div className="menu-separator" />

                    <button onClick={onLogout} className="danger">
                        Logout
                    </button>
                </div>
            )}

            {showReassignModal && (
                <ReassignAccessModal
                    onClose={() => setShowReassignModal(false)}
                />
            )}

            {showAuditModal && (
                <AuditLogExportModal
                    onClose={() => setShowAuditModal(false)}
                />
            )}
        </div>
    );
}
