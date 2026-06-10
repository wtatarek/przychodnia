using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.MedicalRecords;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMedicalRecordService _recordService;
    private readonly IPatientService _patientService;
    private readonly ILogger<IndexModel> _logger;

    public List<MedicalRecordDto> Records { get; set; } = [];
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = "-";
    public string? ErrorMessage { get; set; }

    public IndexModel(
        IMedicalRecordService recordService,
        IPatientService patientService,
        ILogger<IndexModel> logger)
    {
        _recordService = recordService;
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(Guid patientId)
    {
        PatientId = patientId;
        var patient = await _patientService.GetByIdAsync(patientId);
        PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "-";

        try
        {
            Records = await _recordService.GetByPatientAsync(patientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd pobierania kartoteki");
            ErrorMessage = "Nie udało się pobrać kartoteki.";
        }

        return Page();
    }
}
