import {useState} from 'react'
import {View, TextInput, Button, StyleSheet, Alert, Text} from 'react-native'
import {router} from 'expo-router'
import {login} from '../api/login'

export default function Login() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')

  const handleLogin = async () => {
    try {
      await login(email, password)
      router.push('/home')
    } catch {
      Alert.alert('Error', 'Credenciales incorrectas')
    }
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Iniciar sesión</Text>

      <TextInput
        placeholder="Email"
        placeholderTextColor="#888"
        style={styles.input}
        onChangeText={setEmail}
        autoCapitalize="none"
      />

      <TextInput
        placeholder="Contraseña"
        placeholderTextColor="#888"
        secureTextEntry
        style={styles.input}
        onChangeText={setPassword}
      />

      <View style={styles.button}>
        <Button title="Iniciar sesión" onPress={handleLogin} />
      </View>
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    padding: 20,
    backgroundColor: '#fff',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
    textAlign: 'center',
    color: '#14315C',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    padding: 12,
    marginBottom: 15,
    borderRadius: 10,
    color: '#000',
  },
  button: {
    marginTop: 10,
  },
})
