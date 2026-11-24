using BackendProyectos.Data;
using BackendProyectos.Interfaces;
using BackendProyectos.Models;
using ContratoProyectos;

namespace BackendProyectos.Services
{
    public class JefeServices : IJefe
    {
        private readonly BaseDeDatos _context;

        public JefeServices(BaseDeDatos context)
        {
            _context = context;
        }

        public List<Jefe> ObtenerTodos()
        {
            return _context.Jefes.ToList();
        }

        public List<Jefe> ObtenerDisponibles()
        {
            var idsJefesOcupados = _context.Proyectos.Select(p => p.IdJefe).ToList();
            return _context.Jefes
                .Where(j => !idsJefesOcupados.Contains(j.Id))
                .ToList();
        }

        public Jefe? ObtenerPorId(Guid id)
        {
            return _context.Jefes.FirstOrDefault(j => j.Id == id);
        }

        public void Crear(JefeDTOs dto)
        {
            var nombreNormalizado = dto.Nombre.Trim().ToLower();

            if (_context.Jefes.Any(j => j.Nombre.ToLower() == nombreNormalizado))
            {
                throw new Exception("Ya existe un Jefe registrado con ese mismo nombre.");
            }

            var nuevoJefe = new Jefe
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre.Trim(),
                Cargo = dto.Cargo.Trim(),
                Correo = dto.Correo.Trim()
            };

            _context.Jefes.Add(nuevoJefe);
            _context.SaveChanges();
        }
    }
}
