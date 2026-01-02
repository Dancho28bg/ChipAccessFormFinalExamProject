import axios from "axios";
import type { ArchiveItem } from "../types/archive.ts";


const api = axios.create({
    baseURL: "http://localhost:5206/api/access/archive",
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export const fetchArchive = async (): Promise<ArchiveItem[]> => {
    const res = await api.get("/all");
    return res.data;
};


