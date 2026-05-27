using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Grupo
{
    public int Identificador { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Etapa> Etapas { get; set; } = new List<Etapa>();

    public virtual ICollection<Pai> NombrePais { get; set; } = new List<Pai>();
}
