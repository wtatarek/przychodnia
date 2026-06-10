using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.Patients;

[Authorize]
public class EditModel : PageModel
{
    private readonly IPatientService _patientService;

    [BindProperty]
    public CreatePatientRequest Input { get; set; } = new();

    [FromRoute]
    public Guid Id { get; set; }

    public string? ErrorMessage { get; set; }

    public EditModel(IPatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null) return NotFound();

        Id = id;
        Input = new CreatePatientRequest
        {
            Pesel = patient.Pesel,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            InsuranceNumber = patient.InsuranceNumber
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await _patientService.UpdateAsync(Id, Input);
            return RedirectToPage("Index");
        }
        catch (KeyNotFoundException)
        {
            ErrorMessage = "Pacjent nie został znaleziony.";
            return Page();
        }
    }
}
