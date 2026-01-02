import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5206/api/employee",
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export async function searchBamIds(query: string): Promise<string[]> {
    if (!query || query.length < 2) return [];

    const res = await api.get<string[]>("/search", {
        params: { q: query },
    });

    return res.data;
}
