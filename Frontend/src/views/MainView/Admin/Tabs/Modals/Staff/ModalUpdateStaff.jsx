import { useState, useEffect } from 'react';
import Modal from '../../../../../../components/modal';
import { toast } from 'react-toastify';
import { Oval } from 'react-loader-spinner';
import putPerfil from '../../../../../../services/PerfilService/putPerfil';
import getPerfilPorMail from '../../../../../../services/PerfilService/getPerfilPorMail';

const TIPOS_DOCUMENTO = ['CI', 'DNI', 'CPF', 'RUT', 'CC', 'CURP', 'SSN', 'SIN']

export default function ModalUpdateStaff({ open, onClose, onUpdateSuccess, identificador }) {
    const [isLoading, setIsLoading] = useState(false);
    const [telefonos, setTelefonos] = useState(['']);
    const [data, setData] = useState({
        paisDocumento: "",
        tipoDocumento: "",
        numeroDocumento: "",
        direccionPais: "",
        direccionLocalidad: "",
        direccionCalle: "",
        direccionNumero: "",
        direccionCodigoPostal: ""
    });

    const loadPerfil = async () => {
        try {
            const perfil = await getPerfilPorMail(identificador);
            if (!perfil) return;

            setData(perfil);
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        if (identificador) {
            loadPerfil();
        }
    }, [identificador]);

    useEffect(() => {
        if (!open) {
            setIsLoading(false);
            setTelefonos(['']);
        }
    }, [open]);

    const handleChange = (e) => {
        const { name, value } = e.target;

        setData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            setIsLoading(true);

            await putPerfil(identificador, {
                paisDocumento: data.paisDocumento,
                tipoDocumento: data.tipoDocumento,
                numeroDocumento: data.numeroDocumento,
                direccionPais: data.direccionPais,
                direccionLocalidad: data.direccionLocalidad,
                direccionCalle: data.direccionCalle,
                direccionNumero: Number(data.direccionNumero),
                direccionCodigoPostal: Number(data.direccionCodigoPostal),
            });

            toast.success("Perfil actualizado correctamente");

            onUpdateSuccess?.();
            onClose();
        } catch (error) {
            console.error(error);

            toast.error(
                error.response?.data || "Ocurrió un error al actualizar el perfil"
            );
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
            <form onSubmit={handleSubmit} className="w-full max-w-5xl mx-auto space-y-8">
                <div>
                    <h2 className="text-3xl font-bold text-[#14315C]">
                        Editar <span className="text-[#D4AF37]">Funcionario</span>
                    </h2>
                </div>
                <div className="grid gap-4 md:grid-cols-2">

                    <div className="md:col-span-2">
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">
                                Documento de identidad
                            </span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">País del documento</label>
                        <input
                            type="text"
                            name="paisDocumento"
                            value={data.paisDocumento || ""}
                            onChange={handleChange}
                            disabled={isLoading}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Tipo de documento</label>
                        <select
                            name="tipoDocumento"
                            value={data.tipoDocumento?.toUpperCase() || ""}
                            onChange={handleChange}
                            disabled={isLoading}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-4 focus:border-[#D4AF37] focus:outline-none"
                        >
                            <option value="">Seleccionar</option>
                            {TIPOS_DOCUMENTO.map((tipo) => (
                                <option key={tipo} value={tipo}>
                                    {tipo}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="md:col-span-2">
                        <label className="text-sm text-gray-700">Número de documento</label>
                        <input
                            type="text"
                            name="numeroDocumento"
                            value={data.numeroDocumento || ""}
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div className="md:col-span-2 mt-4">
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">
                                Dirección
                            </span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">País</label>
                        <input
                            name="direccionPais"
                            value={data.direccionPais || ""}
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Localidad</label>
                        <input
                            name="direccionLocalidad"
                            value={data.direccionLocalidad || ""}
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div className="md:col-span-2">
                        <label className="text-sm text-gray-700">Calle</label>
                        <input
                            name="direccionCalle"
                            value={data.direccionCalle || ""}
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Número</label>
                        <input
                            name="direccionNumero"
                            value={data.direccionNumero || ""}
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Código Postal</label>
                        <input
                            name="direccionCodigoPostal"
                            value={data.direccionCodigoPostal || ""}
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>
                </div>

                <div className="flex justify-end gap-4">
                    <button type="button" onClick={onClose} className="lg:hidden inline rounded-xl transition-all cursor-pointer px-6 py-3 font-semibold border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        Cancelar
                    </button>

                    <button type="submit" className="cursor-pointer transition-all rounded-xl bg-[#14315C] px-6 py-3 font-semibold text-white hover:bg-[#1c4378]">
                        Editar funcionario
                    </button>
                </div>
            </form>
        </Modal>
    )
}