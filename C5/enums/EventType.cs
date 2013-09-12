using System;

namespace C5
{
    /// <summary>
    /// Event types
    /// </summary>
    [Flags]
    public enum EventType
    {
        /// <summary>
        /// None (0)
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Changed (1) 
        /// </summary>
        Changed = 0x00000001,

        /// <summary>
        /// Cleared (2) 
        /// </summary>
        Cleared = 0x00000002,

        /// <summary>
        /// Added (4) 
        /// </summary>
        Added = 0x00000004,

        /// <summary>
        /// Removed (8) 
        /// </summary>
        Removed = 0x00000008,

        /// <summary>
        ///  Basic (15)
        /// </summary>
        Basic = 0x0000000f,

        /// <summary>
        /// Inserted (16) 
        /// </summary>
        Inserted = 0x00000010,

        /// <summary>
        /// RemovedAt (32) 
        /// </summary>
        RemovedAt = 0x00000020,

        /// <summary>
        /// All (63) 
        /// </summary>
        All = 0x0000003f
    }
}