using BackendProyectos.Models;
using ContratoProyectos;

namespace BackendProyectos.Interfaces
{
    public interface IJefe
    {
        List<Jefe> ObtenerTodos();
        List<Jefe> ObtenerDisponibles();
        Jefe? ObtenerPorId(Guid id);
        void Crear(JefeDTOs dto);
    }
}
