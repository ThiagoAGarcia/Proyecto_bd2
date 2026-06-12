import { useEffect, useState, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import logo from './../assets/FifaUCULogo.png'

export default function NavBar() {
  const [isOpen, setIsOpen] = useState(false)
  const menuRef = useRef(null)
  const navigate = useNavigate()

  const toggleMenu = () => setIsOpen(!isOpen)

  useEffect(() => {
    function handleClickOutside(event) {
      if (menuRef.current && !menuRef.current.contains(event.target)) {
        setIsOpen(false)
      }
    }

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside)
    }
    
    return () => {
      document.removeEventListener('mousedown', handleClickOutside)
    }
  }, [isOpen])

  return (
    <nav className="bg-[#0a1628] px-6 h-23 flex items-center justify-between relative z-50">
      <div className="absolute bottom-0 left-0 right-0 h-[3px] bg-linear-to-r from-[#c8a84b] via-[#006BB6] to-[#c8a84b] animate-stripe"></div>
      
      <a className="flex items-center gap-3 no-underline" href="#">
        <img src={logo} alt="FifaUcu" className="w-10 h-auto" />
      </a>
      
      <div className="flex items-center gap-4">
        <div className="text-right hidden sm:block">
          <div className="text-xl font-medium text-white leading-tight">santiagoaguerre.info</div>
          <div className="text-[13px] text-[#c8a84b] tracking-wider uppercase">Usuario</div>
        </div>
        
        <div className="relative" ref={menuRef}>
          <button onClick={toggleMenu} className="w-12 h-12 rounded-full bg-[#006BB6] border-2 border-[#c8a84b] flex items-center justify-center font-sans font-bold text-xl text-white cursor-pointer shrink-0 hover:scale-105 transition-transform focus:outline-none select-none">
            SA
          </button>

          {isOpen && (
            <div className="absolute top-[calc(100%+10px)] right-0 bg-[#112240] border border-white/10 rounded-xl overflow-hidden min-w-[250px] shadow-2xl z-50 animate-in fade-in slide-in-from-top-2 duration-150">
              
              <div className="p-4 pb-2.5 border-b border-white/10 sm:hidden">
                <div className="text-base font-medium text-white truncate">santiagoaguerre.info</div>
                <div className="text-sm text-[#c8a84b] mt-0.5 uppercase tracking-wider">Usuario</div>
              </div>

              <button onClick={() => { setIsOpen(false); navigate('/perfil'); }} className="flex items-center gap-2.5 px-4 py-3 text-white/75 text-base cursor-pointer hover:bg-white/5 hover:text-white transition-colors border-none bg-transparent w-full text-left">
                <i className="fa-solid fa-user w-4 text-center"></i> Mi perfil
              </button>
              
              <button onClick={() => { setIsOpen(false); }} className="flex items-center gap-2.5 px-4 py-3 text-[#f08080] text-base cursor-pointer hover:bg-[#c0392b]/15 hover:text-[#f08080] transition-colors border-none bg-transparent w-full text-left">
                <i className="fa-solid fa-right-from-bracket w-4 text-center"></i> Cerrar sesión
              </button>
            </div>
          )}
        </div>
      </div>
    </nav>
  )
}