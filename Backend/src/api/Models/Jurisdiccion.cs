using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Jurisdiccion
{
    public string Nombre { get; set; } = null!;

    public string Continente { get; set; } = null!;

    public virtual ICollection<Estadio> Estadios { get; set; } = new List<Estadio>();
}
