import { useState } from "react";
import { queryAuditLogs, exportAuditLogs } from "../../api/auditLogApi";
import { last7Days, last30Days } from "../../utils/datePresets";
import "./AuditLogExportModal.css";

export default function AuditLogExportModal({ onClose }: { onClose: () => void }) {
    const [filters, setFilters] = useState({
        performedBy: "",
        from: null as string | null,
        to: null as string | null,
        page: 1,
        pageSize: 25,
    });

    const [logs, setLogs] = useState<any[]>([]);

    function applyPreset(preset: () => { from: Date; to: Date }) {
        const { from, to } = preset();
        setFilters({
            ...filters,
            from: from.toISOString(),
            to: to.toISOString(),
        });
    }

    async function handleSearch() {
        const result = await queryAuditLogs(filters);
        setLogs(result.items);
    }

    async function handleExport() {
        await exportAuditLogs(filters);
    }

    return (
        <div className="modal-backdrop">
            <div className="modal audit-modal"> 
                <div className="modal-header">
                    <h2>Audit Log</h2>
                    <button className="close-btn" onClick={onClose}>×</button>
                </div>

                <div className="filters">
                    <input
                        placeholder="Performed by (BAM ID)"
                        value={filters.performedBy}
                        onChange={e =>
                            setFilters({ ...filters, performedBy: e.target.value })
                        }
                    />

                    <input
                        type="date"
                        onChange={e =>
                            setFilters({
                                ...filters,
                                from: e.target.value
                                    ? new Date(e.target.value).toISOString()
                                    : null
                            })
                        }
                    />

                    <input
                        type="date"
                        onChange={e =>
                            setFilters({
                                ...filters,
                                to: e.target.value
                                    ? new Date(e.target.value).toISOString()
                                    : null
                            })
                        }
                    />
                </div>

                <div className="presets">
                    <button onClick={() => applyPreset(last7Days)}>Last 7 days</button>
                    <button onClick={() => applyPreset(last30Days)}>Last 30 days</button>
                </div>

                <div className="actions">
                    <button className="primary" onClick={handleSearch}>
                        Search
                    </button>
                    <button className="secondary" onClick={handleExport}>
                        Download TXT
                    </button>
                </div>

                <div className="log-preview">
                    <table>
                        <thead>
                            <tr>
                                <th>Time</th>
                                <th>By</th>
                                <th>Action</th>
                                <th>Target</th>
                                <th>Details</th>
                            </tr>
                        </thead>
                        <tbody>
                            {logs.map((l, i) => (
                                <tr key={i}>
                                    <td>{new Date(l.timestamp).toLocaleString()}</td>
                                    <td>{l.performedBy}</td>
                                    <td>{l.action}</td>
                                    <td>{l.targetType}</td>
                                    <td>{l.details}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    {logs.length === 0 && (
                        <div className="empty">No audit entries found</div>
                    )}
                </div>
            </div>
        </div>
    );
}
