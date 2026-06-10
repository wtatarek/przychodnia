using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicManager.DTOs;
using ClinicManager.Services;

namespace ClinicManager.Pages.MedicalRecords;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IMedicalRecordService _recordService;
    private readonly IPatientService _patientService;
    private readonly IWebHostEnvironment _env;

    [BindProperty]
    public CreateMedicalRecordRequest Input { get; set; } = new();

    [BindProperty]
    public Guid PatientId { get; set; }

    public string PatientName { get; set; } = "-";

    // Plik z formularza
    public IFormFile? UploadedFile { get; set; }

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"
    };

    public CreateModel(IMedicalRecordService recordService, IPatientService patientService, IWebHostEnvironment env)
    {
        _recordService = recordService;
        _patientService = patientService;
        _env = env;
    }

    public async Task<IActionResult> OnGetAsync(Guid patientId)
    {
        PatientId = patientId;
        Input.PatientId = patientId;
        var patient = await _patientService.GetByIdAsync(patientId);
        PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "-";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        string? fileUrl = null;

        if (UploadedFile is not null && UploadedFile.Length > 0)
        {
            var ext = Path.GetExtension(UploadedFile.FileName);
            if (!AllowedExtensions.Contains(ext))
            {
                ModelState.AddModelError("", $"Niedozwolony format: {ext}");
                return Page();
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await UploadedFile.CopyToAsync(stream);

            fileUrl = $"/uploads/{fileName}";
        }

        Input.PatientId = PatientId;
        await _recordService.CreateAsync(Input, fileUrl);
        return RedirectToPage("Index", new { patientId = PatientId });
    }
}
