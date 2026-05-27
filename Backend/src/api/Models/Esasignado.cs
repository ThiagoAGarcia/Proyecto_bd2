using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Esasignado
{
    public string MailFuncionario { get; set; } = null!;

    public int IdentificadorSector { get; set; }

    public int IdentificadorEstadio { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual Funcionario MailFuncionarioNavigation { get; set; } = null!;

    public virtual Sector Sector { get; set; } = null!;
}
