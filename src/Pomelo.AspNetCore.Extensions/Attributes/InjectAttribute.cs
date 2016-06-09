using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute, IBindingSourceMetadata
    {
        /// <inheritdoc />
        public BindingSource BindingSource => BindingSource.Services;
    }
}