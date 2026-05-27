using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Entradum
{
    public int Identificador { get; set; }

    public int? IdentificadorVenta { get; set; }

    public int? IdentificadorPartido { get; set; }

    public string? MailUsuarioTiene { get; set; }

    public string EstadoEntrada { get; set; } = null!;

    public int? IdentificadorSector { get; set; }

    public int? IdentificadorEstadio { get; set; }

    public string? MailFuncionario { get; set; }

    public int? IdentificadorDispositivo { get; set; }

    public string? CodigoQraceptado { get; set; }

    public DateTime FechaHoraIngreso { get; set; }

    public virtual Dispositivo? IdentificadorDispositivoNavigation { get; set; }

    public virtual Partido? IdentificadorPartidoNavigation { get; set; }

    public virtual Funcionario? MailFuncionarioNavigation { get; set; }

    public virtual Usuario? MailUsuarioTieneNavigation { get; set; }

    public virtual Sector? Sector { get; set; }

    public virtual ICollection<Transferencium> Transferencia { get; set; } = new List<Transferencium>();
}
