import { useState, useEffect } from 'react';
import Modal from './../../../../../components/modal';
import { toast } from 'react-toastify';
import getAllUsuarios from '../../../../../services/UsuarioService/getAllUsuarios'
import postTransferencia from '../../../../../services/TransferenciaService/postTransferencia'
import { Oval } from 'react-loader-spinner'

export default function ModalTransfer({ open, onClose, identificador, onTransferSuccess }) {
    const [usuarios, setUsuarios] = useState([])
    const [selectedUser, setSelectedUser] = useState(null)
    const [search, setSearch] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        if (!open) return;

        async function loadUsuarios() {
            try {
                setUsuarios([]);
                setSelectedUser(null)

                const data = await getAllUsuarios();

                setUsuarios(data || [])
            } catch (error) {
                console.error(error);
                setUsuarios([]);
                setSelectedUser(null)
            }
        }

        loadUsuarios();
    }, [open]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (isLoading) return

        if (!selectedUser) {
            toast.error('Debe seleccionar un usuario');
            return;
        }

        setIsLoading(true)

        const BODY = {
            identificadorEntrada: identificador,
            mailUsuarioDestino: selectedUser.mailPerfil
        };

        try {

            await postTransferencia(BODY);

            toast.success('Transferencia realizada con éxito');

            await onTransferSuccess();

            onClose();
        }
        catch (error) {
            toast.error(error.message, {
                position: 'bottom-left',
                autoClose: 3000,
            });
        } finally {
            setIsLoading(false)
        }
    };

    const usuariosFiltrados = usuarios.filter((usuario) => usuario.mailPerfil?.toLowerCase().includes(search.trim().toLowerCase())
    );

    useEffect(() => {
        if (!open) {
            setSearch('');
            setUsuarios([]);
            setSelectedUser(null);
            setIsLoading(false)
        }
    }, [open]);

    return (
        <Modal open={open} onClose={onClose}>
            {isLoading && (
                <div className="absolute inset-0 z-500 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-2xl">
                    <Oval
                        height={35}
                        width={35}
                        color="#052e66"
                        secondaryColor="#e5e7eb"
                        strokeWidth={4}
                        strokeWidthSecondary={4}
                        ariaLabel="loading-create-group"
                    />
                </div>
            )}
            <form onSubmit={handleSubmit} className="w-full flex flex-col max-w-7xl">
                <div className="mb-6">
                    <h2 className="text-3xl font-bold text-[#14315C]">
                        Transferir <span className="text-[#c8a84b]">entrada</span>
                    </h2>

                    <p className="mt-2 text-[#14315C]/70">
                        Seleccioná el usuario al que deseas transferir la entrada.
                    </p>
                </div>

                <div className="mb-6">
                    <input type="text" placeholder="Buscar usuario..." value={search} disabled={isLoading} onChange={(e) => { setSearch(e.target.value); setSelectedUser(null); }} className="w-full rounded-xl border-2 border-[#14315C]/15 px-4 py-3 focus:border-[#D4AF37] focus:outline-none" />
                </div>

                <div className="max-h-[450px] overflow-y-auto rounded-2xl border border-[#14315C]/10">
                    {usuariosFiltrados.length === 0 ? (
                        <div className="p-8 text-center">
                            No se encontraron usuarios registrados.
                        </div>
                    ) : (
                        usuariosFiltrados.map((usuario) => (
                            <button
                                key={usuario.mailPerfil}
                                type="button"

                                onClick={() => !isLoading && setSelectedUser(usuario)}
                                className={`w-full cursor-pointer border-b border-[#14315C]/10 p-4 text-left transition 
                                ${selectedUser?.mailPerfil === usuario.mailPerfil
                                        ? 'bg-[#14315C] text-white'
                                        : ''
                                    }`}
                            >
                                <div className="flex items-center gap-4">
                                    <div
                                        className={`flex h-12 w-12 items-center justify-center rounded-full font-bold
                                        ${selectedUser?.mailPerfil ===
                                                usuario.mailPerfil
                                                ? 'bg-white text-[#14315C]'
                                                : 'bg-[#14315C] text-white'
                                            }`}
                                    >
                                        {usuario.mailPerfil?.charAt(0)?.toUpperCase()}{usuario.mailPerfil?.charAt(1)?.toUpperCase()}
                                    </div>

                                    <div className="flex-1">
                                        <p className="font-semibold">
                                            {usuario.mailPerfil}
                                        </p>

                                        <p className={`text-sm ${selectedUser?.mailPerfil === usuario.mailPerfil
                                            ? 'text-white/80'
                                            : 'text-[#14315C]/60'
                                            }`}
                                        >
                                            {usuario.mailPerfil.split("@", 1)}
                                        </p>
                                    </div>

                                    {selectedUser?.mailPerfil ===
                                        usuario.mailPerfil && (
                                            <i className="fa-solid fa-circle-check text-2xl" />
                                        )}
                                </div>
                            </button>
                        ))
                    )}
                </div>

                {selectedUser && (
                    <div className="mt-5 rounded-xl border bg-[#0a1628] p-4">
                        <p className="font-semibold text-[#c8a84b]/60 uppercase">
                            Usuario seleccionado
                        </p>

                        <p className="text-[#c8a84b]">
                            {selectedUser.mailPerfil}
                        </p>
                    </div>
                )}

                <div className="mt-8 flex md:flex-row flex-col gap-4">
                    <button type="button" disabled={isLoading} onClick={onClose} className="lg:hidden inline flex-1 cursor-pointer rounded-xl bg-red-600 px-6 py-4 font-semibold text-white hover:bg-red-700">
                        Cancelar
                    </button>

                    <button type="submit" disabled={!selectedUser && isLoading} className="flex-1 cursor-pointer rounded-xl bg-[#14315C] px-6 py-4 font-semibold text-white disabled:cursor-not-allowed disabled:opacity-50">
                        Transferir entrada
                    </button>
                </div>
            </form>
        </Modal>
    )
}