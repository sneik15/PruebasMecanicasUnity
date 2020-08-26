using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GestorTraducciones
{
    private static JObject traducciones = new JObject();

    static GestorTraducciones()
    {
        try
        {
            //Recupera de la clase de configuracion el ultimo idioma establecido
            TextAsset jsonTextFile = Resources.Load<TextAsset>($"Traducciones/T{GestorConfiguracion.RecuperarUltimoIdioma()}");
            traducciones = JObject.Parse(jsonTextFile.text);
        }
        catch
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>($"Traducciones/TES-ES");
            traducciones = JObject.Parse(jsonTextFile.text);
        }
    }

    public static bool Inicializar(string idioma)
    {
        bool correcto = true;
        
        return correcto;
    }

    public static string RecuperarCambioIdioma(string nuevoIdioma)
    {
        try
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>($"Traducciones/T{nuevoIdioma}");
            return JObject.Parse(jsonTextFile.text)["CambioIdioma"].ToString();
        }
        catch
        {
            return "";
        }
    }

    public static string TraducirTexto(string texto)
    {
        try
        {
            string nuevoTexto = "";

            bool encontradoIni = false;
            string textoBusc = "";

            for (int i = 0; i < texto.Length; i++)
            {
                if(texto[i] == '{')
                {
                    encontradoIni = true;
                }
                else if (texto[i] == '}')
                {
                    encontradoIni = false;
                    nuevoTexto += traducciones[textoBusc].ToString();
                }
                else if (!encontradoIni)
                {
                    nuevoTexto += texto[i];
                }
                else if(encontradoIni)
                {
                    textoBusc += texto[i];
                }
            }

            return nuevoTexto;
        }
        catch
        {
            return texto;
        }
    }
}

