using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.MedicalRecords;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IMedicalRecordService _recordService;

    public MedicalRecordDto? Record { get; set; }

    public DeleteModel(IMedicalRecordService recordService)
    {
        _recordService = recordService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Record = await _recordService.GetByIdAsync(id);
        if (Record is null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        try
        {
            var record = await _recordService.GetByIdAsync(id);
            await _recordService.DeleteAsync(id);
            return RedirectToPage("Index", new { patientId = record?.PatientId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
