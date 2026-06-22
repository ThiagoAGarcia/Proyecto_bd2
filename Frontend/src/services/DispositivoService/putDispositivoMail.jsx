const API = "http://localhost:5001";
const PATH = "/updateDispositivo";

export default async function putDispositivoMail(dispositivo, mail) {
    try {
        const res = await fetch(`${API}${PATH}/${encodeURIComponent(dispositivo)}/${encodeURIComponent(mail)}`, {
            method: "PUT",
            credentials: "include"
        });
        return await res.json();
    } catch (error) {
        console.log(error.message);
    }
}