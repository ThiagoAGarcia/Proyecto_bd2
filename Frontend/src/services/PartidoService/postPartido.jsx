const API = "http://localhost:5001";
const PATH = "/partido"

export default async function postPartido(BODY) {
    try {
        const res = await fetch(`${API}${PATH}`, {
            method: "POST",
            credentials: "include",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(BODY)
        });
        const login = await res.json();
        return login;
    } catch (error) {
        console.log(error.message);
    }
}