using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.Visits;

[Authorize]
public class EditModel : PageModel
{
    private readonly IVisitService _visitService;
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public CreateVisitRequest Input { get; set; } = new();

    public SelectList DoctorList { get; set; } = new(Array.Empty<object>());
    public string PatientName { get; set; } = "-";
    public string CurrentStatus { get; set; } = "-";
    public string? ErrorMessage { get; set; }

    public EditModel(IVisitService visitService, UserManager<IdentityUser> userManager)
    {
        _visitService = visitService;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var visit = await _visitService.GetByIdAsync(id);
        if (visit is null) return NotFound();

        PatientName = visit.PatientName;
        CurrentStatus = visit.Status;

        Input = new CreateVisitRequest
        {
            PatientId = visit.PatientId,
            AssignedDoctorId = visit.AssignedDoctorId,
            ScheduledAt = visit.ScheduledAt,
            Notes = visit.Notes
        };

        var doctors = await _userManager.GetUsersInRoleAsync("Lekarz");
        DoctorList = new SelectList(doctors, "Id", "Email", visit.AssignedDoctorId);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(id);
            return Page();
        }

        try
        {
            await _visitService.UpdateAsync(id, Input);
            return RedirectToPage("Index");
        }
        catch (KeyNotFoundException)
        {
            ErrorMessage = "Wizyta nie została znaleziona.";
            return Page();
        }
    }
}
