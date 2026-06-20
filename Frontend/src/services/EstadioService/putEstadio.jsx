const API = "http://localhost:5001";
const PATH = "/estadioUpdate";

export default async function putEstadio(identificador, BODY) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(identificador)}`, {
            method: "PUT",
            credentials: "include",
            headers: {"Content-Type":"application/json"},
            body: JSON.stringify(BODY)
        });
        if (!res.ok) throw new Error(`PUT ${PATH}/${identificador} -> ${res.status}`);
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}