using DSW_3P_PR03.Models;

namespace DSW_3P_PR03.Services;

public interface IPersonajeService
{
    List<Personaje> ObtenerTodos();
    Personaje? ObtenerPorId(int id);
    List<Personaje> ObtenerPorVideojuego(string nombre);
    List<Personaje> ObtenerPorNivelMinimo(int nivel);
    Personaje Agregar(Personaje personaje);
    bool Actualizar(int id, Personaje personaje);
    bool Eliminar(int id);
}