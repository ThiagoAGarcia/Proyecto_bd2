const API = "http://localhost:5001";
const PATH = "/deletePartido";

export default async function deletePartido(identificador) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(identificador)}`,
            {
                method: "DELETE",
                credentials: "include"
            });
        const data = await res.json();
        return data;
    } catch(error) {
        console.log(error.message);
    }
}