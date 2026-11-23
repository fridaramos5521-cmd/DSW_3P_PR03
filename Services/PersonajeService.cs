using DSW_3P_PR03.Models;
using DSW_3P_PR03.Repositories;

namespace DSW_3P_PR03.Services;

public class PersonajeService : IPersonajeService
{
    private readonly IPersonajeRepository _repository;

    public PersonajeService(IPersonajeRepository repository)
    {
        _repository = repository;
    }

    private static void Validar(Personaje personaje)
    {
        if (string.IsNullOrWhiteSpace(personaje.Nombre))
            throw new ArgumentException("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(personaje.Videojuego))
            throw new ArgumentException("El videojuego es obligatorio.");

        if (string.IsNullOrWhiteSpace(personaje.Clase))
            throw new ArgumentException("La clase es obligatoria.");

        if (personaje.Nivel < 1 || personaje.Nivel > 100)
            throw new ArgumentException("El nivel debe estar entre 1 y 100.");

        if (personaje.Poder <= 0)
            throw new ArgumentException("El poder debe ser mayor que cero.");

        if (!Uri.TryCreate(personaje.ImagenUrl, UriKind.Absolute, out var url)
            || (url.Scheme != Uri.UriSchemeHttp
                && url.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException(
                "ImagenUrl debe ser una URL absoluta HTTP o HTTPS."
            );
        }
    }

    public List<Personaje> ObtenerTodos()
        => _repository.ObtenerTodos();

    public Personaje? ObtenerPorId(int id)
        => _repository.ObtenerPorId(id);

    public List<Personaje> ObtenerPorVideojuego(string nombre)
        => _repository.ObtenerPorVideojuego(nombre);

    public List<Personaje> ObtenerPorNivelMinimo(int nivel)
        => _repository.ObtenerPorNivelMinimo(nivel);

    public Personaje Agregar(Personaje personaje)
    {
        Validar(personaje);

        personaje.Id = 0;
        personaje.Disponible = true;

        return _repository.Agregar(personaje);
    }

    public bool Actualizar(int id, Personaje personaje)
    {
        if (_repository.ObtenerPorId(id) is null)
            return false;

        Validar(personaje);
        personaje.Id = id;

        return _repository.Actualizar(id, personaje);
    }

    public bool Eliminar(int id)
        => _repository.Eliminar(id);
}