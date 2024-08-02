using GerberBackend.Core.Entities.Auth;
using GerberBackend.Core.Entities.Gerber;
using GerberBackend.Core.Entities.Gerber.Elements;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GerberBackend.Core.DbContext;

public class ApplicationContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    { }

    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    public DbSet<OrderGerber> Gerbers { get; set; }
    public DbSet<BaseMaterial> BaseMaterials { get; set; }
    public DbSet<Layer> Layers { get; set; }
    public DbSet<BoardThickness> BoardThickness { get; set; }
    public DbSet<FoilThickness> FoilThicknesses { get; set; }
    public DbSet<ContourMachining> ContourMachinings { get; set; }
    public DbSet<DataNumbering> DataNumberings { get; set; }
    public DbSet<DrillFile> DrillFiles { get; set; }
    public DbSet<MarkingColor> MarkingColors { get; set; }
    public DbSet<MaskColor> MaskColors { get; set; }
    public DbSet<GerberFileBinary> GerberFileBinaries { get; set; }
    public DbSet<MaskSide> MaskSides { get; set; }
    public DbSet<MaskType> MaskTypes { get; set; }
    public DbSet<MarkingSide> MarkingSides { get; set; }
    public DbSet<Vias> Vias { get; set; }
    public DbSet<EdgeConnectors> EdgeConnectors { get; set; }
    public DbSet<MainSites> MainSites { get; set; }
    public DbSet<MinimalConductor> MinimalConductors { get; set; }
    public DbSet<AngleChamfer> AngleChamfers { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(e =>
        {
            e.ToTable("Users");
        });

        builder.Entity<OrderGerber>(e =>
        {
            e.ToTable("OrderGerber");
        });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.BoardWindow, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.Count, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.Quantity, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.Size, e =>
            {
                e.Property(b => b.X);
                e.Property(b => b.Y);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.Lamellas, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.ConnectorsCount, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.BuildTime, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.Status, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.Order, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .OwnsOne(g => g.Price, e =>
            {
                e.Property(b => b.Value);
            });

        builder.Entity<OrderGerber>()
            .HasOne(g => g.User)
            .WithMany(u => u.GerberList)
            .HasForeignKey(g => g.UserId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.BaseMaterial)
            .WithMany(b => b.OrderGerbers)
            .HasForeignKey(g => g.BaseMaterialId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.Layer)
            .WithMany(l => l.OrderGerbers)
            .HasForeignKey(g => g.LayerId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.BoardThickness)
            .WithMany(bt => bt.OrderGerbers)
            .HasForeignKey(g => g.BoardThicknessId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.FoilThickness)
            .WithMany(bt => bt.OrderGerbers)
            .HasForeignKey(g => g.FoilThicknessId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.ContourMachining)
            .WithMany(cm => cm.OrderGerbers)
            .HasForeignKey(g => g.ContourMachiningId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.DataNumbering)
            .WithMany(dn => dn.OrderGerbers)
            .HasForeignKey(g => g.DataNumberingId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.DrillFile)
            .WithMany(df => df.OrderGerbers)
            .HasForeignKey(g => g.DrillFileId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.MarkingColor)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.MarkingColorId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.MaskColor)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.MaskColorId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.GerberFileBinary)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.GerberFileId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.MarkingSide)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.MarkingSideId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.MaskType)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.MaskTypeId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.MaskSide)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.MaskSideId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.Vias)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.ViasId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.MainSites)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.MainSitesId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.EdgeConnectors)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.EdgeConnectorId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.MinimalConductor)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.MinimalConductorId);

        builder.Entity<OrderGerber>()
            .HasOne(g => g.AngleChamfer)
            .WithMany(mc => mc.OrderGerbers)
            .HasForeignKey(g => g.AngleChamferId);
    }
}