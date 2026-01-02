import { useState } from "react";
import { useSetAtom } from "jotai";
import { loginAtom } from "../../atoms/authAtoms";
import { login as loginApi } from "../../api/authApi";
import { useNavigate } from "react-router-dom";
import "./LoginPage.css";

export default function LoginPage() {
    const [bamId, setBamId] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const setLogin = useSetAtom(loginAtom);
    const navigate = useNavigate();

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError("");

        if (!bamId.trim()) {
            setError("Please enter a BAM ID.");
            return;
        }

        try {
            setLoading(true);

            const auth = await loginApi({ bamId });

            console.log("Login successful!", auth);

            setLogin(auth);

            navigate("/", { replace: true });

        } catch (err: any) {
            console.error("LOGIN FAILED:", err);
            setError(err.message || "Login failed");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="login-container">
            <form className="login-box" onSubmit={handleSubmit}>
                <h1>ChipAccess Login</h1>

                <input
                    type="text"
                    placeholder="Enter BAM ID"
                    value={bamId}
                    onChange={(e) => setBamId(e.target.value)}
                    required
                />

                <button type="submit" disabled={loading}>
                    {loading ? "Logging in..." : "Login"}
                </button>

                {error && <p className="error">{error}</p>}
            </form>
        </div>
    );
}
