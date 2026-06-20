import { useEffect, useState } from 'react'
import getAllMyEstadios from '../../../../services/EstadioService/getAllMyEstadios'
import ModalCreateStadium from './Modals/Stadium/ModalCreateStadium'
import ModalEliminateStadium from './Modals/Stadium/ModalEliminateStadium'
import ModalUpdateStadium from './Modals/Stadium/ModalUpdateStadium'

export default function StadiumManagement() {
    const [data, setData] = useState([]);
    const [open, setOpen] = useState(false);

    const [openEliminar, setOpenEliminar] = useState(false);

    const [openEditar, setOpenEditar] = useState(false);

    const [identificador, setIdentificador] = useState(null)

    const loadEstadios = async () => {
        try {
            const data = await getAllMyEstadios()

            if (!data) return

            setData(data)
        } catch (error) {
            console.error(error)
        }
    }

    useEffect(() => {
        loadEstadios()
    }, [])

    return (
        <>
            <div className="text-xl">
                <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
                    <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none sm:pb-0 pb-4">
                        Gestión <span className="text-[#c8a84b]">Estadios</span>
                    </h1>
                    <button onClick={() => setOpen(true)} className="inline-flex items-center gap-1.5 px-2 py-2 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] border-none bg-[#c8a84b] text-[#0a1628] hover:bg-[#e0c472]">
                        <i className="fa-solid fa-plus" /> Crear estadio
                    </button>
                </div>

                <div className="flex flex-col gap-2.5">
                    {data.length === 0 ? (
                        <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
                            No tenés hay estadios creados.
                        </p>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-5">
                            {data.map((estadio) => (
                                <div key={estadio.identificador} className="bg-white border border-[#d0dcea] rounded-2xl overflow-hidden shadow-sm hover:shadow-lg transition-all duration-300">
                                    <div className="relative h-52 overflow-hidden">
                                        <img src={estadio.imagen} alt={estadio.nombre} className="w-full h-full object-cover hover:scale-105 transition-transform duration-500" />

                                        <div className="absolute inset-0 bg-linear-to-t from-black/70 via-black/20 to-transparent" />

                                        <h3 className="absolute bottom-4 left-4 text-white text-xl font-bold">
                                            {estadio.nombre}
                                        </h3>
                                    </div>

                                    <div className="p-4">
                                        <div className="space-y-2 text-sm text-[#5f6f86]">
                                            <p className="flex items-center gap-2">
                                                <i className="fa-solid fa-location-dot text-[#c8a84b]" />
                                                Localidad:<span className='font-bold'>{estadio.direccionLocalidad}</span>
                                            </p>

                                            <p className="flex items-center gap-2">
                                                <i className="fa-solid fa-road text-[#c8a84b]" />
                                                Dirección:<span className='font-bold'>{estadio.direccionCalle} {estadio.direccionNumero}</span>
                                            </p>

                                            <p className="flex items-center gap-2">
                                                <i className="fa-solid fa-envelope text-[#c8a84b]" />
                                                Código postal:<span className='font-bold'>{estadio.direccionCodigoPostal}</span>
                                            </p>

                                            <p className="flex items-center gap-2 capitalize">
                                                <i className="fa-solid fa-earth-americas text-[#c8a84b]" />
                                                País:<span className='font-bold'>{estadio.nombrePais}</span>
                                            </p>
                                        </div>

                                        <div className="flex gap-2 mt-5">
                                            <button onClick={() => {setOpenEditar(true); setIdentificador(estadio.identificador);}} className="cursor-pointer flex-1 bg-[#0a1628] text-[#c8a84b] py-2.5 rounded-lg font-medium hover:bg-[#13203a] transition">
                                                <i className="fa-solid fa-pen mr-2" />
                                                Editar
                                            </button>

                                            <button onClick={() => {setOpenEliminar(true); setIdentificador(estadio.identificador);}} className="cursor-pointer px-4 py-2.5 rounded-lg bg-red-50 text-red-600 hover:bg-red-100 transition">
                                                <i className="fa-solid fa-trash-can" />
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div >
            <ModalCreateStadium
                open={open}
                onClose={() => setOpen(false)}
                onCreateSuccess={loadEstadios}
            />

            <ModalEliminateStadium
                open={openEliminar}
                onClose={() => setOpenEliminar(false)}
                onCreateSuccess={loadEstadios}
                identificador={identificador}
            />

            <ModalUpdateStadium
                open={openEditar}
                onClose={() => setOpenEditar(false)}
                onUpdateSuccess={loadEstadios}
                identificador={identificador}
            />

            
        </>
    )
}