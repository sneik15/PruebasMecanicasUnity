using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerico : MonoBehaviour, IItemBase
{
    public string nombre;
    public string descripcion;
    public TipoObjeto tipoObjeto;
    public Sprite icono;

    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public GameObject ObjetoRelacionado { get; set; }
    public TipoObjeto TipoObjeto { get; set; }
    public Sprite Icono { get; set; }

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
        Nombre = nombre;
        Descripcion = descripcion;
        ObjetoRelacionado = transform.gameObject;
        TipoObjeto = tipoObjeto;
        Icono = icono;
        tiempoNecesario = float.Parse(GestorDatos.DatosGenericos["Items"]["TiempoDesaparicionS"].ToString());
        tiempoPasado = 0;
        autodestruir = false;
    }

    private void Update()
    {
        if (autodestruir)
        {
            tiempoPasado += Time.deltaTime;
            if(tiempoPasado >= tiempoNecesario)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public override string ToString()
    {
        return Nombre;
    }
}
