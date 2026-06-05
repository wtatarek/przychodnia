namespace ClinicManager.DTOs;

/// <summary>
/// DTO dla listy pacjentów (widok uproszczony, np. wyniki wyszukiwania).
/// </summary>
public class PatientListResponse
{
    public Guid Id { get; set; }
    public string Pesel { get; set; } = null!;
    public string FullName { get; set; } = null!; // "FirstName LastName"
}
