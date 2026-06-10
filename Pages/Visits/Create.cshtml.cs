using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.Visits;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IVisitService _visitService;
    private readonly IPatientService _patientService;
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public CreateVisitRequest Input { get; set; } = new();

    public SelectList PatientList { get; set; } = new(Array.Empty<object>());
    public SelectList DoctorList { get; set; } = new(Array.Empty<object>());

    public CreateModel(
        IVisitService visitService,
        IPatientService patientService,
        UserManager<IdentityUser> userManager)
    {
        _visitService = visitService;
        _patientService = patientService;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        // Lista pacjentów do dropdowna (FirstName LastName)
        var patients = await _patientService.GetAllAsync();
        var patientItems = patients.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = $"{p.FirstName} {p.LastName} (PESEL: {p.Pesel})"
        }).ToList();
        PatientList = new SelectList(patientItems, "Value", "Text");

        // Lista lekarzy (z Identity) do dropdowna
        var doctors = await _userManager.GetUsersInRoleAsync("Lekarz");
        DoctorList = new SelectList(doctors, "Id", "Email");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(); // odśwież dropdowny
            return Page();
        }

        await _visitService.CreateAsync(Input);
        return RedirectToPage("Index");
    }
}
