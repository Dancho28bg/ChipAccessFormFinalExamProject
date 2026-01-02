import { useEffect, useState } from "react";
import { searchBamIds } from "../../api/employeeApi";
import { createReassignment } from "../../api/reassignmentApi";
import "./ReassignAccessModal.css";

interface Props {
    onClose: () => void;
}

export default function ReassignAccessModal({ onClose }: Props) {
    const [oldQuery, setOldQuery] = useState("");
    const [newQuery, setNewQuery] = useState("");

    const [oldOptions, setOldOptions] = useState<string[]>([]);
    const [newOptions, setNewOptions] = useState<string[]>([]);

    const [oldBamId, setOldBamId] = useState("");
    const [newBamId, setNewBamId] = useState("");

    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (oldQuery.trim()) searchBamIds(oldQuery).then(setOldOptions);
        else setOldOptions([]);
    }, [oldQuery]);

    useEffect(() => {
        if (newQuery.trim()) searchBamIds(newQuery).then(setNewOptions);
        else setNewOptions([]);
    }, [newQuery]);

    async function handleSubmit() {
        setError("");

        if (!oldBamId || !newBamId) {
            setError("Both BAM IDs are required.");
            return;
        }

        if (oldBamId === newBamId) {
            setError("Old and New BAM ID cannot be the same.");
            return;
        }

        try {
            setLoading(true);
            await createReassignment(oldBamId, newBamId);
            onClose();
        } catch {
            setError("Failed to create reassignment.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="modal-backdrop">
            <div className="modal">
                <div className="modal-header">
                    <div className="modal-title">Reassign Access</div>
                    <button className="close-btn" onClick={onClose} aria-label="Close">
                        ×
                    </button>
                </div>

                <div className="field">
                    <div className="field-label">Old BAM ID</div>
                    <input
                        value={oldQuery}
                        onChange={(e) => setOldQuery(e.target.value)}
                        placeholder="Search BAM ID…"
                    />
                    {oldOptions.length > 0 && (
                        <ul className="dropdown">
                            {oldOptions.map((b) => (
                                <li
                                    key={b}
                                    className={b === oldBamId ? "selected" : ""}
                                    onClick={() => {
                                        setOldBamId(b);
                                        setOldQuery(b);
                                        setOldOptions([]);
                                    }}
                                >
                                    {b}
                                </li>
                            ))}
                        </ul>
                    )}
                </div>

                <div className="field">
                    <div className="field-label">New BAM ID</div>
                    <input
                        value={newQuery}
                        onChange={(e) => setNewQuery(e.target.value)}
                        placeholder="Search BAM ID…"
                    />
                    {newOptions.length > 0 && (
                        <ul className="dropdown">
                            {newOptions.map((b) => (
                                <li
                                    key={b}
                                    className={b === newBamId ? "selected" : ""}
                                    onClick={() => {
                                        setNewBamId(b);
                                        setNewQuery(b);
                                        setNewOptions([]);
                                    }}
                                >
                                    {b}
                                </li>
                            ))}
                        </ul>
                    )}
                </div>

                {error && <div className="error">{error}</div>}

                <div className="actions">
                    <button
                        className="primary"
                        onClick={handleSubmit}
                        disabled={loading}
                    >
                        {loading ? "Submitting…" : "Submit"}
                    </button>
                </div>
            </div>
        </div>
    );
}
