const API = 'http://localhost:5001'
const PATH = '/register'

export default async function postRegisterService(BODY) {
  try {
    const res = await fetch(`${API}${PATH}`, {
      method: 'POST',
      headers: {'Content-Type': 'application/json'},
      body: JSON.stringify(BODY),
    })

    const data = await res.json()

    if (!res.ok) {
      return {
        success: false,
        description: data.description || 'Error desconocido en el servidor',
        status: res.status,
      }
    }

    return data
  } catch (error) {
    console.error('Error en postRegisterService:', error)
    return {
      success: false,
      description: 'Error de conexión con el servidor',
    }
  }
}