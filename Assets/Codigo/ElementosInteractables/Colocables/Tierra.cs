using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tierra : MonoBehaviour
{
    public Material MaterialBase;
    public Material MaterialRegado;

    private MeshRenderer[] Surcos;
    private bool regado;
    private int diaRegado;
    private int diaSinRegar;
    private int diaSinRecoger;

    private bool plantada;
    private bool estropeada;
    private bool plantaLista;
    private int progresoCrecimiento;
    private int objetivoCrecimiento;
    private Estaciones[] estacionObjetivo;
    private string plantaPlantada;
    private string nombreFases;
    private TiposCultivos tipoCultivo;
    private bool tienePalo;
    private GameObject plantaVisual;
    private int maxDiasSinRecoger;

    public bool Regado { get => regado; set => Regar(value); }
    public bool Estropeada { get => estropeada; }
    public bool Plantada { get => plantada; }
    public bool PlantaLista { get => plantaLista; }

    private void Awake()
    {
        maxDiasSinRecoger = int.Parse(GestorDatos.DatosGenericos["Cultivos"]["DiasEstropeoCultivos"].ToString());
    }

    void Start()
    {
        Surcos = new MeshRenderer[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform hijo = transform.GetChild(i);
            Surcos[i] = hijo.gameObject.GetComponent<MeshRenderer>();
        }
    }

    void FixedUpdate()
    {
        if (plantada && !estropeada && !EstacionValida())
        {
            Regar(false);
            PlantaEstropeada();
        }
        else if (plantada && !plantaLista && !estropeada && regado && Global.DiaActual != diaRegado)
        {
            Regar(false);
            PlantaCrece();
        }
        else if (plantada && !plantaLista && !estropeada && !regado && Global.DiaActual != diaSinRegar)
        {
            Regar(false);
            PlantaEstropeada();
        }
        else if (plantaLista && Global.DiaActual != diaSinRecoger)
        {
            if(regado && Global.DiaActual != diaRegado)
            {
                Regar(false);
            }
            PlantaCrece();
        }
    }

    public bool Plantar(string planta)
    {
        bool correcto = false;
        Enum.TryParse(GestorDatos.Cultivos[planta]["Tipo"].ToString(), out TiposCultivos tipoCultivo);
        
        if (!plantada)
        {
            if (tipoCultivo == TiposCultivos.NORMAL || tienePalo)
            {
                this.tipoCultivo = tipoCultivo;
                plantaPlantada = planta;
                plantada = true;
                estropeada = false;
                plantaLista = false;
                progresoCrecimiento = 0;
                objetivoCrecimiento = int.Parse(GestorDatos.Cultivos[plantaPlantada]["TiempoCrecimiento"].ToString());
                JArray estaciones = JArray.Parse(GestorDatos.Cultivos[plantaPlantada]["Estaciones"].ToString());
                estacionObjetivo = new Estaciones[estaciones.Count];
                for (int i = 0; i < estaciones.Count; i++)
                {
                    if (Enum.TryParse(estaciones[i].ToString(), out Estaciones estacionParse))
                    {
                        estacionObjetivo[i] = estacionParse;
                    }
                }
                nombreFases = GestorDatos.Cultivos[plantaPlantada]["NombreFases"].ToString();
                ActualizarModeloCrecimieto();
                if (!regado){
                    Regar(false);
                }
            }
        }
        return correcto;
    }

    public string RecogerPlantaLista(out int numero)
    {
        string planta = "";
        numero = 0;
        if (plantaLista)
        {
            planta = GestorDatos.Cultivos[plantaPlantada]["NombreDrop"].ToString();
            numero = UnityEngine.Random.Range(int.Parse(GestorDatos.Cultivos[plantaPlantada]["DropMin"].ToString()), int.Parse(GestorDatos.Cultivos[plantaPlantada]["DropMax"].ToString()) + 1);
            plantada = false;
            estropeada = false;
            plantaLista = false;
            progresoCrecimiento = 0;
            plantaPlantada = "";
            nombreFases = "";
            if (plantaVisual != null)
            {
                Destroy(plantaVisual);
            }
        }
        return planta;
    }

    private void ActualizarModeloCrecimieto()
    {
        int numfase;
        if(progresoCrecimiento < (objetivoCrecimiento - 1) / 2)
        {
            numfase = 0;
        }
        else if(progresoCrecimiento < objetivoCrecimiento)
        {
            numfase = 1;
        }
        else
        {
            numfase = 2;
        }
        GameObject faseActual;
        if (estropeada)
        {
            faseActual = Resources.Load<GameObject>("Prefabs/AyudaMecanicas/Cultivos/" + nombreFases + numfase + "ESTRO");
        }
        else
        {
            faseActual = Resources.Load<GameObject>("Prefabs/AyudaMecanicas/Cultivos/" + nombreFases + numfase);
        }
        if(plantaVisual != null)
        {
            Destroy(plantaVisual);
        }
        plantaVisual = Instantiate(faseActual, transform.position, transform.rotation);
    }

    private void Regar(bool regar)
    {
        if (regar)
        {
            if (!regado)
            {
                regado = true;
                diaRegado = Global.DiaActual;
                foreach (MeshRenderer surco in Surcos)
                {
                    surco.material = MaterialRegado;
                }
            }
        }
        else
        {
            //Aqui mirar si se quitara el regado por tiempo o sera en otra partec
            regado = false;
            diaSinRegar = Global.DiaActual;
            foreach (MeshRenderer surco in Surcos)
            {
                surco.material = MaterialBase;
            }
        }
    }

    private void PlantaEstropeada()
    {
        if (plantada) 
        { 
            estropeada = true;
            plantaLista = false;
            ActualizarModeloCrecimieto();
        }
    }

    private void PlantaCrece()
    {
        if (plantada && !estropeada)
        {
            progresoCrecimiento++;
            if (progresoCrecimiento >= objetivoCrecimiento)
            {
                diaSinRecoger = Global.DiaActual;
                plantaLista = true;
                if(progresoCrecimiento >= (objetivoCrecimiento + maxDiasSinRecoger))
                {
                    PlantaEstropeada();
                }
            }
            ActualizarModeloCrecimieto();
        }
    }

    private bool EstacionValida()
    {
        return estacionObjetivo.Contains(Global.EstacionActual);
    }
}
