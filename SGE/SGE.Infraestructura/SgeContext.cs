using Microsoft.EntityFrameworkCore;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios; 

namespace SGE.Infraestructura;

public class SgeContext : DbContext
{
    public DbSet<Expediente> Expedientes { get; set; }
    public DbSet<Tramite> Tramites { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    public SgeContext()
    {
        // Le dice a EF Core: "Si la base de datos SGE.sqlite no existe, 
        // creala automáticamente con todas las tablas ahora mismo"
        this.Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuramos la base física en la raíz del proyecto ejecutable
        optionsBuilder.UseSqlite("Data Source=SGE.sqlite");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeo de Expediente
        modelBuilder.Entity<Expediente>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.ComplexProperty(e => e.Caratula); 
        });

        // Mapeo de Trámite
        modelBuilder.Entity<Tramite>(entity =>{entity.HasKey(t => t.Id);
        
        entity.ComplexProperty(t => t.contenido);
        });

        // Mapeo de Usuario
        modelBuilder.Entity<Usuario>(entity =>{entity.HasKey(u => u.Id);
        
        entity.Property(u => u.Nombre).IsRequired();

        entity.Property(u => u.CorreoElectronico).IsRequired().HasColumnName("Correo");

        entity.HasIndex(u => u.CorreoElectronico).IsUnique();

        entity.Property(u => u.ContraseñaHash).IsRequired();});
    
    }
}
