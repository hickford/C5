namespace C5
{
    /// <summary>
    /// The symbolic characterization of the speed of lookups for a collection.
    /// The values may refer to worst-case, amortized and/or expected asymtotic 
    /// complexity wrt. the collection size.
    /// </summary>
    public enum Speed
    {
        /// <summary>
        /// Counting the collection with the <code>Count</code> property may not return
        /// (for a synthetic and potentially infinite collection).
        /// </summary>
        PotentiallyInfinite = 1,

        /// <summary>
        /// Lookup operations like <code>Contains(T item)</code> or the <code>Count</code>
        /// property may take time O(n),
        /// where n is the size of the collection.
        /// </summary>
        Linear = 2,

        /// <summary>
        /// Lookup operations like <code>Contains(T item)</code> or the <code>Count</code>
        /// property  takes time O(log n),
        /// where n is the size of the collection.
        /// </summary>
        Log = 3,

        /// <summary>
        /// Lookup operations like <code>Contains(T item)</code> or the <code>Count</code>
        /// property takes time O(1).
        /// </summary>
        Constant = 4
    }
}