using DSW_3P_PR03.Data;
using DSW_3P_PR03.Models;
using Microsoft.EntityFrameworkCore;

namespace DSW_3P_PR03.Repositories;

public class PersonajeSqlRepository : IPersonajeRepository
{
    private readonly PersonajesDbContext _context;

    public PersonajeSqlRepository(PersonajesDbContext context)
    {
        _context = context;
    }

    public List<Personaje> ObtenerTodos()
        => _context.Personajes.AsNoTracking().ToList();

    public Personaje? ObtenerPorId(int id)
        => _context.Personajes.AsNoTracking()
            .FirstOrDefault(p => p.Id == id);

    public List<Personaje> ObtenerPorVideojuego(string nombre)
        => _context.Personajes.AsNoTracking()
            .Where(p => p.Videojuego == nombre)
            .ToList();

    public List<Personaje> ObtenerPorNivelMinimo(int nivel)
        => _context.Personajes.AsNoTracking()
            .Where(p => p.Nivel >= nivel)
            .ToList();

    public Personaje Agregar(Personaje personaje)
    {
        personaje.Id = 0;

        _context.Personajes.Add(personaje);
        _context.SaveChanges();

        return personaje;
    }

    public bool Actualizar(int id, Personaje personaje)
    {
        var actual = _context.Personajes.Find(id);

        if (actual is null)
            return false;

        actual.Nombre = personaje.Nombre;
        actual.Videojuego = personaje.Videojuego;
        actual.Clase = personaje.Clase;
        actual.Nivel = personaje.Nivel;
        actual.Poder = personaje.Poder;
        actual.Disponible = personaje.Disponible;
        actual.ImagenUrl = personaje.ImagenUrl;

        _context.SaveChanges();
        return true;
    }

    public bool Eliminar(int id)
    {
        var actual = _context.Personajes.Find(id);

        if (actual is null)
            return false;

        _context.Personajes.Remove(actual);
        _context.SaveChanges();

        return true;
    }
}