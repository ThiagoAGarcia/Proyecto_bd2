import { useState, useEffect } from 'react';
import Modal from '../../../../../../components/modal';
import { toast } from 'react-toastify';
import { Oval } from 'react-loader-spinner';
import postEstadio from '../../../../../../services/EstadioService/postEstadio';


export default function ModalCreateStadium({ open, onClose, onCreateSuccess }) {
    const [isLoading, setIsLoading] = useState(false);
    const [imagen, setImagen] = useState('');
    const [nombre, setNombre] = useState('');
    const [ciudad, setCiudad] = useState('');
    const [calle, setCalle] = useState('');
    const [numero, setNumero] = useState('');
    const [codigoPostal, setCodigoPostal] = useState('');

    useEffect(() => {
        if (!open) {
            setIsLoading(false);
            setImagen('');
        }
    }, [open]);

    const handleSubmit = async (e) => {
        e.preventDefault()

        const BODY = {
            Nombre: nombre,
            Imagen: imagen,
            DireccionLocalidad: ciudad,
            DireccionCalle: calle,
            DireccionNumero: numero ? parseInt(numero, 10) : null,
            DireccionCodigoPostal: codigoPostal ? parseInt(codigoPostal, 10) : null,
        }

        try {
            setIsLoading(true)
            const crearEstadio = await postEstadio(BODY)
            console.log(crearEstadio);
            if (crearEstadio?.success) {
                toast.success('Estadio creado');
                await onCreateSuccess();
                onClose()
            } else {
                toast.error(crearEstadio?.description || 'Error al registrar', {
                    position: 'bottom-left',
                    autoClose: 3000,
                })
            }
        } catch (error) {
            console.error(error)
            toast.error('Error de conexión con el servidor', {
                position: 'bottom-left',
                autoClose: 3000,
            })
        } finally {
            setIsLoading(false)
        }
    }

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
                        Crear <span className="text-[#D4AF37]">Estadio</span>
                    </h2>
                </div>
                <div className="flex items-center gap-3 mt-5 mb-2">
                    <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Nombre</span>
                    <div className="flex-1 h-px bg-[#045694]/20" />
                </div>

                <div className="grid gap-6 md:grid-cols-2">

                    <div className="md:col-span-2">
                        <input type='text' value={nombre} onChange={(e) => setNombre(e.target.value)} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                    </div>

                    <div className="md:col-span-2">
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Imagen</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                        <input type='text' value={imagen} onChange={(e) => setImagen(e.target.value)} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                        {imagen && (
                            <img src={imagen} alt="Vista previa" className="mt-3 h-82 rounded-xl" />
                        )}
                    </div>

                    <div>
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Ciudad</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                        <input type='text' value={ciudad} onChange={(e) => setCiudad(e.target.value)} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                    </div>

                    <div>
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Calle</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                        <input type='text' value={calle} onChange={(e) => setCalle(e.target.value)} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                    </div>

                    <div>
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Número</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                        <input type='number' value={numero} onChange={(e) => setNumero(e.target.value)} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                    </div>

                    <div>
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">Código Postal</span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                        <input type='number' value={codigoPostal} onChange={(e) => setCodigoPostal(e.target.value)} className="w-full rounded-xl border border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                    </div>
                </div>

                <div className="flex justify-end gap-4">
                    <button type="button" onClick={onClose} className="lg:hidden inline rounded-xl transition-all cursor-pointer px-6 py-3 font-semibold border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        Cancelar
                    </button>

                    <button type="submit" className="cursor-pointer transition-all rounded-xl bg-[#14315C] px-6 py-3 font-semibold text-white hover:bg-[#1c4378]">
                        Crear estadio
                    </button>
                </div>
            </form>
        </Modal>
    )
}