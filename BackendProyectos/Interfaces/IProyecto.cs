using BackendProyectos.Models;
using ContratoProyectos;

namespace BackendProyectos.Interfaces
{
    public interface IProyecto
    {
        List<Proyecto> ObtenerTodos();
        Proyecto? ObtenerPorId(Guid id);
        void Crear(ProyectoDTOs dto);
    }
}
