using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.Patients;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IPatientService _patientService;

    [BindProperty]
    public CreatePatientRequest Input { get; set; } = new();

    public CreateModel(IPatientService patientService)
    {
        _patientService = patientService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        await _patientService.CreateAsync(Input);
        return RedirectToPage("Index");
    }
}
