using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IItemBase
{
    string Nombre { get; set; }
    string Descripcion { get; set; }
    GameObject ObjetoRelacionado { get; set; }
    TipoObjeto TipoObjeto { get; set; }
    Sprite Icono { get; set; }
    bool Autodestruir { get; set; }
}

public enum TipoObjeto
{
    Peque,
    Mediano,
    Grande,
    NoStack
}
