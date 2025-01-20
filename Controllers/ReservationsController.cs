using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Rest_ArleliotRoom.Data;
using WebAPI_Rest_ArleliotRoom.Models.DTOs;
using WebAPI_Rest_ArleliotRoom.Models.Entities;

namespace WebAPI_Rest_ArleliotRoom.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/reservations")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly WebAPI_Rest_ArleliotRoomContext _context;

        public ReservationsController(WebAPI_Rest_ArleliotRoomContext context)
        {
            _context = context;
        }

        // GET: api/Reservations -Récupère toutes les réservations.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations.ToListAsync();
        }

        // GET: api/Reservations/id - Recupère une réservation.
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // GET: api/Reservations/historique - Recupère une liste de réservations simplifiées (DTO) pour correspondre à la classe Reservation côté client
        [HttpGet("historique")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetHistorique([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Reservations
                .Include(r => r.Salle)  // Assure que la relation Salle est chargée
                .Include(r => r.Utilisateur) // Assure que la relation Utilisateur est chargée
                .Where(r => r.HeureDebut < DateTime.Now || r.HeureFin > DateTime.Now)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    Sujet = r.Sujet,
                    Date = r.Date,
                    HeureDebut = r.HeureDebut,
                    HeureFin = r.HeureFin,
                    SalleNom = r.Salle.NomSalle,
                    UtilisateurNom = r.Utilisateur.Nom
                });

            var totalRecords = await query.CountAsync();
            var reservations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { Message = "Les paramètres de pagination doivent être supérieurs à 0." });
            }
            Response.Headers.Add("X-Total-Count", totalRecords.ToString());
            return Ok(reservations);
        }


        // PUT: api/Reservations/id - Modifie une réservation existante.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            _context.Entry(reservation).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reservations.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Reservations - Crée une nouvelle réservation
        [HttpPost]
        public async Task<ActionResult<Reservation>> AddReservation(Reservation reservation)
        {
            //    _context.Reservations.Add(reservation);
            //    await _context.SaveChangesAsync();
            //    return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
            if (_context.Reservations.Any(r =>
                r.SalleId == reservation.SalleId &&
                r.HeureDebut < reservation.HeureFin &&
                reservation.HeureDebut < r.HeureFin))
            {
                return Conflict("La salle est déjà réservée pour ce créneau.");
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReservations), new { id = reservation.Id }, reservation);
        }

        // DELETE: api/Reservations/id - Supprime une réservation
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { Message = "La réservation demandée est introuvable." });
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
       
    }

}
