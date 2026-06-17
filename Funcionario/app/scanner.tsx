import {useEffect, useState} from 'react'
import {View, Text, Alert, StyleSheet} from 'react-native'
import {CameraView, useCameraPermissions} from 'expo-camera'
import {qrCheck} from '../api/qrCheck'

export default function Scanner() {
  const [permission, requestPermission] = useCameraPermissions()
  const [scanned, setScanned] = useState(false)
  const [result, setResult] = useState<'idle' | 'ok' | 'fail'>('idle')

  useEffect(() => {
    if (!permission) return
    if (!permission.granted) requestPermission()
  }, [permission])

  const handleScan = async ({data}: {data: string}) => {
    setScanned(true)

    try {
      const res = await qrCheck(data)

      console.log(res)

      if (res?.valido) {
        setResult('ok')
      } else {
        setResult('fail')
      }
    } catch (e) {
      setResult('fail')
    }

    setTimeout(() => {
      setScanned(false)
      setResult('idle')
    }, 3000)
  }

  if (!permission) {
    return <Text>Solicitando permisos...</Text>
  }

  if (!permission.granted) {
    return <Text>No se concedieron permisos.</Text>
  }

  if (result !== 'idle') {
    return (
      <View
        style={[
          styles.resultContainer,
          result === 'ok' ? styles.ok : styles.fail,
        ]}>
        <Text style={styles.resultText}>
          {result === 'ok' ? 'ACCESO PERMITIDO' : 'ACCESO DENEGADO'}
        </Text>

        <Text style={styles.subText}>
          {result === 'ok' ? 'El QR es válido' : 'El QR no es válido o expiró'}
        </Text>
      </View>
    )
  }

  return (
    <View style={styles.container}>
      <CameraView
        style={styles.camera}
        facing="back"
        barcodeScannerSettings={{
          barcodeTypes: ['qr'],
        }}
        onBarcodeScanned={scanned ? undefined : handleScan}
      />
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  camera: {
    flex: 1,
  },

  resultContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },

  ok: {
    backgroundColor: '#16a34a',
  },

  fail: {
    backgroundColor: '#dc2626',
  },

  resultText: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#fff',
  },

  subText: {
    marginTop: 10,
    fontSize: 16,
    color: '#fff',
  },
})
