import { useEffect, useState } from 'react'
import getPartidos from '../../../../services/PartidoService/getPartidos'
import ModalBuy from './Modals/ModalBuy'

export default function AvailableMatchs() {
    const [data, setData] = useState([])
    const [open, setOpen] = useState(false)
    const [identificadorEstadio, setIdentificadorEstadio] = useState(1)
    const [identificadorPartido, setIdentificadorPartido] = useState(1)
    const [precioBase, setPrecioBase] = useState(null)

    useEffect(() => {
        async function loadPartidos() {
            try {
                const data = await getPartidos()

                if (!data) return

                setData(data)
            } catch (error) {
                console.error(error)
            }
        }

        loadPartidos()
    }, [])

    return (
        <>
            <div className="text-xl">
                <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
                    <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none">
                        Partidos <span className="text-[#c8a84b]">Disponibles</span>
                    </h1>
                </div>

                <div className="flex flex-col gap-2.5">
                    {data.length === 0 ? (
                        <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
                            No tenés partidos registrados.
                        </p>
                    ) : (
                        data.map((partido) => {
                            const [fecha, hora] = partido.fechaHora.split('T')
                            const [anio, mes, dia] = fecha.split('-')
                            const horaFormateada = hora.slice(0, 5)
                            const capitalize = (texto) => texto.charAt(0).toUpperCase() + texto.slice(1);
                            return (
                                < div key={partido.identificador} className="border border-[#d0dcea] rounded-xl p-3.5 transition-all duration-150 hover:border-[#a0b8d8] hover:shadow-[0_2px_10px_rgba(0,107,182,0.08)]" >
                                    <div className="flex md:hidden flex-col gap-3">
                                        <div className="flex items-center gap-3">
                                            <div className="p-2.5 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                                                <img className="w-8 h-auto" src={partido.banderaEquipoLocal} alt={partido.equipoLocal} />
                                                <span className="px-2 font-extrabold text-[#e0c472] font-['Barlow_Condensed']">-</span>
                                                <img className="w-8 h-auto" src={partido.banderaEquipoVisitante} alt={partido.equipoVisitante} />
                                            </div>
                                            <div className="min-w-0">
                                                <div className="text-base font-semibold text-[#0a1628] truncate">{capitalize(partido.equipoLocal)} vs {capitalize(partido.equipoVisitante)}</div>
                                                <div className="text-sm text-[#7a8fa6] mt-0.5">
                                                    <i className="fa-solid fa-location-dot" /> {partido.nombreEstadio}
                                                </div>
                                                <div className="text-sm text-[#7a8fa6]">
                                                    <i className="fa-solid fa-calendar" /> {dia}-{mes} · {horaFormateada}
                                                </div>
                                            </div>
                                        </div>

                                        <div className="flex gap-2 border-t border-[#eaf0f8] pt-2.5">
                                            <button onClick={() => { setOpen(true); setIdentificadorEstadio(partido.identificadorEstadio); setIdentificadorPartido(partido.identificador); setPrecioBase(partido.precio);}} className="flex-1 inline-flex items-center justify-center gap-1.5 px-3 py-2.5 rounded-lg text-sm font-semibold cursor-pointer transition-all font-['Inter'] border-none bg-[#c8a84b] text-[#0a1628] hover:bg-[#e0c472]">
                                                <i className="fa-solid fa-plus" /> Comprar Entrada
                                            </button>
                                        </div>
                                    </div>

                                    <div className="hidden md:flex items-center justify-between gap-3">
                                        <div className="flex items-center gap-3.5 flex-1 min-w-0">
                                            <div className="p-3 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                                                <img className="w-10 h-auto" src={partido.banderaEquipoLocal} alt={partido.equipoLocal} />
                                                <span className="px-3 font-extrabold text-[#e0c472] font-['Barlow_Condensed']">-</span>
                                                <img className="w-10 h-auto" src={partido.banderaEquipoVisitante} alt={partido.equipoVisitante} />
                                            </div>
                                            <div className="min-w-0">
                                                <div className="font-semibold text-[#0a1628] truncate">{capitalize(partido.equipoLocal)} vs {capitalize(partido.equipoVisitante)}</div>
                                                <div className="text-sm text-[#7a8fa6] mt-0.5">
                                                    <i className="fa-solid fa-location-dot text-[11px]" /> {partido.nombreEstadio} · {dia}-{mes}, {horaFormateada}
                                                </div>
                                            </div>
                                        </div>
                                        <div className="flex gap-1.5 shrink-0 ">
                                            <button onClick={() => { setOpen(true); setIdentificadorEstadio(partido.identificadorEstadio); setIdentificadorPartido(partido.identificador); setPrecioBase(partido.precio); }} className="inline-flex items-center gap-1.5 px-3 py-4 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] border-none bg-[#c8a84b] text-[#0a1628] hover:bg-[#e0c472]">
                                                <i className="fa-solid fa-plus" /> Comprar Entrada
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            )
                        })
                    )}
                </div>
            </div >
            <ModalBuy
                open={open}
                onClose={() => setOpen(false)}
                identificadorEstadio={identificadorEstadio}
                identificadorPartido={identificadorPartido}
                precioBase={precioBase}
            />
        </>
    )
}