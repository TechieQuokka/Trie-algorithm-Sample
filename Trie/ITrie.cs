namespace Collections
{
    /// <summary>
    /// The Trie data structure is a tree-like data structure used for storing a dynamic set of strings.
    /// It is commonly used for efficient retrieval and storage of keys in a large dataset.
    /// </summary>
    public interface ITrie<T> : System.IDisposable
    {
        /// <summary>
        /// This is the insertion algorithm.
        /// </summary>
        /// <param name="data"></param>
        void Insert(System.Collections.Generic.IEnumerable<T> data);
        /// <summary>
        /// This is the remove algorithm
        /// </summary>
        /// <param name="target"></param>
        System.Boolean Remove(System.Collections.Generic.IEnumerable<T> target);
        /// <summary>
        /// This is the Search algorithm
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Find Data</returns>
        System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<T>> Search(System.Collections.Generic.IEnumerable<T> target);
    }

}
