const API = 'http://localhost:5001'
const PATH = '/checkSession'

export default async function getSession() {
  try {
    const res = await fetch(`${API}${PATH}`, {
      method: 'GET',
      credentials: 'include',
    })

    if (!res.ok) throw new Error(`GET ${PATH} -> ${res.status}`)

    return await res.json()
  } catch (error) {
    console.log(error.message)
  }
}
