using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Pai
{
    public string Nombre { get; set; } = null!;

    public string Continente { get; set; } = null!;

    public virtual ICollection<Partido> PartidoPaisLocalNavigations { get; set; } = new List<Partido>();

    public virtual ICollection<Partido> PartidoPaisVisitanteNavigations { get; set; } = new List<Partido>();

    public virtual ICollection<Grupo> IdentificadorGrupos { get; set; } = new List<Grupo>();
}
