namespace WebAPI_Rest_ArleliotRoom.Models.DTOs
{
    public class Login
    {
        public string Email { get; set; } = string.Empty;  // Email pour l'identification
        public string MotDePasse { get; set; } = string.Empty;  // Mot de passe
    }
}
