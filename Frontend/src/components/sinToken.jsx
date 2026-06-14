import React, { useEffect } from 'react'
import { RiArrowGoBackFill } from 'react-icons/ri'
import logo from '../assets/FifaUCULogo.png'

const SinToken = () => {
    return (
        <div className="min-h-screen w-full bg-linear-to-br from-[#0a1628] via-[#003A70] to-[#0a1628] flex items-center justify-center px-4">
            <div className="relative max-w-md w-full rounded-3xl border border-white/30 bg-white/95 shadow-2xl overflow-hidden">
                <div className="h-2 w-full bg-[#FDB913]" />

                <div className="flex items-center gap-3 px-8 pt-6 pb-2">
                    <div className="flex h-10 w-10 items-center justify-center text-xs font-semibold tracking-[0.08em]">
                        <img
                            src={logo}
                            alt="ucu"
                            className="h-10 w-auto"
                        />
                    </div>
                    <div>
                        <p className="text-xs font-semibold text-[#003A70] uppercase tracking-[0.18em]">
                            UCUMundial
                        </p>
                        <p className="text-[10px] text-[#003A70]/70 tracking-[0.18em] uppercase">
                            2026
                        </p>
                    </div>
                </div>

                <div className="px-8 pb-8 pt-4 text-center text-slate-800">
                    <div className="mx-auto mb-4 mt-2 flex h-14 w-14 items-center justify-center rounded-2xl bg-[#003A70]/5 border border-[#003A70]/15 shadow-md">
                        <svg
                            className="h-7 w-7 text-[#003A70]"
                            viewBox="0 0 24 24"
                            fill="none"
                            stroke="currentColor"
                            strokeWidth="1.8"
                            strokeLinecap="round"
                            strokeLinejoin="round">
                            <rect x="3" y="11" width="18" height="10" rx="2" ry="2" />
                            <path d="M7 11V8a5 5 0 0 1 10 0v3" />
                            <circle cx="12" cy="16" r="1.4" />
                        </svg>
                    </div>

                    <h1 className="mb-2 text-2xl font-semibold tracking-tight text-[#003A70]">
                        Acceso restringido
                    </h1>

                    <p className="mb-1 text-sm text-slate-700">
                        No encontramos un token válido para esta página.
                    </p>
                    <p className="mb-7 text-sm text-slate-600">
                        Es posible que tu sesión haya expirado o que todavía no hayas
                        iniciado sesión en el sistema.
                    </p>

                    <a
                        href="/"
                        className="inline-flex items-center justify-center rounded-xl bg-[#003A70] px-5 py-2.5 text-sm font-semibold text-white shadow-lg shadow-[#003A70]/30 transition hover:bg-[#00509E] focus:outline-none focus:ring-2 focus:ring-[#FDB913] focus:ring-offset-2 focus:ring-offset-white gap-1">
                        Volver al inicio
                        <span className="ml-1 text-base">
                            <RiArrowGoBackFill size={15} />
                        </span>
                    </a>

                    <p className="mt-5 text-xs text-slate-500">
                        Si el problema persiste, intentá volver a iniciar sesión o ponete en
                        contacto con la biblioteca.
                    </p>
                </div>
            </div>
        </div>
    )
}

export default SinToken