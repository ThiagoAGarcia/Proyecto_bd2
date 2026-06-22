import {useEffect, useState} from 'react'
import getGrupos from '../../services/FixtureService/GetGrupos'
import {useNavigate} from 'react-router-dom'
import GrupoCard from './cards/GrupoCard'
import {IoArrowBack} from 'react-icons/io5'

function GruposView() {
  const [grupos, setGrupos] = useState({})
  const navigate = useNavigate()

  useEffect(() => {
    async function cargar() {
      const data = await getGrupos()
      if (data) setGrupos(data)
    }
    cargar()
  }, [])

  return (
    <div className="min-h-screen bg-[#0a1628] p-8">
      <div className="flex items-center justify-between mb-10">
        <button
          onClick={() => navigate(-1)}
          className="flex items-center gap-2 text-white bg-[#16233c] hover:bg-[#223557] transition px-4 py-2 rounded-full shadow-md">
          <IoArrowBack size={18} />
          Volver
        </button>

        <h1 className="text-white text-center text-4xl font-bold">
          FIFA WORLD CUP 2026
        </h1>

        <div className="w-[140px]" />
      </div>

      <div className="grid lg:grid-cols-4 md:grid-cols-3 sm:grid-cols-2 gap-8">
        {Object.entries(grupos).map(([grupo, equipos]) => (
          <GrupoCard
            key={grupo}
            grupo={grupo}
            equipos={equipos}
            onClick={() => navigate(`/partidos/${grupo}`)}
          />
        ))}
      </div>
    </div>
  )
}

export default GruposView
