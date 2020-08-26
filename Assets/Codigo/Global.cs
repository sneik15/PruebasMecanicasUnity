using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Global
{
    #region General
    public static bool JuegoPausado { get; set; }
    #endregion

    #region Jugador
    //Mecanicas
    public static IItemBase ItemSelec { get; set; }
    public static bool MovBloq { get; set; }
    public static bool JugadorSuelo { get; set; }
    public static bool InventarioAbierto { get; set; }
    public static bool CofreAbierto { get; set; }
    public static bool MoviendoItems { get; set; }
    #endregion

    #region Tiempo
    public static readonly float MinutosDiaReal = 1440;
    public static Estaciones EstacionActual { get; set; }
    public static DiasSemana DiaSemanaActual { get; set; }
    public static int DiaActual { get; set; }
    public static float SegundoActual { get; set; }
    public static bool Dia { get; set; }
    public static string HoraTexto { get; set; }
    #endregion
}
