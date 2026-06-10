using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.MedicalRecords;

[Authorize]
public class EditModel : PageModel
{
    private readonly IMedicalRecordService _recordService;

    [BindProperty]
    public CreateMedicalRecordRequest Input { get; set; } = new();

    public Guid RecordId { get; set; }
    public Guid PatientId { get; set; }

    public EditModel(IMedicalRecordService recordService)
    {
        _recordService = recordService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var record = await _recordService.GetByIdAsync(id);
        if (record is null) return NotFound();

        RecordId = id;
        PatientId = record.PatientId;
        Input = new CreateMedicalRecordRequest
        {
            PatientId = record.PatientId,
            Description = record.Description
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await _recordService.UpdateAsync(RecordId, Input);
            return RedirectToPage("Index", new { patientId = Input.PatientId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
