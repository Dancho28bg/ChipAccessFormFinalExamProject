import { useEffect, useState } from "react";
import { useAtom } from "jotai";
import { userAtom } from "../../atoms/authAtoms";
import { getMyAccesses, getAdminList } from "../../api/dashboardApi";
import "./DashboardPage.css";
import { useNavigate } from "react-router-dom";

export default function DashboardPage() {
    const [user] = useAtom(userAtom);
    const navigate = useNavigate();

    const [loading, setLoading] = useState(true);
    const [myCount, setMyCount] = useState<number>(0);
    const [adminCounts, setAdminCounts] = useState<Record<string, number>>({});
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function load() {
            try {
                setLoading(true);

                const my = await getMyAccesses();
                setMyCount(my.length || 0);

                const isAdmin =
                    user?.role?.toLowerCase() === "admin" ||
                    user?.role?.toLowerCase() === "itadmin";

                if (isAdmin) {
                    const statuses = ["Active", "PendingManager", "PendingAdmin", "Revoked"];
                    const results = await Promise.all(
                        statuses.map((s) => getAdminList(s).catch(() => []))
                    );

                    const map: Record<string, number> = {};
                    statuses.forEach((s, i) => {
                        map[s] = results[i].length || 0;
                    });
                    setAdminCounts(map);
                }
            } catch (err: any) {
                setError(err.message || "Failed to load dashboard");
            } finally {
                setLoading(false);
            }
        }

        load();
    }, [user]);

    function goto(path: string) {
        navigate(path);
    }

    return (
        <div className="dashboard-root">
            <header className="dashboard-header">
                <div>
                    <h1>Welcome{user?.bamId ? `, ${user.bamId}` : ""}!</h1>
                    <div className="muted">{user?.email ?? ""}</div>
                </div>

                <div className="quick-actions">
                    <button onClick={() => goto("/my-accesses")}>My Accesses</button>
                    <button onClick={() => goto("/create-request")}>Request Access</button>
                    {(user?.role?.toLowerCase() === "admin" ||
                        user?.role?.toLowerCase() === "itadmin") && (
                        <button className="admin" onClick={() => goto("/admin")}>
                            Admin Panel
                        </button>
                    )}
                </div>
            </header>

            <section className="dashboard-cards">
                <div className="card">
                    <div className="card-title">My access requests</div>
                    <div className="card-value">
                        {loading ? "…" : myCount}
                    </div>
                    <div className="card-sub">Total requests you've created</div>
                </div>

                {(user?.role?.toLowerCase() === "admin" ||
                    user?.role?.toLowerCase() === "itadmin") && (
                    <>
                        <div className="card">
                            <div className="card-title">Pending</div>
                            <div className="card-value">
                                {loading ? "…" : adminCounts["pending"] ?? 0}
                            </div>
                            <div className="card-sub">Requests waiting for approval</div>
                        </div>

                        <div className="card">
                            <div className="card-title">Active</div>
                            <div className="card-value">
                                {loading ? "…" : adminCounts["active"] ?? 0}
                            </div>
                            <div className="card-sub">Active accesses</div>
                        </div>

                        <div className="card">
                            <div className="card-title">Revoked</div>
                            <div className="card-value">
                                {loading ? "…" : adminCounts["revoked"] ?? 0}
                            </div>
                            <div className="card-sub">Revoked accesses</div>
                        </div>
                    </>
                )}
            </section>

            {error && <div className="dashboard-error">Error: {error}</div>}
        </div>
    );
}
