namespace ClinicManager.DTOs;

/// <summary>
/// DTO zwracany przy GET (pełne dane pacjenta).
/// </summary>
public class PatientDto
{
    public Guid Id { get; set; }
    public string Pesel { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? InsuranceNumber { get; set; }
}
