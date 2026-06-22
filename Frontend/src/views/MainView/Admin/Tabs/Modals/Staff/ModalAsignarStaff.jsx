import { useState, useEffect } from 'react';
import Modal from '../../../../../../components/modal';
import { toast } from 'react-toastify';
import { Oval } from 'react-loader-spinner';
import getDispositivosNoAsignados from '../../../../../../services/DispositivoService/getDispositivosNoAsignados';
import putDispositivoMail from '../../../../../../services/DispositivoService/putDispositivoMail';

export default function ModalAsignarStaff({ open, onClose, onAsignarSuccess, mailFuncionario }) {
    const [isLoading, setIsLoading] = useState(false);
    const [data, setData] = useState([]);
    const [dispositivoSeleccionado, setDispositivoSeleccionado] = useState(null);

    const loadDispositivos = async () => {
        try {
            const dispositivos = await getDispositivosNoAsignados();
            if (!dispositivos) return;

            setData(dispositivos);
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        if (mailFuncionario) {
            loadDispositivos();
        }
    }, [mailFuncionario]);

    useEffect(() => {
        if (!open) {
            setIsLoading(false);
            setData([]);
            setDispositivoSeleccionado(null);
        }
    }, [open]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!dispositivoSeleccionado) {
            toast.error("Seleccioná un dispositivo");
            return;
        }

        try {
            setIsLoading(true);

            const response = await putDispositivoMail(
                dispositivoSeleccionado,
                mailFuncionario
            );

            if (response.success) {
                toast.success(response.message);

                await onAsignarSuccess()

                onClose();
            } else {
                toast.error(response.message || "No se pudo asignar el dispositivo");
            }
        } catch (error) {
            console.error(error);
            toast.error("Ocurrió un error al asignar el dispositivo");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <Modal open={open} onClose={onClose}>
            {isLoading && (
                <div className="absolute inset-0 z-300 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-2xl">
                    <Oval height={35} width={35} color="#052e66" secondaryColor="#e5e7eb" strokeWidth={4} strokeWidthSecondary={4} ariaLabel="loading-create-group" />
                </div>
            )}
            <form onSubmit={handleSubmit} className="w-full max-w-4xl mx-auto space-y-8">
                <div>
                    <h2 className="text-3xl font-bold text-[#14315C]">
                        Asignar <span className="text-[#D4AF37]">Dispositivo</span>
                    </h2>

                    <p className="mt-2 text-sm text-gray-500">
                        Seleccioná uno de los dispositivos disponibles para asignarlo al funcionario.
                    </p>
                </div>

                <div className="flex items-center gap-3">
                    <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">
                        Dispositivos disponibles
                    </span>
                    <div className="flex-1 h-px bg-[#045694]/20" />
                </div>

                <div className="max-h-[420px] overflow-y-auto rounded-2xl border border-gray-200 bg-gray-50 p-3 space-y-3">

                    {data.length === 0 && (
                        <div className="flex h-40 items-center justify-center text-gray-500">
                            No hay dispositivos disponibles.
                        </div>
                    )}

                    {data.map((dispositivo) => (
                        <label key={dispositivo.identificador} className={`block cursor-pointer rounded-2xl border bg-white p-5 transition-all ${dispositivoSeleccionado === dispositivo.identificador ? "border-[#D4AF37] ring-2 ring-[#D4AF37]/30 shadow-md" : "border-gray-200 hover:border-[#045694]/50 hover:shadow-sm"}`}>
                            <input type="radio" name="dispositivo" value={dispositivo.identificador} checked={dispositivoSeleccionado === dispositivo.identificador} onChange={() => setDispositivoSeleccionado(dispositivo.identificador)} className="hidden" />
                            <div className="flex justify-between items-center">
                                <div className="space-y-1">
                                    <h3 className="font-semibold text-lg text-[#14315C]">
                                        Dispositivo #{dispositivo.identificador}
                                    </h3>
                                </div>

                                <div className={`h-6 w-6 rounded-full border-2 flex items-center justify-center transition-all ${dispositivoSeleccionado === dispositivo.identificador ? "border-[#D4AF37] bg-[#D4AF37]" : "border-gray-300"}`}>
                                    {dispositivoSeleccionado === dispositivo.identificador && (
                                        <div className="h-2.5 w-2.5 rounded-full bg-white" />
                                    )}
                                </div>
                            </div>
                        </label>
                    ))}
                </div>

                <div className="flex justify-end gap-4">
                    <button type="button" onClick={onClose} className="cursor-pointer rounded-xl border border-[#d0dcea] px-6 py-3 font-semibold text-[#0a1628] hover:bg-[#f0f4fa] transition-all">
                        Cancelar
                    </button>

                    <button type="submit" disabled={!dispositivoSeleccionado || isLoading} className="cursor-pointer rounded-xl bg-[#14315C] px-6 py-3 font-semibold text-white hover:bg-[#1c4378] disabled:opacity-50 disabled:cursor-not-allowed transition-all">
                        Asignar dispositivo
                    </button>
                </div>
            </form>
        </Modal>
    )
}