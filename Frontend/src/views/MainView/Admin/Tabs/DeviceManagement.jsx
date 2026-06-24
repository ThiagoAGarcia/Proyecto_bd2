import {useEffect, useState} from 'react'
import getAllDispositivos from '../../../../services/DispositivoService/getAllDispositivos'
import postDispositivo from '../../../../services/DispositivoService/postDispositivo'
import deleteDispositivo from '../../../../services/DispositivoService/deleteDispositivo'
import {toast} from 'react-toastify'
import {Oval} from 'react-loader-spinner'

export default function StadiumManagement() {
  const [data, setData] = useState([])
  const [isLoading, setIsLoading] = useState(false)

  const loadDispositivos = async () => {
    try {
      const data = await getAllDispositivos()

      if (!data) return

      setData(data)
    } catch (error) {
      console.error(error)
    }
  }

  useEffect(() => {
    loadDispositivos()
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()

    try {
      setIsLoading(true)

      const crearDispositivo = await postDispositivo()

      if (crearDispositivo?.success) {
        toast.success('Dispositivo creado')
        await loadDispositivos()
      } else {
        toast.error(crearDispositivo?.description || 'Error al registrar', {
          position: 'bottom-left',
          autoClose: 3000,
        })
      }
    } catch (error) {
      console.error(error)

      toast.error('Error de conexión con el servidor', {
        position: 'bottom-left',
        autoClose: 3000,
      })
    } finally {
      setIsLoading(false)
    }
  }

  const handleEliminar = async (identificador) => {
    try {
      setIsLoading(true)

      const eliminarDispositivo = await deleteDispositivo(identificador)

      if (eliminarDispositivo?.success) {
        toast.success('Dispositivo eliminado')
        await loadDispositivos()
      } else {
        toast.error(eliminarDispositivo?.description)
      }
    } catch (error) {
      console.error(error)

      toast.error(error?.message || 'Error de conexión con el servidor')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <>
      {isLoading && (
        <div className="absolute inset-0 z-300 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-2xl">
          <Oval
            height={35}
            width={35}
            color="#052e66"
            secondaryColor="#e5e7eb"
            strokeWidth={4}
            strokeWidthSecondary={4}
          />
        </div>
      )}

      <div className="text-xl">
        <div className="sm:flex justify-between sm:items-end items-start w-full pb-6">
          <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none sm:pb-0 pb-4">
            Gestión <span className="text-[#c8a84b]">Dispositivos</span>
          </h1>

          <button
            onClick={handleSubmit}
            className="inline-flex items-center gap-1.5 px-2 py-2 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] border-none bg-[#c8a84b] text-[#0a1628] hover:bg-[#e0c472]">
            <i className="fa-solid fa-plus" />
            Crear nuevo dispositivo
          </button>
        </div>

        {data.length === 0 ? (
          <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
            No hay dispositivos creados.
          </p>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
            {data.map((dispositivo) => (
              <div
                key={dispositivo.identificador}
                className="
                                    bg-white
                                    border
                                    border-[#d0dcea]
                                    rounded-2xl
                                    p-6
                                    shadow-sm
                                    hover:shadow-xl
                                    hover:-translate-y-1
                                    transition-all
                                    duration-300
                                    flex
                                    flex-col
                                    gap-5
                                ">
                <div className="flex items-start justify-between">
                  <div className="w-14 h-14 rounded-xl bg-[#0a1628] flex items-center justify-center">
                    <i className="fa-solid fa-microchip text-[#c8a84b] text-2xl" />
                  </div>

                  <span
                    className={`px-3 py-1 rounded-full text-xs font-semibold ${
                      dispositivo.mailFuncionario
                        ? 'bg-green-100 text-green-700'
                        : 'bg-red-100 text-red-700'
                    }`}>
                    {dispositivo.mailFuncionario ? 'Activo' : 'No activado'}
                  </span>
                </div>

                <div className="space-y-4">
                  <div>
                    <p className="text-xs text-[#8A93A6] uppercase tracking-widest mb-1">
                      Identificador
                    </p>

                    <h3 className="text-3xl font-bold text-[#0a1628]">
                      {dispositivo.identificador}
                    </h3>
                  </div>

                  {dispositivo.mailFuncionario && (
                    <div>
                      <p className="text-xs text-[#8A93A6] uppercase tracking-widest mb-1">
                        Funcionario asignado
                      </p>

                      <h3 className="text-base font-semibold text-[#0a1628] break-all">
                        {dispositivo.mailFuncionario}
                      </h3>
                    </div>
                  )}
                </div>

                <button
                  onClick={() => handleEliminar(dispositivo.identificador)}
                  className=" mt-auto flex items-center justify-center gap-2 w-full cursor-pointer px-4 py-2.5 rounded-lg bg-red-50 text-red-600 hover:bg-red-100 transition
                                    ">
                  <i className="fa-solid fa-trash-can" />
                  Eliminar
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </>
  )
}
