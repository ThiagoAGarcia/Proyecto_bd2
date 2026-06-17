const API = 'http://localhost:5001'
const PATH = '/qr/entrada'

export default async function actualizarQr(
  identificadorEntrada,
  identificadorDispositivo,
) {
  try {
    const res = await fetch(`${API}${PATH}`, {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        identificadorEntrada,
        identificadorDispositivo,
      }),
    })
    if (!res.ok) throw new Error(`POST ${PATH} -> ${res.status}`)
    const qr = await res.json()
    console.log(qr)
    return qr
  } catch (error) {
    console.log(error.message)
  }
}
