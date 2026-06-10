using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.Patients;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IPatientService _patientService;

    public PatientDto? Patient { get; set; }

    public DeleteModel(IPatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Patient = await _patientService.GetByIdAsync(id);
        if (Patient is null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        try
        {
            await _patientService.DeleteAsync(id);
            return RedirectToPage("Index");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
