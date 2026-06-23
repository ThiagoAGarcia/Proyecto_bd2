import {useEffect, useState} from 'react'
import {useParams, useNavigate} from 'react-router-dom'
import {IoArrowBack} from 'react-icons/io5'
import getGrupos from '../../services/FixtureService/GetGrupos'
import getPartidos from '../../services/FixtureService/GetPartidos'
import PartidoCard from './cards/PartidoCard'

function PartidosView() {
  const {grupo} = useParams()
  const navigate = useNavigate()

  const [grupos, setGrupos] = useState({})
  const [partidos, setPartidos] = useState([])

  useEffect(() => {
    async function cargar() {
      const dataGrupos = await getGrupos()
      const dataPartidos = await getPartidos()

      if (dataGrupos) setGrupos(dataGrupos)
      if (dataPartidos) setPartidos(dataPartidos)
    }
    cargar()
  }, [])

  function obtenerGrupo(partido) {
    for (const [g, equipos] of Object.entries(grupos)) {
      const nombres = equipos.map((e) => e.nombre)

      if (
        nombres.includes(partido.equipoLocal) &&
        nombres.includes(partido.equipoVisitante)
      ) {
        return g
      }
    }
    return null
  }

  const partidosFiltrados = partidos.filter((p) => obtenerGrupo(p) === grupo)

  return (
    <div className="min-h-screen bg-[#0a1628] p-8">
      <button
        onClick={() => navigate('/grupos')}
        className="cursor-pointer flex items-center gap-2 text-white bg-[#16233c] hover:bg-[#223557] transition px-4 py-2 rounded-full shadow-md mb-6">
        <IoArrowBack size={18} />
        Volver a grupos
      </button>

      <h1 className="text-white text-center text-4xl font-bold mb-10">
        GRUPO {grupo}
      </h1>

      <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
        {partidosFiltrados.map((p) => (
          <PartidoCard
            key={p.identificador}
            partido={p}
            grupos={grupos}
            grupoActual={grupo}
          />
        ))}
      </div>
    </div>
  )
}

export default PartidosView
