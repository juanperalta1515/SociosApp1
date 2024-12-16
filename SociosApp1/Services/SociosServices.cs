using CsvHelper;
using CsvHelper.Configuration;

using SociosApp1.Models;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SociosApp1.Services
{
    public class SociosServices
    {
        public List<Socio> LeerCsv(string filePath)
        {
            var socios = new List<Socio>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false
            };

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {
                    socios = csv.GetRecords<Socio>().ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al leer el archivo CSV.", ex);
            }

            return socios;
        }

        public int ObtenerCantidadTotal(List<Socio> socios) => socios.Count;

        public double CalcularPromedioEdadRacing(List<Socio> socios) =>
            socios.Where(s => s.Equipo == "Racing").Average(s => s.Edad);

        public List<Socio> ObtenerPrimerosCasadosUniversitarios(List<Socio> socios)
        {
            return socios.Where(s => s.EstadoCivil == "Casado" && s.NivelEstudios == "Universitario")
                  .OrderBy(s => s.Edad)
                  .Take(100)
                  .ToList();
        }

        public List<string> ObtenerNombresMasComunesRiver(List<Socio> socios) =>
            socios.Where(s => s.Equipo == "Racing")
                  .GroupBy(s => s.Nombre)
                  .OrderByDescending(g => g.Count())
                  .Take(5)
                  .Select(g => g.Key)
                  .ToList();

        public List<(string Equipo, double PromedioEdad, int EdadMinima, int EdadMaxima, int CantidadSocios)>
            ObtenerEstadisticasEquipos(List<Socio> socios) =>
            socios.GroupBy(s => s.Equipo)
                  .Select(g => (
                      Equipo: g.Key,
                      PromedioEdad: g.Average(s => s.Edad),
                      EdadMinima: g.Min(s => s.Edad),
                      EdadMaxima: g.Max(s => s.Edad),
                      CantidadSocios: g.Count()
                  ))
                  .OrderByDescending(e => e.CantidadSocios)
                  .ToList();
    }
}
