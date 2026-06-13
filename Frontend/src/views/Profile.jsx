import { useEffect, useState, useRef } from 'react'
import Footer from "../components/footer";
import NavBar from "../components/navBar";
import getPerfilMe from '../services/PerfilService/getPerfilMe'
import getPerfilPorMail from '../services/PerfilService/getPerfilPorMail'
import getTelefonoPorMail from '../services/TelefonoService/getTelefonoPorMail';
import ModalProfile from './ModalProfile'

export default function Profile() {
    const [role, setRole] = useState('Cargando...')
    const [email, setEmail] = useState('Cargando...')
    const [siglas, setSiglas] = useState('XX')
    const [datos, setDatos] = useState("")
    const [telefonos, setTelefonos] = useState([])
    const [telefonoSeleccionado, setTelefonoSeleccionado] = useState(null);
    const [open, setOpen] = useState(false)

    useEffect(() => {
        async function loadUser() {
            try {
                const data = await getPerfilMe()

                if (!data) return

                setEmail(data.mail)
                setSiglas(data.mail.substring(0, 2))
                setRole(data.role)
            } catch (error) {
                console.error(error)
            }
        }

        loadUser()
    }, [])

    useEffect(() => {
        async function loadUserProfile() {
            try {
                if (!email || email === "Cargando...") return;

                const perfil = await getPerfilPorMail(email);

                if (!perfil) return;

                setDatos(perfil);
            } catch (error) {
                console.error(error);
            }
        }

        loadUserProfile();
    }, [email]);

    useEffect(() => {
        async function loadPhoneNumbers() {
            try {
                if (!email || email === "Cargando...") return;

                const telefono = await getTelefonoPorMail(email);

                if (!telefono) return;

                setTelefonos(telefono);
            } catch (error) {
                console.error(error);
            }
        }

        loadPhoneNumbers();
    }, [email]);


    console.log(datos)

    return (
        <>
            <div className="flex flex-col min-h-screen bg-gray-50">
                <NavBar />

                <main className="flex-1 w-full max-w-7xl mx-auto px-4 sm:px-1 py-12">
                    <section className="relative overflow-hidden rounded-2xl bg-linear-to-br from-[#0B1F3A] via-[#14315C] to-[#1E4976] px-8 sm:px-14 py-12 shadow-lg">
                        <div className="pointer-events-none absolute inset-0 opacity-[0.07]" style={{ backgroundImage: "repeating-linear-gradient(135deg, #D4AF37 0px, #D4AF37 1px, transparent 1px, transparent 14px)", }} />
                        <div className="pointer-events-none absolute -right-12 -top-12 w-56 h-56 rounded-full border border-[#D4AF37]/20" />
                        <div className="pointer-events-none absolute -right-6 -top-6 w-40 h-40 rounded-full border border-[#D4AF37]/20" />

                        <div className="relative flex flex-col sm:flex-row items-center sm:items-start gap-8">
                            <div className="shrink-0">
                                <div className="w-32 h-32 sm:w-36 sm:h-36 rounded-full bg-[#0B1F3A] border-4 border-[#D4AF37] flex items-center justify-center shadow-[0_0_0_6px_rgba(212,175,55,0.15)]">
                                    <span className="font-serif text-5xl text-[#E8C66B] tracking-wide">
                                        {siglas.toUpperCase()}
                                    </span>
                                </div>
                            </div>

                            <div className="text-center sm:text-left">
                                <h1 className="font-serif text-4xl sm:text-5xl text-white tracking-tight">
                                    {email.split("@", 1)}
                                </h1>
                                <p className="mt-2 text-base sm:text-lg text-[#9FB8D9]">
                                    Identificación · {datos.numeroDocumento}
                                </p>
                                <div className={`mt-4 inline-flex items-center gap-2 rounded-full px-4 py-1.5 text-sm border ${role === "Cargando..." ? "bg-red-500/15 border-red-500/40 text-red-400" : "bg-[#D4AF37]/15 border-[#D4AF37]/40 text-[#E8C66B]"}`}>
                                    <i className={`w-4 h-4 text-center ${role === "Cargando..." ? "fa-solid fa-xmark" : "fa-solid fa-check" }`}></i>
                                    {role === "Cargando..." ? "No identificado" : `Rol de ${role}`}
                                </div>
                            </div>
                        </div>
                    </section>

                    <div className="mt-6 grid grid-cols-1 lg:grid-cols-2 gap-8">
                        <section className="rounded-2xl bg-white border border-[#E7E2D4] shadow-sm">
                            <div className="border-b border-[#E7E2D4] px-7 py-5 flex items-center gap-4">
                                <h2 className="font-serif text-2xl text-[#14315C] whitespace-nowrap">
                                    Datos personales
                                </h2>
                                <span className="h-px flex-1 bg-linear-to-r from-[#D4AF37]/60 to-transparent" />
                            </div>
                            <dl className="divide-y divide-[#F0EDE3]">
                                <div className="px-7 py-5 flex items-start gap-4">
                                    <span className="mt-0.5 text-[#C9A227]"><i className="fa-regular fa-envelope"></i></span>
                                    <div>
                                        <p className="text-sm uppercase tracking-wide text-[#8A93A6]">Correo Electrónico</p>
                                        <p className="text-base text-[#1F2A3C] mt-1">{email}</p>
                                    </div>
                                </div>
                                <div className="px-7 py-5 flex items-start gap-4">
                                    <span className="mt-0.5 text-[#C9A227]"><i className="fa-regular fa-address-card"></i></span>
                                    <div>
                                        <p className="text-sm uppercase tracking-wide text-[#8A93A6]">Documento</p>
                                        <p className="text-base uppercase text-[#1F2A3C] mt-1">{datos.tipoDocumento}. {datos.numeroDocumento}</p>
                                    </div>
                                </div>
                                <div className="px-7 py-5 flex items-start gap-4">
                                    <span className="mt-0.5 text-[#C9A227]"><i className="fa-solid fa-location-dot"></i></span>
                                    <div>
                                        <p className="text-sm uppercase tracking-wide text-[#8A93A6]">Dirección</p>
                                        <p className="text-base text-[#1F2A3C] mt-1">{datos.direccionCalle} {datos.direccionNumero}, {datos.direccionCodigoPostal} {datos.direccionLocalidad}, {datos.direccionPais}</p>
                                    </div>
                                </div>
                            </dl>
                        </section>

                        <section className="rounded-2xl bg-white border border-[#E7E2D4] shadow-sm">
                            <div className="border-b border-[#E7E2D4] px-7 py-5 flex items-center gap-4">
                                <h2 className="font-serif text-2xl text-[#14315C] whitespace-nowrap">
                                    Teléfonos de contacto
                                </h2>
                                <span className="h-px flex-1 bg-linear-to-r from-[#D4AF37]/60 to-transparent" />
                            </div>

                            {telefonos.length === 0 ? (
                                <p className="px-7 py-12 text-base text-[#8A93A6] text-center">
                                    No tenés teléfonos registrados.
                                </p>
                            ) : (
                                <ul className="divide-y divide-[#F0EDE3] overflow-y-auto h-67">
                                    {telefonos.map((tel) => (
                                        <li key={tel} className="px-7 py-5 flex items-center justify-between gap-4">
                                            <div className="flex items-center gap-4 min-w-0">
                                                <span className="flex items-center justify-center w-12 h-12 rounded-full bg-[#EEF3FA] text-[#1E4976] shrink-0">
                                                    <i className="fa-solid fa-phone"></i>
                                                </span>
                                                <div className="min-w-0">
                                                    <p className="text-base font-medium text-[#1F2A3C] truncate">
                                                        {tel}
                                                    </p>
                                                    <p className="text-sm text-[#8A93A6]">Movil</p>
                                                </div>
                                            </div>
                                            <button onClick={() => { setTelefonoSeleccionado(tel); setOpen(true); }} className="shrink-0 text-center cursor-pointer text-[#B7894A] hover:text-[#B3261E] hover:bg-[#FBE9E7] p-3 px-4 rounded-full transition focus:outline-none focus-visible:ring-2 focus-visible:ring-[#D4AF37]">
                                                <i className="fa-regular fa-trash-can"></i>
                                            </button>
                                        </li>
                                    ))}
                                </ul>
                            )}
                        </section>

                    </div>
                </main>
                <Footer />

            </div>
            <ModalProfile
                open={open}
                onClose={() => setOpen(false)}
                telefonoSeleccionado={telefonoSeleccionado}
                setTelefonoSeleccionado={setTelefonoSeleccionado}
                setTelefonos={setTelefonos}
            />
        </>
    );
}