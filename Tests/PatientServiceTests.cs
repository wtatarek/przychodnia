using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Models;
using ClinicManager.Services;

namespace ClinicManager.Tests;

public class PatientServiceTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly PatientService _service;

    public PatientServiceTests()
    {
        // Baza InMemory – każdy test dostaje czystą bazę
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);

        var logger = new Mock<ILogger<PatientService>>();
        _service = new PatientService(_db, logger.Object);
    }

    [Fact]
    public async Task CreateAsync_DodajePacjenta_IZwracaDto()
    {
        // Arrange
        var request = new CreatePatientRequest
        {
            Pesel = "12345678901",
            FirstName = "Jan",
            LastName = "Kowalski"
        };

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("12345678901", result.Pesel);
        Assert.Equal("Jan", result.FirstName);
        Assert.Equal("Kowalski", result.LastName);
    }

    [Fact]
    public async Task GetAllAsync_ZwracaPacjentowPosortowanych()
    {
        // Arrange
        await _service.CreateAsync(new CreatePatientRequest { Pesel = "22222222222", FirstName = "Anna", LastName = "Zielińska" });
        await _service.CreateAsync(new CreatePatientRequest { Pesel = "11111111111", FirstName = "Bartosz", LastName = "Adamski" });

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Adamski", result[0].LastName); // sortowanie po LastName
        Assert.Equal("Zielińska", result[1].LastName);
    }

    [Fact]
    public async Task SearchAsync_PoPesel_ZwracaPacjenta()
    {
        // Arrange
        await _service.CreateAsync(new CreatePatientRequest { Pesel = "99999999999", FirstName = "Test", LastName = "Testowy" });

        // Act
        var result = await _service.SearchAsync("99999999999", null);

        // Assert
        Assert.Single(result);
        Assert.Equal("99999999999", result[0].Pesel);
    }

    [Fact]
    public async Task DeleteAsync_SoftDelete_UstawiaIsDeleted()
    {
        // Arrange
        var created = await _service.CreateAsync(new CreatePatientRequest
        {
            Pesel = "88888888888",
            FirstName = "Do",
            LastName = "Usunięcia"
        });

        // Act
        await _service.DeleteAsync(created.Id);

        // Assert – po soft delete GetAllAsync nie zwraca usuniętego
        var all = await _service.GetAllAsync();
        Assert.DoesNotContain(all, p => p.Id == created.Id);
    }

    [Fact]
    public async Task UpdateAsync_AktualizujeDanePacjenta()
    {
        // Arrange
        var created = await _service.CreateAsync(new CreatePatientRequest
        {
            Pesel = "77777777777",
            FirstName = "Stare",
            LastName = "Imię"
        });

        var update = new CreatePatientRequest
        {
            Pesel = "77777777777",
            FirstName = "Nowe",
            LastName = "Imię"
        };

        // Act
        var updated = await _service.UpdateAsync(created.Id, update);

        // Assert
        Assert.Equal("Nowe", updated.FirstName);
        Assert.Equal("Imię", updated.LastName);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
