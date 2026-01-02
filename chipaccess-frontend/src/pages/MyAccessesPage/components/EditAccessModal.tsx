import { useState } from "react";
import "./CreateEditAccessModal.css";
import { updateAccessRequest } from "../../../api/accessApi";

interface Props {
    item: any;
    onClose: () => void;
    onSaved: () => void;
}

export default function EditAccessModal({ item, onClose, onSaved }: Props) {
    const [approver, setApprover] = useState(item.approver);
    const [accessNeeded, setAccessNeeded] = useState(item.accessNeeded);
    const [reason, setReason] = useState(item.reason);
    const [expirationDate, setExpirationDate] = useState(
        item.expirationDate ? item.expirationDate.substring(0, 10) : ""
    );

    const handleSubmit = async () => {
        await updateAccessRequest(item.id, {
            approver,
            accessNeeded,
            reason,
            expirationDate,
        });
        onSaved();
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

                <h3>Edit Access Request</h3>

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
                        Save Changes
                    </button>
                </div>
            </div>
        </div>
    );
}
