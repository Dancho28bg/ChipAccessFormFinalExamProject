import { useState } from "react";
import {
    acceptReassignment,
    rejectReassignment,
} from "../../api/reassignmentApi";
import "./PendingReassignmentsModal.css";

export interface Reassignment {
    id: number;
    oldBamId: string;
    requestedBy: string;
    requestedAt: string;
}

interface Props {
    items: Reassignment[];
    onDone: () => void;
}

export default function PendingReassignmentsModal({
    items,
    onDone,
}: Props) {
    const [loadingId, setLoadingId] = useState<number | null>(null);
    const [rejectingId, setRejectingId] = useState<number | null>(null);
    const [rejectReason, setRejectReason] = useState("");

    async function handleAccept(id: number) {
        try {
            setLoadingId(id);
            await acceptReassignment(id);
            window.location.reload(); // 🔥 force UI refresh

        } finally {
            setLoadingId(null);
        }
    }

    async function handleReject(id: number) {
        if (!rejectReason.trim()) {
            alert("Please provide a reason for rejection.");
            return;
        }

        try {
            setLoadingId(id);
            await rejectReassignment(id, rejectReason);
            window.location.reload();
        } finally {
            setLoadingId(null);
            setRejectingId(null);
            setRejectReason("");
        }
    }

    return (
        <div className="modal-backdrop">
            <div className="modal">
                <h2>Pending Access Reassignments</h2>

                <p>
                    The following access entries were reassigned to you.
                    Please accept or reject them.
                </p>

                <ul className="pending-list">
                    {items.map(item => (
                        <li key={item.id} className="pending-item">
                            <div>
                                <strong>From:</strong> {item.oldBamId}
                            </div>
                            <div>
                                <strong>Requested by:</strong> {item.requestedBy}
                            </div>

                            <div className="actions">
                                <button
                                    onClick={() => handleAccept(item.id)}
                                    disabled={loadingId === item.id}
                                >
                                    {loadingId === item.id
                                        ? "Applying..."
                                        : "Accept"}
                                </button>

                                <button
                                    onClick={() => setRejectingId(item.id)}
                                    disabled={loadingId === item.id}
                                >
                                    Reject
                                </button>
                            </div>

                            {rejectingId === item.id && (
                                <div className="reject-box">
                                    <textarea
                                        placeholder="Reason for rejection"
                                        value={rejectReason}
                                        onChange={e =>
                                            setRejectReason(e.target.value)
                                        }
                                    />
                                    <div className="actions">
                                        <button
                                            onClick={() =>
                                                handleReject(item.id)
                                            }
                                        >
                                            Confirm Reject
                                        </button>
                                        <button
                                            onClick={() =>
                                                setRejectingId(null)
                                            }
                                        >
                                            Cancel
                                        </button>
                                    </div>
                                </div>
                            )}
                        </li>
                    ))}
                </ul>

                <button onClick={onDone} className="close-btn">
                    Close
                </button>
            </div>
        </div>
    );
}
