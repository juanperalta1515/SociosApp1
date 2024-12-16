using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SociosApp1.Models;
using SociosApp1.Services;

using System.IO;

namespace SociosApp1.Controllers
{
    public class SociosController : Controller
    {
        private readonly SociosServices _sociosService;

        public SociosController()
        {
            _sociosService = new SociosServices();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcesarArchivo(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("archivo", "Por favor selecciona un archivo válido.");
                return View("Index");
            }

            try
            {
                var resultados = new ResultadosViewModel();

                using (var reader = new StreamReader(archivo.OpenReadStream()))
                {
                    var lineas = reader.ReadToEnd().Split(Environment.NewLine);
                    var socios = lineas
                        .Skip(1) // Saltar cabecera
                        .Where(linea => !string.IsNullOrWhiteSpace(linea))
                        .Select(linea =>
                        {
                            var datos = linea.Split(';');
                            return new Socio
                            {
                                Nombre = datos[0],
                                Edad = int.Parse(datos[1]),
                                Equipo = datos[2],
                                // Estado Civil y Nivel de Estudios pueden ser agregados según necesidad
                            };
                        })
                        .ToList();

                    // Calcular los resultados
                    resultados.CantidadTotal = socios.Count;
                    resultados.PromedioEdadRacing = socios
                        .Where(s => s.Equipo == "Racing")
                        .Average(s => s.Edad);

                    resultados.PrimerosCienCasadosUniversitarios = socios
                        .Where(s => s.EstadoCivil =="Casados")
                        .OrderBy(s => s.Edad)
                        .Take(100)
                        .ToList();

                    resultados.NombresComunesRiver = socios
                        .Where(s => s.Equipo == "River")
                        .GroupBy(s => s.Nombre)
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .Select(g => g.Key)
                        .ToList();

                    resultados.EstadisticasEquipos = socios
                    .GroupBy(s => s.Equipo)
                        .Select(g => new EstadisticasEquipo
                        {
                            Equipo = g.Key,
                            PromedioEdad = g.Average(s => s.Edad),
                            MenorEdad = g.Min(s => s.Edad),
                            MayorEdad = g.Max(s => s.Edad)
                        })
                        .OrderByDescending(e => e.PromedioEdad)
                        .ToList();
                }

                return View("Resultados", resultados);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al procesar el archivo: {ex.Message}");
                return View("Index");
            }
        }

    }
}
