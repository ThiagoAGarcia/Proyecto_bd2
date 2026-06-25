const API = "http://localhost:5001";
const PATH = "/estadioDelete";

export default async function deleteEstadio(identificador) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(identificador)}`, {
            method: "DELETE",
            credentials: "include"
        });
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}