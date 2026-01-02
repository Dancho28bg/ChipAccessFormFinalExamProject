import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5206/api/AuditLog",
});

api.interceptors.request.use(config => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export async function queryAuditLogs(filters: any) {
    const res = await api.post("/query", filters);
    return res.data;
}

export async function exportAuditLogs(filters: any) {
    const res = await api.post("/export", filters, {
        responseType: "blob",
    });

    const url = window.URL.createObjectURL(res.data);
    const a = document.createElement("a");
    a.href = url;
    a.download = "audit-log.txt";
    a.click();
}
