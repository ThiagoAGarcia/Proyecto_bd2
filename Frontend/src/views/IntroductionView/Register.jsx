import { IoEye, IoEyeOff, IoAddCircleOutline, IoTrashOutline } from 'react-icons/io5'
import { useEffect, useState } from 'react'
import postRegister from '../../services/introductionService/postRegister.jsx'
import { ToastContainer, toast } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css'
import { Oval } from 'react-loader-spinner'
import { useNavigate } from 'react-router-dom'
import logo from './../../assets/FifaUCULogo.png'

const TIPOS_DOCUMENTO = ['Cédula', 'Pasaporte', 'DNI', 'Otro']

function Register() {
  useEffect(() => {
    localStorage.removeItem('token')
    localStorage.removeItem('role')
    localStorage.removeItem('ci')
    localStorage.removeItem('roles')
  }, [])

  const navigate = useNavigate()
  const [verPwd, setVerPwd] = useState(false)
  const [isLoading, setIsLoading] = useState(false)
  const [errores, setErrores] = useState({})

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
    confirmPassword: '',
  })

  const [telefonos, setTelefonos] = useState([''])

  const handleChange = (e) => {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
    setErrores((prev) => ({ ...prev, [name]: undefined }))
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
    const regexEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/

    if (!regexEmail.test(form.mail)) err.mail = 'Ingresá un correo electrónico válido.'
    if (!form.tipoDocumento) err.tipoDocumento = 'Seleccioná un tipo de documento.'
    if (!form.paisDocumento.trim()) err.paisDocumento = 'Ingresá el país del documento.'
    if (!form.numeroDocumento.trim()) err.numeroDocumento = 'Ingresá el número de documento.'
    if (!form.direccionPais.trim()) err.direccionPais = 'Ingresá el país de residencia.'
    if (!form.direccionLocalidad.trim()) err.direccionLocalidad = 'Ingresá la localidad.'
    if (!form.direccionCalle.trim()) err.direccionCalle = 'Ingresá la calle.'
    if (!form.direccionNumero.trim()) err.direccionNumero = 'Ingresá el número de puerta.'
    if (!form.direccionCodigoPostal.trim()) err.direccionCodigoPostal = 'Ingresá el código postal.'
    if (form.password.length < 8) err.password = 'La contraseña debe tener al menos 8 caracteres.'
    if (form.confirmPassword !== form.password) err.confirmPassword = 'Las contraseñas no coinciden.'

    const telefonosValidos = telefonos.filter((t) => t.trim() !== '')
    if (telefonosValidos.length === 0) err.telefonos = 'Agregá al menos un teléfono.'

    return err
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    const err = validar()
    setErrores(err)
    if (Object.keys(err).length > 0) return

    const telefonosValidos = telefonos
      .filter((t) => t.trim() !== '')
      .map((t) => ({ mailPerfil: form.mail, telefono: t }))

    const BODY = {
      mail: form.mail,
      paisDocumento: form.paisDocumento,
      tipoDocumento: form.tipoDocumento,
      numeroDocumento: form.numeroDocumento,
      direccionPais: form.direccionPais,
      direccionLocalidad: form.direccionLocalidad,
      direccionCalle: form.direccionCalle,
      direccionNumero: form.direccionNumero,
      direccionCodigoPostal: form.direccionCodigoPostal,
      password: form.password,
      confirmPassword: form.confirmPassword,
      telefonos: telefonosValidos,
    }

    try {
      setIsLoading(true)
      const register = await postRegister(BODY)
      if (register?.success) {
        navigate('/')
      } else {
        toast.error(register?.description || 'Error al registrar', {
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
    <>
      <ToastContainer />

      {isLoading && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm">
          <Oval
            height={35}
            width={35}
            color="#0e7490"
            visible={true}
            ariaLabel="loading-register"
            secondaryColor="#e5e7eb"
            strokeWidth={4}
            strokeWidthSecondary={4}
          />
        </div>
      )}

      <div className="min-h-screen w-full bg-[#045694] flex flex-col items-center justify-center py-10 px-4">
        <img
          src={logo}
          alt="FifaUcu"
          className="w-40 h-auto mb-8"
        />

        <div className="w-full sm:w-[85%] md:w-[70%] lg:w-[55%] xl:w-[45%] bg-white rounded-2xl shadow-xl p-10">
          <h1 className="text-3xl font-semibold text-black mb-8 text-center">Crear cuenta</h1>

          <form onSubmit={handleSubmit} className="flex flex-col gap-1">

            <SectionLabel>Datos de cuenta</SectionLabel>

            <Field label="Correo electrónico" error={errores.mail}>
              <input
                name="mail"
                type="text"
                value={form.mail}
                onChange={handleChange}
                disabled={isLoading}
                placeholder="ejemplo@mail.com"
                className={inputCls(errores.mail)}
              />
            </Field>

            <div className="flex gap-4">
              <Field label="Contraseña" error={errores.password} className="flex-1">
                <div className="relative">
                  <input
                    name="password"
                    type={verPwd ? 'text' : 'password'}
                    value={form.password}
                    onChange={handleChange}
                    disabled={isLoading}
                    placeholder="mínimo 8 caracteres"
                    className={inputCls(errores.password) + ' pr-10'}
                  />
                  <i
                    className="absolute top-1/2 -translate-y-1/2 right-3 cursor-pointer text-gray-500"
                    onClick={() => !isLoading && setVerPwd(!verPwd)}>
                    {verPwd ? <IoEyeOff size={18} /> : <IoEye size={18} />}
                  </i>
                </div>
              </Field>

              <Field label="Confirmar contraseña" error={errores.confirmPassword} className="flex-1">
                <div className="relative">
                  <input
                    name="confirmPassword"
                    type={verPwd ? 'text' : 'password'}
                    value={form.confirmPassword}
                    onChange={handleChange}
                    disabled={isLoading}
                    placeholder="repetir contraseña"
                    className={inputCls(errores.confirmPassword) + ' pr-10'}
                  />
                  <i
                    className="absolute top-1/2 -translate-y-1/2 right-3 cursor-pointer text-gray-500"
                    onClick={() => !isLoading && setVerPwd(!verPwd)}>
                    {verPwd ? <IoEyeOff size={18} /> : <IoEye size={18} />}
                  </i>
                </div>
              </Field>
            </div>

            <SectionLabel>Documento de identidad</SectionLabel>

            <div className="flex gap-4">
              <Field label="Tipo de documento" error={errores.tipoDocumento} className="flex-1">
                <div className="relative">
                  <select
                    name="tipoDocumento"
                    value={form.tipoDocumento}
                    onChange={handleChange}
                    disabled={isLoading}
                    className={selectCls(errores.tipoDocumento)}>
                    <option value="">Seleccioná</option>
                    {TIPOS_DOCUMENTO.map((t) => (
                      <option key={t} value={t}>{t}</option>
                    ))}
                  </select>
                  <span className="absolute top-1/2 -translate-y-1/2 right-3 pointer-events-none text-gray-500 text-xs">▼</span>
                </div>
              </Field>

              <Field label="País del documento" error={errores.paisDocumento} className="flex-1">
                <input
                  name="paisDocumento"
                  type="text"
                  value={form.paisDocumento}
                  onChange={handleChange}
                  disabled={isLoading}
                  placeholder="ej. Uruguay"
                  className={inputCls(errores.paisDocumento)}
                />
              </Field>
            </div>

            <Field label="Número de documento" error={errores.numeroDocumento}>
              <input
                name="numeroDocumento"
                type="text"
                value={form.numeroDocumento}
                onChange={handleChange}
                disabled={isLoading}
                placeholder="sin puntos ni guiones"
                className={inputCls(errores.numeroDocumento)}
              />
            </Field>

            <SectionLabel>Dirección</SectionLabel>

            <div className="flex gap-4">
              <Field label="País" error={errores.direccionPais} className="flex-1">
                <input
                  name="direccionPais"
                  type="text"
                  value={form.direccionPais}
                  onChange={handleChange}
                  disabled={isLoading}
                  placeholder="ej. Uruguay"
                  className={inputCls(errores.direccionPais)}
                />
              </Field>
              <Field label="Localidad" error={errores.direccionLocalidad} className="flex-1">
                <input
                  name="direccionLocalidad"
                  type="text"
                  value={form.direccionLocalidad}
                  onChange={handleChange}
                  disabled={isLoading}
                  placeholder="ej. Montevideo"
                  className={inputCls(errores.direccionLocalidad)}
                />
              </Field>
            </div>

            <div className="flex gap-4">
              <Field label="Calle" error={errores.direccionCalle} className="flex-1">
                <input
                  name="direccionCalle"
                  type="text"
                  value={form.direccionCalle}
                  onChange={handleChange}
                  disabled={isLoading}
                  placeholder="nombre de la calle"
                  className={inputCls(errores.direccionCalle)}
                />
              </Field>
              <Field label="Número" error={errores.direccionNumero} className="w-28">
                <input
                  name="direccionNumero"
                  type="text"
                  value={form.direccionNumero}
                  onChange={handleChange}
                  disabled={isLoading}
                  placeholder="1234"
                  className={inputCls(errores.direccionNumero)}
                />
              </Field>
              <Field label="Código postal" error={errores.direccionCodigoPostal} className="w-36">
                <input
                  name="direccionCodigoPostal"
                  type="text"
                  value={form.direccionCodigoPostal}
                  onChange={handleChange}
                  disabled={isLoading}
                  placeholder="11300"
                  className={inputCls(errores.direccionCodigoPostal)}
                />
              </Field>
            </div>

            <SectionLabel>Teléfonos</SectionLabel>

            <div className="flex flex-col gap-2 mb-1">
              {telefonos.map((tel, i) => (
                <div key={i} className="flex items-center gap-2">
                  <input
                    type="tel"
                    value={tel}
                    onChange={(e) => handleTelefonoChange(i, e.target.value)}
                    disabled={isLoading}
                    placeholder={`ej. +598 99 123 456`}
                    className={inputCls(i === 0 && errores.telefonos) + ' flex-1'}
                  />
                  {telefonos.length > 1 && (
                    <button
                      type="button"
                      onClick={() => eliminarTelefono(i)}
                      disabled={isLoading}
                      className="text-red-400 hover:text-red-600 transition-colors p-1 cursor-pointer"
                      title="Eliminar teléfono">
                      <IoTrashOutline size={20} />
                    </button>
                  )}
                </div>
              ))}
            </div>

            {errores.telefonos && (
              <p className="text-red-500 text-xs mb-1">{errores.telefonos}</p>
            )}

            <button
              type="button"
              onClick={agregarTelefono}
              disabled={isLoading}
              className="flex items-center gap-1.5 text-sm text-cyan-700 font-medium hover:text-cyan-900 transition-colors w-fit mb-4">
              <IoAddCircleOutline size={18} />
              Agregar teléfono
            </button>

            {/* ── SUBMIT ── */}
            <button
              type="submit"
              disabled={isLoading}
              className="w-full font-semibold bg-cyan-700 hover:bg-cyan-800 transition-colors rounded-full py-2.5 text-white cursor-pointer disabled:opacity-60 disabled:cursor-not-allowed mt-2">
              {isLoading ? 'Registrando...' : 'CREAR CUENTA'}
            </button>

            <div className="w-full flex justify-center items-center mt-4">
              <span className="text-sm">
                ¿Ya tenés una cuenta?{' '}
                <a href="/" className="font-bold border-b border-black hover:border-transparent transition-all">
                  INICIAR SESIÓN
                </a>
              </span>
            </div>
          </form>
        </div>
      </div>
    </>
  )
}

/* ── Helpers de UI ── */

function SectionLabel({ children }) {
  return (
    <div className="flex items-center gap-3 mt-5 mb-2">
      <span className="text-xs font-semibold uppercase tracking-widest text-[#045694]">{children}</span>
      <div className="flex-1 h-px bg-[#045694]/20" />
    </div>
  )
}

function Field({ label, error, children, className = '' }) {
  return (
    <div className={`flex flex-col mb-1 ${className}`}>
      <label className="text-sm text-gray-700 mb-1">{label}</label>
      {children}
      {error && <p className="text-red-500 text-xs mt-0.5">{error}</p>}
    </div>
  )
}

function inputCls(hasError) {
  return `w-full border-b ${hasError ? 'border-red-400' : 'border-gray-400'} p-2 rounded-sm focus:outline-none focus:border-cyan-700 bg-transparent transition-colors`
}

function selectCls(hasError) {
  return `appearance-none w-full border-b ${hasError ? 'border-red-400' : 'border-gray-400'} p-2 pr-8 rounded-sm focus:outline-none focus:border-cyan-700 bg-transparent transition-colors`
}

export default Register
