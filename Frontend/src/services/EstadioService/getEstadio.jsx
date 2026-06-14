const API = "http://localhost:5001";
const PATH = "/estadio";

export default async function getEstadio(identificador) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(identificador)}`, {
            method: "GET",
            credentials: "include"
        });
        if (!res.ok) throw new Error(`GET ${PATH}/${identificador} -> ${res.status}`);
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}