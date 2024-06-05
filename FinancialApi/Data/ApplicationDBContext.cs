using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancialApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialApi.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }

        public DbSet<VerifyOTP> VerifyOTPs { get; set; }
        public DbSet<User> Users { get; set; }
    }
}