using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Login
{
    public string MailPerfil { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual Perfil MailPerfilNavigation { get; set; } = null!;
}
