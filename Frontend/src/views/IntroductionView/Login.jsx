import { useEffect, useState } from 'react'
import LoginService from '../../services/introductionService/getLogin.jsx'
import { Oval } from 'react-loader-spinner'

function App() {
    const [isLoading, setIsLoading] = useState(false)
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

            <div className="w-full h-[100vh] flex flex-col justify-center items-center">
                <img
                    src="./public/logo.png"
                    alt="Logo de la Universidad Católica de Uruguay"
                    className="w-50 h-auto"
                />
                <form
                    onSubmit={(e) => e.preventDefault()}
                    className="flex flex-col justify-center text-center items-center shadow-xl rounded-2xl w-full sm:w-[70%] md:w-[50%] lg:w-[30%] h-120 p-12 bg-white">
                    <h1 className="text-4xl text-blue-900">Inicio de sesión</h1>
                    <div className="w-full flex flex-col justify-center items-center mt-10">
                        <form
                            onSubmit={(e) => e.preventDefault()}
                            id="loginForm"
                            className="w-full flex flex-col justify-start items-start">
                            <label htmlFor="emailInput">Email</label>
                            <input
                                type="text"
                                id="emailInput"
                                className="w-full border-b mb-6 p-2 rounded-sm focus:border-blue-900 focus:outline-none bg-[rgb(232,240,254)]"
                                placeholder="ejemplo@correo.ucu.edu.uy"
                                disabled={isLoading}
                            />

                            <section className="relative w-full text-left">
                                <label htmlFor="passwordInput">Contraseña</label>
                                <input
                                    id="passwordInput"
                                    className="w-full border-b mb-6 p-2 rounded-sm focus:border-blue-900 focus:outline-none bg-[rgb(232,240,254)]"
                                    placeholder="contraseña"
                                    disabled={isLoading}
                                />
                            </section>

                            <section className="w-full flex justify-center items-center">
                                <button
                                    type="submit"
                                    className="w-40 h-auto bg-blue-900 rounded-full p-2 text-white cursor-pointer disabled:opacity-60 disabled:cursor-not-allowed"
                                    
                                    disabled={isLoading}>
                                    {isLoading ? 'Ingresando...' : 'Iniciar sesión'}
                                </button>
                            </section>

                            <div className="w-full flex justify-center items-center mt-5">
                                <span>
                                    ¿No tienes un usuario?{' '}
                                    <a href="/register" className="border-b hover:border-b-0">
                                        Registrarse
                                    </a>
                                </span>
                            </div>
                        </form>
                    </div>
                </form>
            </div>
        </>
    )
}

export default App