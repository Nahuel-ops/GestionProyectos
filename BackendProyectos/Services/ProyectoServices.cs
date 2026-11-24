using BackendProyectos.Data;
using BackendProyectos.Interfaces;
using BackendProyectos.Models;
using ContratoProyectos;
using Microsoft.EntityFrameworkCore;

namespace BackendProyectos.Services
{
    public class ProyectoServices : IProyecto
    {
        private readonly BaseDeDatos _context;

        public ProyectoServices(BaseDeDatos context)
        {
            _context = context;
        }

        public List<Proyecto> ObtenerTodos()
        {
            return _context.Proyectos
                .Include(p => p.Jefe)
                .ToList();
        }

        public Proyecto? ObtenerPorId(Guid id)
        {
            return _context.Proyectos
                .Include(p => p.Jefe)
                .FirstOrDefault(p => p.Id == id);
        }

        public void Crear(ProyectoDTOs dto)
        {
            var nombreNormalizado = dto.Nombre.Trim().ToLower();

            // Regla 2: Proyectos no pueden tener el mismo nombre
            if (_context.Proyectos.Any(p => p.Nombre.ToLower() == nombreNormalizado))
            {
                throw new Exception("Ya existe un Proyecto registrado con ese mismo nombre.");
            }

            // Verificar que el Jefe exista
            if (!_context.Jefes.Any(j => j.Id == dto.IdJefe))
            {
                throw new Exception("El Jefe seleccionado no existe.");
            }

            // Regla 3: Un Jefe solo puede estar a cargo de 1 proyecto
            if (_context.Proyectos.Any(p => p.IdJefe == dto.IdJefe))
            {
                throw new Exception("Este Jefe ya está a cargo de otro proyecto.");
            }

            var nuevoProyecto = new Proyecto
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion.Trim(),
                Estado = dto.Estado.Trim(),
                IdJefe = dto.IdJefe
            };

            _context.Proyectos.Add(nuevoProyecto);
            _context.SaveChanges();
        }
    }
}
