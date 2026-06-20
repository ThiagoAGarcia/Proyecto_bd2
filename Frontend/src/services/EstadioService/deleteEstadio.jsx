const API = "http://localhost:5001";
const PATH = "/estadioDelete";

export default async function deleteEstadio(identificador) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(identificador)}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (!res.ok) throw new Error(`DELETE ${PATH}/${identificador} -> ${res.status}`);
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}