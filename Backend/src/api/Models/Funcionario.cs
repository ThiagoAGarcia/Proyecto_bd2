using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Funcionario
{
    public string MailPerfil { get; set; } = null!;

    public int NumeroLegajo { get; set; }

    public virtual ICollection<Dispositivofuncionario> Dispositivofuncionarios { get; set; } = new List<Dispositivofuncionario>();

    public virtual ICollection<Entradum> Entrada { get; set; } = new List<Entradum>();

    public virtual ICollection<Esasignado> Esasignados { get; set; } = new List<Esasignado>();

    public virtual Perfil MailPerfilNavigation { get; set; } = null!;
}
