import AsyncStorage from '@react-native-async-storage/async-storage'
export async function login(email: string, password: string) {
  const response = await fetch(
    `http://10.55.24.199:5001/loginCheckFuncionario`,
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

  const data = await response.json()
  AsyncStorage.setItem('email', email)

  return data
}
