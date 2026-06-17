export async function login(email: string, password: string) {
  const response = await fetch(
    `http://172.20.10.2:5001/loginCheckFuncionario`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        MailPerfil: email,
        password,
      }),
    },
  )

  if (!response.ok) {
    throw new Error('Credenciales incorrectas')
  }

  const data = await response.json()

  return data
}
