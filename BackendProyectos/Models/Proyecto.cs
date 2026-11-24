using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProyectos.Models
{
    public class Proyecto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        public Guid IdJefe { get; set; }
        [ForeignKey("IdJefe")]
        public Jefe? Jefe { get; set; }
    }
}
