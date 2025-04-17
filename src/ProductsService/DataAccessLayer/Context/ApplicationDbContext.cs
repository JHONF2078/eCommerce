using ProductsService.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductsService.DataAccessLayer.Context
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //tabla virtual en la base de datos. Product es una clase que representa una entidad
        public DbSet<Product> Products { get; set; }

        //para configurar detalles de las tablas(restricciones, relaciones, nombres personalizados, etc.).
        //Es un método de configuración fluida (Fluent API) que te da más control y flexibilidad que las data annotations.
        //FluentValidation.AspNetCore
       // Es una librería independiente para validar datos de entrada, como formularios, peticiones HTTP, DTOs, etc.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Id)            // ← propiedad en el modelo
                .HasColumnName("ProductID");
        }
    }
}
