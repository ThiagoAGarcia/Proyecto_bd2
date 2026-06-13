import Modal from '../components/modal';
import deleteTelefono from '../services/TelefonoService/deleteTelefonos';
import { toast } from 'react-toastify';

export default function ProfileModal({ open, onClose, telefonoSeleccionado, setTelefonoSeleccionado, setTelefonos }) {

    async function handleDeleteTelefono() {
        try {
            await deleteTelefono(telefonoSeleccionado);

            setTelefonos((prev) =>
                prev.filter((tel) => tel !== telefonoSeleccionado)
            );

            toast.success("Teléfono eliminado correctamente");

            onClose()
            setTelefonoSeleccionado(null);
        } catch (error) {
            console.error(error);
            toast.error("No se pudo eliminar el teléfono");
        }
    }
    return (
        <Modal open={open} onClose={onClose}>
            <div className="flex items-center text-xl gap-3 mb-4">
                <div className="w-15 h-15 rounded-full bg-red-100 flex items-center justify-center text-red-600">
                    <i className="fa-solid text-2xl fa-trash"></i>
                </div>

                <h3 className="text-2xl font-semibold text-[#14315C]">
                    Eliminar teléfono
                </h3>
            </div>

            <p className="text-xl text-gray-600 mb-6">
                ¿Estás seguro de que querés eliminar el teléfono
                <span className="font-semibold"> {telefonoSeleccionado}</span>?
            </p>

            <div className="flex justify-end gap-3">
                <button onClick={() => { onClose() }} className="px-4 py-2 rounded-lg border text-xl border-gray-300 text-gray-700 hover:bg-gray-50 cursor-pointer">
                    Cancelar
                </button>

                <button onClick={handleDeleteTelefono} className="px-4 py-2 rounded-lg text-xl bg-red-600 text-white hover:bg-red-700 cursor-pointer">
                    Eliminar
                </button>
            </div>
        </Modal>
    )
}