using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchLineupRepository _matchLineupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly MatchValidationHelper _matchValidationHelper;
        private readonly ILogger<MatchLineupService> _logger;

        public MatchLineupService(
            IMatchLineupRepository matchLineupRepository,
            IMatchRepository matchRepository,
            MatchValidationHelper matchValidationHelper,
            ILogger<MatchLineupService> logger)
        {
            _matchLineupRepository = matchLineupRepository;
            _matchRepository = matchRepository;
            _matchValidationHelper = matchValidationHelper;
            _logger = logger;
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);

            // V1 - El partido debe existir
            if (match == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");
            }

            _logger.LogInformation(
                "Retrieving lineup for match ID: {MatchId}", matchId);

            return await _matchLineupRepository
                .GetByMatchAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);

            // V1 - El partido debe existir
            if (match == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");
            }

            _logger.LogInformation(
                "Retrieving lineup for team {TeamId} in match {MatchId}",
                teamId,
                matchId);

            return await _matchLineupRepository
                .GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task<MatchLineup> CreateAsync(MatchLineup lineup)
        {
            // V1 - El partido debe existir
            var match = await _matchRepository
                .GetByIdAsync(lineup.MatchId);

            if (match == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {lineup.MatchId}");
            }

            // V6 - El partido debe estar Scheduled
            if (match.Status != MatchStatus.Scheduled)
            {
                throw new InvalidOperationException(
                    "Solo se pueden registrar alineaciones en partidos Scheduled");
            }

            // V2 (El jugador debe de existir) + V3 (El jugador debe pertenecer al HomeTeam o AwayTeam del partido
            // Reutilizamos el helper que ya teníamos para validar esto

            var player = await _matchValidationHelper
                .ValidatePlayerInMatchAsync(lineup.PlayerId, match);

            // V4 - No duplicar jugador
            var exists = await _matchLineupRepository
                .ExistsByMatchAndPlayerAsync(
                    lineup.MatchId,
                    lineup.PlayerId);

            if (exists)
            {
                throw new InvalidOperationException(
                    "El jugador ya está registrado en la alineación de este partido");
            }

            // V5 - Máximo 11 titulares
            if (lineup.IsStarter) // Si es == true, es decir, si es titular, entra
            {
                var startersCount = await _matchLineupRepository
                    .CountStartersByMatchAndTeamAsync(
                        lineup.MatchId,
                        player.TeamId);

                if (startersCount >= 11)
                {
                    throw new InvalidOperationException(
                        "El equipo ya tiene 11 titulares registrados en este partido");
                }
            }

            _logger.LogInformation(
                "Adding player {PlayerId} to lineup for match {MatchId}",
                lineup.PlayerId,
                lineup.MatchId);

            return await _matchLineupRepository
                .CreateAsync(lineup);
        }

        public async Task DeleteAsync(int id)
        {
            var lineup = await _matchLineupRepository
                .GetByIdAsync(id);

            if (lineup == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró la alineación con ID {id}");
            }

            _logger.LogInformation(
                "Deleting lineup with ID: {LineupId}", id);

            await _matchLineupRepository
                .DeleteAsync(id);
        }
    }
}