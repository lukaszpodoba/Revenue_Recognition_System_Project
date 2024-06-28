using JWT.Models;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Contexts;

public class DatabaseContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Individual> Individuals { get; set; }
    public DbSet<Business> Businesses { get; set; }
    public DbSet<Software> Softwares { get; set; }
    public DbSet<Agreement> Agreements { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<AppUserModel> AppUserModels { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Client>()
            .ToTable("Clients");

        modelBuilder.Entity<Business>()
            .ToTable("Businesses");

        modelBuilder.Entity<Individual>()
            .ToTable("Individuals");
        
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Client)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.NoAction); 
        
        modelBuilder.Entity<Software>().HasData(new Software
        {
            SoftwareId = 1,
            SoftwareName = "Kaspersky",
            SoftwareCategory = "Antivirus",
            SoftwareDescription = "Antivirus software",
            SoftwareCurrentVersion = "1.0",
            SoftwareSubscriptionPrice = null,
            SoftwareIsSubscriptionPurchase = false,
            SoftwareOneTimePrice = 1000,
            SoftwareIsOneTimePurchase = true
        });
        
        modelBuilder.Entity<Software>().HasData(new Software
        {
            SoftwareId = 2,
            SoftwareName = "Microsoft Office",
            SoftwareCategory = "Office",
            SoftwareDescription = "Office software",
            SoftwareCurrentVersion = "1.0",
            SoftwareSubscriptionPrice = null,
            SoftwareIsSubscriptionPurchase = false,
            SoftwareOneTimePrice = 300,
            SoftwareIsOneTimePurchase = true
        });

        modelBuilder.Entity<Discount>().HasData(new Discount
        {
            DiscountId = 1,
            DiscountName = "Student",
            DiscountFrom = DateTime.Parse("2023-01-01"),
            DiscountUntil = DateTime.Parse("2024-12-31"),
            DiscountPercentageValue = 50.0,
            DiscountType = "Agreement",
            SoftwareId = 1
        });
        
        modelBuilder.Entity<Discount>().HasData(new Discount
        {
            DiscountId = 2,
            DiscountName = "Sale",
            DiscountFrom = DateTime.Parse("2023-01-01"),
            DiscountUntil = DateTime.Parse("2024-12-31"),
            DiscountPercentageValue = 30.0,
            DiscountType = "Agreement",
            SoftwareId = 1
        });
        
        modelBuilder.Entity<Discount>().HasData(new Discount
        {
            DiscountId = 3,
            DiscountName = "Summer",
            DiscountFrom = DateTime.Parse("2022-01-01"),
            DiscountUntil = DateTime.Parse("2022-12-31"),
            DiscountPercentageValue = 80.0,
            DiscountType = "Agreement",
            SoftwareId = 1
        });
    }
}