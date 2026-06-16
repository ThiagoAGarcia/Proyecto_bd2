const API = "http://localhost:5001";
const PATH = "/entrada"

export default async function postEntrada(BODY) {
    try {
        const res = await fetch(`${API}${PATH}`, {
            method: "POST",
            credentials: "include",
            headers: {"Content-Type":"application/json"},
            body: JSON.stringify(BODY)
        });
    if (!res.ok) throw new Error(`POST ${PATH} -> ${res.status}`);
    const login = await res.json();
    return login;
    } catch(error) {
        console.log(error.message);
    }
}