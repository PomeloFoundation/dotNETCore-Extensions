// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lazy.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
//
// </copyright>
// <summary>
//   Re-implements the bare necessities of Lazy for .NET 2.0 and 3.5
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System
{
    /// <summary>
    /// Re-implements the bare necessities of Lazy for .NET 2.0 and 3.5
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    internal sealed class Lazy<T>
    {
        /// <summary>
        /// Lock object for thread safety.
        /// </summary>
        private readonly object synclock = new object();
        
        /// <summary>
        /// Creation delegate
        /// </summary>
        private readonly Func<T> valueFactory;
        
        /// <summary>
        /// Value indicating whether the creation delegate has been called.
        /// </summary>
        private bool created;
        
        /// <summary>
        /// The actual value.
        /// </summary>
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class. 
        /// When lazy initialization occurs, the specified initialization function is used.
        /// </summary>
        /// <param name="valueFactory">The delegate that produces the value when it is needed.</param>
        internal Lazy(Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }

            this.valueFactory = valueFactory;
        }

        /// <summary>
        /// Gets the lazily initialized value of the current Lazy{T} instance.
        /// </summary>
        /// <value>The lazily initialized value of the current Lazy{T} instance.</value>
        internal T Value
        {
            get
            {
                if (!this.created)
                {
                    lock (this.synclock)
                    {
                        if (!this.created)
                        {
                            this.value = this.valueFactory();
                            this.created = true;
                        }
                    }
                }

                return this.value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether whether a value has been created for this Lazy{T} instance.
        /// </summary>
        /// <returns>true if a value has been created for this Lazy{T} instance; otherwise, false.</returns>
        internal bool IsValueCreated
        {
            get
            {
                lock (this.synclock)
                {
                    return this.created;
                }
            }
        }

        /// <summary>
        /// Creates and returns a string representation of the Lazy{T}.Value.
        /// </summary>
        /// <returns>The string representation of the Lazy{T}.Value property.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
