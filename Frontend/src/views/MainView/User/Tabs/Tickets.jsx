import { useState, useEffect } from 'react'
import getMyEntradas from '../../../../services/EntradaService/getMyEntradas'
import ModalTransfer from '../Tabs/Modals/ModalTransfer'

export default function Tickets() {
  const [entradas, setEntradas] = useState([])
  const [open, setOpen] = useState(false)
  const [identificador, setIdentificador] = useState(null)

  const loadEntradas = async () => {
    try {
      const data = await getMyEntradas();

      if (!data) return;

      setEntradas(data);
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
    loadEntradas();
  }, []);

  return (
    <>
      <div className="text-xl">
        <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
          <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none">
            Mis <span className="text-[#c8a84b]">entradas</span>
          </h1>
        </div>

        <div className="flex flex-col gap-2.5">
          {entradas.length === 0 ? (
            <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
              No tenés entradas compradas.
            </p>
          ) : (
            entradas.map((entrada) => {
              const [fecha, hora] = entrada.fechaHora.split('T')
              const [anio, mes, dia] = fecha.split('-')
              const horaFormateada = hora.slice(0, 5)
              const capitalize = (texto) => texto.charAt(0).toUpperCase() + texto.slice(1);
              return (
                <div key={entrada.identificador} className="border border-[#d0dcea] rounded-xl p-3.5 transition-all duration-150 hover:border-[#a0b8d8] hover:shadow-[0_2px_10px_rgba(0,107,182,0.08)]">
                  <div className="flex md:hidden flex-col gap-3">
                    <div className="flex items-center gap-3">
                      <div className="p-3 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                        <img className="w-10 h-auto" src={entrada.banderaEquipoLocal} alt={entrada.equipoLocal} />
                        <span className="px-2 font-extrabold text-[#e0c472] font-['Barlow_Condensed']">-</span>
                        <img className="w-10 h-auto" src={entrada.banderaEquipoVisitante} alt={entrada.equipoVisitante} />
                      </div>
                      <div className="min-w-0">
                        <div className="text-base mt-1 font-semibold text-[#0a1628]">{capitalize(entrada.equipoLocal)} vs {capitalize(entrada.equipoVisitante)}</div>
                        <div className="text-sm text-[#7a8fa6] mt-0.5">
                          <i className="fa-solid fa-location-dot" /> {entrada.nombreEstadio} ({entrada.nombreSector})
                        </div>
                        <div className="text-sm text-[#7a8fa6]">
                          <i className="fa-solid fa-calendar" /> {dia}-{mes} · <i className="fa-solid fa-clock" /> {horaFormateada}
                        </div>
                      </div>
                    </div>

                    <div className="flex gap-2 border-t border-[#eaf0f8] pt-2.5">
                      <button className="flex-1 inline-flex items-center justify-center gap-1.5 px-3 py-2.5 rounded-lg text-sm font-semibold cursor-pointer transition-all font-['Inter'] bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        <i className="fa-solid fa-qrcode" /> Ver entrada
                      </button>
                      <button onClick={() => { setOpen(true); setIdentificador(entrada.identificador); }} className="flex-1 inline-flex items-center justify-center gap-1.5 px-3 py-2.5 rounded-lg text-sm font-semibold cursor-pointer transition-all font-['Inter'] bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        <i className="fa-solid fa-paper-plane" /> Transferir
                      </button>
                    </div>
                  </div>

                  <div className="hidden md:flex items-center justify-between gap-3">
                    <div className="flex items-center gap-3.5 flex-1 min-w-0">
                      <div className="p-4 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                        <img className="w-11 h-auto" src={entrada.banderaEquipoLocal} alt={entrada.equipoLocal} />
                        <span className="px-3 font-extrabold text-[#e0c472] font-['Barlow_Condensed']">-</span>
                        <img className="w-11 h-auto" src={entrada.banderaEquipoVisitante} alt={entrada.equipoVisitante} />
                      </div>
                      <div className="min-w-0">
                        <div className="font-semibold text-[#0a1628]">{capitalize(entrada.equipoLocal)} vs {capitalize(entrada.equipoVisitante)}</div>
                        <div className="text-sm text-[#7a8fa6] mt-0.5">
                          <i className="fa-solid fa-location-dot text-[11px]" /> {entrada.nombreEstadio} ({entrada.nombreSector})
                        </div>
                        <div className="text-sm text-[#7a8fa6] mt-0.5">
                          <i className="fa-solid fa-calendar" /> {dia}-{mes} · <i className="fa-solid fa-clock" /> {horaFormateada}
                        </div>
                      </div>
                    </div>
                    <div className="flex gap-1.5 shrink-0">
                      <button className="inline-flex items-center gap-1.5 px-3 py-4 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        <i className="fa-solid fa-qrcode" /> Ver
                      </button>
                      <button onClick={() => { setOpen(true); setIdentificador(entrada.identificador); }} className="inline-flex items-center gap-1.5 px-3 py-4 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        <i className="fa-solid fa-paper-plane" /> Transferir
                      </button>
                    </div>
                  </div>
                </div>
              )
            })
          )}
        </div>
      </div>
      <ModalTransfer
        open={open}
        onClose={() => setOpen(false)}
        identificador={identificador}
        onTransferSuccess={loadEntradas}
      />
    </>
  )
}