using BookingApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace BookingApp.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<BookingApp.Models.Register> Register { get; set; } = default!;

    public DbSet<BookingApp.Models.Login> Login { get; set; } = default!;

    public DbSet<BookingApp.Models.BookingModel> BookingModel { get; set; } = default!;

    public DbSet<BookingApp.Models.BookingFacilities> BookingFacilities { get; set; } = default!;



}
