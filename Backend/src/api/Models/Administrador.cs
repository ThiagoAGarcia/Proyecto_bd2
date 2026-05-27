using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Administrador
{
    public string MailPerfil { get; set; } = null!;

    public DateOnly FechaAsignacionCargo { get; set; }

    public virtual Perfil MailPerfilNavigation { get; set; } = null!;

    public virtual ICollection<Partido> IdentificadorPartidos { get; set; } = new List<Partido>();
}
