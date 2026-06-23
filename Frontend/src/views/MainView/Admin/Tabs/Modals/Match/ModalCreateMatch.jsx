import { useState, useEffect } from 'react';
import Modal from '../../../../../../components/modal';
import { toast } from 'react-toastify';
import getAllEquipos from '../../../../../../services/EquipoService/getAllEquipos';
import getAllMyEstadios from '../../../../../../services/EstadioService/getAllMyEstadios';
import postPartido from '../../../../../../services/PartidoService/postPartido';
import { Oval } from 'react-loader-spinner'

const fases = ['Grupos', 'Octavos', 'Cuartos', 'Semifinales', 'Final'];

export default function ModalCreateMatch({ open, onClose, onCreateSuccess }) {
    const [equipos, setEquipos] = useState([]);
    const [estadios, setEstadios] = useState([]);
    const [isLoading, setIsLoading] = useState(false);

    const [form, setForm] = useState({
        fase: '',
        equipoLocal: '',
        equipoVisitante: '',
        identificadorEstadio: '',
        fechaHora: '',
        precio: '',
    })

    useEffect(() => {
        if (!open) return;

        async function loadData() {
            try {
                setEquipos([]);
                setEstadios([]);

                const dataEquipo = await getAllEquipos();

                const dataEstadio = await getAllMyEstadios();

                setEquipos(dataEquipo || []);
                setEstadios(dataEstadio || []);
            } catch (error) {
                console.error(error);
                setEquipos([]);
                setEstadios([]);
            }
        }

        loadData();
    }, [open]);

    useEffect(() => {
        if (!open) {
            setEquipos([]);
            setEstadios([]);
            setForm({
                fase: '',
                equipoLocal: '',
                equipoVisitante: '',
                identificadorEstadio: '',
                fechaHora: '',
                precio: '',
            });
        }
    }, [open]);

    const handleChange = (e) => {
        const { name, value } = e.target;

        setForm(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (
            !form.fase || !form.equipoLocal ||
            !form.equipoVisitante ||
            !form.identificadorEstadio ||
            !form.fechaHora ||
            !form.precio
        ) {
            toast.error("Complete todos los campos.");
            return;
        }

        if (form.equipoLocal === form.equipoVisitante) {
            toast.error("Los equipos no pueden ser iguales.");
            return;
        }

        if (Number(form.precio) < 0) {
            toast.error("El precio no puede ser menor a 0.");
            return;
        }

        const fechaPartido = new Date(form.fechaHora);
        const hoy = new Date();

        if (fechaPartido < hoy) {
            toast.error("La fecha del partido no puede ser anterior a la fecha actual.");
            return;
        }

        const BODY = {
            fase: form.fase,
            equipoLocal: form.equipoLocal,
            equipoVisitante: form.equipoVisitante,
            identificadorEstadio: Number(form.identificadorEstadio),
            fechaHora: new Date(form.fechaHora).toISOString(),
            precio: Number(form.precio),
        }

        try {
            setIsLoading(true)
            const crearPartido = await postPartido(BODY)
            if (crearPartido?.success) {
                toast.success('Partido creado');
                await onCreateSuccess();
                onClose()
            } else {
                toast.error(crearPartido?.message || 'Error al crear partido', {
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
            setIsLoading(false)
        }
    }

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
                        <select name="equipoLocal" value={form.equipoLocal} onChange={handleChange} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none">
                            <option value="">Seleccionar equipo local...</option>

                            {equipos.filter(equipo => equipo.nombre !== form.equipoVisitante).map((equipo) => (
                                <option key={equipo.nombre} value={equipo.nombre}>
                                    {capitalize(equipo.nombre)}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div>
                        <select name="equipoVisitante" value={form.equipoVisitante} onChange={handleChange} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none">
                            <option value="">Seleccionar equipo visitante...</option>

                            {equipos.filter(equipo => equipo.nombre !== form.equipoLocal).map((equipo) => (
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

                        <select name="fase" value={form.fase} onChange={handleChange} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none">
                            <option value="">
                                Seleccionar fase...
                            </option>

                            {fases.map((fase) => (
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

                        <input type="datetime-local" name="fechaHora" value={form.fechaHora} onChange={handleChange} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                    </div>

                    <div>
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Precio base</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>

                        <input type="number" name="precio" value={form.precio} onChange={handleChange} min="0" className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                    </div>


                    <div className="md:col-span-2">
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Estadio</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>

                        <select name="identificadorEstadio" value={form.identificadorEstadio} onChange={handleChange} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none">
                            <option value="">Seleccionar estadio...</option>

                            {estadios.map((estadio) => (
                                <option key={estadio.identificador} value={estadio.identificador}>
                                    {estadio.nombre}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                <div className="flex justify-end gap-4">
                    <button type="button" onClick={onClose} className="lg:hidden inline rounded-xl transition-all cursor-pointer px-6 py-3 font-semibold border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        Cancelar
                    </button>

                    <button type="submit" className="cursor-pointer transition-all rounded-xl bg-[#14315C] px-6 py-3 font-semibold text-white hover:bg-[#1c4378]">
                        Crear partido
                    </button>
                </div>
            </form>
        </Modal>
    )
}