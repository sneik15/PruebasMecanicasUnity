using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IHerramientaBase : IItemBase
{
    Tier Tier { get; set; }
    TipoHerramienta TipoHerramientaU { get; set; }
    int DurabilidadAc { get; set; }
    int DurabilidadTOT { get; set; }
}

public enum Tier
{
    Madera,
    Piedra,
    Bronce,
    Hierro,
    Especial
}

public enum TipoHerramienta
{
    Hacha,
    Pico,
    Pala,
    Azada,
    CanyaPescar,
    Regadera
}
