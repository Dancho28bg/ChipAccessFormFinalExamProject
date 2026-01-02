export interface AccessItem {
    id: number;
    bamId: string;
    employeeName: string;
    approver: string;
    accessNeeded: string;
    reason: string;
    createdDate: string;
    modifiedDate?: string;
    expirationDate?: string;
    revokedDate?: string;
    status: string;
}

export interface CreateAccessRequest {
    approver: string;
    accessNeeded: string;
    reason: string;
    expirationDate: string;
}

export interface UpdateAccessRequest {
    approver: string;
    accessNeeded: string;
    reason: string;
    expirationDate: string;
}
