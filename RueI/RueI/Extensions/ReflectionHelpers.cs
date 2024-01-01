namespace RueI.Extensions;

using RueI.Displays;
using RueI.Displays.Scheduling;
using RueI.Elements;

/// <summary>
/// Contains methods designed for use by reflection.
/// </summary>
public static class ReflectionHelpers
{
    /// <summary>
    /// Gets a <see cref="Action{T1, T2, T3, T4}"/> that can be used to add an element, with a <see cref="TimedElemRef{T}"/> as a closure.
    /// </summary>
    /// <returns>A <see cref="Action{T1, T2, T3, T4}"/> that can be used to add an element to a <see cref="ReferenceHub"/>.</returns>
    /// <remarks>
    /// This method is not intended to be used when using RueI as a direct dependency.
    /// </remarks>
    public static Action<ReferenceHub, string, float, TimeSpan> GetElementShower()
    {
        TimedElemRef<SetElement> elemRef = new();
        return (hub, content, name, span) => ShowTempFunctional(hub, content, name, span, elemRef);
    }

    private static void ShowTempFunctional(ReferenceHub hub, string content, float position, TimeSpan time, object elemRef)
    {
        DisplayCore core = DisplayCore.Get(hub);
        core.ShowTempFunctional(content, position, time, (TimedElemRef<SetElement>)elemRef);
    }
}
