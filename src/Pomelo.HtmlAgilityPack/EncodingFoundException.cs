// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Text;

namespace Pomelo.HtmlAgilityPack
{
    internal class EncodingFoundException : Exception
    {
        #region Fields

        private Encoding _encoding;

        #endregion

        #region Constructors

        internal EncodingFoundException(Encoding encoding)
        {
            _encoding = encoding;
        }

        #endregion

        #region Properties

        internal Encoding Encoding
        {
            get { return _encoding; }
        }

        #endregion
    }
}