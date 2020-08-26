using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public static class GestorDatos
{
    //Estadisticas elementos
    public static JObject Herramientas { get; set; }
    public static JObject Drops { get; set; }
    public static JObject DatosGenericos { get; set; }
    public static JObject Cultivos { get; set; }

    public static bool Inicializado { get; set; }

    static GestorDatos()
    {
        CargarHerramientas();
        CargaDrops();
        CargaDatosGenericos();
        CargaCultivos();

        Inicializado = true;
    }

    private static bool CargarHerramientas()
    {
        bool correcto = true;
        try
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>("JSONDatos/Herramientas");
            Herramientas = JObject.Parse(jsonTextFile.text);
        }
        catch
        {
            correcto = false;
        }
        return correcto;
    }

    private static bool CargaDrops()
    {
        bool correcto = true;
        try
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>("JSONDatos/Drops");
            Drops = JObject.Parse(jsonTextFile.text);
        }
        catch
        {
            correcto = false;
        }
        return correcto;
    }
    
    private static bool CargaDatosGenericos()
    {
        bool correcto = true;
        try
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>("JSONDatos/DatosGenericos");
            DatosGenericos = JObject.Parse(jsonTextFile.text);
        }
        catch
        {
            correcto = false;
        }
        return correcto;
    }
    
    private static bool CargaCultivos()
    {
        bool correcto = true;
        try
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>("JSONDatos/Cultivos");
            Cultivos = JObject.Parse(jsonTextFile.text);
        }
        catch
        {
            correcto = false;
        }
        return correcto;
    }
}

