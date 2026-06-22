const API = 'http://localhost:5001'
const PATH = '/RankingPartidosMayorVendidos'

export default async function RankingPartidosMayorVendidos() {
  try {
    const res = await fetch(`${API}${PATH}`, {
      credentials: 'include',
      method: 'GET',
    })

    if (!res.ok) throw new Error(`GET ${PATH} -> ${res.status}`)
    return await res.json()
  } catch (error) {
    console.log(error.message)
  }
}
