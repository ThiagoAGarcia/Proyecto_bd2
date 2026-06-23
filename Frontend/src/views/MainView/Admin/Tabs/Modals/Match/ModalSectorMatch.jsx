import { useState, useEffect } from 'react';
import Modal from '../../../../../../components/modal';
import { toast } from 'react-toastify';
import getAllSectores from '../../../../../../services/SectorService/getAllSectores';
import { Oval } from 'react-loader-spinner'

import putUpdateHabilita from '../../../../../../services/HabilitaService/putUpdateHabilita'

export default function ModalSectorMatch({ open, onClose, estadio, identificador, onSectorSuccess }) {
    const [sectores, setSectores] = useState([]);

    const [isLoading, setIsLoading] = useState(false);

    const [habilita, setHabilita] = useState([]);

    const [sectorSeleccionado, setSectorSeleccionado] = useState('')

    useEffect(() => {
        if (!open || !estadio) return;

        async function loadSector() {
            try {
                setSectores([]);

                const dataSectores = await getAllSectores(estadio);

                setSectores(dataSectores || []);
            } catch (error) {
                console.error(error);
                setSectores([])
            }
        }

        loadSector();
    }, [open, estadio]);

    useEffect(() => {
        if (!open) {
            setSectores([]);
            setIsLoading(false);
            setHabilita([]);
            setSectorSeleccionado('');
        }
    }, [open]);


    const handleAgregarSector = () => {
        const sector = sectores.find(
            (s) => s.identificador === sectorSeleccionado
        );

        if (!sector) return;

        setHabilita([...habilita, sector]);
        setSectorSeleccionado('');
    };

    const sectoresDisponibles = sectores.filter(
        (sector) =>
            !habilita.some((h) => h.identificador === sector.identificador)
    );

    const handleEliminarSector = (id) => {
        setHabilita(
            habilita.filter((s) => s.identificador !== id)
        );
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            setIsLoading(true);

            const actualizarPartido = await putUpdateHabilita({ identificadorEstadio: estadio, identificadorPartido: identificador, sectores: habilita.map(s => s.identificador) });
            if (actualizarPartido?.success) {
                toast.success('Partido actualizado');
                await onSectorSuccess();
                onClose()
            } else {
                toast.error(actualizarPartido?.message || 'Error al crear partido', {
                    position: 'bottom-left',
                    autoClose: 3000,
                })
            }
        } catch (error) {
            toast.error(actualizarPartido?.message || 'Error de conexión con el servidor', {
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
                        Editar <span className="text-[#D4AF37]">Sectores</span>
                    </h2>
                </div>
                <div className="flex items-center gap-3 mb-3">
                    <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Sectores disponibles</span>
                    <div className="flex-1 h-px bg-[#045694]/20" />
                </div>
                <div className="rounded-2xl border border-[#14315C]/10 bg-[#F8F9FB] p-5">


                    <div className="flex gap-3 mb-4">
                        <select value={sectorSeleccionado} onChange={(e) => setSectorSeleccionado(Number(e.target.value))} className="flex-1 rounded-xl border focus:border-[#D4AF37] border-gray-300 px-4 py-3 focus:outline-none">
                            <option value="">Seleccionar sector...</option>

                            {sectoresDisponibles.map((sector) => (
                                <option key={sector.identificador} value={sector.identificador}>
                                    {sector.nombre}
                                </option>
                            ))}
                        </select>

                        <button type="button" onClick={handleAgregarSector} className="cursor-pointer transition-all rounded-xl px-5 py-3 border-none bg-[#0a1628] text-[#c8a84b]/60 hover:bg-[#0a1628]/90">
                            <i className="fa-solid fa-plus mr-2 text-[#c8a84b]"></i>
                            Agregar
                        </button>
                    </div>

                    <section className="flex gap-6">
                        {habilita.length === 0 ? (
                            <p className="px-7 py-12 w-full text-base text-[#8A93A6] text-center">
                                No tenés sectores registrados.
                            </p>
                        ) : (
                            <ul className="divide-y divide-[#F0EDE3] overflow-y-auto h-70 w-full">
                                {habilita.map((sector) => (
                                    <li key={sector.identificador} className="px-4 py-4 mb-3 flex items-center justify-between gap-4 bg-white rounded-2xl">
                                        <div className="flex items-center gap-4 min-w-0">
                                            <span className="flex items-center justify-center w-12 h-12 rounded-full bg-[#EEF3FA] text-[#1E4976] shrink-0">
                                                <i className="fa-solid fa-door-closed"></i>
                                            </span>

                                            <div className="min-w-0">
                                                <p className="text-base font-medium text-[#1F2A3C] truncate">
                                                    {sector.nombre}
                                                </p>

                                                <p className="text-sm text-[#8A93A6]">
                                                    Capacidad: {sector.capacidadMaxima}
                                                </p>
                                            </div>
                                        </div>

                                        <button className="shrink-0 text-center cursor-pointer text-[#B7894A] hover:text-[#B3261E] hover:bg-[#FBE9E7] p-3 px-4 rounded-full transition" type="button" onClick={() => handleEliminarSector(sector.identificador)}>
                                            <i className="fa-regular fa-trash-can"></i>
                                        </button>
                                    </li>
                                ))}
                            </ul>
                        )}
                    </section>
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