using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Partido
{
    public int Identificador { get; set; }

    public string Fase { get; set; } = null!;

    public string PaisLocal { get; set; } = null!;

    public string PaisVisitante { get; set; } = null!;

    public int IdentificadorEstadio { get; set; }

    public DateTime FechaHora { get; set; }

    public virtual ICollection<Entradum> Entrada { get; set; } = new List<Entradum>();

    public virtual Estadio IdentificadorEstadioNavigation { get; set; } = null!;

    public virtual Pai PaisLocalNavigation { get; set; } = null!;

    public virtual Pai PaisVisitanteNavigation { get; set; } = null!;

    public virtual ICollection<Administrador> MailAdministradors { get; set; } = new List<Administrador>();

    public virtual ICollection<Sector> Sectors { get; set; } = new List<Sector>();
}
