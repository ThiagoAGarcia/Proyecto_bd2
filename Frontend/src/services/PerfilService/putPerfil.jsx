const API = "http://localhost:5001";
const PATH = "/perfilPut";

export default async function putPerfil(identificador, BODY) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(identificador)}`, {
            method: "PUT",
            credentials: "include",
            headers: {"Content-Type":"application/json"},
            body: JSON.stringify(BODY)
        });
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}