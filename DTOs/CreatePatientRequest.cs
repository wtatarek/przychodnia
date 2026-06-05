using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

/// <summary>
/// DTO do tworzenia nowego pacjenta. Walidacja przez DataAnnotations.
/// </summary>
public class CreatePatientRequest
{
    [Required(ErrorMessage = "PESEL jest wymagany")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "PESEL musi mieć dokładnie 11 znaków")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL musi składać się z 11 cyfr")]
    public string Pesel { get; set; } = null!;

    [Required(ErrorMessage = "Imię jest wymagane")]
    [StringLength(100, ErrorMessage = "Imię nie może przekraczać 100 znaków")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Nazwisko jest wymagane")]
    [StringLength(100, ErrorMessage = "Nazwisko nie może przekraczać 100 znaków")]
    public string LastName { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Numer ubezpieczenia nie może przekraczać 50 znaków")]
    public string? InsuranceNumber { get; set; }
}
