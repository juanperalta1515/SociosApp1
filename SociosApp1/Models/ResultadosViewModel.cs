namespace SociosApp1.Models
{
    public class ResultadosViewModel
    {
        public int CantidadTotal { get; set; }
        public double PromedioEdadRacing { get; set; }
        public List<Socio>? PrimerosCienCasadosUniversitarios { get; set; }
        public List<string>? NombresComunesRiver { get; set; }
        public List<EstadisticasEquipo> EstadisticasEquipos { get; set; }
    }
}
