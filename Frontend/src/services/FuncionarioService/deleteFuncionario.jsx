const API = "http://localhost:5001";
const PATH = "/funcionarioPerfil";

export default async function deleteFuncionario(identificador) {
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