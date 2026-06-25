const API = "http://localhost:5001";
const PATH = "/deleteAsignado";

export default async function deleteAsignado(identificadorEstadio, identificadorSector, identificadorPartido) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(identificadorEstadio)}/${encodeURIComponent(identificadorSector)}/${encodeURIComponent(identificadorPartido)}`, {
            method: "DELETE",
            credentials: "include"
        });
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}