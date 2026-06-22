const API = "http://localhost:5001";
const PATH = "/updateHabilita";

export default async function putUpdateHabilita(body) {
    const res = await fetch(`${API}${PATH}`, {
        method: "PUT",
        credentials: "include",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(body)
    });

    if (!res.ok) {
        throw new Error(`PUT ${PATH} -> ${res.status}`);
    }

    return await res.json();
}