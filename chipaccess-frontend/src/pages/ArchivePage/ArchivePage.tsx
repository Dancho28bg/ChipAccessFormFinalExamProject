import { useEffect, useState } from "react";
import { useAtom } from "jotai";
import { userAtom } from "../../atoms/authAtoms";
import type { ArchiveItem } from "../../types/archive";
import { fetchArchive } from "../../api/archiveApi";
import { FaSearch, FaSortUp, FaSortDown } from "react-icons/fa";
import "./ArchivePage.css";

type SortKey = keyof ArchiveItem;

export default function ArchivePage() {
    const [items, setItems] = useState<ArchiveItem[]>([]);
    const [search, setSearch] = useState("");
    const [sortKey, setSortKey] = useState<SortKey>("archivedAt");
    const [asc, setAsc] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [user] = useAtom(userAtom);

    useEffect(() => {
        if (!user) return;

        const role = (user.role ?? "").toLowerCase();
        if (!role.includes("admin") && !role.includes("it")) {
            setError("Unauthorized");
            setLoading(false);
            return;
        }

        fetchArchive()
            .then(setItems)
            .catch(() => setError("Failed to load archive"))
            .finally(() => setLoading(false));
    }, [user]);

    const handleSort = (key: SortKey) => {
        if (key === sortKey) {
            setAsc(!asc);
        } else {
            setSortKey(key);
            setAsc(true);
        }
    };

    const arrow = (key: SortKey) =>
        sortKey === key ? asc ? <FaSortUp /> : <FaSortDown /> : null;

    const filtered = items
        .filter((a) =>
            `
            ${a.bamId}
            ${a.approver}
            ${a.accessNeeded}
            ${a.reason}
            ${a.finalStatus}
        `.toLowerCase().includes(search.toLowerCase())
        )
        .sort((a, b) => {
            const A = a[sortKey];
            const B = b[sortKey];
            if (!A || !B) return 0;
            if (A < B) return asc ? -1 : 1;
            if (A > B) return asc ? 1 : -1;
            return 0;
        });

    return (
        <div className="archive-container">
            <h2>Archived Access Requests</h2>

            <div className="archive-actions">
                <div className="search-box">
                    <FaSearch />
                    <input
                        placeholder="Search archive..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                    />
                </div>
            </div>

            {loading && <p>Loading...</p>}
            {error && <p className="error">{error}</p>}

            {!loading && filtered.length > 0 && (
                <table className="archive-table">
                    <thead>
                        <tr>
                            <th onClick={() => handleSort("bamId")}>BamId {arrow("bamId")}</th>
                            <th onClick={() => handleSort("approver")}>Approver {arrow("approver")}</th>
                            <th onClick={() => handleSort("accessNeeded")}>Access {arrow("accessNeeded")}</th>
                            <th onClick={() => handleSort("finalStatus")}>Status {arrow("finalStatus")}</th>
                            <th onClick={() => handleSort("archivedAt")}>Archived date {arrow("archivedAt")}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filtered.map((a) => (
                            <tr key={a.id}>
                                <td>{a.bamId}</td>
                                <td>{a.approver}</td>
                                <td>{a.accessNeeded}</td>
                                <td>{a.finalStatus}</td>
                                <td>{new Date(a.archivedAt).toLocaleDateString()}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}
