using Microsoft.EntityFrameworkCore;
using api_proj.Models;

namespace api_proj.Data;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Users ----------
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Users_Role", "[Role] IN ('Organiser', 'Participant')"));
        });

        // ---------- Events ----------
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasOne(e => e.Organiser)
                  .WithMany(u => u.OrganisedEvents)
                  .HasForeignKey(e => e.OrganiserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Events_Type", "[EventType] IN ('Run', 'Walk', 'Cycle')"));
        });

        // ---------- Categories ----------
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasOne(c => c.Event)
                  .WithMany(e => e.Categories)
                  .HasForeignKey(c => c.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Routes ----------
        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasOne(r => r.Event)
                  .WithMany(e => e.Routes)
                  .HasForeignKey(r => r.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Enrolments ----------
        modelBuilder.Entity<Enrolment>(entity =>
        {
            entity.HasIndex(en => new { en.ParticipantId, en.EventId }).IsUnique();

            entity.HasOne(en => en.Participant)
                  .WithMany(u => u.Enrolments)
                  .HasForeignKey(en => en.ParticipantId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(en => en.Event)
                  .WithMany(e => e.Enrolments)
                  .HasForeignKey(en => en.EventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(en => en.Category)
                  .WithMany(c => c.Enrolments)
                  .HasForeignKey(en => en.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Enrolments_Status", "[EnrolmentStatus] IN ('Pending', 'Confirmed', 'Cancelled')"));
        });

        // ---------- Results ----------
        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasIndex(r => r.EnrolmentId).IsUnique();

            entity.HasOne(r => r.Enrolment)
                  .WithOne(en => en.Result)
                  .HasForeignKey<Result>(r => r.EnrolmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Results_Position", "[FinishPosition] > 0 AND [FinishPosition] <= [TotalFinishers]"));
        });
    }
}
