import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5206/api/Reassignment",
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export async function createReassignment(
    oldBamId: string,
    newBamId: string
) {
    const res = await api.post("/reassignAccess", {
        oldBamId,
        newBamId,
    });

    return res.data;
}

export async function getMyPendingReassignments() {
    const res = await api.get("/my-pending");
    return res.data;
}

export async function acceptReassignment(id: number) {
    await api.post(`/accept/${id}`);
}

export async function rejectReassignment(id: number, reason: string) {
    await api.post(`/reject/${id}`, { reason });
}
