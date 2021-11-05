using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RehberCore.Models;

namespace RehberCore.Models
{
    public class RehberContext : DbContext
    {
        public RehberContext(DbContextOptions<RehberContext> options) : base(options)
        {
        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Number> Numbers { get; set; }
        public DbSet<RehberCore.Models.ContactDTO> ContactDTO { get; set; }
    }
}
