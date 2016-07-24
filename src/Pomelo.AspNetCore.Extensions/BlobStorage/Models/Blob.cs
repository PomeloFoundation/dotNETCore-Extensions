using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Extensions.BlobStorage.Models
{
    public class Blob : Blob<Guid>
    {
    }
}
