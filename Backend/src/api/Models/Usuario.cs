using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Usuario
{
    public string MailPerfil { get; set; } = null!;

    public DateOnly FechaRegistro { get; set; }

    public string EstadoVerificado { get; set; } = null!;

    public virtual ICollection<Entradum> Entrada { get; set; } = new List<Entradum>();

    public virtual Perfil MailPerfilNavigation { get; set; } = null!;

    public virtual ICollection<Transferencium> TransferenciumMailUsuarioRealizaNavigations { get; set; } = new List<Transferencium>();

    public virtual ICollection<Transferencium> TransferenciumMailUsuarioRecibeNavigations { get; set; } = new List<Transferencium>();

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
