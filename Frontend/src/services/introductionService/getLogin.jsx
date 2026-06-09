const API = "http://localhost:5000";
const PATH = "/login"

export default async function postLoginService(BODY) {
    try {
        const res = await fetch(`${API}${PATH}`, {
            method: "POST",
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