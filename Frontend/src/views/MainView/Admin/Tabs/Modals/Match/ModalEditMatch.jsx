import { useState, useEffect } from 'react';
import Modal from '../../../../../../components/modal';
import { toast } from 'react-toastify';
import getPartido from '../../../../../../services/PartidoService/getPartido'
import getAllEquipos from '../../../../../../services/EquipoService/getAllEquipos';
import getAllMyEstadios from '../../../../../../services/EstadioService/getAllMyEstadios';
import { Oval } from 'react-loader-spinner'

import putPartido from '../../../../../../services/PartidoService/putPartido'

const fases = ['Grupos', 'Octavos', 'Cuartos', 'Semifinales', 'Final'];

export default function ModalEditMatch({ open, onClose, identificador, onUpdateSuccess }) {
    const [partido, setPartido] = useState({});
    const [equipos, setEquipos] = useState([]);
    const [estadios, setEstadios] = useState([]);
    const [fase, setFase] = useState('');

    const [isLoading, setIsLoading] = useState(false);

    const [estadio, setEstadio] = useState(null);

    useEffect(() => {
        if (!open || identificador == null) return;

        async function loadData() {
            try {
                setPartido({});
                setEquipos([]);
                setEstadios([]);

                const dataEquipo = await getAllEquipos();

                const dataEstadio = await getAllMyEstadios();

                const data = await getPartido(identificador);

                setEquipos(dataEquipo || []);
                setEstadios(dataEstadio || []);
                setPartido(data || {});
                setEstadio(data?.identificadorEstadio);

            } catch (error) {
                console.error(error);
                setPartido({});
                setEquipos([]);
                setEstadios([]);
            }
        }

        loadData();
    }, [open, identificador]);

    useEffect(() => {
        if (!open) {
            setPartido({});
            setEquipos([]);
            setEstadios([]);
            setIsLoading(false);
            setEstadio(null);
        }
    }, [open]);

    const handleChangeEstadio = async (idEstadio) => {
        setEstadio(idEstadio);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (partido.equipoLocal === partido.equipoVisitante) {
            toast.error("Los equipos no pueden ser iguales.");
            return;
        }

        if (Number(partido.precio) < 0) {
            toast.error("El precio no puede ser menor a 0.");
            return;
        }

        const fechaPartido = new Date(partido.fechaHora);
        const hoy = new Date();

        if (fechaPartido < hoy) {
            toast.error("La fecha del partido no puede ser anterior a la fecha actual.");
            return;
        }

        try {
            setIsLoading(true);

            const editarPartido = await putPartido(identificador, {
                fase: fase || partido.fase,
                equipoLocal: partido.equipoLocal,
                equipoVisitante: partido.equipoVisitante,
                identificadorEstadio: estadio,
                fechaHora: partido.fechaHora,
                precio: partido.precio
            });

            if (editarPartido?.success) {
                toast.success('Partido editado correctamente');
                await onUpdateSuccess();
                onClose()
            } else {
                toast.error(editarPartido?.message || 'Error al editar partido', {
                    position: 'bottom-left',
                    autoClose: 3000,
                })
            }
        } catch (error) {
            toast.error('Error de conexión con el servidor', {
                position: 'bottom-left',
                autoClose: 3000,
            })
        } finally {
            setIsLoading(false);
        }
    };

    const capitalize = (texto = '') => texto.charAt(0).toUpperCase() + texto.slice(1);

    return (
        <Modal open={open} onClose={onClose}>
            {isLoading && (
                <div className="absolute inset-0 z-50 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-2xl">
                    <Oval height={35} width={35} color="#052e66" secondaryColor="#e5e7eb" strokeWidth={4} strokeWidthSecondary={4} ariaLabel="loading-create-group" />
                </div>
            )}
            <form onSubmit={handleSubmit} className="w-full max-w-5xl mx-auto space-y-8">
                <div>
                    <h2 className="text-3xl font-bold text-[#14315C]">
                        Editar <span className="text-[#D4AF37]">Partido</span>
                    </h2>
                </div>
                <div className="flex items-center gap-3 mt-5 mb-2">
                    <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Equipo local vs. Equipo visitante</span>
                    <div className="flex-1 h-px bg-[#045694]/20" />
                </div>

                <div className="grid gap-6 md:grid-cols-2">

                    <div>
                        <select value={partido.equipoLocal || ''} onChange={(e) => setPartido({ ...partido, equipoLocal: e.target.value })} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none">
                            {equipos.filter((equipo) => equipo.nombre !== partido.equipoVisitante).map((equipo) => (
                                <option key={equipo.nombre} value={equipo.nombre}>
                                    {capitalize(equipo.nombre)}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div>
                        <select value={partido.equipoVisitante || ''} onChange={(e) => setPartido({ ...partido, equipoVisitante: e.target.value })} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none">
                            {equipos.filter((equipo) => equipo.nombre !== partido.equipoLocal).map((equipo) => (
                                <option key={equipo.nombre} value={equipo.nombre}>
                                    {capitalize(equipo.nombre)}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="md:col-span-2">
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Fase</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>

                        <select value={fase || partido.fase || ''} onChange={(e) => setFase(e.target.value)} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none">
                            <option value={partido.fase}>
                                {partido.fase}
                            </option>



                            {fases.filter(f => f !== partido.fase).map(fase => (
                                <option key={fase} value={fase}>
                                    {fase}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div>
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Fecha y hora</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>

                        <input
                            type="datetime-local"
                            value={partido.fechaHora || ''}
                            onChange={(e) =>
                                setPartido({
                                    ...partido,
                                    fechaHora: e.target.value
                                })
                            }
                            className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Precio base</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>

                        <input
                            type="number"
                            value={partido.precio || ''}
                            onChange={(e) =>
                                setPartido({
                                    ...partido,
                                    precio: Number(e.target.value)
                                })
                            }
                            className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>


                    <div className="md:col-span-2">
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Estadio</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>

                        <select value={estadio || partido.identificadorEstadio || ''} onChange={(e) => handleChangeEstadio(Number(e.target.value))} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" >
                            <option value={partido.identificadorEstadio}>{partido.nombreEstadio}</option>
                            {estadios.map((estadio) => {
                                if (estadio.nombre === partido.nombreEstadio) {
                                    return null;
                                }

                                return (
                                    <option key={estadio.identificador} value={estadio.identificador}>
                                        {estadio.nombre}
                                    </option>
                                );
                            })}
                        </select>
                    </div>
                </div>

                <div className="flex justify-end gap-4">
                    <button type="button" onClick={onClose} className="lg:hidden inline rounded-xl transition-all cursor-pointer px-6 py-3 font-semibold border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        Cancelar
                    </button>

                    <button type="submit" className="cursor-pointer transition-all rounded-xl bg-[#14315C] px-6 py-3 font-semibold text-white hover:bg-[#1c4378]">
                        Guardar cambios
                    </button>
                </div>
            </form>
        </Modal>
    )
}