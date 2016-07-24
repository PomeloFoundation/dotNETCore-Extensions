using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace FileUploadSite
{
    public class FuContext : DbContext, IBlobStorageDbContext
    {
        public FuContext(DbContextOptions opt) : base(opt) { }

        public DbSet<Blob> Blobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.SetupBlobStorage();
        }
    }
}
