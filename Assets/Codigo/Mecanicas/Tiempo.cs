using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tiempo : MonoBehaviour
{
    private float segundosDiaObj;
    private float segundosNocheObj;
    private float segundoDiaNoche;
    private float diaObjEstacion;

    public float MultiplicadorTiempoPruebas = 1;
    public Text DiaHoraUI;

    void Awake()
    {
        Global.EstacionActual = Estaciones.PRIMAVERA;
        Global.DiaSemanaActual = DiasSemana.LUNES;
        CambiarEstacion();
        RestablecerDia();
    }

    void FixedUpdate()
    {
        Global.SegundoActual += Time.deltaTime * MultiplicadorTiempoPruebas;
        segundoDiaNoche += Time.deltaTime * MultiplicadorTiempoPruebas;

        if(Global.SegundoActual > (segundosDiaObj + segundosNocheObj))
        {
            Global.SegundoActual -= (segundosDiaObj + segundosNocheObj);
            Global.DiaActual++;
            CambiarDiaSemana();
        }

        if(Global.Dia && segundoDiaNoche > segundosDiaObj)
        {
            segundoDiaNoche -= segundosDiaObj;
            Global.Dia = false;
        }
        else if (!Global.Dia && segundoDiaNoche > segundosNocheObj)
        {
            segundoDiaNoche -= segundosNocheObj;
            Global.Dia = true;
        }

        if(Global.DiaActual > diaObjEstacion)
        {
            Global.DiaActual = 1;
            switch (Global.EstacionActual)
            {
                case Estaciones.INVIERNO:
                    Global.EstacionActual = Estaciones.PRIMAVERA;
                    break;
                default:
                    Global.EstacionActual++;
                    break;
            }
            CambiarEstacion();
        }

        Global.HoraTexto = CalcularHoraVisual();

        DiaHoraUI.text = Enum.GetName(typeof(Estaciones), Global.EstacionActual) + ", " + Enum.GetName(typeof(DiasSemana), Global.DiaSemanaActual) + " dia " + Global.DiaActual + ", HORA: " + Global.HoraTexto;
    }

    private string CalcularHoraVisual()
    {
        float segundoActualJuego = (Global.MinutosDiaReal * Global.SegundoActual) / (segundosDiaObj + segundosNocheObj);
        int hora = (int)(segundoActualJuego / 60);
        int minuto = (int)(segundoActualJuego % 60);
        string horaFinal = hora > 9 ? hora.ToString() : "0" + hora.ToString();
        string minutoFinal = minuto > 9 ? minuto.ToString() : "0" + minuto.ToString();
        return horaFinal + ":" + minutoFinal;
    }

    private float CalcularPosicionSol()
    {
        return (Global.SegundoActual * 90) / 600;
    }

    private void CambiarDiaSemana()
    {
        switch (Global.DiaSemanaActual)
        {
            case DiasSemana.DOMINGO:
                Global.DiaSemanaActual = DiasSemana.LUNES;
                break;
            default:
                Global.DiaSemanaActual++;
                break;
        }
    }

    private void CambiarEstacion()
    {
        segundosDiaObj = float.Parse(GestorDatos.DatosGenericos["Tiempo"][Enum.GetName(typeof(Estaciones), Global.EstacionActual)]["DIA"].ToString());
        segundosNocheObj = float.Parse(GestorDatos.DatosGenericos["Tiempo"][Enum.GetName(typeof(Estaciones), Global.EstacionActual)]["NOCHE"].ToString());
        diaObjEstacion = float.Parse(GestorDatos.DatosGenericos["Tiempo"][Enum.GetName(typeof(Estaciones), Global.EstacionActual)]["DIAS"].ToString());
    }

    private void RestablecerDia()
    {
        Global.DiaActual = 1;
        Global.Dia = true;
        Global.SegundoActual = 300;
        segundoDiaNoche = 0;
    }
}

public enum Estaciones
{
    PRIMAVERA,
    VERANO,
    OTONYO,
    INVIERNO
}

public enum DiasSemana
{
    LUNES,
    MARTES,
    MIERCOLES,
    JUEVES,
    VIERNES,
    SABADO,
    DOMINGO
}
