const API = 'http://localhost:5001'
const PATH = '/qr'

export default async function deleteQr(identificadorEntrada) {
  try {
    const res = await fetch(`${API}${PATH}/${identificadorEntrada}`, {
      method: 'DELETE',
      credentials: 'include',
    })
    if (!res.ok)
      throw new Error(`DELETE ${PATH}/${identificadorEntrada} -> ${res.status}`)
    return await res.json()
  } catch (error) {
    console.log(error.message)
  }
}
