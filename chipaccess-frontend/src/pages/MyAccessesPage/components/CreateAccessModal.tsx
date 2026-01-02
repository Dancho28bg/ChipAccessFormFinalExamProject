import { useState } from "react";
import "./CreateEditAccessModal.css";
import { createAccessRequest } from "../../../api/accessApi";

interface Props {
    onClose: () => void;
    onCreated: () => void;
}

export default function CreateAccessModal({ onClose, onCreated }: Props) {
    const [approver, setApprover] = useState("");
    const [accessNeeded, setAccessNeeded] = useState("");
    const [reason, setReason] = useState("");
    const [expirationDate, setExpirationDate] = useState("");

    const handleSubmit = async () => {
        const dto = {
            approver,
            accessNeeded,
            reason,
            expirationDate,
        };

        await createAccessRequest(dto);
        onCreated();
        onClose();
    };

    return (
        <div className="modal-overlay">
            <div className="modal-box">
                <button
                    className="modal-close"
                    onClick={onClose}
                    aria-label="Close"
                >
                    ×
                </button>

                <h3>Create Access Request</h3>

                <label>Approver BamID</label>
                <input
                    type="text"
                    value={approver}
                    onChange={(e) => setApprover(e.target.value)}
                />

                <label>Access Needed</label>
                <input
                    type="text"
                    value={accessNeeded}
                    onChange={(e) => setAccessNeeded(e.target.value)}
                />

                <label>Reason</label>
                <textarea
                    value={reason}
                    onChange={(e) => setReason(e.target.value)}
                />

                <label>Expiration Date</label>
                <input
                    type="date"
                    value={expirationDate}
                    onChange={(e) => setExpirationDate(e.target.value)}
                />

                <div className="modal-actions">
                    <button className="submit" onClick={handleSubmit}>
                        Submit
                    </button>
                </div>
            </div>
        </div>
    );
}
