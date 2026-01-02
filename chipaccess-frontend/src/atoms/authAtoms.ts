import { atom } from "jotai";

export const userAtom = atom(
    JSON.parse(localStorage.getItem("user") || "null")
);

export const tokenAtom = atom<string | null>(
    localStorage.getItem("token") || null
);

export const loginAtom = atom(
    null,
    (get, set, auth: any) => {

        const user = {
            bamId: auth.bamId,
            email: auth.email,
            role: auth.role,
            token: auth.token
        };

        localStorage.setItem("user", JSON.stringify(user));
        localStorage.setItem("token", auth.token);

        set(userAtom, user);
        set(tokenAtom, auth.token);
    }
);

export const logoutAtom = atom(
    null,
    (get, set) => {
        localStorage.removeItem("user");
        localStorage.removeItem("token");

        set(userAtom, null);
        set(tokenAtom, null);
    }
);
