namespace RueI.Displays;

using System.Text;
using NorthwoodLib.Pools;
using RueI.Extensions;
using RueI.Elements;
using RueI.Parsing.Records;

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

        StringBuilder sb = StringBuilderPool.Shared.Rent();
        float totalOffset = 0;

        float lastPosition = 0;
        float lastOffset = 0;

        elements.Sort(CompareElement);

        for (int i = 0; i < elements.Count; i++)
        {
            IElement curElement = elements[i];

            ParsedData parsedData = curElement.ParsedData;
            float funcPos = curElement.GetFunctionalPosition();

            if (i != 0)
            {
                float calcedOffset = CalculateOffset(lastPosition, lastOffset, funcPos);
                sb.Append($"<line-height={calcedOffset}px>\n</line-height>");
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
        sb.Insert(0, $"<line-height={totalOffset}px>\n");
        return StringBuilderPool.Shared.ToStringReturn(sb);
    }

    /// <summary>
    /// Calculates the offset for two hints.
    /// </summary>
    /// <param name="hintOnePos">The first hint's vertical position.</param>
    /// <param name="hintOneTotalLines">The first hint's total line-height, excluding the vertical position.</param>
    /// <param name="hintTwoPos">The second hint's vertical position.</param>
    /// <returns>A float indicating the new offset.</returns>
    public static float CalculateOffset(float hintOnePos, float hintOneTotalLines, float hintTwoPos)
    {
        float calc = hintOnePos + (2 * hintOneTotalLines) - hintTwoPos;
        return calc / -2;
    }

    private static int CompareElement(IElement first, IElement second) => first.ZIndex - second.ZIndex;
}