using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Telefono
{
    public string MailPerfil { get; set; } = null!;

    public int Telefono1 { get; set; }

    public virtual Perfil MailPerfilNavigation { get; set; } = null!;
}
