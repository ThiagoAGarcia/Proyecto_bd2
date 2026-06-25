import Modal from '../../../../../../components/modal';
import { useState, useEffect } from 'react';
import deleteEstadio from '../../../../../../services/EstadioService/deleteEstadio';
import { toast } from 'react-toastify';
import { Oval } from 'react-loader-spinner';

export default function ModalEliminateStadium({ open, onClose, onCreateSuccess, identificador }) {
    const [isLoading, setIsLoading] = useState(false);

    async function handleDeleteEstadio() {
        try {
            setIsLoading(true);

            const eliminarEstadio = await deleteEstadio(identificador);

            if (eliminarEstadio?.success) {
                toast.success("Estadio eliminado correctamente");
                await onCreateSuccess();
                onClose();

            } else {
                toast.error(eliminarEstadio?.message || 'Error al eliminar', {
                    position: 'bottom-left',
                    autoClose: 3000,
                })
            }
        } catch (error) {
            toast.error("No se pudo eliminar el estadio");
        } finally {
            setIsLoading(false);
        }
    }

    return (
        <Modal open={open} onClose={isLoading ? undefined : onClose}>
            {isLoading && (
                <div className="absolute inset-0 z-300 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-2xl">
                    <Oval height={35} width={35} color="#052e66" secondaryColor="#e5e7eb" strokeWidth={4} strokeWidthSecondary={4} ariaLabel="loading-create-group" />
                </div>
            )}
            <div className="flex items-center text-xl gap-3 mb-4">
                <div className="w-15 h-15 rounded-full bg-red-100 flex items-center justify-center text-red-600">
                    <i className="fa-solid text-2xl fa-trash"></i>
                </div>

                <h3 className="text-2xl font-semibold text-[#14315C]">
                    Eliminar estadio
                </h3>
            </div>

            <p className="text-xl text-gray-600 mb-6">
                ¿Estás seguro de que querés eliminar el estadio?
            </p>

            <div className="flex justify-end gap-3">
                <button onClick={onClose} disabled={isLoading} className="px-4 py-2 rounded-lg border text-xl border-gray-300 text-gray-700 hover:bg-gray-50 cursor-pointer">
                    Cancelar
                </button>

                <button onClick={handleDeleteEstadio} disabled={isLoading} className="px-4 py-2 rounded-lg text-xl bg-red-600 text-white hover:bg-red-700 cursor-pointer">
                    Eliminar
                </button>
            </div>
        </Modal>
    )
}