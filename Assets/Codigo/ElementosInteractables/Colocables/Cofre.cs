using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cofre : MonoBehaviour
{
    public SlotInventario[] Inventario;
    public int CapacidadInventario;

    private void Awake()
    {
        CapacidadInventario = int.Parse(GestorDatos.DatosGenericos["Cofres"]["Espacios"][gameObject.tag].ToString());
        Inventario = new SlotInventario[CapacidadInventario];
    }
}
