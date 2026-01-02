import { authFetch } from "./authFetch";

const BASE = import.meta.env.VITE_API_URL ?? "http://localhost:5206";

export async function getMyAccesses() {
    const res = await authFetch(`${BASE}/api/Access/my`);

    if (!res.ok) throw new Error("Failed to fetch my accesses");
    return res.json();
}

export async function getAdminList(status: string) {
    const res = await authFetch(`${BASE}/getByAccessStatus/${status}`);

    if (!res.ok) throw new Error(`Failed to fetch ${status} list`);
    return res.json();
}
