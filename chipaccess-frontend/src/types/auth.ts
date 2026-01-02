export interface LoginRequest {
    bamId: string;
}

export interface AuthResponse {
    bamId: string;
    email: string;
    role: string;
    token: string;
}
