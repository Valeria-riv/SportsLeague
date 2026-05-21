using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface IMatchLineupRepository: IGenericRepository<MatchLineup>
    {
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId); // Obtiene la alineación completa del partido
        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId); // Obtiene la alineación de un equipo específico
        Task<bool> ExistsByMatchAndPlayerAsync(int matchId, int playerId); // Verifica si un jugador está registrado en un partido
        Task<int> CountStartersByMatchAndTeamAsync(int matchId, int teamId); // Cuenta titulares de un equipo en un partido
    }
}