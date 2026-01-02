export function decodeJwt(token: string): any {
    try {
        const payload = token.split(".")[1];
        const decoded = atob(payload);
        return JSON.parse(decoded);
    } catch (err) {
        return null;
    }
}

export function extractUserFromToken(token: string) {
    const payload = decodeJwt(token);
    if (!payload) return null;

    return {
        bamId: payload["nameidentifier"],
        email: payload["email"],
        role: payload["role"],
    };
}
