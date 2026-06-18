const API = "http://localhost:5001";
const PATH = "/allMyEstadios"

export default async function getAllMyEstadios() {
    try {
        const res = await fetch(`${API}${PATH}`, {
            method: "GET",
            credentials: "include"
        });
    if (!res.ok) throw new Error(`GET ${PATH} -> ${res.status}`);
    const login = await res.json();
    return login;
    } catch(error) {
        console.log(error.message);
    }
}