using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework4
{
    public class BankDbContext : DbContext
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=NationalBankDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerAccount>()
                .HasKey(ca => new { ca.CustomerId, ca.AccountNumber });

            modelBuilder.Entity<CustomerAccount>()
                .HasOne(ca => ca.Customer)
                .WithMany(c => c.CustomerAccounts)
                .HasForeignKey(ca => ca.CustomerId);

            modelBuilder.Entity<CustomerAccount>()
                .HasOne(ca => ca.Account)
                .WithMany(a => a.CustomerAccounts)
                .HasForeignKey(ca => ca.AccountNumber);

            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Manager)
                .WithOne(m => m.Branch)
                .HasForeignKey<Manager>(m => m.BranchCode);

            // Helping By AI
            modelBuilder.Entity<Branch>().HasData(
                new Branch { Code = "CAI-01", Name = "Cairo Main Branch", Address = "Tahrir Square, Cairo", PhoneNumber = "0223456789" },
                new Branch { Code = "ALX-01", Name = "Alexandria Branch", Address = "Corniche St, Alexandria", PhoneNumber = "0354321098" }
            );

            modelBuilder.Entity<Manager>().HasData(
                new Manager { Id = 1, FullName = "Hassan Mohamed", Email = "hassan@bank.com", PhoneNumber = "01001234567", HireDate = new DateTime(2018, 5, 1), Address = "Cairo", BranchCode = "CAI-01" },
                new Manager { Id = 2, FullName = "Mona Ibrahim", Email = "mona@bank.com", PhoneNumber = "01009876543", HireDate = new DateTime(2020, 8, 15), Address = "Alex", BranchCode = "ALX-01" }
            );
        }
    }
}
