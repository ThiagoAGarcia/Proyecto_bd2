const API = 'http://localhost:5001'
const PATH = '/loginCheck'

export default async function postRegisterService(BODY) {
  try {
    const res = await fetch(`${API}${PATH}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(BODY),
    })

    const data = await res.json()

    console.log(data)

    if (res.status !== 201) {
      return {
        success: false,
        description: data.message || 'Error desconocido en el servidor',
        status: res.status,
      }
    }

    return {
      success: true,
      ...data,
    }
  } catch (error) {
    console.error('Error en postRegisterService:', error)
    return {
      success: false,
      description: 'Error de conexión con el servidor',
    }
  }
}