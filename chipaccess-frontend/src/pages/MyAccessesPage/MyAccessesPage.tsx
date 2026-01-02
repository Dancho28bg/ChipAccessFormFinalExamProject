import { useEffect, useMemo, useState } from "react";
import { useAtom } from "jotai";
import { userAtom } from "../../atoms/authAtoms";
import type { AccessItem } from "../../types/access";
import "./MyAccessesPage.css";

import {
    fetchMyAccesses,
    fetchManagedByMe,
    fetchAllAccesses,
    approveAccess,
    rejectAccess,
    revokeAccess,
} from "../../api/accessApi";

import {
    FaCheck,
    FaTimes,
    FaPowerOff,
    FaPlus,
    FaEdit,
    FaSearch,
    FaSortUp,
    FaSortDown,
} from "react-icons/fa";

import CreateAccessModal from "./components/CreateAccessModal";
import EditAccessModal from "./components/EditAccessModal";

const MANAGER_VISIBLE_STATUSES = [
    "PendingManager",
    "PendingAdmin",
    "Active",
    "ExpiringSoon",
];

type SortKey = keyof AccessItem;

export default function MyAccessesPage() {
    const [items, setItems] = useState<AccessItem[]>([]);
    const [expanded, setExpanded] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [showModal, setShowModal] = useState(false);
    const [editItem, setEditItem] = useState<AccessItem | null>(null);
    const [search, setSearch] = useState("");

    const [sortKey, setSortKey] = useState<SortKey>("createdDate");
    const [sortDir, setSortDir] = useState<"asc" | "desc">("desc");

    const [user] = useAtom(userAtom);

    useEffect(() => {
        async function load() {
            if (!user) return;

            setLoading(true);
            setError("");

            try {
                const role = (user.role ?? "").toLowerCase();
                let data: AccessItem[] = [];

                if (role.includes("admin") || role.includes("it")) {
                    data = await fetchAllAccesses();
                } else if (role.includes("manager")) {
                    const raw = await fetchManagedByMe();
                    data = raw.filter(a =>
                        MANAGER_VISIBLE_STATUSES.includes(a.status)
                    );
                } else {
                    data = await fetchMyAccesses();
                }

                setItems(data);
            } catch (err: any) {
                setError(err.message ?? "Failed to load accesses");
            } finally {
                setLoading(false);
            }
        }

        load();
    }, [user]);

    const reloadPage = () => location.reload();

    const handleApprove = async (id: number) => {
        await approveAccess(id);
        reloadPage();
    };

    const handleReject = async (id: number) => {
        await rejectAccess(id);
        reloadPage();
    };

    const handleRevoke = async (id: number) => {
        await revokeAccess(id);
        reloadPage();
    };

    const visibleItems = useMemo(() => {
        const q = search.toLowerCase();

        const filtered = items.filter((a) => {
            const haystack = `
                ${a.employeeName}
                ${a.bamId}
                ${a.approver}
                ${a.accessNeeded}
                ${a.reason}
                ${a.status}
            `.toLowerCase();

            return haystack.includes(q);
        });

        return [...filtered].sort((a, b) => {
            const av = a[sortKey];
            const bv = b[sortKey];

            if (!av || !bv) return 0;

            if (sortKey.includes("Date")) {
                const ad = new Date(av as string).getTime();
                const bd = new Date(bv as string).getTime();
                return sortDir === "asc" ? ad - bd : bd - ad;
            }

            return sortDir === "asc"
                ? String(av).localeCompare(String(bv))
                : String(bv).localeCompare(String(av));
        });
    }, [items, search, sortKey, sortDir]);

    const onSort = (key: SortKey) => {
        if (sortKey === key) {
            setSortDir(sortDir === "asc" ? "desc" : "asc");
        } else {
            setSortKey(key);
            setSortDir("asc");
        }
    };

    const SortIcon = ({ col }: { col: SortKey }) => {
        if (sortKey !== col) return null;
        return sortDir === "asc" ? <FaSortUp /> : <FaSortDown />;
    };

    return (
        <div className="my-accesses-container">
            <h2>Access Requests</h2>

            <div className="page-actions">
                <div className="search-box">
                    <FaSearch />
                    <input
                        type="text"
                        placeholder="Search access requests..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                    />
                </div>

                <button className="create-button" onClick={() => setShowModal(true)}>
                    <FaPlus /> Create Access Request
                </button>
            </div>

            {showModal && (
                <CreateAccessModal
                    onClose={() => setShowModal(false)}
                    onCreated={reloadPage}
                />
            )}

            {editItem && (
                <EditAccessModal
                    item={editItem}
                    onClose={() => setEditItem(null)}
                    onSaved={reloadPage}
                />
            )}

            {loading && <p>Loading...</p>}
            {error && <p className="error">{error}</p>}
            {!loading && visibleItems.length === 0 && <p>No requests found.</p>}

            {visibleItems.length > 0 && (
                <table className="access-table">
                    <thead>
                        <tr>
                            <th onClick={() => onSort("employeeName")}>Name <SortIcon col="employeeName" /></th>
                            <th onClick={() => onSort("bamId")}>BamId <SortIcon col="bamId" /></th>
                            <th onClick={() => onSort("approver")}>Approver <SortIcon col="approver" /></th>
                            <th onClick={() => onSort("accessNeeded")}>Access Needed <SortIcon col="accessNeeded" /></th>
                            <th>Reason</th>
                            <th onClick={() => onSort("createdDate")}>Created <SortIcon col="createdDate" /></th>
                            <th onClick={() => onSort("expirationDate")}>Expires <SortIcon col="expirationDate" /></th>
                            <th onClick={() => onSort("status")}>Status <SortIcon col="status" /></th>
                        </tr>
                    </thead>

                    <tbody>
                        {visibleItems.map((a) => {
                            const role = (user?.role ?? "").toLowerCase();

                            const isAdmin = role.includes("admin") || role.includes("it");
                            const isManager = role.includes("manager");
                            const isUser = !isAdmin && !isManager;

                            const canApprove =
                                (isManager && a.status === "PendingManager") ||
                                (isAdmin && a.status === "PendingAdmin");

                            const canReject = canApprove;

                            const canRevoke =
                                isAdmin &&
                                (a.status === "Active" || a.status === "ExpiringSoon");

                            const canEdit =
                                isUser &&
                                a.bamId === user?.bamId &&
                                (a.status === "PendingManager" || a.status === "PendingAdmin");

                            return (
                                <>
                                    <tr
                                        key={a.id}
                                        className="clickable-row"
                                        onClick={() => setExpanded(expanded === a.id ? null : a.id)}
                                    >
                                        <td>{a.employeeName}</td>
                                        <td>{a.bamId}</td>
                                        <td>{a.approver}</td>
                                        <td>{a.accessNeeded}</td>
                                        <td>{a.reason}</td>
                                        <td>{new Date(a.createdDate).toLocaleDateString()}</td>
                                        <td>
                                            {a.expirationDate
                                                ? new Date(a.expirationDate).toLocaleDateString()
                                                : "—"}
                                        </td>
                                        <td className={`status status-${a.status}`}>
                                            {a.status}
                                        </td>
                                    </tr>

                                    {expanded === a.id && (
                                        <tr className="expanded-row">
                                            <td colSpan={8}>
                                                <div className="action-panel">
                                                    {canEdit && (
                                                        <button
                                                            className="btn edit"
                                                            onClick={() => setEditItem(a)}
                                                        >
                                                            <FaEdit /> Edit
                                                        </button>
                                                    )}

                                                    {canApprove && (
                                                        <button
                                                            className="btn approve"
                                                            onClick={() => handleApprove(a.id)}
                                                        >
                                                            <FaCheck /> Approve
                                                        </button>
                                                    )}

                                                    {canReject && (
                                                        <button
                                                            className="btn reject"
                                                            onClick={() => handleReject(a.id)}
                                                        >
                                                            <FaTimes /> Reject
                                                        </button>
                                                    )}

                                                    {canRevoke && (
                                                        <button
                                                            className="btn revoke"
                                                            onClick={() => handleRevoke(a.id)}
                                                        >
                                                            <FaPowerOff /> Revoke Access
                                                        </button>
                                                    )}

                                                    {!canEdit && !canApprove && !canRevoke && (
                                                        <span className="muted-action">
                                                            No actions available
                                                        </span>
                                                    )}
                                                </div>
                                            </td>
                                        </tr>
                                    )}
                                </>
                            );
                        })}
                    </tbody>
                </table>
            )}
        </div>
    );
}
