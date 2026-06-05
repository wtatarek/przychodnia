using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicManager.Models;

namespace ClinicManager.Data.Configurations;

/// <summary>
/// Konfiguracja EF Core dla encji Patient.
/// Indeks nieklastrowany na PESEL, global query filter dla soft-delete.
/// </summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // Tabela
        builder.ToTable("Patients");

        // PK
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        // PESEL – 11 znaków, wymagany, indeks nieklastrowany (zadanie dodatkowe: indeksy)
        builder.Property(p => p.Pesel).HasMaxLength(11).IsRequired();
        builder.HasIndex(p => p.Pesel)
            .IsUnique()
            .HasDatabaseName("IX_Patients_Pesel")
            .IsClustered(false); // non-clustered index – zgodnie z wymaganiami README

        // FirstName
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();

        // LastName
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();

        // InsuranceNumber – opcjonalny
        builder.Property(p => p.InsuranceNumber).HasMaxLength(50);

        // Soft delete – global query filter (automatyczne filtrowanie IsDeleted = false)
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
