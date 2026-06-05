using Riok.Mapperly.Abstractions;
using ClinicManager.DTOs;
using ClinicManager.Models;

namespace ClinicManager.Mappers;

/// <summary>
/// Mapperly – automatyczne mapowanie Patient ↔ DTO.
/// Nigdy nie mapujemy ręcznie.
/// </summary>
[Mapper]
public static partial class PatientMapper
{
    // Patient → PatientDto (pomijamy IsDeleted – to pole wewnętrzne)
    [MapperIgnoreSource(nameof(Patient.IsDeleted))]
    public static partial PatientDto ToDto(Patient patient);

    // Patient → PatientListResponse (uproszczony widok: Pesel + FullName)
    [MapProperty(nameof(Patient.FirstName), nameof(PatientListResponse.FullName))]
    [MapperIgnoreSource(nameof(Patient.LastName))]
    [MapperIgnoreSource(nameof(Patient.InsuranceNumber))]
    [MapperIgnoreSource(nameof(Patient.IsDeleted))]
    public static partial PatientListResponse ToListResponse(Patient patient);

    // CreatePatientRequest → Patient (Id nadajemy ręcznie, IsDeleted = false domyślnie)
    [MapperIgnoreTarget(nameof(Patient.Id))]
    [MapperIgnoreTarget(nameof(Patient.IsDeleted))]
    public static partial Patient ToEntity(CreatePatientRequest request);

    // Aktualizacja istniejącej encji z DTO
    [MapperIgnoreTarget(nameof(Patient.Id))]
    [MapperIgnoreTarget(nameof(Patient.IsDeleted))]
    public static partial void UpdateEntity(CreatePatientRequest request, Patient patient);
}
