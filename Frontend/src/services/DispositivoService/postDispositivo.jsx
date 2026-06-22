const API = "http://localhost:5001";
const PATH = "/nuevoDispositivo"

export default async function postDispositivo() {
    try {
        const res = await fetch(`${API}${PATH}`, {
            method: "POST",
            credentials: "include"
        });
        if (!res.ok) throw new Error(`POST ${PATH} -> ${res.status}`);
        const login = await res.json();
        return login;
    } catch (error) {
        console.log(error.message);
    }
}