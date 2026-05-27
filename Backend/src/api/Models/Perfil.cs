using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Perfil
{
    public string Mail { get; set; } = null!;

    public string PaisDocumento { get; set; } = null!;

    public string TipoDocumento { get; set; } = null!;

    public int NumeroDocumento { get; set; }

    public string DireccionLocalidad { get; set; } = null!;

    public int DireccionNumero { get; set; }

    public int DireccionCodigoPostal { get; set; }

    public virtual Administrador? Administrador { get; set; }

    public virtual Funcionario? Funcionario { get; set; }

    public virtual Login? Login { get; set; }

    public virtual Telefono? Telefono { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
