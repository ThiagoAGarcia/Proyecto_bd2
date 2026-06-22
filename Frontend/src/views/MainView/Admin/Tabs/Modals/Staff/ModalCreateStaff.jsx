import { useState, useEffect } from 'react';
import Modal from '../../../../../../components/modal';
import { toast } from 'react-toastify';
import { Oval } from 'react-loader-spinner';
import postPerfil from '../../../../../../services/PerfilService/postPerfil';
import postTelefono from '../../../../../../services/TelefonoService/postTelefono';
import postLoginFuncionario from '../../../../../../services/LoginService/postLoginFuncionario';
import postFuncionario from '../../../../../../services/FuncionarioService/postFuncionario';

const TIPOS_DOCUMENTO = ['CI', 'DNI', 'CPF', 'RUT', 'CC', 'CURP', 'SSN', 'SIN']

export default function ModalCreateStadium({ open, onClose, onCreateSuccess }) {
    const [isLoading, setIsLoading] = useState(false);
    const [telefonos, setTelefonos] = useState(['']);

    const [form, setForm] = useState({
        mail: '',
        paisDocumento: '',
        tipoDocumento: '',
        numeroDocumento: '',
        direccionPais: '',
        direccionLocalidad: '',
        direccionCalle: '',
        direccionNumero: '',
        direccionCodigoPostal: '',
        password: '',
        numeroLegajo: '',
    })

    useEffect(() => {
        if (!open) {
            setIsLoading(false);
            setForm({
                mail: '',
                paisDocumento: '',
                tipoDocumento: '',
                numeroDocumento: '',
                direccionPais: '',
                direccionLocalidad: '',
                direccionCalle: '',
                direccionNumero: '',
                direccionCodigoPostal: '',
                password: '',
                numeroLegajo: '',
            });
            setTelefonos(['']);
        }
    }, [open]);

    const handleChange = (e) => {
        const { name, value } = e.target
        setForm((prev) => ({ ...prev, [name]: value }))
    }

    const handleTelefonoChange = (index, value) => {
        setTelefonos((prev) => {
            const updated = [...prev]
            updated[index] = value
            return updated
        })
    }

    const agregarTelefono = () => {
        setTelefonos((prev) => [...prev, ''])
    }

    const eliminarTelefono = (index) => {
        setTelefonos((prev) => prev.filter((_, i) => i !== index))
    }

    const validar = () => {
        const err = {}

        if (!form.mail) {
            err.mail = 'Mail es requerido'
        } else if (form.mail.includes(' ')) {
            err.mail = 'El mail no puede contener espacios'
        } else if (!form.mail.includes('@') || !form.mail.includes('.')) {
            err.mail = 'El mail no es válido'
        }
        if (!form.password) {
            err.password = 'Contraseña es requerida'
        } else if (form.password.length < 8) {
            err.password = 'La contraseña debe tener al menos 8 caracteres'
        } else if (form.password.length > 64) {
            err.password = 'La contraseña no puede tener más de 64 caracteres'
        } else if (/^[A-Za-z]+$/.test(form.password)) {
            err.password = 'La contraseña debe contener al menos un número o símbolo'
        }
        if (!form.paisDocumento.trim()) {
            err.paisDocumento = 'El país del documento es requerido'
        } else if (form.paisDocumento.trim().length < 2) {
            err.paisDocumento = 'Debe ingresar un país válido'
        }
        if (!form.tipoDocumento) {
            err.tipoDocumento = 'Debe seleccionar un tipo de documento'
        }
        if (!form.numeroDocumento.trim()) {
            err.numeroDocumento = 'El número de documento es requerido'
        } else if (!/^[A-Za-z0-9]+$/.test(form.numeroDocumento.trim())) {
            err.numeroDocumento =
                'El documento solo puede contener letras y números'
        }
        if (!form.numeroLegajo.trim()) {
            err.numeroLegajo = 'El número de legajo es requerido'
        } else if (!/^\d+$/.test(form.numeroLegajo.trim())) {
            err.numeroLegajo = 'El legajo debe contener únicamente números'
        }
        if (!form.direccionPais.trim()) {
            err.direccionPais = 'El país es requerido'
        }
        if (!form.direccionLocalidad.trim()) {
            err.direccionLocalidad = 'La localidad es requerida'
        }
        if (!form.direccionCalle.trim()) {
            err.direccionCalle = 'La calle es requerida'
        }
        if (!form.direccionNumero.trim()) {
            err.direccionNumero = 'El número es requerido'
        } else if (!/^\d+$/.test(form.direccionNumero.trim())) {
            err.direccionNumero = 'Debe ser un número válido'
        } else if (parseInt(form.direccionNumero, 10) <= 0) {
            err.direccionNumero = 'Debe ser mayor a 0'
        }
        if (!form.direccionCodigoPostal.trim()) {
            err.direccionCodigoPostal = 'El código postal es requerido'
        } else if (!/^\d+$/.test(form.direccionCodigoPostal.trim())) {
            err.direccionCodigoPostal = 'Debe contener únicamente números'
        }
        telefonos.forEach((telefono, index) => {
            if (
                telefono.trim() !== '' &&
                !/^[0-9+\-\s()]+$/.test(telefono.trim())
            ) {
                err[`telefono${index}`] =
                    `El teléfono ${index + 1} contiene caracteres inválidos`
            }
        })

        return err
    }

    /***************************************************************************************** */

    const handleSubmit = async (e) => {
        e.preventDefault()
        const errores = validar()

        if (Object.keys(errores).length > 0) {
            Object.values(errores).forEach((mensaje) => toast.error(mensaje))
            return
        }

        const telefonosValidos = telefonos
            .filter((t) => t.trim() !== '')
            .map((t) => ({ mailPerfil: form.mail, telefono: t }))

        const BODY = {
            mail: form.mail,
            paisDocumento: form.paisDocumento,
            tipoDocumento: form.tipoDocumento,
            numeroDocumento: form.numeroDocumento,
            direccionPais: form.direccionPais,
            direccionCalle: form.direccionCalle,
            direccionLocalidad: form.direccionLocalidad,
            direccionNumero: form.direccionNumero ? parseInt(form.direccionNumero, 10) : null,
            direccionCodigoPostal: form.direccionCodigoPostal ? parseInt(form.direccionCodigoPostal, 10) : null,
        }
        const BODYFuncionario = {
            mailPerfil: form.mail,
            numeroLegajo: form.numeroLegajo,
        }

        const BODYLogin = {
            mailPerfil: form.mail,
            password: form.password,
        }

        const BODYTelefono = {
            telefonos: telefonosValidos,
        }

        try {
            setIsLoading(true)
            const registerPerfil = await postPerfil(BODY)
            if (registerPerfil?.success) {
                if (telefonosValidos.length > 0) {
                    const registerTelefono = await postTelefono({
                        telefonos: telefonosValidos,
                    });

                    if (!registerTelefono?.success) {
                        toast.error(registerTelefono?.description || 'Error al registrar teléfono');
                        return;
                    }
                }

                const registerFuncionario = await postFuncionario(BODYFuncionario);

                if (registerFuncionario?.success) {
                    const registerLogin = await postLoginFuncionario(BODYLogin);

                    if (registerLogin?.success) {
                        toast.success('Funcionario creado');
                        await onCreateSuccess();
                        onClose();
                    } else {
                        toast.error(registerLogin?.message || 'Error al registrar LOGIN');
                    }
                } else {
                    toast.error(registerFuncionario?.message || 'Error al registrar FUNCIONARIO');
                }

            } else {
                toast.error(registerPerfil?.message || 'Error al registrar', {
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

    /***************************************************************************************** */

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
                        Crear <span className="text-[#D4AF37]">Funcionario</span>
                    </h2>
                </div>
                <div className="grid gap-4 md:grid-cols-2">

                    <div className="md:col-span-2">
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">
                                Datos de cuenta
                            </span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Correo electrónico</label>
                        <input
                            type="email"
                            name="mail"
                            value={form.mail}
                            disabled={isLoading}
                            onChange={handleChange}
                            placeholder="ejemplo@mail.com"
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Contraseña</label>
                        <input
                            type="password"
                            name="password"
                            value={form.password}
                            disabled={isLoading}
                            onChange={handleChange}
                            placeholder="mínimo 8 caracteres"
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div className="md:col-span-2 mt-4">
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
                            value={form.paisDocumento}
                            disabled={isLoading}
                            placeholder="ej. Uruguay"
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Tipo de documento</label>
                        <select
                            name="tipoDocumento"
                            value={form.tipoDocumento}
                            disabled={isLoading}
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        >
                            <option value="">Seleccionar</option>
                            {TIPOS_DOCUMENTO.map((tipo) => (
                                <option key={tipo} value={tipo}>
                                    {tipo}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Número de documento</label>
                        <input
                            type="text"
                            name="numeroDocumento"
                            value={form.numeroDocumento}
                            disabled={isLoading}
                            placeholder="sin puntos ni guiones"
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Número de legajo</label>
                        <input
                            type="text"
                            name="numeroLegajo"
                            value={form.numeroLegajo}
                            disabled={isLoading}
                            placeholder="1001"
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
                            type="text"
                            name="direccionPais"
                            value={form.direccionPais}
                            disabled={isLoading}
                            placeholder="ej. Uruguay"
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Localidad</label>
                        <input
                            type="text"
                            name="direccionLocalidad"
                            value={form.direccionLocalidad}
                            disabled={isLoading}
                            placeholder="ej. Montevideo"
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div className="md:col-span-2">
                        <label className="text-sm text-gray-700">Calle</label>
                        <input
                            type="text"
                            name="direccionCalle"
                            value={form.direccionCalle}
                            disabled={isLoading}
                            placeholder="nombre de la calle"
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Número</label>
                        <input
                            type="number"
                            name="direccionNumero"
                            value={form.direccionNumero}
                            disabled={isLoading}
                            placeholder="1234"
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div>
                        <label className="text-sm text-gray-700">Código Postal</label>
                        <input
                            type="number"
                            name="direccionCodigoPostal"
                            value={form.direccionCodigoPostal}
                            disabled={isLoading}
                            placeholder="11300"
                            onChange={handleChange}
                            className="w-full rounded-xl border-b border-gray-300 px-4 py-3 focus:border-[#D4AF37] focus:outline-none"
                        />
                    </div>

                    <div className="md:col-span-2 mt-4">
                        <div className="flex items-center gap-3 mb-2">
                            <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">
                                Teléfonos (opcional)
                            </span>
                            <div className="flex-1 h-px bg-[#045694]/20" />
                        </div>
                    </div>

                    <div className="md:col-span-2 space-y-3">
                        <label className="text-sm text-gray-700">Telefonos ingresados</label>
                        {telefonos.map((tel, i) => (
                            <div key={i} className="flex items-center gap-2">
                                <input
                                    type="tel"
                                    value={tel}
                                    onChange={(e) => handleTelefonoChange(i, e.target.value)}
                                    disabled={isLoading}
                                    placeholder={`ej. +598 99 123 456`}
                                    className="w-full py-4 rounded-xl border-b border-gray-300 px-4 focus:border-[#D4AF37] focus:outline-none"
                                />
                                {telefonos.length > 1 && (
                                    <button type="button" onClick={() => eliminarTelefono(i)} disabled={isLoading} className="text-red-400 hover:text-red-600 transition-colors p-1 cursor-pointer" title="Eliminar teléfono">
                                        <i className="fa-solid fa-trash-can text-xl" />
                                    </button>
                                )}
                            </div>
                        ))}
                    </div>
                    <div className="md:col-span-2">
                        <button type="button" onClick={agregarTelefono} disabled={isLoading} className="flex items-center gap-2 text-sm font-medium text-cyan-700 hover:text-cyan-900 cursor-pointer">
                            <i className="fa-solid fa-plus" />
                            Agregar teléfono
                        </button>
                    </div>

                </div>

                <div className="flex justify-end gap-4">
                    <button type="button" onClick={onClose} className="lg:hidden inline rounded-xl transition-all cursor-pointer px-6 py-3 font-semibold border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                        Cancelar
                    </button>

                    <button type="submit" className="cursor-pointer transition-all rounded-xl bg-[#14315C] px-6 py-3 font-semibold text-white hover:bg-[#1c4378]">
                        Crear funcionario
                    </button>
                </div>
            </form>
        </Modal>
    )
}