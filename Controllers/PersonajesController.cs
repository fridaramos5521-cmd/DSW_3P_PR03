using DSW_3P_PR03.Models;
using DSW_3P_PR03.Services;
using Microsoft.AspNetCore.Mvc;

namespace DSW_3P_PR03.Controllers;

[ApiController]
[Route("api/personajes")]
public class PersonajesController : ControllerBase
{
    private readonly IPersonajeService _service;

    public PersonajesController(IPersonajeService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
        => Ok(_service.ObtenerTodos());

    [HttpGet("{id:int}")]
    public IActionResult ObtenerPorId(int id)
    {
        var personaje = _service.ObtenerPorId(id);

        return personaje is null
            ? NotFound(new { mensaje = "El personaje no existe." })
            : Ok(personaje);
    }

    [HttpGet("videojuego/{nombre}")]
    public IActionResult ObtenerPorVideojuego(string nombre)
        => Ok(_service.ObtenerPorVideojuego(nombre));

    [HttpGet("nivel/{nivel:int}")]
    public IActionResult ObtenerPorNivelMinimo(int nivel)
    {
        if (nivel < 1 || nivel > 100)
            return BadRequest(new
            {
                mensaje = "El nivel debe estar entre 1 y 100."
            });

        return Ok(_service.ObtenerPorNivelMinimo(nivel));
    }

    [HttpPost]
    public IActionResult Agregar([FromBody] Personaje personaje)
    {
        try
        {
            var creado = _service.Agregar(personaje);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = creado.Id },
                creado
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Actualizar(
        int id,
        [FromBody] Personaje personaje)
    {
        try
        {
            return _service.Actualizar(id, personaje)
                ? NoContent()
                : NotFound(new { mensaje = "El personaje no existe." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        return _service.Eliminar(id)
            ? NoContent()
            : NotFound(new { mensaje = "El personaje no existe." });
    }
}