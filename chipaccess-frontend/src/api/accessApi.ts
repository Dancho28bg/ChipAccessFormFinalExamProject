import axios from "axios";
import type {
    AccessItem,
    CreateAccessRequest,
    UpdateAccessRequest
} from "../types/access.ts";

const API_URL = "http://localhost:5206/api/access";

const api = axios.create({
    baseURL: API_URL,
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});


export const fetchAllAccesses = async (): Promise<AccessItem[]> => {
    const res = await api.get("/all");
    return res.data;
};

export const fetchAccessById = async (id: number): Promise<AccessItem> => {
    const res = await api.get(`/${id}`);
    return res.data;
};

export const fetchMyAccesses = async (): Promise<AccessItem[]> => {
    const res = await api.get("/my");
    return res.data;
};

export const fetchAccessByBamId = async (bamId: string): Promise<AccessItem[]> => {
    const res = await api.get(`/by-bam/${bamId}`);
    return res.data;
};

export const fetchManagedByMe = async (): Promise<AccessItem[]> => {
    const res = await api.get("/managed-by/me");
    return res.data;
};

export const fetchByStatus = async (status: string): Promise<AccessItem[]> => {
    const res = await api.get(`/by-status/${status}`);
    return res.data;
};

export const createAccessRequest = async (
    data: CreateAccessRequest
): Promise<AccessItem> => {
    const res = await api.post("/create", data);
    return res.data;
};

export const updateAccessRequest = async (
    id: number,
    data: UpdateAccessRequest
): Promise<void> => {
    await api.put(`/edit/${id}`, data);
};

export const approveAccess = async (id: number): Promise<void> => {
    await api.put(`/approve/${id}`);
};

export const rejectAccess = async (id: number): Promise<void> => {
    await api.put(`/reject/${id}`);
};

export const revokeAccess = async (id: number): Promise<void> => {
    await api.post(`/revoke/${id}`);
};

export const deactivateAccess = async (id: number): Promise<void> => {
    await api.post(`/deactivate/${id}`);
};


