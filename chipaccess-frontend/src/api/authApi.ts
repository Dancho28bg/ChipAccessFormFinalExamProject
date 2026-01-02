import type { LoginRequest, AuthResponse } from "../types/auth";

const BASE_URL = `http://localhost:5206/api/auth`;

export async function login(request: LoginRequest): Promise<AuthResponse> {
    const response = await fetch(`${BASE_URL}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
    });

    if (!response.ok) {
        const text = await response.text();
        throw new Error(text || "Login failed");
    }

    return await response.json();
}
