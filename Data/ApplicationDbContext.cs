using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using divelog.Models;

namespace divelog.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    //Tabeller
    public DbSet<Dive> Dives { get; set; }
    public DbSet<DiveParticipant> DiveParticipants { get; set; }
    public DbSet<DivePurpose> DivePurposes { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<DiveRole> DiveRoles { get; set; } = null!;
    public DbSet<PersonRole> PersonRoles { get; set; }
    public DbSet<Group> Groups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Relation mellan Person och Group
        modelBuilder.Entity<Person>()
            .HasOne(p => p.Group)
            .WithMany(g => g.Persons)
            .HasForeignKey(p => p.GroupId)
            //Man kan inte radera en grupp som innehåller personer
            .OnDelete(DeleteBehavior.Restrict);

        //Relation mellan Person och PersonRoll
        modelBuilder.Entity<PersonRole>()
            .HasOne(pr => pr.Person)
            .WithMany(p => p.PersonRoles)
            .HasForeignKey(pr => pr.PersonId)
            //Om man tar bort en person så försvinner personen och alla personroller kopplade till personen
            .OnDelete(DeleteBehavior.Cascade);

        //Roll som länkas till person måste vara unik
        modelBuilder.Entity<PersonRole>()
            .HasIndex(pr => new { pr.PersonId, pr.DiveRoleId })
            .IsUnique();

        //Relation mellan DiveRole och PersonRole
        modelBuilder.Entity<PersonRole>()
            .HasOne(pr => pr.DiveRole)
            .WithMany(r => r.PersonRoles)
            .HasForeignKey(pr => pr.DiveRoleId)
            //Om man tar bort en dykroll tas alla koppingar till personer bort
            .OnDelete(DeleteBehavior.Cascade);

        //Relation mellan Dive och DiveParticipant
        modelBuilder.Entity<DiveParticipant>()
            .HasOne(dp => dp.Dive)
            .WithMany(d => d.DiveParticipants)
            .HasForeignKey(dp => dp.DiveId)
            //Om man tar bort ett dyk tas alla deltagare bort
            .OnDelete(DeleteBehavior.Cascade);

        //Relation mellan Person och DiveParticipant
        modelBuilder.Entity<DiveParticipant>()
            .HasOne(dp => dp.Person)
            .WithMany()
            .HasForeignKey(dp => dp.PersonId)
            //Man kan inte radera en person som finns i gamla dyk
            .OnDelete(DeleteBehavior.Restrict);

        //Relation mellan DiveRole och DiveParticipant
        modelBuilder.Entity<DiveParticipant>()
            .HasOne(dp => dp.DiveRole)
            .WithMany()
            .HasForeignKey(dp => dp.DiveRoleId)
            //Man kan inte ta bort en roll som en person har
            .OnDelete(DeleteBehavior.Restrict);

        //Samma person får inte finnas två gånger i samma dyk
        modelBuilder.Entity<DiveParticipant>()
            .HasIndex(dp => new { dp.DiveId, dp.PersonId })
            .IsUnique();

        //Index för filtrering och statistik
        modelBuilder.Entity<Dive>()
            .HasIndex(d => d.Date);

        modelBuilder.Entity<DiveParticipant>()
            .HasIndex(dp => dp.DiveRoleId);

        //Seed-data
        //Skapar fyra standardgrupper som alltid finns i databasen
        modelBuilder.Entity<Group>().HasData(
            new Group { Id = 1, Name = "Grupp 1" },
            new Group { Id = 2, Name = "Grupp 2" },
            new Group { Id = 3, Name = "Grupp 3" },
            new Group { Id = 4, Name = "Grupp 4" }
        );

        //Seed-data
        //Skapar tre standardroller som alltid finns i databasen
        modelBuilder.Entity<DiveRole>().HasData(
            new DiveRole { Id = 1, Name = "Dykare" },
            new DiveRole { Id = 2, Name = "Dykarledare" },
            new DiveRole { Id = 3, Name = "Dykarskötare" }
        );

        //Seed-data
        //Skapar syften som alltid finns i databasen
        modelBuilder.Entity<DivePurpose>().HasData(
            new DivePurpose { Id = 1, Name = "Övning" },
            new DivePurpose { Id = 2, Name = "Larm" },
            new DivePurpose { Id = 3, Name = "Djupdyk" },
            new DivePurpose { Id = 4, Name = "Slutet rum" },
            new DivePurpose { Id = 5, Name = "Haveribana" }
        );
    }
}

