function PartidoCard({partido, grupos, grupoActual}) {
  const grupo = grupos[grupoActual]

  const local = grupo?.find((e) => e.nombre === partido.equipoLocal)
  const visitante = grupo?.find((e) => e.nombre === partido.equipoVisitante)

  return (
    <div className="bg-[#16233c] rounded-3xl shadow-lg p-5 hover:scale-105 transition">
      <div className="text-yellow-400 text-sm font-bold mb-4 text-center">
        {partido.fase}
      </div>

      <div className="flex items-center justify-between">
        <img src={local?.bandera} className="w-14 h-10 object-cover rounded" />

        <span className="text-yellow-400 font-bold">VS</span>

        <img
          src={visitante?.bandera}
          className="w-14 h-10 object-cover rounded"
        />
      </div>

      <div className="text-gray-400 text-sm mt-4 text-center">
        {new Date(partido.fechaHora).toLocaleString('es-UY', {
          day: '2-digit',
          month: '2-digit',
          hour: '2-digit',
          minute: '2-digit',
        })}
      </div>
    </div>
  )
}

export default PartidoCard
