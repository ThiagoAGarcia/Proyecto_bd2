using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Transferencium
{
    public int Identificador { get; set; }

    public int IdentificadorEntrada { get; set; }

    public string MailUsuarioRealiza { get; set; } = null!;

    public string MailUsuarioRecibe { get; set; } = null!;

    public virtual Entradum IdentificadorEntradaNavigation { get; set; } = null!;

    public virtual Usuario MailUsuarioRealizaNavigation { get; set; } = null!;

    public virtual Usuario MailUsuarioRecibeNavigation { get; set; } = null!;
}
