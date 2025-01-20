using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Rest_ArleliotRoom.Data;
using WebAPI_Rest_ArleliotRoom.Models.Entities;

namespace WebAPI_Rest_ArleliotRoom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SallesController : ControllerBase
    {
        private readonly WebAPI_Rest_ArleliotRoomContext _context;

        public SallesController(WebAPI_Rest_ArleliotRoomContext context)
        {
            _context = context;
        }

        // GET: api/Salles - Récupère et affiche la liste des salles disponible.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Salle>>> GetSalle()
        {
            return await _context.Salles.ToListAsync();
        }

        // GET: api/Salles/{id} - Récupère et affiche une salle
        [HttpGet("{id}")]
        public async Task<ActionResult<Salle>> GetSalle(int id)
        {
            var salle = await _context.Salles.FindAsync(id);
            if (salle == null)
            {
                return NotFound(new { Message = "Salle introuvable." });
            }
            return salle;
        }

        // POST: api/Salles - Ajoute une nouvelle salle.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Salle>> AddSalle([FromBody] Salle salle)
        {
            if (_context.Salles.Any(s => s.NomSalle == salle.NomSalle))
            {
                return Conflict("Une salle avec ce nom existe déjà.");
            }

            _context.Salles.Add(salle);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSalle), new { id = salle.Id }, salle);
        }

        // PUT: api/Salles/{id} - Met à jour une salle.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSalle(int id, [FromBody] Salle salle)
        {
            if (id != salle.Id)
            {
                return BadRequest("L'ID fourni ne correspond pas.");
            }

            if (_context.Salles.Any(s => s.NomSalle == salle.NomSalle && s.Id != id))
            {
                return Conflict("Une autre salle avec ce nom existe déjà.");
            }

            _context.Entry(salle).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Salles.Any(e => e.Id == id))
                {
                    return NotFound("Salle introuvable.");
                }
                throw;
            }
            return NoContent();
        }

        // DELETE: api/Salles/{id} - Supprime une salle
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSalle(int id)
        {
            var salle = await _context.Salles.FindAsync(id);
            if (salle == null)
            {
                return NotFound("Salle introuvable.");
            }

            _context.Salles.Remove(salle);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> AddSalle([FromBody] Salle salle)
        //{
        //    _context.Salles.Add(salle);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetSalle), new { id = salle.Id }, salle);

        //}
    }
}

