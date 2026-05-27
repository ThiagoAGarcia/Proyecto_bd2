using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Dispositivofuncionario
{
    public string MailFuncionario { get; set; } = null!;

    public int IdentificadorDispositivo { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual Dispositivo IdentificadorDispositivoNavigation { get; set; } = null!;

    public virtual Funcionario MailFuncionarioNavigation { get; set; } = null!;
}
