import Modal from '../../../../../../components/modal';
import deleteEstadio from '../../../../../../services/EstadioService/deleteEstadio';
import { toast } from 'react-toastify';

export default function ProfileModal({ open, onClose, onCreateSuccess, identificador }) {

    async function handleDeleteEstadio() {
        try {
            await deleteEstadio(identificador);

            toast.success("Estadio eliminado correctamente");

            await onCreateSuccess();

            onClose()
        } catch (error) {
            console.error(error);
            toast.error("No se pudo eliminar el estadio");
        }
    }
    return (
        <Modal open={open} onClose={onClose}>
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
                <button onClick={() => { onClose() }} className="px-4 py-2 rounded-lg border text-xl border-gray-300 text-gray-700 hover:bg-gray-50 cursor-pointer">
                    Cancelar
                </button>

                <button onClick={handleDeleteEstadio} className="px-4 py-2 rounded-lg text-xl bg-red-600 text-white hover:bg-red-700 cursor-pointer">
                    Eliminar
                </button>
            </div>
        </Modal>
    )
}