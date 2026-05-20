namespace SportsLeague.Domain.Entities
{
    public class TournamentSponsor : AuditBase
    {
        public int TournamentId { get; set; } // FK to Tournament
        public int SponsorId { get; set; } // FK to Sponsor
        public decimal ContractAmount { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navegation Properties
        public Tournament Tournament { get; set; } = null!;
        public Sponsor Sponsor { get; set; } = null!;

    }
}
