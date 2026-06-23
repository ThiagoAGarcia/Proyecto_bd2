import React, {useEffect, useState} from 'react'
import RankingCompradores from '../../../../services/EstadisticasService/RankingCompreadores'
import RankingPartidosMayorVendidos from '../../../../services/EstadisticasService/RankingMayorPartidoVendido'
import {BarChart, Bar, XAxis, YAxis, Tooltip} from 'recharts'
const StatsManagement = () => {
  const [ranking, setRanking] = useState([])
  const [rankingPartidos, setRankingPartidos] = useState([])
  useEffect(() => {
    const fetchData = async () => {
      try {
        const rankingCompradores = await RankingCompradores()
        const rankingPartidos = await RankingPartidosMayorVendidos()
        setRanking(rankingCompradores)
        setRankingPartidos(rankingPartidos)

        console.log('Ranking de compradores:', rankingCompradores)
        console.log('Ranking de partidos:', rankingPartidos)
      } catch (error) {
        console.error('Error fetching ranking de compradores:', error)
      }
    }

    fetchData()
  }, [])

  return (
    <div className="text-xl">
      <div className="sm:flex justify-between sm:items-end items-start w-full pb-4">
        <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none sm:pb-0 pb-4">
          <span className="text-[#c8a84b]">Estadisticas</span>
        </h1>
      </div>
      <div className="flex gap-5">
        <div className="flex flex-col gap-5">
          <h2 className="font-sans text-2xl font-bold text-[#0a1628] mt-8 uppercase tracking-wide leading-none sm:pb-0 pb-4">
            Mayores Compradores
          </h2>
          <BarChart
            data={ranking}
            width={500}
            height={400}
            className="width-[50%] border border-[#d0dcea] rounded-2xl shadow-sm hover:shadow-lg transition-all duration-300 p-5">
            <XAxis
              dataKey="usuarioComprador"
              angle={-45}
              textAnchor="end"
              height={190}
            />
            <YAxis />
            <Tooltip />
            <Bar dataKey="ventas" />
          </BarChart>
        </div>
        <div className="flex flex-col gap-5">
          <h2 className="font-sans text-2xl font-bold text-[#0a1628] mt-8 uppercase tracking-wide leading-none sm:pb-0 pb-4">
            Mayores Partidos Vendidos
          </h2>
          <BarChart
            data={rankingPartidos}
            width={500}
            height={400}
            className="width-[50%] border border-[#d0dcea] rounded-2xl shadow-sm hover:shadow-lg transition-all duration-300 p-5">
            <XAxis
              dataKey="partido"
              angle={-45}
              textAnchor="end"
              height={190}
            />
            <YAxis />
            <Tooltip />
            <Bar dataKey="cant_ventas" />
          </BarChart>
        </div>
      </div>
      <div className="flex flex-col gap-5"></div>
    </div>
  )
}

export default StatsManagement
