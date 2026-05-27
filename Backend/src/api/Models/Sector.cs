using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Sector
{
    public int Identificador { get; set; }

    public int IdentificadorEstadio { get; set; }

    public string Nombre { get; set; } = null!;

    public int CapMax { get; set; }

    public int TarifaExtra { get; set; }

    public virtual ICollection<Entradum> Entrada { get; set; } = new List<Entradum>();

    public virtual ICollection<Esasignado> Esasignados { get; set; } = new List<Esasignado>();

    public virtual Estadio IdentificadorEstadioNavigation { get; set; } = null!;

    public virtual ICollection<Partido> IdentificadorPartidos { get; set; } = new List<Partido>();
}
