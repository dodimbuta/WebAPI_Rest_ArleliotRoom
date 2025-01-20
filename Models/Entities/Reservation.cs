namespace WebAPI_Rest_ArleliotRoom.Models.Entities
{
    public class Reservation
    {

        public int Id { get; set; }  // Identifiant unique
        public int SalleId { get; set; }  // Clé étrangère vers Salle
        public required Salle Salle { get; set; }  // Relation avec Salle
        public int UtilisateurId { get; set; }  // Clé étrangère vers Utilisateur
        public required Utilisateur Utilisateur { get; set; }  // Relation avec Utilisateur
        public string Sujet { get; set; } = string.Empty;  // Sujet ou objet de la réunion
        public DateTime Date { get; set; }
        public DateTime HeureDebut { get; set; }  // Heure de début
        public DateTime HeureFin { get; set; }  // Heure de fin        

    }
}
