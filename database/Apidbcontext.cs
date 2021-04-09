using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using petmanagement.models;

public class Apidbcontext : DbContext
{
    public Apidbcontext(DbContextOptions<Apidbcontext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        builder.Entity<userprofilecreate>(cfg =>
        {
            cfg.HasKey(e => e.Userid);

            cfg.Property(e => e.Address)
                .IsRequired();
            cfg.Property(e => e.Country)
                .IsRequired();
            cfg.Property(e => e.Name)
                .IsRequired();
            cfg.Property(e => e.Phoneno)
                .IsRequired();
            cfg.Property(e => e.PAN)
                .IsRequired();
            cfg.Property(e => e.Email)
                .IsRequired();
            cfg.Property(e => e.State)
                .IsRequired();
        });
    }
}