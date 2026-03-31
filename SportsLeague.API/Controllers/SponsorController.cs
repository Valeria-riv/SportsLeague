using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;
    private readonly ILogger<SponsorController> _logger;

    public SponsorController(
        ISponsorService sponsorService,
        IMapper mapper,
        ILogger<SponsorController> logger)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        var sponsorsDto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
        return Ok(sponsorsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);
        if (sponsor == null)
            return NotFound(new { message = $"Sponsor with ID {id} not found" });

        var sponsorDto = _mapper.Map<SponsorResponseDTO>(sponsor);
        return Ok(sponsorDto);
    }

    [HttpPost]
    public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            var created = await _sponsorService.CreateAsync(sponsor);

            var sponsorWithData = await _sponsorService.GetByIdAsync(created.Id);
            var responseDto = _mapper.Map<SponsorResponseDTO>(sponsorWithData);

            return CreatedAtAction(
                nameof(GetById),
                new { id = responseDto.Id },
                responseDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            await _sponsorService.UpdateAsync(id, sponsor);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/tournaments")]
    public async Task<ActionResult<TournamentSponsorResponseDTO>> LinkToTournament(int id, TournamentSponsorRequestDTO dto)
    {
        try
        {
            await _sponsorService.LinkToTournamentAsync(id, dto.TournamentId, dto.ContractAmount);

            var tsList = await _sponsorService.GetTournamentsBySponsorAsync(id);
            var justCreated = tsList.OrderByDescending(x => x.JoinedAt).First();

            var responseDto = _mapper.Map<TournamentSponsorResponseDTO>(justCreated);

            return Created("", responseDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetTournaments(int id)
    {
        try
        {
            var list = await _sponsorService.GetTournamentsBySponsorAsync(id);
            var listDto = _mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(list);
            return Ok(listDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}/tournaments/{tournamentId}")]
    public async Task<ActionResult> UnlinkFromTournament(int id, int tournamentId)
    {
        try
        {
            await _sponsorService.UnlinkFromTournamentAsync(id, tournamentId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}