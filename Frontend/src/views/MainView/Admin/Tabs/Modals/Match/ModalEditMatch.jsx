import { useState, useEffect } from 'react';
import Modal from '../../../../../../components/modal';
import { toast } from 'react-toastify';
import getPartido from '../../../../../../services/PartidoService/getPartido'
import getAllEquipos from '../../../../../../services/EquipoService/getAllEquipos';
import getAllMyEstadios from '../../../../../../services/EstadioService/getAllMyEstadios';
import getAllHabilita from '../../../../../../services/HabilitaService/getAllHabilita';
import getAllSectores from '../../../../../../services/SectorService/getAllSectores';
import { Oval } from 'react-loader-spinner'

import getAllNoAsignados from '../../../../../../services/EsAsignadoService/getAllNoAsignados';
import getAllAsignados from '../../../../../../services/EsAsignadoService/getAllAsignados';

import putPartido from '../../../../../../services/PartidoService/putPartido'
import putUpdateHabilita from '../../../../../../services/HabilitaService/putUpdateHabilita'

export default function ModalEditMatch({ open, onClose, identificador, onUpdateSuccess }) {
    const [partido, setPartido] = useState({});
    const [equipos, setEquipos] = useState([]);
    const [estadios, setEstadios] = useState([]);
    const [sectores, setSectores] = useState([]);

    const [isLoading, setIsLoading] = useState(false);

    const [habilita, setHabilita] = useState([]);

    const [sectorSeleccionado, setSectorSeleccionado] = useState('');
    const [funcionarioSeleccionado, setFuncionarioSeleccionado] = useState('');
    const [estadio, setEstadio] = useState(null);
    const [estadioCambiado, setEstadioCambiado] = useState(false);

    const [losNoAsignados, setLosNoAsignados] = useState([]);

    const [yaAsignados, setYaAsignados] = useState([])
    const [sectorFuncionario, setSectorFuncionario] = useState(null);

    const [fase, setFase] = useState('')

    /********************************************** CARGAR TODOS LOS DATOS DEL MODAL **********************************************/

    useEffect(() => {
        if (!open) return;

        async function loadData() {
            try {
                setPartido({});
                setEquipos([]);
                setEstadios([]);
                setLosNoAsignados([]);

                const dataEquipo = await getAllEquipos();

                const dataEstadio = await getAllMyEstadios();

                const dataAsignados = await getAllNoAsignados();

                const data = await getPartido(identificador);

                setLosNoAsignados(dataAsignados || []);
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

    /********************************************** CARGAR LOS SECTORES QUE SE TIENEN Y LOS HABILITADOS **********************************************/

    useEffect(() => {
        if (!open || !estadio) return;

        async function loadSector() {
            const dataSectores = await getAllSectores(estadio);

            setSectores(dataSectores || []);

            if (!estadioCambiado) {
                const data = await getAllHabilita(estadio, identificador);
                setHabilita(data || []);
            }
        }

        loadSector();
    }, [open, estadio, identificador, estadioCambiado]);

    /********************************************** CARGAR LOS DATOS DE LOS QUE YA ESTAN ASIGNADOS **********************************************/

    useEffect(() => {
        if (!open || !estadio || !sectorFuncionario || !identificador) return;

        async function loadFuncionariosYaAsignados() {
            try {
                const data = await getAllAsignados(estadio, sectorFuncionario, identificador);
                setYaAsignados(data || []);
            } catch (error) {
                console.error(error);
                setYaAsignados([]);
            }
        }

        loadFuncionariosYaAsignados();
    }, [open, estadio, sectorFuncionario, identificador]);

    /********************************************** SETEAR TODO A NULL CUANDO SE ABRE EL MODAL **********************************************/

    useEffect(() => {
        if (!open) {
            setPartido({});
            setEquipos([]);
            setEstadios([]);
            setSectores([]);
            setIsLoading(false);
            setHabilita([]);
            setEstadioCambiado(false);
            setEstadio(null);
            setSectorSeleccionado('');
            setLosNoAsignados([]);
            setYaAsignados([]);
            setSectorFuncionario('');
        }
    }, [open]);

    /********************************************** HABILITAR SECTORES O ELIMINARLOS **********************************************/

    const handleAgregarSector = () => {
        const sector = sectores.find(
            (s) => s.identificador === sectorSeleccionado
        );

        if (!sector) return;

        setHabilita([...habilita, sector]);
        setSectorSeleccionado('');
    };

    const handleEliminarSector = (id) => {
        if (
            sectorFuncionario === id &&
            yaAsignados.length > 0
        ) {
            toast.error('No se puede eliminar el sector porque tiene un funcionario asignado');
            return;
        }

        setHabilita(
            habilita.filter((s) => s.identificador !== id)
        );
    };

    /********************************************** ESTADIO **********************************************/

    const handleChangeEstadio = async (idEstadio) => {
        setEstadioCambiado(true);
        setEstadio(idEstadio);
        setHabilita([]);
    };

    /********************************************** AGREGAR FUNCIONARIO **********************************************/

    const handleAgregarFuncionario = () => {
        if (yaAsignados.length > 0) {
            toast.error('Este sector ya tiene un funcionario asignado');
            return;
        }

        const funcionario = losNoAsignados.find(
            (s) => s.mailFuncionario === funcionarioSeleccionado
        );

        if (!funcionario) return;

        setYaAsignados([funcionario]);

        setLosNoAsignados(prev =>
            prev.filter(
                f => f.mailFuncionario !== funcionario.mailFuncionario
            )
        );

        setFuncionarioSeleccionado('');
    };

    /********************************************** ELIMINAR FUNCIONARIO **********************************************/

    const handleEliminarFuncionario = (mail) => {
        const funcionario = yaAsignados.find(
            f => f.mailFuncionario === mail
        );

        if (!funcionario) return;

        setYaAsignados(prev =>
            prev.filter(
                f => f.mailFuncionario !== mail
            )
        );

        setLosNoAsignados(prev => [
            ...prev,
            funcionario
        ]);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            setIsLoading(true);

            await putPartido(identificador, {
                fase: fase || partido.fase,
                equipoLocal: partido.equipoLocal,
                equipoVisitante: partido.equipoVisitante,
                identificadorEstadio: estadio,
                fechaHora: partido.fechaHora,
                precio: partido.precio
            });

            await putUpdateHabilita({
                identificadorEstadio: estadio,
                identificadorPartido: identificador,
                sectores: habilita.map(s => s.identificador)
            });

            toast.success('Partido actualizado correctamente');

            await onUpdateSuccess();

            onClose();
        } catch (error) {
            console.error(error);
            toast.error('Error al actualizar el partido');
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

                            {['Grupos', 'Octavos', 'Cuartos', 'Semifinales', 'Final'].filter(f => f !== partido.fase).map(f => (
                                <option key={f} value={f}>
                                    {f}
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

                <div className="rounded-2xl border border-[#14315C]/10 bg-[#F8F9FB] p-5">
                    <div className="flex items-center gap-3 mb-3">
                        <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Sectores disponibles</span>
                        <div className="flex-1 h-px bg-[#045694]/20" />
                    </div>

                    <div className="flex gap-3 mb-4">
                        <select value={sectorSeleccionado} onChange={(e) => setSectorSeleccionado(Number(e.target.value))} className="flex-1 rounded-xl border focus:border-[#D4AF37] border-gray-300 px-4 py-3 focus:outline-none">
                            <option value="">Seleccionar sector...</option>

                            {sectores.filter((s) => !habilita.some((h) => h.identificador === s.identificador)).map((sector) => (
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

                {/*************************************************************************** FUNCIONARIOS ASIGNADOS ********************************************************************/}

                <div className="rounded-2xl border border-[#14315C]/10 bg-[#F8F9FB] p-5">
                    <div className="flex items-center gap-3 mb-3">
                        <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Funcionarios disponibles</span>
                        <div className="flex-1 h-px bg-[#045694]/20" />
                    </div>

                    <div className="flex gap-3 mb-4">
                        <select value={sectorFuncionario ?? ''} onChange={(e) => setSectorFuncionario(Number(e.target.value))} className="flex-1 rounded-xl border focus:border-[#D4AF37] border-gray-300 px-4 py-3 focus:outline-none">
                            <option value="">Seleccionar sector...</option>

                            {habilita.map((sector) => (
                                <option key={sector.identificador} value={sector.identificador}>
                                    {sector.nombre}
                                </option>
                            ))}
                        </select>
                    </div>

                    {/************************************ PRONTO PARA ARRIBA ************************************/}

                    <div className="flex gap-3 mb-4">
                        <select
                            value={funcionarioSeleccionado ?? ''}
                            onChange={(e) => setFuncionarioSeleccionado(e.target.value)}
                            disabled={yaAsignados.length > 0}
                            className="flex-1 rounded-xl border focus:border-[#D4AF37] border-gray-300 px-4 py-3 focus:outline-none"
                        >
                            <option value="">Seleccionar funcionario...</option>

                            {losNoAsignados.map((noAsignado) => (
                                <option key={noAsignado.mailFuncionario} value={noAsignado.mailFuncionario}>
                                    {noAsignado.mailFuncionario}
                                </option>
                            ))}
                        </select>

                        <button
                            type="button"
                            onClick={handleAgregarFuncionario}
                            disabled={yaAsignados.length > 0}
                            className="cursor-pointer transition-all rounded-xl px-5 py-3 border-none bg-[#0a1628] text-[#c8a84b]/60 hover:bg-[#0a1628]/90 disabled:opacity-50"
                        >
                            <i className="fa-solid fa-plus mr-2 text-[#c8a84b]"></i>
                            Agregar
                        </button>
                    </div>

                    <section className="flex gap-6">
                        {yaAsignados.length === 0 ? (
                            <p className="px-7 py-8 w-full text-base text-[#8A93A6] text-center">
                                No tenés funcionarios asignados.
                            </p>
                        ) : (
                            <ul className="divide-y divide-[#F0EDE3] h-20 w-full">
                                {yaAsignados.map((func) => (
                                    <li key={func.identificador ?? func.mailFuncionario} className="px-4 py-4 mb-3 flex items-center justify-between gap-4 bg-white rounded-2xl">
                                        <div className="flex items-center gap-4 min-w-0">
                                            <span className="flex items-center justify-center w-12 h-12 rounded-full bg-[#EEF3FA] text-[#1E4976] shrink-0">
                                                <i className="fa-solid fa-user-gear"></i>
                                            </span>

                                            <div className="min-w-0">
                                                <p className="text-base font-medium text-[#1F2A3C] truncate">
                                                    {func.mailFuncionario}
                                                </p>

                                            </div>
                                        </div>

                                        <button className="shrink-0 text-center cursor-pointer text-[#B7894A] hover:text-[#B3261E] hover:bg-[#FBE9E7] p-3 px-4 rounded-full transition" type="button" onClick={() => handleEliminarFuncionario(func.mailFuncionario)}>
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