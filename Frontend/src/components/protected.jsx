import { Navigate, Outlet } from "react-router-dom";
import { useEffect, useState } from "react";
import getPerfilMe from "../services/PerfilService/getPerfilMe";

export default function Protected({ allowedRoles }) {
    const [valid, setValid] = useState(null);

    useEffect(() => {
        async function checkAuth() {
            try {
                const perfil = await getPerfilMe();

                if (!perfil) {
                    setValid(false);
                    return;
                }

                if (
                    allowedRoles &&
                    allowedRoles.length > 0 &&
                    !allowedRoles.includes(perfil.role)
                ) {
                    setValid(false);
                    return;
                }

                setValid(true);
            } catch (error) {
                setValid(false);
            }
        }

        checkAuth();
    }, [allowedRoles]);

    if (valid === null) {
        return <div>Verificando acceso...</div>;
    }

    if (!valid) {
        return <Navigate to="/sin-token" />;
    }

    return <Outlet />;
}