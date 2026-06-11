const API = 'http://localhost:5001'
const PATH = '/telefono'

export default async function postTelefono(BODY) {
  const res = await fetch(`${API}${PATH}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(BODY),
  })

  const data = await res.json().catch(() => null)

  return {
    success: res.ok,
    status: res.status,
    ...data,
  }
}