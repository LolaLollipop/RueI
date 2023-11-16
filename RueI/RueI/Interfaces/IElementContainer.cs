namespace RueI.Interfaces
{
    /// <summary>
    /// Defines a container for multiple elements.
    /// </summary>
    public interface IElementContainer
    {
        /// <summary>
        /// Gets the elements of this <see cref="IElementContainer"/>.
        /// </summary>
        public List<IElement> Elements { get; }
    }
}
