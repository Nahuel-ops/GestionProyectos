using BackendProyectos.Data;
using BackendProyectos.Models;
using Bogus;

namespace BackendProyectos.Factories
{
    public static class ProyectosFactories
    {
        public static void Seed(BaseDeDatos context)
        {
            Randomizer.Seed = new Random(2026);

            var cargos = new[]
            {
                "Gerente de Operaciones", "Líder Técnico", "Director de Innovación",
                "Coordinador de Sistemas", "Jefe de Infraestructura", "Arquitecto de Software"
            };

            var estados = new[] { "En Progreso", "Planificado", "En Revisión", "Finalizado" };

            // 1. Sembrar 10 Jefes (nombres únicos garantizados)
            if (!context.Jefes.Any())
            {
                var nombresUsados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var fakerJefe = new Faker("es");
                var listaJefes = new List<Jefe>();

                while (listaJefes.Count < 10)
                {
                    var nombreCompleto = fakerJefe.Name.FullName();
                    if (nombresUsados.Add(nombreCompleto))
                    {
                        listaJefes.Add(new Jefe
                        {
                            Id = Guid.NewGuid(),
                            Nombre = nombreCompleto,
                            Cargo = fakerJefe.PickRandom(cargos),
                            Correo = fakerJefe.Internet.Email(nombreCompleto)
                        });
                    }
                }

                context.Jefes.AddRange(listaJefes);
                context.SaveChanges();
            }

            // 2. Sembrar 5 Proyectos iniciales (1 jefe por proyecto, dejando 5 jefes libres para pruebas)
            if (!context.Proyectos.Any())
            {
                var jefesDisponibles = context.Jefes.Take(5).ToList();
                var fakerProyecto = new Faker("es");
                var listaProyectos = new List<Proyecto>();

                for (int i = 0; i < jefesDisponibles.Count; i++)
                {
                    listaProyectos.Add(new Proyecto
                    {
                        Id = Guid.NewGuid(),
                        Nombre = $"Proyecto {fakerProyecto.Company.CatchPhrase()} #{i + 1}",
                        Descripcion = fakerProyecto.Lorem.Sentence(10),
                        Estado = fakerProyecto.PickRandom(estados),
                        IdJefe = jefesDisponibles[i].Id
                    });
                }

                context.Proyectos.AddRange(listaProyectos);
                context.SaveChanges();
            }
        }
    }
}
