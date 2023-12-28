namespace Example
{
    using System.Runtime.CompilerServices;

    using Hints;

    using RueI;
    using RueI.Displays;
    using RueI.Displays.Scheduling;
    using RueI.Elements;
    using RueI.Extensions;

    /// <summary>
    /// Provides a <see cref="IHintProvider"/> for the plugin.
    /// </summary>
    public static class HintProvider
    {
        /// <summary>
        /// Gets the <see cref="IHintProvider"/> for the plugin.
        /// </summary>
        public static IHintProvider Provider { get; } = GetProvider();

        private static IHintProvider GetProvider()
        {
            try
            {
                LoadRueI();
                return new RueIHintProvider();
            }
            catch (TypeLoadException)
            {
                return new VanillaHintProvider();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // inlining could cause type load errors
        private static void LoadRueI() => RueIMain.EnsureInit();
    }

    /// <summary>
    /// Defines a contract for a class that provides hint utility.
    /// </summary>
    public interface IHintProvider
    {
        /// <summary>
        /// Shows a string to the given <see cref="ReferenceHub"/> using hints for the specified duration.
        /// </summary>
        /// <param name="hub">The <see cref="ReferenceHub"/> to show to.</param>
        /// <param name="duration">How long to show the content for.</param>
        /// <param name="content">The string to show to the <see cref="ReferenceHub"/>.</param>
        public void ShowString(ReferenceHub hub, TimeSpan duration, string content);
    }

    /// <summary>
    /// Implements <see cref="IHintProvider"/> by providing base-game, vanilla hint functionality.
    /// </summary>
    public class VanillaHintProvider : IHintProvider
    {
        /// <summary>
        /// Shows a string to the given <see cref="ReferenceHub"/> using hints for the specified duration.
        /// </summary>
        /// <param name="hub">The <see cref="ReferenceHub"/> to show to.</param>
        /// <param name="duration">How long to show the hint for.</param>
        /// <param name="content">The content to show to the <see cref="ReferenceHub"/>.</param>
        public void ShowString(ReferenceHub hub, TimeSpan duration, string content)
        {
            hub.hints.Show(new TextHint(content, new HintParameter[] { new StringHintParameter(content) }, null, (float)duration.TotalSeconds));
        }
    }

    /// <summary>
    /// Implements <see cref="IHintProvider"/> by utilizing Ruei.
    /// </summary>
    public class RueIHintProvider : IHintProvider
    {
        private readonly TimedElemRef<SetElement> elemRef = new();

        /// <summary>
        /// Shows a string to the given <see cref="ReferenceHub"/> using RueI elements for the specified duration.
        /// </summary>
        /// <param name="hub">The <see cref="ReferenceHub"/> to show to.</param>
        /// <param name="duration">How long to show the element for.</param>
        /// <param name="content">The content of the element.</param>
        public void ShowString(ReferenceHub hub, TimeSpan duration, string content)
        {
            DisplayCore.Get(hub).ShowTemp(content, Ruetility.FunctionalToScaledPosition(0), duration, elemRef);
        }
    }
}