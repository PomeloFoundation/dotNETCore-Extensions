using System;

namespace Pomelo.AspNetCore.Extensions.BlobStorage
{
    public interface IBlobHandler<TModel>
        where TModel : Models.Blob
    {
        TModel Handle(TModel obj, Base64StringFile file);
    }

    public interface IBlobHandler : IBlobHandler<Models.Blob>
    {
    }
}
