using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WsApiexamen.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WsApiexamen.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class wsApiExamen : ControllerBase
    {
        private readonly ILogger<wsApiExamen> _logger;
        private readonly BdiExamenContext _context;

        public wsApiExamen(BdiExamenContext context, ILogger<wsApiExamen> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("AgregarExamen")]
        public async Task<IActionResult> AgregarExamen(int id, string nombre, string descripcion)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var examen = new TblExamen { IdExamen= id, Nombre = nombre, Descripcion = descripcion };

                _context.TblExamen.Add(examen);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    Exito = true,
                    Mensaje = ""
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al insertar examen");

                return BadRequest(new
                {
                    Exito = false,
                    Mensaje = ex.Message + " InnerException:" + ex.InnerException?.Message
                });
            }
        }

        [HttpPut("ActualizarExamen")]
        public async Task<IActionResult> ActualizarExamen(int id, string nombre, string descripcion)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var examen = await _context.TblExamen.FindAsync(id);
                if (examen == null)
                {
                    return NotFound(new
                    {
                        Exito = false,
                        Mensaje = "Examen no encontrado"
                    });
                }
                examen.Nombre = nombre;
                examen.Descripcion = descripcion;
                _context.TblExamen.Update(examen);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new
                {
                    Exito = true,
                    Mensaje = ""
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar examen {id}", id);
                return BadRequest(new
                {
                    Exito = false,
                    Mensaje = ex.Message + " InnerException:" + ex.InnerException?.Message
                });
            }
        }
        [HttpDelete("EliminarExamen")]
        public async Task<IActionResult> EliminarExamen(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var examen = await _context.TblExamen.FindAsync(id);
                if (examen == null)
                {
                    return NotFound(new
                    {
                        Exito = false,
                        Mensaje = "Examen no encontrado"
                    });
                }
                _context.TblExamen.Remove(examen);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new
                {
                    Exito = true,
                    Mensaje = ""
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al eliminar examen {id}", id);
                return BadRequest(new
                {
                    Exito = false,
                    Mensaje = ex.Message + " InnerException:" + ex.InnerException?.Message
                });
            }
        }
        [HttpGet("ConsultarExamen")]
        public async Task<IActionResult> ConsultarExamen(int? id, string? nombre, string? descripcion)
        {
            try
            {
                var query = _context.TblExamen.AsQueryable();
                if (id.HasValue)
                {
                    query = query.Where(e => e.IdExamen == id.Value);
                }
                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(e => e.Nombre != null && e.Nombre.Contains(nombre));
                }
                if (!string.IsNullOrEmpty(descripcion))
                {
                    query = query.Where(e => e.Descripcion != null && e.Descripcion.Contains(descripcion));
                }
                var examenes = await query.ToListAsync();
                return Ok(examenes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener examenes.Id: {Id}, Nombre: {Nombre}, Descripción: {Descripcion}", id, nombre, descripcion);
                return BadRequest(new
                {
                    Exito = false,
                    Mensaje = ex.Message + " InnerException:" + ex.InnerException?.Message
                });
            }
        }
    }
}