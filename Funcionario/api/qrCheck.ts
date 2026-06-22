export async function qrCheck(token: string, mailPerfil: string) {
  const response = await fetch(
    `http://192.168.1.22:5001/qr/token?token=${encodeURIComponent(token)}&mailPerfil=${encodeURIComponent(mailPerfil)}`,
    {
      method: 'GET',
      credentials: 'include',
    },
  )

  if (!response.ok) {
    throw new Error('Credenciales incorrectas')
  }

  const data = await response.json()

  return data
}
