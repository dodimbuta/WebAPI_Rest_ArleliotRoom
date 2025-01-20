namespace WebAPI_Rest_ArleliotRoom.Models.Entities
{
    public class Salle
    {
        public int Id { get; set; }
        public required string NomSalle { get; set; }
        public int Capacite { get; set; }
        public required string Equipements { get; set; }
        public bool Statut { get; set; }
    }
}
