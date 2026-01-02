export interface ArchiveItem {
    id: number;
    bamId: string;
    employeeName: string;
    approver: string;
    accessNeeded: string;
    reason: string;
    createdDate: string;
    expirationDate?: string;
    revokedDate?: string;
    finalStatus: string;
    archivedAt: string;
}
