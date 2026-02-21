using apiexamen;
using ClienteExamen.Models;
using Microsoft.AspNetCore.Mvc;
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
            HttpContext.Session.SetString("filterUsarSPs", model.UsarSPs.ToString());
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
            HttpContext.Session.SetString("filterUsarSPs", model.UsarSPs.ToString());
            var clsExamen = new ClsExamen(
               useSPs: model.UsarSPs,
               connectionString: _connectionString,
               apiUrl: _apiBaseUrl
            );
            var resultado = await clsExamen.ActualizarExamen((int)model.Id, model.Nombre, model.Descripcion, CancellationToken.None);
            model.Codigo = resultado ? "Actualización exitosa" : "Error en la actualización";
            model.Mensaje = model.Codigo;
            var modelRetorno = await CargarGridAsync(model.Mensaje);
            return View("Index", modelRetorno);
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
            HttpContext.Session.SetString("filterUsarSPs", model.UsarSPs.ToString());
            var clsExamen = new ClsExamen(
               useSPs: model.UsarSPs,
               connectionString: _connectionString,
               apiUrl: _apiBaseUrl
           );
            var resultado = await clsExamen.EliminarExamen((int)model.Id, CancellationToken.None);
            model.Codigo = resultado ? "Eliminación exitosa" : "Error en la eliminación";
            model.Mensaje = model.Codigo;
            var modelRetorno = await CargarGridAsync(model.Mensaje);
            return View("Index", modelRetorno);
            //return RedirectToAction("Index", modelRetorno);

        }

        [HttpPost]
        public async Task<IActionResult> Consultar(ExamenViewModel model)
        {
            HttpContext.Session.SetString("filterID", model.Id?.ToString() ?? string.Empty);
            HttpContext.Session.SetString("filterNombre", model.Nombre ?? string.Empty);
            HttpContext.Session.SetString("filterDescripcion", model.Descripcion ?? string.Empty);
            HttpContext.Session.SetString("filterUsarSPs", model.UsarSPs.ToString());

            var modelRetorno = await CargarGridAsync("");
            return View("Index", modelRetorno);
        }

        private async Task<ExamenViewModel> CargarGridAsync(string mensaje)
        {
            ExamenViewModel model = new ExamenViewModel
            {
                Id = int.TryParse(HttpContext.Session.GetString("filterID"), out var elId) ? elId : null,
                Nombre = HttpContext.Session.GetString("filterNombre"),
                Descripcion = HttpContext.Session.GetString("filterDescripcion"),
                UsarSPs = bool.TryParse(HttpContext.Session.GetString("filterUsarSPs"), out var valor) ? valor : true,
                Mensaje = mensaje
            };


            var clsExamen = new ClsExamen(
               useSPs: model.UsarSPs,
               connectionString: _connectionString,
               apiUrl: _apiBaseUrl
            );
            var examenes = await clsExamen.ConsultarExamen(model.Id, model.Nombre, model.Descripcion, CancellationToken.None);
            model.ListaExamenes = examenes.ToList();

            return model;
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
