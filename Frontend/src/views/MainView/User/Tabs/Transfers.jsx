import { useState, useEffect } from 'react'
import getMyTransferencias from '../../../../services/TransferenciaService/getMyTransferencias'

export default function Transfers() {
    const [transferencias, setTransferencias] = useState([])

    useEffect(() => {
        async function loadTransferencias() {
            try {
                const data = await getMyTransferencias()

                if (!data) return

                setTransferencias(data)
            } catch (error) {
                console.error(error)
            }
        }

        loadTransferencias()
    }, [])

    console.log(transferencias)
    return (
        <>
            <div className="text-xl">
                <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
                    <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none">
                        <span className="text-[#c8a84b]">Transferencias</span> realizadas
                    </h1>
                </div>

                <div className="flex flex-col gap-2.5">
                    {transferencias.length === 0 ? (
                        <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
                            No realizaste transferencias de entradas.
                        </p>
                    ) : (
                        transferencias.map((transferencia) => {
                            const [fecha, hora] = transferencia.fechaHora.split('T')
                            const [anio, mes, dia] = fecha.split('-')
                            const horaFormateada = hora.slice(0, 5)
                            const capitalize = (texto) => texto.charAt(0).toUpperCase() + texto.slice(1);
                            return (
                                <div key={transferencia.identificador} className="border border-[#d0dcea] rounded-xl p-3.5 transition-all duration-150 hover:border-[#a0b8d8] hover:shadow-[0_2px_10px_rgba(0,107,182,0.08)]">

                                    <div className="flex lg:hidden flex-col gap-3">
                                        <div className="flex items-center gap-3">
                                            <div className="p-2.5 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                                                <img className="w-8 h-auto" src={transferencia.banderaEquipoLocal} alt={transferencia.equipoLocal} />
                                                <span className="px-2 font-extrabold text-[#e0c472] font-['Barlow_Condensed']">-</span>
                                                <img className="w-8 h-auto" src={transferencia.banderaEquipoVisitante} alt={transferencia.equipoVisitante} />
                                            </div>
                                            <div className="min-w-0">
                                                <div className="text-base font-semibold text-[#0a1628] truncate">
                                                    {capitalize(transferencia.equipoLocal)} vs {capitalize(transferencia.equipoVisitante)}
                                                </div>
                                                <div className="text-xs text-[#7a8fa6] mt-0.5">
                                                    <i className="fa-solid fa-calendar text-[10px]" /> {dia}-{mes}, {horaFormateada}
                                                </div>
                                            </div>

                                        </div>
                                        <div className="rounded-lg bg-[#f4f7fb] border border-[#e4eaf3] px-3 py-2.5 flex flex-col gap-1.5">
                                            <div className="flex items-center gap-2 text-sm">
                                                <span className="text-[#7a8fa6] w-14 shrink-0">De</span>
                                                <span className="text-[#0a1628] font-medium truncate">{transferencia.mailUsuarioRealiza}</span>
                                            </div>
                                            <div className="border-t border-[#e4eaf3]" />
                                            <div className="flex items-center gap-2 text-sm">
                                                <span className="text-[#7a8fa6] w-14 shrink-0">Para</span>
                                                <span className="text-[#c8a84b] font-semibold truncate">{transferencia.mailUsuarioRecibe}</span>
                                            </div>
                                        </div>
                                        <span className="inline-flex items-center justify-center gap-1.5 px-3 py-1.5 rounded-full bg-[#fef3c7] text-[#92400e] text-sm font-semibold tracking-wide border border-[#fde68a]">
                                            <i className="fa-solid fa-paper-plane text-sm" /> Transferida
                                        </span>
                                    </div>

                                    <div className="hidden lg:flex items-center justify-between gap-4">
                                        <div className="flex items-center gap-3.5 flex-1 min-w-0">
                                            <div className="p-3 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                                                <img className="w-10 h-auto" src={transferencia.banderaEquipoLocal} alt={transferencia.equipoLocal} />
                                                <span className="px-3 font-extrabold text-[#e0c472] font-['Barlow_Condensed'] text-xl">-</span>
                                                <img className="w-10 h-auto" src={transferencia.banderaEquipoVisitante} alt={transferencia.equipoVisitante} />
                                            </div>
                                            <div className="min-w-0">
                                                <div className="font-semibold text-[#0a1628] truncate text-xl">
                                                    {capitalize(transferencia.equipoLocal)} vs {capitalize(transferencia.equipoVisitante)}
                                                </div>
                                                <div className="text-sm text-[#7a8fa6] mt-0.5">
                                                    <i className="fa-solid fa-calendar text-[11px]" /> {dia}-{mes} / <i className="fa-solid fa-clock text-[11px]" />  {horaFormateada}
                                                </div>
                                            </div>
                                        </div>

                                        <div className="flex items-center gap-0 rounded-lg overflow-hidden border border-[#e4eaf3] shrink-0 text-sm">
                                            <div className="flex items-center gap-2 px-4 py-3 bg-[#f4f7fb]">
                                                <span className="text-[#7a8fa6] text-sm font-medium uppercase tracking-wider">De</span>
                                                <span className="text-[#0a1628] font-medium">{transferencia.mailUsuarioRealiza}</span>
                                            </div>
                                            <div className="flex items-center justify-center bg-[#0a1628] px-3 py-3 self-stretch">
                                                <i className="fa-solid fa-arrow-right text-[#c8a84b] text-sm" />
                                            </div>
                                            <div className="flex items-center gap-2 px-4 py-3 bg-[#f4f7fb]">
                                                <span className="text-[#7a8fa6] text-sm font-medium uppercase tracking-wider">Para</span>
                                                <span className="text-[#c8a84b] font-semibold">{transferencia.mailUsuarioRecibe}</span>
                                            </div>
                                        </div>

                                        <span className="shrink-0 inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-[#fef3c7] text-[#92400e] text-xs font-semibold tracking-wide border border-[#fde68a]">
                                            <i className="fa-solid fa-paper-plane text-[10px]" /> Transferida
                                        </span>
                                    </div>

                                </div>
                            )
                        })
                    )}
                </div>
            </div>
        </>
    )
}