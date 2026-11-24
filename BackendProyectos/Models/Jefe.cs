using System.Text.Json.Serialization;

namespace BackendProyectos.Models
{
    public class Jefe
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        [JsonIgnore]
        public Proyecto? Proyecto { get; set; }
    }
}
