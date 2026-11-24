using Bogus;
using Contratos.Data;
using Contratos.Models;

namespace Contratos.Factories
{
    /// <summary>
    /// Datos de ejemplo: 10 jefes y 10 proyectos (un jefe por proyecto).
    /// </summary>
    static class DatosFactories
    {
        private const int CantidadJefes = 10;
        private const int CantidadProyectos = 10;

        private static readonly string[] Cargos =
        {
            "Gerente de Proyectos", "Director de Operaciones", "Jefe de Equipo",
            "Coordinador General", "Líder de Desarrollo"
        };

        private static readonly string[] Estados = { "Activo", "En curso", "Finalizado", "Pausado" };

        public static void Seed(BaseDatos context)
        {
            var faker = new Faker("es");

            if (!context.Jefes.Any())
            {
                var nombres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var jefes = new List<Jefe>();

                for (var i = 0; i < CantidadJefes; i++)
                {
                    string nombre;
                    do
                    {
                        nombre = $"{faker.Name.FirstName()} {faker.Name.LastName()}";
                    }
                    while (!nombres.Add(nombre));

                    jefes.Add(new Jefe
                    {
                        Id = Guid.NewGuid(),
                        Nombre = nombre,
                        Cargo = faker.PickRandom(Cargos),
                        Correo = faker.Internet.Email()
                    });
                }

                context.Jefes.AddRange(jefes);
                context.SaveChanges();
            }

            if (!context.Proyectos.Any())
            {
                var jefes = context.Jefes.Take(CantidadProyectos).ToList();
                var nombres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var proyectos = new List<Proyecto>();

                for (var i = 0; i < jefes.Count; i++)
                {
                    string nombre;
                    do
                    {
                        nombre = $"Proyecto {faker.Company.CompanyName()}";
                    }
                    while (!nombres.Add(nombre));

                    proyectos.Add(new Proyecto
                    {
                        Id = Guid.NewGuid(),
                        Nombre = nombre,
                        Descripcion = faker.Lorem.Sentence(8),
                        Estado = faker.PickRandom(Estados),
                        IdJefe = jefes[i].Id
                    });
                }

                context.Proyectos.AddRange(proyectos);
                context.SaveChanges();
            }
        }
    }
}
