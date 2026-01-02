import { useAtom } from "jotai";
import { tokenAtom } from "../../atoms/authAtoms";
import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";

interface Props {
    children: ReactNode;
}

export default function ProtectedRoute({ children }: Props) {
    const [token] = useAtom(tokenAtom);

    if (!token) {
        return <Navigate to="/login" replace />;
    }

    return <>{children}</>;
}
