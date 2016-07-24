using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Pomelo.AspNetCore.Extensions.BlobStorage.Models
{
    public class Blob<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        [MaxLength(128)]
        public string FileName { get; set; }

        [MaxLength(128)]
        public string ContentType { get; set; }

        public long ContentLength { get; set; }

        public DateTime Time { get; set; }

        [JsonIgnore]
        public byte[] Bytes { get; set; }
    }
}
