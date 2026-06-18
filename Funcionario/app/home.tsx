import {View, Button, StyleSheet} from 'react-native'
import {router} from 'expo-router'

export default function Home() {
  return (
    <View style={styles.container}>
      <Button title="Escanear QR" onPress={() => router.push('/scanner')} />
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
})
