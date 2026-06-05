namespace ClinicManager.Models;

/// <summary>
/// Encja pacjenta. Soft delete przez IsDeleted.
/// </summary>
public class Patient
{
    public Guid Id { get; set; }

    // PESEL – 11 cyfr, często filtrowany → indeks nieklastrowany w konfiguracji
    public string Pesel { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    // Numer ubezpieczenia (opcjonalny)
    public string? InsuranceNumber { get; set; }

    // Soft delete – nigdy nie usuwamy twardo (RODO)
    public bool IsDeleted { get; set; } = false;

    // Nawigacje (dodane później przy kolejnych modułach)
    // public ICollection<MedicalRecord> MedicalRecords { get; set; }
    // public ICollection<Visit> Visits { get; set; }
}
