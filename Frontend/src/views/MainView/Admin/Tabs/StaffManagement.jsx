import { useEffect, useState } from 'react'
import getAllFuncionarios from '../../../../services/FuncionarioService/getAllFuncionarios'

import ModalCreateStaff from './Modals/Staff/ModalCreateStaff';
import ModalEliminateStaff from './Modals/Staff/ModalEliminateStaff';
import ModalUpdateStaff from './Modals/Staff/ModalUpdateStaff';
import ModalAsignarStaff from './Modals/Staff/ModalAsignarStaff';

export default function StaffManagement() {
    const [data, setData] = useState([]);
    const [open, setOpen] = useState(false);

    const [openEliminar, setOpenEliminar] = useState(false);

    const [openEditar, setOpenEditar] = useState(false);

    const [openAsignar, setOpenAsignar] = useState(false);

    const [identificador, setIdentificador] = useState(null);

    const loadFuncionarios = async () => {
        try {
            const data = await getAllFuncionarios()

            if (!data) return

            setData(data)
        } catch (error) {
            console.error(error)
        }
    }

    useEffect(() => {
        loadFuncionarios()
    }, [])

    return (
        <>
            <div className="text-xl">
                <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
                    <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none sm:pb-0 pb-4">
                        Gestión <span className="text-[#c8a84b]">Funcionarios</span>
                    </h1>
                    <button onClick={() => setOpen(true)} className="inline-flex items-center gap-1.5 px-2 py-2 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] border-none bg-[#c8a84b] text-[#0a1628] hover:bg-[#e0c472]">
                        <i className="fa-solid fa-plus" /> Crear funcionario
                    </button>
                </div>

                <div className="flex flex-col gap-2.5">
                    {data.length === 0 ? (
                        <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
                            No hay funcionarios creados.
                        </p>
                    ) : (
                        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-5">
                            {data.map((funcionario) => (
                                <div key={funcionario.mailPerfil} className="bg-white border border-[#d0dcea] rounded-2xl shadow-sm hover:shadow-lg transition-all duration-300 p-5">
                                    <div className="flex justify-between items-center mb-4">
                                        <div>
                                            <h3 className="text-xl font-bold text-[#0a1628]">
                                                {funcionario.mailPerfil.split("@", 1)}
                                            </h3>
                                        </div>

                                        <div className="w-14 h-14 rounded-full bg-[#c8a84b]/20 flex items-center justify-center">
                                            <button className="w-12 uppercase h-12 rounded-full bg-[#0a1628] border-2 border-[#c8a84b] flex items-center justify-center font-sans font-bold text-xl text-white cursor-pointer shrink-0 hover:scale-105 transition-transform focus:outline-none select-none">
                                                {funcionario.mailPerfil.substring(0, 2)}
                                            </button>
                                        </div>
                                    </div>

                                    <div className="space-y-3 text-[#5f6f86]">

                                        <p className="flex items-center gap-2">
                                            <i className="fa-solid fa-envelope text-[#c8a84b]" />
                                            Mail<span className='hidden lg:inline'>funcionario:</span>
                                            <span className="font-semibold text-[#0a1628] break-all">
                                                {funcionario.mailPerfil}
                                            </span>
                                        </p>

                                        <p className="flex items-center gap-2">

                                            <i className="fa-solid fa-hashtag text-[#c8a84b]" />
                                            Número legajo:
                                            <span className="font-bold text-[#0a1628]">
                                                {funcionario.numeroLegajo}
                                            </span>
                                        </p>

                                        <p className="flex items-center gap-2">

                                            <i className="fa-solid fa-mobile-screen-button text-[#c8a84b]" />
                                            Dispositivo:
                                            {funcionario.identificador ? (
                                                <>
                                                    <span className="font-bold text-[#0a1628]">
                                                        #{funcionario.identificador}
                                                    </span>
                                                </>
                                            ) : (
                                                <span className="font-bold text-[#0a1628]">
                                                    No Asignado
                                                </span>
                                            )}
                                        </p>

                                    </div>

                                        {funcionario.identificador ? (
                                            <div className="grid grid-cols-1 gap-2 mt-6">
                                                <button onClick={() => { setIdentificador(funcionario.mailPerfil); setOpenEditar(true); }} className="cursor-pointer bg-[#0a1628] text-[#c8a84b] py-2 rounded-lg font-medium hover:bg-[#13203a] transition">
                                                    <i className="fa-solid fa-pen mr-2" />
                                                    Editar
                                                </button>
                                            </div>
                                        ) : (
                                            <div className="grid grid-cols-1 lg:grid-cols-2 gap-2 mt-6">
                                                <button onClick={() => { setIdentificador(funcionario.mailPerfil); setOpenEditar(true); }} className="cursor-pointer bg-[#0a1628] text-[#c8a84b] py-2 rounded-lg font-medium hover:bg-[#13203a] transition">
                                                    <i className="fa-solid fa-pen mr-2" />
                                                    Editar
                                                </button>

                                                <button onClick={() => { setIdentificador(funcionario.mailPerfil); setOpenAsignar(true); }} className="cursor-pointer bg-[#c8a84b] text-[#0a1628] py-2 rounded-lg font-medium hover:bg-[#e0c472] transition">
                                                    <i className="fa-solid fa-link mr-2" />
                                                    Asignar Disp.
                                                </button>
                                            </div>
                                        )}

                                    <button onClick={() => { setIdentificador(funcionario.mailPerfil); setOpenEliminar(true); }} className="cursor-pointer mt-3 w-full bg-red-50 text-red-600 py-2 rounded-lg hover:bg-red-100 transition">
                                        <i className="fa-solid fa-trash-can mr-2" />
                                        Eliminar
                                    </button>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div >

            <ModalCreateStaff
                open={open}
                onClose={() => setOpen(false)}
                onCreateSuccess={loadFuncionarios}
            />

            <ModalEliminateStaff
                open={openEliminar}
                onClose={() => setOpenEliminar(false)}
                onDeleteSuccess={loadFuncionarios}
                identificador={identificador}
            />

            <ModalUpdateStaff
                open={openEditar}
                onClose={() => setOpenEditar(false)}
                onUpdateSuccess={loadFuncionarios}
                identificador={identificador}
            />

            <ModalAsignarStaff
                open={openAsignar}
                onClose={() => setOpenAsignar(false)}
                onAsignarSuccess={loadFuncionarios}
                mailFuncionario={identificador}
            />

        </>
    )
}