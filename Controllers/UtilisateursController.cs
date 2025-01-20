using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Rest_ArleliotRoom.Data;
using WebAPI_Rest_ArleliotRoom.Models.Entities;

namespace WebAPI_Rest_ArleliotRoom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilisateursController : ControllerBase
    {
        private readonly WebAPI_Rest_ArleliotRoomContext _context;

        public UtilisateursController(WebAPI_Rest_ArleliotRoomContext context)
        {
            _context = context;
        }

        // GET: api/Utilisateurs - Liste des utilisateurs
        [HttpGet]
        [Authorize(Roles = "Admin")] // Seul un admin peut voir la liste des utilisateur
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            return await _context.Utilisateurs.ToListAsync();
        }

        // GET: api/Utilisateurs/{id} - Récupère un utilisateur
        [HttpGet("{id}")]
        [Authorize] //Admin tout comme l'utilisateur lui-même
        public async Task<ActionResult<Utilisateur>> GetUtilisateur(int id)
        {
            var currentUserId = int.Parse(User.FindFirst("id")?.Value);
            if (id != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid("Vous n'êtes pas autorisé à consulter cet utilisateur.");
            }

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound("Utilisateur introuvable.");
            }

            return utilisateur;
        }

        // POST: api/Utilisateurs - Créer un nouvel utilisateur
        [HttpPost]
        [Authorize(Roles = "Admin")] // Seul un admin peut créer des utilisateur
        public async Task<ActionResult<Utilisateur>> PostUtilisateur([FromBody] Utilisateur utilisateur)
        {
            if (_context.Utilisateurs.Any(u => u.Email == utilisateur.Email))
            {
                return Conflict("Un utilisateur avec cet email existe déjà.");
            }

            utilisateur.Role = utilisateur.Role ?? "User";
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUtilisateur), new { id = utilisateur.Id }, utilisateur);
        }

        // PUT: api/Utilisateurs/{id} - Mettre à jour un utilisateur
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Seul un admin peut mettre à jour les infos  d'un utilisateur
        public async Task<IActionResult> PutUtilisateur(int id, [FromBody] Utilisateur utilisateur)
        {
            if (id != utilisateur.Id)
            {
                return BadRequest("L'ID fourni ne correspond pas.");
            }

            _context.Entry(utilisateur).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Utilisateurs.Any(e => e.Id == id))
                {
                    return NotFound("Utilisateur introuvable.");
                }
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Seul un admin peut deleter un utilisateur
        public async Task<IActionResult> DeleteUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound("Utilisateur introuvable.");
            }

            _context.Utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
