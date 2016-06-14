// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Diagnostics;

namespace Pomelo.HtmlAgilityPack
{
    internal class HtmlConsoleListener : TraceListener
    {
        #region Public Methods

        public override void Write(string Message)
        {
            Write(Message, "");
        }

        public override void Write(string Message, string Category)
        {
            Console.Write("T:" + Category + ": " + Message);
        }

        public override void WriteLine(string Message)
        {
            Write(Message + "\n");
        }

        public override void WriteLine(string Message, string Category)
        {
            Write(Message + "\n", Category);
        }

        #endregion
    }
}