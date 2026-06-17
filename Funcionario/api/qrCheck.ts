export async function qrCheck(token: string) {
  const response = await fetch(`http://192.168.1.22:5001/qr/token/${token}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  })

  console.log(response)
  if (!response.ok) {
    throw new Error('Credenciales incorrectas')
  }

  const data = await response.json()

  return data
}
