using Modelos;

namespace ClienteExamen.Models
{
    public class ExamenViewModel
    {
        public int? Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool UsarSPs { get; set; } = true;
        public string Codigo { get; set; }
        public string Mensaje { get; set; }

        public List<Examen> ListaExamenes { get; set; } = new();

    }
}
