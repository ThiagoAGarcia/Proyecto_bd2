const API = "http://localhost:5001";
const PATH = "/transferencia"

export default async function postTransferencia(BODY) {
    const res = await fetch(`${API}${PATH}`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(BODY)
    });

    const data = await res.json();

    if (!res.ok) {
        throw new Error(data.message);
    }

    return data;
}