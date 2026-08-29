using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChorusAttendanceWeb.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Piece> Pieces => Set<Piece>();
        public DbSet<PiecePartAssignment> PiecePartAssignments => Set<PiecePartAssignment>();
        public DbSet<Practice> Practices => Set<Practice>();
        public DbSet<PracticePiece> PracticePieces => Set<PracticePiece>();
        public DbSet<MemberPiecePart> MemberPieceParts => Set<MemberPiecePart>();
        public DbSet<Attendance> Attendances => Set<Attendance>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.Part).HasConversion<string>().HasMaxLength(20);
            });

            builder.Entity<Piece>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Title).IsRequired().HasMaxLength(200);
                b.HasMany(p => p.PartAssignments)
                    .WithOne(a => a.Piece)
                    .HasForeignKey(a => a.PieceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PiecePartAssignment>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.Part).HasConversion<string>().HasMaxLength(20);
                b.Property(a => a.Division).HasConversion<string>().HasMaxLength(20);
                b.HasIndex(a => new { a.PieceId, a.Part }).IsUnique();
            });

            builder.Entity<Practice>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Title).HasMaxLength(200);
                b.Property(p => p.Place).HasMaxLength(200);
            });

            builder.Entity<PracticePiece>(b =>
            {
                b.HasKey(pp => new { pp.PracticeId, pp.PieceId });
                b.Property(pp => pp.RecordingUrl).HasMaxLength(500);
                b.HasOne(pp => pp.Practice)
                    .WithMany(p => p.Pieces)
                    .HasForeignKey(pp => pp.PracticeId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(pp => pp.Piece)
                    .WithMany()
                    .HasForeignKey(pp => pp.PieceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<MemberPiecePart>(b =>
            {
                b.HasKey(m => m.Id);
                b.Property(m => m.SubPart).IsRequired().HasMaxLength(50);
                b.HasIndex(m => new { m.UserId, m.PieceId }).IsUnique();
                b.HasOne(m => m.User)
                    .WithMany()
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(m => m.Piece)
                    .WithMany()
                    .HasForeignKey(m => m.PieceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Attendance>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
                b.HasIndex(a => new { a.PracticeId, a.UserId }).IsUnique();
                b.HasOne(a => a.Practice)
                    .WithMany()
                    .HasForeignKey(a => a.PracticeId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(a => a.User)
                    .WithMany()
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
