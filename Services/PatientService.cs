using Microsoft.EntityFrameworkCore;
using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;
using ClinicManager.Models;

namespace ClinicManager.Services;

/// <summary>
/// Implementacja serwisu pacjentów.
/// Wszystkie operacje na bazie asynchroniczne, z logowaniem błędów.
/// </summary>
public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PatientService> _logger;

    public PatientService(ApplicationDbContext db, ILogger<PatientService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Pobiera wszystkich aktywnych (nieusuniętych) pacjentów.
    /// Global query filter automatycznie wyklucza IsDeleted = true.
    /// </summary>
    public async Task<List<PatientDto>> GetAllAsync()
    {
        _logger.LogInformation("Pobieranie listy pacjentów");

        var patients = await _db.Patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();

        return patients.Select(PatientMapper.ToDto).ToList();
    }

    /// <summary>
    /// Pobiera pacjenta po ID. Zwraca null jeśli nie znaleziono.
    /// </summary>
    public async Task<PatientDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Pobieranie pacjenta {PatientId}", id);

        var patient = await _db.Patients.FindAsync(id);
        return patient is not null ? PatientMapper.ToDto(patient) : null;
    }

    /// <summary>
    /// Wyszukiwanie pacjentów po PESEL lub nazwisku.
    /// Jeśli oba parametry puste – zwraca pustą listę.
    /// </summary>
    public async Task<List<PatientListResponse>> SearchAsync(string? pesel, string? lastName)
    {
        _logger.LogInformation("Wyszukiwanie pacjentów – PESEL: {Pesel}, Nazwisko: {LastName}", pesel, lastName);

        var query = _db.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pesel))
        {
            query = query.Where(p => p.Pesel == pesel);
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            query = query.Where(p => p.LastName.Contains(lastName));
        }

        // Jeśli oba puste – zwracamy pustą listę (nie wszystkie)
        if (string.IsNullOrWhiteSpace(pesel) && string.IsNullOrWhiteSpace(lastName))
        {
            return [];
        }

        var patients = await query
            .OrderBy(p => p.LastName)
            .Take(50) // limit wyników
            .ToListAsync();

        return patients.Select(PatientMapper.ToListResponse).ToList();
    }

    /// <summary>
    /// Tworzy nowego pacjenta.
    /// </summary>
    public async Task<PatientDto> CreateAsync(CreatePatientRequest request)
    {
        _logger.LogInformation("Tworzenie pacjenta: {FirstName} {LastName}", request.FirstName, request.LastName);

        var patient = PatientMapper.ToEntity(request);
        patient.Id = Guid.NewGuid();

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        return PatientMapper.ToDto(patient);
    }

    /// <summary>
    /// Aktualizuje dane pacjenta. Rzuca wyjątek jeśli nie znaleziono.
    /// </summary>
    public async Task<PatientDto> UpdateAsync(Guid id, CreatePatientRequest request)
    {
        _logger.LogInformation("Aktualizacja pacjenta {PatientId}", id);

        var patient = await _db.Patients.FindAsync(id)
            ?? throw new KeyNotFoundException($"Pacjent o ID {id} nie został znaleziony");

        PatientMapper.UpdateEntity(request, patient);
        await _db.SaveChangesAsync();

        return PatientMapper.ToDto(patient);
    }

    /// <summary>
    /// Soft delete – ustawia IsDeleted = true. Nigdy nie usuwa twardo.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogWarning("Usuwanie (soft delete) pacjenta {PatientId}", id);

        var patient = await _db.Patients.FindAsync(id)
            ?? throw new KeyNotFoundException($"Pacjent o ID {id} nie został znaleziony");

        patient.IsDeleted = true;
        await _db.SaveChangesAsync();
    }
}
