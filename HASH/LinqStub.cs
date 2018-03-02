using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    /// <summary>
    /// Shuts the compiler up.
    /// </summary>
    /// <remarks>The default C# templates include the System.Linq namespace, which produces an error
    /// for projects that don't target .NET 3.5 or higher because LINQ is not supported, thus there
    /// is no LINQ namespace. This file creates a class in the System.Linq namespace because I'm
    /// too lazy to delete System.Linq from every file or change the template, but not so lazy
    /// I can't write all this silly stuff out.</remarks>
    class LinqStub
    {
        /// <summary>
        /// No, you can't have me.
        /// </summary>
        private LinqStub() { }
    }
}
