using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GestorConfiguracion
{
    private static JObject Configuracion;
    static GestorConfiguracion()
    {
        //Aqui se leeria el archivo de configuracion JSON cuando esa parte este hecha
        Configuracion = new JObject();
        Configuracion["IDIOMA"] = "ES-ES";
    }

    public static string RecuperarUltimoIdioma()
    {
        return Configuracion["IDIOMA"].ToString();
    }
}
