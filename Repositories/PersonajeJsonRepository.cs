using System.Text.Json;
using DSW_3P_PR03.Models;

namespace DSW_3P_PR03.Repositories;

public class PersonajeJsonRepository : IPersonajeRepository
{
    private static readonly object Candado = new();

    private readonly string _ruta;

    private readonly JsonSerializerOptions _opciones = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public PersonajeJsonRepository(IWebHostEnvironment environment)
    {
        _ruta = Path.Combine(
            environment.ContentRootPath,
            "Data",
            "Personajes.json"
        );

        Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);

        if (!File.Exists(_ruta))
            File.WriteAllText(_ruta, "[]");
    }

    private List<Personaje> Leer()
    {
        string json = File.ReadAllText(_ruta);

        return JsonSerializer.Deserialize<List<Personaje>>(json, _opciones)
               ?? new List<Personaje>();
    }

    private void Guardar(List<Personaje> personajes)
    {
        string json = JsonSerializer.Serialize(personajes, _opciones);
        File.WriteAllText(_ruta, json);
    }

    public List<Personaje> ObtenerTodos()
    {
        lock (Candado)
            return Leer();
    }

    public Personaje? ObtenerPorId(int id)
    {
        lock (Candado)
            return Leer().FirstOrDefault(p => p.Id == id);
    }

    public List<Personaje> ObtenerPorVideojuego(string nombre)
    {
        lock (Candado)
            return Leer()
                .Where(p => p.Videojuego.Equals(
                    nombre,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
    }

    public List<Personaje> ObtenerPorNivelMinimo(int nivel)
    {
        lock (Candado)
            return Leer().Where(p => p.Nivel >= nivel).ToList();
    }

    public Personaje Agregar(Personaje personaje)
    {
        lock (Candado)
        {
            var personajes = Leer();

            personaje.Id = personajes.Count == 0
                ? 1
                : personajes.Max(p => p.Id) + 1;

            personajes.Add(personaje);
            Guardar(personajes);

            return personaje;
        }
    }

    public bool Actualizar(int id, Personaje personaje)
    {
        lock (Candado)
        {
            var personajes = Leer();
            var actual = personajes.FirstOrDefault(p => p.Id == id);

            if (actual is null)
                return false;

            actual.Nombre = personaje.Nombre;
            actual.Videojuego = personaje.Videojuego;
            actual.Clase = personaje.Clase;
            actual.Nivel = personaje.Nivel;
            actual.Poder = personaje.Poder;
            actual.Disponible = personaje.Disponible;
            actual.ImagenUrl = personaje.ImagenUrl;

            Guardar(personajes);
            return true;
        }
    }

    public bool Eliminar(int id)
    {
        lock (Candado)
        {
            var personajes = Leer();
            var actual = personajes.FirstOrDefault(p => p.Id == id);

            if (actual is null)
                return false;

            personajes.Remove(actual);
            Guardar(personajes);

            return true;
        }
    }
}