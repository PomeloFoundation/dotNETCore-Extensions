﻿using System;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace Microsoft.EntityFrameworkCore
{
    public interface IBlobStorageDbContext<TModel, TKey>
        where TKey: IEquatable<TKey>
        where TModel : Blob<TKey>
    {
        DbSet<TModel> Blobs { get; set; }
        int SaveChanges();
    }

    public interface IBlobStorageDbContext : IBlobStorageDbContext<Blob, Guid>
    {
    }
}
