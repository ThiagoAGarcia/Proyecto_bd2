import { useEffect, useState } from 'react'
import getMyPartidos from '../../../../services/PartidoService/getMyPartidos'
import getMyPartidosSectores from '../../../../services/PartidoService/getMyPartidosSectores'
import ModalEditMatch from './Modals/Match/ModalEditMatch'
import ModalCreateMatch from './Modals/Match/ModalCreateMatch'
import ModalSectorMatch from './Modals/Match/ModalSectorMatch'
import ModalAsignarMatch from './Modals/Match/ModalAsignarMatch'
import ModalEliminateMatch from './Modals/Match/ModalEliminateMatch'

export default function MatchManagement() {
    const [data, setData] = useState([]);
    const [dataSectores, setDataSectores] = useState([]);

    const [open, setOpen] = useState(false);
    const [identificadorSector, setIdentificadorSector] = useState(null);
    const [openCreate, setOpenCreate] = useState(false);
    const [openUpdate, setOpenUpdate] = useState(false);
    const [openAsignar, setOpenAsignar] = useState(false);
    const [openDelete, setOpenDelete] = useState(false);

    const [identificadorPartido, setIdentificadorPartido] = useState(null);

    const [estadio, setEstadio] = useState(null);

    const loadPartidos = async () => {
        try {
            setIdentificadorPartido(null)
            const data = await getMyPartidos();
            const dataSec = await getMyPartidosSectores();

            if (!data) return
            if (!dataSec) return

            setData(data);
            setDataSectores(dataSec);
        } catch (error) {
            console.error(error)
            setIdentificadorPartido(null);
        }
    }

    useEffect(() => {
        loadPartidos()
    }, [])

    return (
        <>
            <div className="text-xl">
                <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
                    <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none sm:pb-0 pb-4">
                        Gestión <span className="text-[#c8a84b]">Partidos</span>
                    </h1>
                    <button onClick={() => { setOpenCreate(true); }} className="inline-flex items-center gap-1.5 px-2 py-2 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] border-none bg-[#c8a84b] text-[#0a1628] hover:bg-[#e0c472]">
                        <i className="fa-solid fa-plus" /> Crear partido
                    </button>
                </div>

                <div className="flex flex-col gap-2.5">
                    {data.length === 0 ? (
                        <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
                            No tenés partidos registrados.
                        </p>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-5">
                            {data.map((partido) => {
                                const [fecha, hora] = partido.fechaHora.split("T");
                                const [, mes, dia] = fecha.split("-");
                                const horaFormateada = hora.slice(0, 5);

                                const capitalize = (texto) => texto.charAt(0).toUpperCase() + texto.slice(1);

                                const sectoresPartido = dataSectores.filter((sector) => sector.identificadorPartido === partido.identificador);

                                return (
                                    <div key={partido.identificador} className="bg-white border border-[#d0dcea] rounded-2xl overflow-hidden shadow-sm hover:shadow-lg hover:-translate-y-1 transition-all duration-300">
                                        <div className="relative bg-[#0a1628] h-52 flex flex-col items-center justify-center">
                                            <div className="absolute inset-0 bg-linear-to-br from-[#0a1628] via-[#12223d] to-[#0a1628]" />

                                            <div className="relative flex items-center gap-6">
                                                <img src={partido.banderaEquipoLocal} alt={partido.equipoLocal} className="w-20 h-20 object-contain drop-shadow-lg" />

                                                <span className="text-4xl font-black text-[#c8a84b]">
                                                    VS
                                                </span>

                                                <img src={partido.banderaEquipoVisitante} alt={partido.equipoVisitante} className="w-20 h-20 object-contain drop-shadow-lg" />
                                            </div>

                                            <h3 className="relative mt-5 text-white text-xl font-bold text-center px-4">
                                                {capitalize(partido.equipoLocal)} vs{" "}
                                                {capitalize(partido.equipoVisitante)}
                                            </h3>
                                        </div>

                                        <div className="p-5">

                                            <div className={`gap-y-6 ${sectoresPartido.length > 0 ? "flex flex-col" : ""}`}>

                                                <div className="flex-1 space-y-3 text-sm text-[#5f6f86]">

                                                    <p className="flex items-center gap-2">
                                                        <i className="fa-solid fa-location-dot text-[#c8a84b]" />
                                                        Estadio:
                                                        <span className="font-bold text-[#0a1628]">
                                                            {partido.nombreEstadio}
                                                        </span>
                                                    </p>

                                                    <p className="flex items-center gap-2">
                                                        <i className="fa-solid fa-calendar text-[#c8a84b]" />
                                                        Fecha:
                                                        <span className="font-bold text-[#0a1628]">
                                                            {dia}/{mes}
                                                        </span>
                                                    </p>

                                                    <p className="flex items-center gap-2">
                                                        <i className="fa-solid fa-clock text-[#c8a84b]" />
                                                        Hora:
                                                        <span className="font-bold text-[#0a1628]">
                                                            {horaFormateada}
                                                        </span>
                                                    </p>

                                                    <p className="flex items-center gap-2">
                                                        <i className="fa-solid fa-ticket text-[#c8a84b]" />
                                                        Precio base:
                                                        <span className="font-bold text-[#0a1628]">
                                                            ${partido.precio}
                                                        </span>
                                                    </p>

                                                    <p className="flex items-center gap-2">
                                                        <i className="fa-solid fa-users text-[#c8a84b]" />
                                                        Sectores disponibles:
                                                    </p>

                                                </div>

                                                {sectoresPartido.length > 0 ? (
                                                    <div className="flex-1">
                                                        <div className="space-y-2 max-h-40 overflow-y-auto pr-1">

                                                            {sectoresPartido.map((sector, index) => (
                                                                <div key={index} className="bg-[#f8fafc] border border-[#e4e8ef] rounded-xl p-3 flex items-center justify-between">
                                                                    <div>
                                                                        <div className="font-semibold text-[#0a1628]">
                                                                            {sector.nombreSector}
                                                                        </div>

                                                                        {sector.mailFuncionario ? (
                                                                            <div className="text-sm text-[#5f6f86] mt-1 font-bold">
                                                                                <i className="fa-solid fa-user-gear text-[#c8a84b] mr-1" />
                                                                                {sector.mailFuncionario}
                                                                            </div>
                                                                        ) : (
                                                                            <div className="text-base text-[#5f6f86] mt-1">
                                                                                No hay funcionario asignado
                                                                            </div>
                                                                        )}
                                                                    </div>

                                                                    <button onClick={() => { setIdentificadorSector(sector.identificadorSector); setIdentificadorPartido(partido.identificador); setEstadio(partido.identificadorEstadio); setOpenAsignar(true); }} className="cursor-pointer ml-4 px-3 py-1 bg-[#c8a84b] text-[#0a1628] rounded-lg hover:opacity-90 transition">
                                                                        Asignar
                                                                    </button>
                                                                </div>
                                                            ))}

                                                        </div>
                                                    </div>
                                                ) : (
                                                    <div className="flex-1">
                                                        <div className="flex space-y-2 items-center justify-center text-center max-h-46 min-h-46 overflow-y-auto">
                                                            <span>No hay sectores habilitados</span>
                                                        </div>
                                                    </div>
                                                )}

                                            </div>

                                            <div className="flex gap-2 mt-6 flex-col">

                                                <button onClick={() => { setIdentificadorPartido(partido.identificador); setEstadio(partido.identificadorEstadio); setOpenUpdate(true); }} className="cursor-pointer flex-1 bg-[#0a1628] text-[#c8a84b] py-2.5 rounded-lg font-medium hover:bg-[#13203a] transition">
                                                    <i className="fa-solid fa-door-open mr-2" />
                                                    Editar sectores
                                                </button>

                                                <button onClick={() => { setIdentificadorPartido(partido.identificador); setOpen(true); }} className="flex-1 inline-flex items-center justify-center font-medium py-2.5 rounded-lg cursor-pointer transition-all bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#dce6f5]">
                                                    <i className="fa-solid fa-pen mr-2" />
                                                    Editar partido
                                                </button>

                                                <button onClick={() => { setIdentificadorPartido(partido.identificador); setOpenDelete(true); }} className="cursor-pointer px-4 py-2.5 rounded-lg bg-red-50 text-red-600 hover:bg-red-100 transition">
                                                    <i className="fa-solid fa-trash-can mr-2" />
                                                    Eliminar partido
                                                </button>

                                            </div>

                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </div>
            </div>
            <ModalEditMatch
                open={open}
                onClose={() => setOpen(false)}
                identificador={identificadorPartido}
                onUpdateSuccess={loadPartidos}
            />

            <ModalAsignarMatch
                open={openAsignar}
                onClose={() => setOpenAsignar(false)}
                onAsignarSuccess={loadPartidos}
                identificadorPartido={identificadorPartido}
                identificadorSector={identificadorSector}
                identificadorEstadio={estadio}
            />

            <ModalSectorMatch
                open={openUpdate}
                onClose={() => setOpenUpdate(false)}
                estadio={estadio}
                identificador={identificadorPartido}
                onSectorSuccess={loadPartidos}
            />

            <ModalCreateMatch
                open={openCreate}
                onClose={() => setOpenCreate(false)}
                onCreateSuccess={loadPartidos}
            />

            <ModalEliminateMatch
                open={openDelete}
                onClose={() => setOpenDelete(false)}
                onDeleteSuccess={loadPartidos}
                identificador={identificadorPartido}
            />
        </>
    )
}