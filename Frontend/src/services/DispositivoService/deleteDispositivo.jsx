const API = "http://localhost:5001";
const PATH = "/dispositivo/borrar";

export default async function deleteDispositivo(identificador) {
    const res = await fetch(
        `${API}${PATH}/${encodeURIComponent(identificador)}`,
        {
            method: "DELETE",
            credentials: "include"
        }
    );

    const data = await res.json();

    return data;
}