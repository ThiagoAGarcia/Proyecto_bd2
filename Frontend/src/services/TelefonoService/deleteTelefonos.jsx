const API = "http://localhost:5001";
const PATH = "/telefonos";

export default async function deleteTelefono(phone) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(phone)}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (!res.ok) throw new Error(`DELETE ${PATH}/${phone} -> ${res.status}`);
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}