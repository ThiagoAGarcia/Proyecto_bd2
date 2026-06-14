const API = "http://localhost:5001";
const PATH = "/telefono"

export default async function getTelefonoPorMail(mail) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(mail)}`, {
            method: "GET",
            credentials: "include"
        });
        if (!res.ok) throw new Error(`GET ${PATH}/${mail} -> ${res.status}`);
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}