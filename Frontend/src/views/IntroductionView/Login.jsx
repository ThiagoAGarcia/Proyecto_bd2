import { IoEye, IoEyeOff } from 'react-icons/io5'
import { useEffect, useState } from 'react'
import LoginService from '../../services/introductionService/postLogin.jsx'
import { useNavigate } from 'react-router-dom'
import { toast } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css'
import { Oval } from 'react-loader-spinner'
import logo from './../../assets/FifaUCULogo.png';

function Login() {
    const [verPwd, setVerPwd] = useState(true)
    const [isLoading, setIsLoading] = useState(false)
    const navigate = useNavigate()

    useEffect(() => {
        localStorage.removeItem('token')
        localStorage.removeItem('role')
        localStorage.removeItem('ci')
        localStorage.removeItem('roles')
    }, [])

    const commitLogin = async () => {

        console.log("commitLogin ejecutado");

        if (isLoading) return

        const mailPerfil = document.getElementById('emailInput').value.trim()
        const password = document.getElementById('passwordInput').value.trim()
        console.log("commitLogin ejecutado");
        if (!mailPerfil || !password) {
            toast.error('Debes completar todos los campos', {
                position: 'bottom-left',
                autoClose: 3000,
            })
            return
        }

        const regexEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/

        if (!regexEmail.test(mailPerfil)) {
            toast.error(
                'Ingrese un correo electrónico válido',
                {
                    position: 'bottom-left',
                    autoClose: 3000,
                }
            )
            return
        }
        
        try {
            setIsLoading(true)

            const BODY = { mailPerfil, password }
            const logged = await LoginService(BODY)
            console.log(logged);
            console.log(mailPerfil)
            console.log(password)

            if (logged?.message) {
                localStorage.setItem('token', logged.access_token)
                localStorage.setItem('role', JSON.stringify(logged.role))
                localStorage.setItem('roles', JSON.stringify(logged.roles))
                localStorage.setItem('ci', JSON.stringify(logged.ci))

                if (logged.role.includes('Administrador')) {
                    navigate('/main-admin')
                    return
                }

                if (logged.role.includes('Funcionario')) {
                    navigate('/main-librarian')
                    return
                }

                if (logged.role.includes('Usuario')) {
                    navigate('/main')
                    return
                }
            } else {
                toast.error(logged?.description || 'Correo o contraseña incorrectos', {
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
            {isLoading && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm">
                    <div className="flex flex-col items-center">
                        <Oval
                            height={35}
                            width={35}
                            color="#1d4ed8"
                            visible={true}
                            ariaLabel="loading-login"
                            secondaryColor="#e5e7eb"
                            strokeWidth={4}
                            strokeWidthSecondary={4}
                        />
                    </div>
                </div>
            )}

            <div className="w-full h-screen bg-[#045694] flex flex-row justify-center items-center">
                <img
                    src={logo}
                    alt="FifaUcu"
                    className="w-70 pr-5 h-auto"
                />
                <form
                    onSubmit={(e) => {
                        e.preventDefault();
                        commitLogin();
                    }}
                    className="flex flex-col justify-center text-center items-center shadow-xl rounded-2xl w-full sm:w-[70%] md:w-[50%] lg:w-[30%] h-120 p-12 bg-white"
                >
                    <h1 className="text-4xl text-black font-semibold">
                        Iniciar sesión
                    </h1>

                    <div className="w-full flex flex-col justify-start items-start mt-10">
                        <label htmlFor="emailInput">Correo electrónico</label>

                        <input
                            type="text"
                            id="emailInput"
                            className="w-full border-b border-gray-400 mb-6 p-2 rounded-sm focus:outline-none"
                            placeholder="ejemplo@mail.com"
                            disabled={isLoading}
                        />

                        <section className="relative w-full text-left">
                            <label htmlFor="passwordInput">Contraseña</label>
                            <i
                                className="absolute top-9 right-5 cursor-pointer text-gray-500"
                                onClick={() => !isLoading && setVerPwd(!verPwd)}>
                                {verPwd ? <IoEyeOff size={20} /> : <IoEye size={20} />}
                            </i>
                            <input
                                type={verPwd ? 'password' : 'text'}
                                id="passwordInput"
                                className="w-full border-b border-gray-400 mb-6 p-2 rounded-sm focus:outline-none"
                                placeholder="ej. 12345678"
                                disabled={isLoading}
                            />
                        </section>

                        <section className="w-full flex justify-center items-center">
                            <button
                                type="submit"
                                className="w-full font-semibold h-auto bg-cyan-700 hover:bg-cyan-800 transition-colors rounded-full p-2 text-white cursor-pointer disabled:opacity-60 disabled:cursor-not-allowed"
                                disabled={isLoading}>
                                {isLoading ? 'Ingresando...' : 'INICIAR SESIÓN'}
                            </button>
                        </section>

                        <div className="w-full flex justify-center items-center mt-5">
                            <span>
                                ¿No dispones de una cuenta?{' '}
                                <a href="/register" className="border-b font-bold hover:border-b-0">
                                    REGISTRARSE
                                </a>
                            </span>
                        </div>
                    </div>
                </form>
            </div>
        </>
    )
}

export default Login