﻿/*
* It was Canada's idea: http://www.codeproject.com/Articles/2670/Accessing-alternative-data-streams-of-files-on-an
*/

using System;

namespace NewsGator.Install.Common.Utilities.NTFSStreams
{
    /// <summary>
    /// Represents the attributes of a file stream.
    /// </summary>
    [Flags]
    public enum FileStreamAttributes
    {
        /// <summary>
        /// No attributes.
        /// </summary>
        None = 0,
        /// <summary>
        /// Set if the stream contains data that is modified when read.
        /// </summary>
        ModifiedWhenRead = 1,
        /// <summary>
        /// Set if the stream contains security data.
        /// </summary>
        ContainsSecurity = 2,
        /// <summary>
        /// Set if the stream contains properties.
        /// </summary>
        ContainsProperties = 4,
        /// <summary>
        /// Set if the stream is sparse.
        /// </summary>
        Sparse = 8,
    }
}
