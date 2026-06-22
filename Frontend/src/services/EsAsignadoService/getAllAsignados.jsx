const API = "http://localhost:5001";
const PATH = "/allAsignados";

export default async function getAllAsignados(estadio, sector, partido) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(estadio)}/${encodeURIComponent(sector)}/${encodeURIComponent(partido)}`, {
            method: "GET",
            credentials: "include"
        });
        if (!res.ok) throw new Error(`GET ${PATH}/${estadio}/${sector}/${partido} -> ${res.status}`);
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}