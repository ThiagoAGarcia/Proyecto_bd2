import Modal from './../../../../../components/modal'
import QRCode from 'react-qr-code'
import actualizarQr from '../../../../../services/QrService/actualizarQr'
import { useState, useEffect } from 'react'

export default function ModalQr({ open, onClose, partido, entrada }) {
  const [qr, setQr] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (!open || !partido) return

    let interval

    async function cargarQr() {
      try {
        setLoading(true)

        const data = await actualizarQr(entrada.identificador, 1)

        setQr(data.token)
      } catch (error) {
        console.error(error)
      } finally {
        setLoading(false)
      }
    }

    cargarQr()

    interval = setInterval(cargarQr, 30000)

    return () => clearInterval(interval)
  }, [open, partido, entrada])

  return (
    <Modal open={open} onClose={onClose}>
      <div className="w-full flex flex-col">
        <div className="mb-4">
          <h2 className="text-2xl font-bold text-[#14315C]">
            Escanear <span className="text-[#c8a84b]">entrada</span>
          </h2>

          <p className="mt-1 text-sm text-[#14315C]/70">
            Presentá este código QR al ingresar al estadio.
          </p>
        </div>

        <div className="overflow-hidden rounded-2xl border border-[#14315C]/10">
          <div className="bg-[#14315C] p-4 text-white">
            <h3 className="text-xl font-bold">{partido.nombre}</h3>

            <div className=" flex flex-col gap-1 text-sm text-white/80">
              <p>
                <i className="fa-regular fa-calendar mr-2" />
                {partido.fecha}
              </p>

              <p>
                <i className="fa-solid fa-location-dot mr-2" />
                {partido.estadio}
              </p>
            </div>
          </div>

          <div className="flex flex-col items-center bg-white p-5">
            {loading ? (
              <div className="flex h-50 items-center justify-center">
                <p className="text-[#14315C]/70">Generando QR...</p>
              </div>
            ) : (
              <div className="rounded-xl bg-white p-4 shadow-md">
                <QRCode value={qr} size={200} />
              </div>
            )}

            <p className="mt-4 text-center text-sm text-[#14315C]/70">
              Escaneá este código para validar tu entrada.
            </p>

            <div className="mt-3 rounded-lg bg-[#c8a84b]/10 px-4 py-2">
              <p className="text-sm font-semibold text-[#14315C]">
                QR válido por 30 segundos
              </p>
            </div>
          </div>
        </div>

        <div className="mt-5 lg:mt-0">
          <button
            onClick={onClose}
            className="w-full lg:hidden inline cursor-pointer rounded-xl bg-[#14315C] px-6 py-3 font-semibold text-white transition hover:bg-[#0f2748]">
            Cerrar
          </button>
        </div>
      </div>
    </Modal>
  )
}
