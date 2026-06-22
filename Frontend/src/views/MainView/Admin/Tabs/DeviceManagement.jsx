import { useEffect, useState } from 'react'
import getAllDispositivos from '../../../../services/DispositivoService/getAllDispositivos'
import postDispositivo from '../../../../services/DispositivoService/postDispositivo'
import deleteDispositivo from '../../../../services/DispositivoService/deleteDispositivo'
import { toast } from 'react-toastify';
import { Oval } from 'react-loader-spinner';

export default function StadiumManagement() {
    const [data, setData] = useState([]);

    const [identificador, setIdentificador] = useState(null);

    const [isLoading, setIsLoading] = useState(false);

    const [identificadorEliminar, setIdentificadorEliminar] = useState('')


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
                toast.success('Dispositivo creado');
                await loadDispositivos();
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
            setIsLoading(true);

            const eliminarDispositivo = await deleteDispositivo(identificador);

            if (eliminarDispositivo?.success) {
                toast.success('Dispositivo eliminado');
                await loadDispositivos();
            } else {
                toast.error(eliminarDispositivo?.description);
            }
        } catch (error) {
            console.error(error);
            toast.error(error?.message || 'Error de conexión con el servidor');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <>
            {isLoading && (
                <div className="absolute inset-0 z-300 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-2xl">
                    <Oval height={35} width={35} color="#052e66" secondaryColor="#e5e7eb" strokeWidth={4} strokeWidthSecondary={4} ariaLabel="loading-create-group" />
                </div>
            )}
            <div className="text-xl">
                <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
                    <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none sm:pb-0 pb-4">
                        Gestión <span className="text-[#c8a84b]">Dispositivos</span>
                    </h1>
                    <button onClick={handleSubmit} className="inline-flex items-center gap-1.5 px-2 py-2 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] border-none bg-[#c8a84b] text-[#0a1628] hover:bg-[#e0c472]">
                        <i className="fa-solid fa-plus" /> Crear nuevo dispositivo
                    </button>
                </div>

                <div className="flex flex-col gap-2.5">
                    {data.length === 0 ? (
                        <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
                            No hay dispositivos creados.
                        </p>
                    ) : (
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 xl:grid-cols-4 gap-4">
                            {data.map((dispositivo) => (
                                <div key={dispositivo.identificador} className="bg-white border border-[#d0dcea] rounded-2xl p-5 shadow-sm hover:shadow-lg transition-all duration-300">
                                    <div className="flex items-center justify-between mb-4">
                                        <div className="w-12 h-12 rounded-xl bg-[#0a1628] flex items-center justify-center">
                                            <i className="fa-solid fa-microchip text-[#c8a84b] text-xl" />
                                        </div>
                                        {dispositivo.mailFuncionario ? (
                                            <span className="px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-700">
                                                Activo
                                            </span>
                                        ) : (
                                            <span className="px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-700">
                                                No activado
                                            </span>
                                        )}


                                    </div>

                                    <div className="flex flex-col lg:flex-row lg:items-center justify-between">
                                        <div>
                                            <p className="text-sm text-[#8A93A6] uppercase tracking-wide">
                                                Identificador
                                            </p>

                                            <h3 className="text-lg font-bold text-[#0a1628] break-all">
                                                {dispositivo.identificador}
                                            </h3>
                                        </div>
                                        {dispositivo.mailFuncionario ? (
                                            <>
                                                <div>
                                                    <p className="text-sm text-[#8A93A6] uppercase tracking-wide">
                                                        Funcionario asignado
                                                    </p>

                                                    <h3 className="text-lg font-bold text-[#0a1628] break-all">
                                                        {dispositivo.mailFuncionario}
                                                    </h3>
                                                </div>
                                            </>
                                        ) : (null)}

                                    </div>


                                    <button onClick={() => handleEliminar(dispositivo.identificador)} className="flex w-full mt-3 justify-center items-center gap-1.5 px-3 py-3 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                                        <i className="fa-solid fa-trash-can" />
                                        Eliminar
                                    </button>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div >

        </>
    )
}