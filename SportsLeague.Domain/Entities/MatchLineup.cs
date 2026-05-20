namespace SportsLeague.Domain.Entities
{
    public class MatchLineup : AuditBase
    {
        public int MatchId { get; set; } // FK to Match
        public int PlayerId { get; set; } // FK to Player
        public bool IsStarter { get; set; } // true = Titular, false = Suplente 
        public string Position { get; set; } = string.Empty;

        // Navigation Properties
        public Match Match { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
