using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regadera : MonoBehaviour, IHerramientaBase
{
    public Tier tier;
    public string nombre;
    public string descripcion;
    public TipoHerramienta tipoHerramientaU;
    public int durabilidadAc;
    public int durabilidadTOT;
    public TipoObjeto tipoObjeto;
    public Sprite icono;

    public Tier Tier { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public TipoHerramienta TipoHerramientaU { get; set; }
    public GameObject ObjetoRelacionado { get; set; }
    public TipoObjeto TipoObjeto { get; set; }
    public int DurabilidadAc { get; set; }
    public int DurabilidadTOT { get; set; }
    public Sprite Icono { get; set; }

    public int AguaActual { get; private set; }

    private int capacidadRegadera;

    //Autodestruccion
    public bool Autodestruir
    {
        get => autodestruir;
        set
        {
            if (!value)
            {
                tiempoPasado = 0;
            }
            autodestruir = value;
        }
    }

    private float tiempoPasado;
    private float tiempoNecesario;
    private bool autodestruir;

    void Awake()
    {
        Tier = tier;
        Nombre = nombre;
        Descripcion = descripcion;
        TipoHerramientaU = tipoHerramientaU;
        ObjetoRelacionado = transform.gameObject;
        TipoObjeto = tipoObjeto;
        DurabilidadAc = durabilidadAc;
        DurabilidadTOT = durabilidadTOT;
        Icono = icono;
        tiempoNecesario = float.Parse(GestorDatos.DatosGenericos["Items"]["TiempoDesaparicionS"].ToString());
        capacidadRegadera = int.Parse(GestorDatos.Herramientas[Enum.GetName(typeof(TipoHerramienta), TipoHerramientaU)][Enum.GetName(typeof(Tier), Tier)]["Capacidad"].ToString());
        tiempoPasado = 0;
        autodestruir = false;
        AguaActual = capacidadRegadera;
    }

    private void Update()
    {
        if (autodestruir)
        {
            tiempoPasado += Time.deltaTime;
            if (tiempoPasado >= tiempoNecesario)
            {
                Destroy(gameObject);
            }
        }
    }

    public override string ToString()
    {
        return Nombre;
    }

    public bool Usar()
    {
        if (AguaActual > 0)
        {
            AguaActual--;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Recargar()
    {
        AguaActual = capacidadRegadera;
    }
}
