using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Etapa
{
    public int Identificador { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdentificadorGrupo { get; set; }

    public virtual Grupo IdentificadorGrupoNavigation { get; set; } = null!;
}
