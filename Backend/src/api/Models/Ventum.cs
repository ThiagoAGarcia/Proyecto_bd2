using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Ventum
{
    public int Identificador { get; set; }

    public DateOnly Fecha { get; set; }

    public int PorcentakeComision { get; set; }

    public int MontoTotal { get; set; }

    public string MailUsuarioComprado { get; set; } = null!;

    public virtual Usuario MailUsuarioCompradoNavigation { get; set; } = null!;
}
