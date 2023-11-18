namespace RueI;

using System.Text;
using NorthwoodLib.Pools;
using RueI.Extensions;
using RueI.Records;

/// <summary>
/// Provides a means of combining <see cref="IElement"/>s.
/// </summary>
public static class ElemCombiner
{
    /// <summary>
    /// Combines multiple <see cref="IElement"/>s into a string.
    /// </summary>
    /// <param name="enumElems">The <see cref="IEnumerable{T}"/> of <see cref="IElement"/>s to combine.</param>
    /// <returns>A <see cref="string"/> with all of the combined <see cref="IElement"/>s.</returns>
    public static string Combine(IEnumerable<IElement> enumElems)
    {
        List<IElement> elements = enumElems.ToPooledList();

        if (!elements.Any())
        {
            return string.Empty;
        }

        PluginAPI.Core.Log.Info(elements.Count.ToString());
        StringBuilder sb = StringBuilderPool.Shared.Rent();
        float totalOffset = 0;

        float lastPosition = 0;
        float lastOffset = 0;

        elements.Sort();

        for (int i = 0; i < elements.Count; i++)
        {
            IElement curElement = elements[i];

            ParsedData parsedData = curElement.ParsedData;

            float funcPos = curElement.GetFunctionalPosition();

            if (i != 0)
            {
                float calcedOffset = ElementHelpers.CalculateOffset(lastPosition, lastOffset, funcPos);
                sb.Append($"<line-height={calcedOffset}px>\n");
                totalOffset += calcedOffset;
            }
            else
            {
                totalOffset += funcPos;
            }

            sb.Append(parsedData.Content);

            totalOffset += parsedData.Offset;
            lastPosition = funcPos;
            lastOffset = parsedData.Offset;
        }

        ListPool<IElement>.Shared.Return(elements);
        return $"<line-height={totalOffset}px>\n" + StringBuilderPool.Shared.ToStringReturn(sb);
    }
}