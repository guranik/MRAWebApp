using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ReviewAggregatorWebApp.Model;

public partial class Db8428Context : DbContext
{
    public Db8428Context()
    {
    }

    public Db8428Context(DbContextOptions<Db8428Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Director> Directors { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string projectRoot = Directory.GetCurrentDirectory();
        while (projectRoot.Contains("bin"))
        {
            projectRoot = Directory.GetParent(projectRoot).FullName;
        }

        ConfigurationBuilder builder = new();

        builder.SetBasePath(projectRoot);

        builder.AddJsonFile("appsettings.json");

        IConfigurationRoot configuration = builder.AddUserSecrets<Program>().Build();

        string connectionString = "";


        string secretPass = configuration["RemoteDb:password"];
        string secretUser = configuration["RemoteDb:login"];
        SqlConnectionStringBuilder sqlConnectionStringBuilder = new(configuration.GetConnectionString("RemoteConnection"))
        {
            Password = secretPass,
            UserID = secretUser
        };

        connectionString = sqlConnectionStringBuilder.ConnectionString;

        _ = optionsBuilder
                        .UseSqlServer(connectionString)
                        .Options;
        optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
    } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasMany(d => d.Movies).WithMany(p => p.Countries)
                .UsingEntity<Dictionary<string, object>>(
                    "CountryMovie",
                    r => r.HasOne<Movie>().WithMany().HasForeignKey("MoviesId"),
                    l => l.HasOne<Country>().WithMany().HasForeignKey("CountriesId"),
                    j =>
                    {
                        j.HasKey("CountriesId", "MoviesId");
                        j.ToTable("CountryMovie");
                    });
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasMany(d => d.Movies).WithMany(p => p.Genres)
                .UsingEntity<Dictionary<string, object>>(
                    "GenreMovie",
                    r => r.HasOne<Movie>().WithMany().HasForeignKey("MoviesId"),
                    l => l.HasOne<Genre>().WithMany().HasForeignKey("GenresId"),
                    j =>
                    {
                        j.HasKey("GenresId", "MoviesId");
                        j.ToTable("GenreMovie");
                    });
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(e => e.Rating).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Director).WithMany(p => p.Movies).HasForeignKey(d => d.DirectorId);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasOne(d => d.Movie).WithMany(p => p.Reviews).HasForeignKey(d => d.MovieId);

            entity.HasOne(d => d.User).WithMany(p => p.Reviews).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
