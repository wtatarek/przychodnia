using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.Patients;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<IndexModel> _logger;

    public List<PatientDto> Patients { get; set; } = [];
    public string? Pesel { get; set; }
    public string? LastName { get; set; }
    public string? ErrorMessage { get; set; }

    public IndexModel(IPatientService patientService, ILogger<IndexModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task OnGetAsync(string? pesel, string? lastName)
    {
        Pesel = pesel;
        LastName = lastName;

        try
        {
            if (!string.IsNullOrWhiteSpace(pesel) || !string.IsNullOrWhiteSpace(lastName))
            {
                var results = await _patientService.SearchAsync(pesel, lastName);
                // Konwersja: PatientListResponse → PatientDto (tylko do wyświetlania)
                Patients = results.Select(r => new PatientDto
                {
                    Id = r.Id,
                    Pesel = r.Pesel,
                    FirstName = r.FullName.Split(' ')[0],
                    LastName = r.FullName.Split(' ').Length > 1 ? r.FullName.Split(' ')[1] : ""
                }).ToList();
            }
            else
            {
                Patients = await _patientService.GetAllAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd pobierania pacjentów");
            ErrorMessage = "Nie udało się pobrać listy pacjentów.";
        }
    }
}
