function GrupoCard({grupo, equipos, onClick}) {
  return (
    <div
      onClick={onClick}
      className="cursor-pointer bg-[#16233c] rounded-3xl shadow-xl p-5 hover:scale-105 transition">
      <h2 className="text-center text-2xl text-yellow-400 font-bold mb-5">
        GRUPO {grupo}
      </h2>

      {equipos.map((e) => (
        <div
          key={e.nombre}
          className="flex items-center bg-[#223557] rounded-2xl p-3 mb-3">
          <img src={e.bandera} className="w-12 h-8 object-cover rounded mr-4" />
          <span className="text-white capitalize">{e.nombre}</span>
        </div>
      ))}
    </div>
  )
}

export default GrupoCard
