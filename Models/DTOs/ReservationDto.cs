using WebAPI_Rest_ArleliotRoom.Models.Entities;

namespace WebAPI_Rest_ArleliotRoom.Models.DTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public string Sujet { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime HeureDebut { get; set; }
        public DateTime HeureFin { get; set; }
        public string SalleNom { get; set; } = string.Empty;
        public string UtilisateurNom { get; set; } = string.Empty;
    }
}
