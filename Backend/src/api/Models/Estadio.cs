using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Estadio
{
    public int Identificador { get; set; }

    public string Nombre { get; set; } = null!;

    public string NombreJurisdiccion { get; set; } = null!;

    public string DireccionLocalidad { get; set; } = null!;

    public string DireccionCalle { get; set; } = null!;

    public int DireccionNumero { get; set; }

    public int DireccionCodigoPostal { get; set; }

    public virtual Jurisdiccion NombreJurisdiccionNavigation { get; set; } = null!;

    public virtual ICollection<Partido> Partidos { get; set; } = new List<Partido>();

    public virtual ICollection<Sector> Sectors { get; set; } = new List<Sector>();
}
