const API = "http://localhost:5001";
const PATH = "/partido";

export default async function getPartido(identificador) {
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