namespace RueI.Interfaces
{
    /// <summary>
    /// Defines an element that can be set.
    /// </summary>
    public interface ISettable : IElement
    {
        /// <summary>
        /// Sets the content of this element.
        /// </summary>
        /// <param name="text">The new element.</param>
        public void Set(string text);
    }
}
