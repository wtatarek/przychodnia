using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.Visits;

[Authorize]
public class CancelModel : PageModel
{
    private readonly IVisitService _visitService;

    public VisitDto? Visit { get; set; }

    public CancelModel(IVisitService visitService)
    {
        _visitService = visitService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Visit = await _visitService.GetByIdAsync(id);
        if (Visit is null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        try
        {
            await _visitService.CancelAsync(id);
            return RedirectToPage("Index");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
