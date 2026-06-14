const API = "http://localhost:5001";
const PATH = "/allHabilita";

export default async function getAllHabilita(estadio, partido) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(estadio)}/${encodeURIComponent(partido)}`, {
            method: "GET",
            credentials: "include"
        });
        if (!res.ok) throw new Error(`GET ${PATH}/${estadio}/${partido} -> ${res.status}`);
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}