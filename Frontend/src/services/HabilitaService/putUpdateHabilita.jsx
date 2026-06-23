const API = "http://localhost:5001";
const PATH = "/updateHabilita";

export default async function putUpdateHabilita(body) {
    try {
    const res = await fetch(`${API}${PATH}`, {
        method: "PUT",
        credentials: "include",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(body)
    });
        return await res.json();
    } catch {
        console.log(error.message);
    }
}