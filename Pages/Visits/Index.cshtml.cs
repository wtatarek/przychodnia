using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.Visits;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IVisitService _visitService;
    private readonly ILogger<IndexModel> _logger;

    public List<VisitDto> Visits { get; set; } = [];
    public string? ErrorMessage { get; set; }

    public IndexModel(IVisitService visitService, ILogger<IndexModel> logger)
    {
        _visitService = visitService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Visits = await _visitService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd pobierania wizyt");
            ErrorMessage = "Nie udało się pobrać listy wizyt.";
        }
    }
}
