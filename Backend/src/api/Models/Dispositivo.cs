using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Dispositivo
{
    public int Identificador { get; set; }

    public virtual ICollection<Dispositivofuncionario> Dispositivofuncionarios { get; set; } = new List<Dispositivofuncionario>();

    public virtual ICollection<Entradum> Entrada { get; set; } = new List<Entradum>();
}
