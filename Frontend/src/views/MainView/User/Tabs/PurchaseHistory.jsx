import { useState, useEffect } from 'react'
import getMyVentas from '../../../../services/VentaService/getMyVentas'

export default function Tickets() {
    const [ventas, setVentas] = useState([])

    useEffect(() => {
        async function loadVentas() {
            try {
                const data = await getMyVentas()

                if (!data) return

                setVentas(data)
            } catch (error) {
                console.error(error)
            }
        }

        loadVentas()
    }, [])

    return (
        <>
            <div className="text-xl">
                <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
                    <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none">
                        Compras de <span className="text-[#c8a84b]">entradas</span>
                    </h1>
                </div>

                <div className="flex flex-col gap-2.5">
                    {ventas.length === 0 ? (
                        <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
                            No tenés entradas compradas.
                        </p>
                    ) : (
                        ventas.map((venta) => {
                            const [fecha, hora] = venta.fecha.split('T')
                            const [anio, mes, dia] = fecha.split('-')
                            const capitalize = (texto) => texto.charAt(0).toUpperCase() + texto.slice(1);
                            return (
                                <div key={venta.identificador} className="border border-[#d0dcea] rounded-xl p-3.5 transition-all duration-150 hover:border-[#a0b8d8] hover:shadow-[0_2px_10px_rgba(0,107,182,0.08)]">
                                    <div className="flex flex-col gap-3">
                                        <div className="flex items-center justify-between gap-3">
                                            <div className="p-3 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                                                <img className="w-10 h-auto" src={venta.banderaEquipoLocal} alt={venta.equipoLocal} />
                                                <span className="px-2 font-extrabold text-[#e0c472] font-['Barlow_Condensed']">-</span>
                                                <img className="w-10 h-auto" src={venta.banderaEquipoVisitante} alt={venta.equipoVisitante} />
                                            </div>

                                            <div className="flex-1 min-w-0">
                                                <div className="md:text-xl text-base mt-1 font-semibold text-[#0a1628]">
                                                    {capitalize(venta.equipoLocal)} vs {capitalize(venta.equipoVisitante)}
                                                </div>

                                                <div className="text-base text-[#7a8fa6]">
                                                    <i className="fa-solid fa-calendar" /> {dia}-{mes}
                                                </div>
                                            </div>

                                            <div className="shrink-0 rounded-lg bg-[#f0f4fa] px-4 py-2 text-center">
                                                <div className="text-xs uppercase text-[#8A93A6] font-semibold">
                                                    Entradas
                                                </div>
                                                <div className="text-lg font-bold text-[#0a1628]">
                                                    X{venta.cantidadEntradas}
                                                </div>
                                            </div>
                                        </div>

                                        <div className="flex gap-2 border-t border-[#eaf0f8] pt-2.5">
                                            <div className="flex-1 flex flex-col items-center justify-center rounded-lg py-2.5 bg-[#f0f4fa]">
                                                <span className="md:text-base text-[10px] uppercase tracking-wider text-[#8A93A6] font-semibold">Entrada</span>
                                                <span className="md:text-base text-sm font-semibold text-[#0a1628]">{venta.precio}</span>
                                            </div>
                                            <div className="flex-1 flex flex-col items-center justify-center rounded-lg py-2.5 bg-[#f0f4fa]">
                                                <span className="md:text-base text-[10px] uppercase tracking-wider text-[#8A93A6] font-semibold">Tarifa</span>
                                                <span className="md:text-base text-sm font-semibold text-[#0a1628]">{venta.tarifaExtra}</span>
                                            </div>
                                            <div className="flex-1 flex flex-col items-center justify-center rounded-lg py-2.5 bg-[#f0f4fa]">
                                                <span className="md:text-base text-[10px] uppercase tracking-wider text-[#8A93A6] font-semibold">Comisión</span>
                                                <span className="md:text-base text-sm font-semibold text-[#0a1628]">{venta.porcentajeComision}</span>
                                            </div>
                                            <div className="flex-1 flex flex-col items-center justify-center rounded-lg py-2.5 bg-[#0a1628]">
                                                <span className="md:text-base text-[10px] uppercase tracking-wider text-[#c8a84b]/60 font-semibold">Total</span>
                                                <span className="md:text-base text-sm font-semibold text-[#c8a84b]">{venta.montoTotal}</span>
                                            </div>
                                        </div>
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