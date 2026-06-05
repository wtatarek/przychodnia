using ClinicManager.DTOs;

namespace ClinicManager.Services;

/// <summary>
/// Interfejs serwisu pacjentów – wszystkie operacje asynchroniczne.
/// </summary>
public interface IPatientService
{
    Task<List<PatientDto>> GetAllAsync();
    Task<PatientDto?> GetByIdAsync(Guid id);
    Task<List<PatientListResponse>> SearchAsync(string? pesel, string? lastName);
    Task<PatientDto> CreateAsync(CreatePatientRequest request);
    Task<PatientDto> UpdateAsync(Guid id, CreatePatientRequest request);
    Task DeleteAsync(Guid id); // soft delete – ustawia IsDeleted = true
}
