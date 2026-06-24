const API = "http://localhost:5001";
const PATH = "/allNoAsignados";

export default async function getAllNoAsignados(partido) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(partido)}`, {
            method: "GET",
            credentials: "include"
        });
        if (!res.ok) { return []; }
        return await res.json();
    } catch (error) {
        console.log(error.message);
        return [];
    }
}