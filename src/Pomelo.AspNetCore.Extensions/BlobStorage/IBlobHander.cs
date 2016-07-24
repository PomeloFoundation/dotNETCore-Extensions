using System;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public interface IBlobHandler<TModel, TKey>
        where TKey : IEquatable<TKey>
        where TModel : Models.Blob<TKey>
    {
        TModel Handle(TModel obj, Base64StringFile file);
    }

    public interface IBlobHandler : IBlobHandler<Models.Blob, Guid>
    {
    }
}
