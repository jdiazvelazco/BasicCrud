using apiexamen;
using ClienteExamen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ClienteExamen.Controllers
{
    public class ExamenController : Controller
    {
        private readonly ILogger<ExamenController> _logger;

        private string _connectionString;// = "Server=DELLJDV\\SQLEXPRESS01;Database=bdiExamen;Trusted_Connection=True;TrustServerCertificate=True;";
        private string _apiBaseUrl;// = "https://localhost:7137/wsApiExamen";

        public ExamenController(IConfiguration configuration, IOptions<ApiSettings> apiSettings, ILogger<ExamenController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _apiBaseUrl = apiSettings.Value.ApiBaseUrl;
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            var model = new ExamenViewModel
            {
                Id = null,
                Nombre = "",
                Descripcion = "",
                UsarSPs = true,
                Codigo = "",
                Mensaje = ""
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(ExamenViewModel model)
        {
            if (model.Id == null)
            {   model.Codigo = "Error";
                model.Mensaje = "El campo Id es obligatorio";
                return View("Index", model);
            }
             var clsExamen = new ClsExamen(
                useSPs: model.UsarSPs,
                connectionString: _connectionString,
                apiUrl: _apiBaseUrl
            );

            var resultado = await clsExamen.AgregarExamen((int)model.Id, model.Nombre, model.Descripcion, CancellationToken.None);
            model.Codigo = resultado.Procesado ? "Éxitoso" : "Error";
            model.Mensaje = model.Codigo + " " + resultado.Mensaje;

            return View("Index", model);
        }
        [HttpPost]
        public async Task<IActionResult> Actualizar(ExamenViewModel model)
        {
            if (model.Id == null)
            {
                model.Codigo = "Error";
                model.Mensaje = "El campo Id es obligatorio";
                return View("Index", model);
            }
            var clsExamen = new ClsExamen(
               useSPs: model.UsarSPs,
               connectionString: _connectionString,
               apiUrl: _apiBaseUrl
           );
            var resultado = await clsExamen.ActualizarExamen((int)model.Id, model.Nombre, model.Descripcion, CancellationToken.None);
            model.Codigo = resultado ? "Actualización exitosa" : "Error en la actualización";
            model.Mensaje = model.Codigo;
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(ExamenViewModel model)
        {
            if (model.Id == null)
            {
                model.Codigo = "Error";
                model.Mensaje = "El campo Id es obligatorio";
                return View("Index", model);
            }
            var clsExamen = new ClsExamen(
               useSPs: model.UsarSPs,
               connectionString: _connectionString,
               apiUrl: _apiBaseUrl
           );
            var resultado = await clsExamen.EliminarExamen((int)model.Id, CancellationToken.None);
            model.Codigo = resultado ? "Eliminación exitosa" : "Error en la eliminación";
            model.Mensaje = model.Codigo;
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Consultar(ExamenViewModel model)
        {
            var clsExamen = new ClsExamen(
               useSPs: model.UsarSPs,
               connectionString: _connectionString,
               apiUrl: _apiBaseUrl
           );
            var resultado = await clsExamen.ConsultarExamen(model.Id, model.Nombre, model.Descripcion, CancellationToken.None);
            List<Examen> listaExamenes = resultado.Select(static o => new Examen
                {
                    IdExamen = (int?)o.GetType().GetProperty("IdExamen")?.GetValue(o) ?? 0,
                    Nombre = (string?)o.GetType().GetProperty("Nombre")?.GetValue(o) ?? string.Empty,
                    Descripcion = (string?)o.GetType().GetProperty("Descripcion")?.GetValue(o) ?? string.Empty
                }).ToList();
            model.ListaExamenes = listaExamenes;// resultado.Cast<Examen>().ToList();
            
            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
