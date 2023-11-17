namespace RueI;

using System.Text;
using Hints;
using NorthwoodLib.Pools;
using RueI.Displays;
using RueI.Extensions;
using RueI.Records;

/// <summary>
/// Is responsible for managing all of the <see cref="DisplayBase"/>s for a <see cref="ReferenceHub"/>.
/// </summary>
public class DisplayCore
{
    private List<DisplayBase> displays = new();

    static DisplayCore()
    {
        RoundRestarting.RoundRestart.OnRestartTriggered += OnRestart;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayCore"/> class.
    /// <param name="hub">The hub to create the display for.</param>
    private DisplayCore(ReferenceHub hub)
    {
        this.Hub = hub;

        DisplayCoordinators.Add(hub, this);
        Scheduler = new(this);
    }

    /// <summary>
    /// Gets the <see cref="Displays.Scheduler"/> for this <see cref="DisplayCore"/>.
    /// </summary>
    public Scheduler Scheduler { get; }

    /// <summary>
    /// Gets a dictionary containing the DisplayCoordinators for each ReferenceHub.
    /// </summary>
    internal static Dictionary<ReferenceHub, DisplayCore> DisplayCoordinators { get; } = new();

    /// <summary>
    /// Gets the <see cref="ReferenceHub"/> that this display is for.
    /// </summary>
    internal ReferenceHub Hub { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not updates will currently be ignored.
    /// </summary>
    internal bool IgnoreUpdate { get; set; } = false;

    /// <summary>
    /// Gets a DisplayCore from a <see cref="ReferenceHub"/>, or creates it if it doesn't exist.
    /// </summary>
    /// <param name="hub">The hub to get the display for.</param>
    /// <returns>The DisplayCore.</returns>
    public static DisplayCore Get(ReferenceHub hub)
    {
        if (DisplayCoordinators.TryGetValue(hub, out DisplayCore value))
        {
            return value;
        }
        else
        {
            return new DisplayCore(hub);
        }
    }

    /// <summary>
    /// Updates this display.
    /// </summary>
    /// <param name="priority">The priority of the update - defaults to 100.</param>
    public void Update(int priority = 100)
    {
        if (IgnoreUpdate)
        {
            return;
        }

        Scheduler.Schedule(TimeSpan.Zero, () => { }, priority);
    }

    /// <summary>
    /// Updates this display, skipping all checks.
    /// </summary>
    internal void InternalUpdate()
    {
        string text = ParseElements();
        ServerConsole.AddLog(text);
        Hub.connectionToClient.Send(new HintMessage(new TextHint(text, new HintParameter[] { new StringHintParameter(text) }, null, 99999)));
    }

    /// <summary>
    /// Adds a display to this <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="display">The display to add.</param>
    internal void AddDisplay(DisplayBase display) => displays.Add(display);

    /// <summary>
    /// Removes a display from this <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="display">The display to remove.</param>
    internal void RemoveDisplay(DisplayBase display) => displays.Remove(display);

    /// <summary>
    /// Cleans up the dictionary after the server restarts.
    /// </summary>
    private static void OnRestart()
    {
        DisplayCoordinators.Clear();
    }

    private IEnumerable<IElement> GetAllElements() => displays.SelectMany(x => x.GetAllElements());

    private string ParseElements()
    {
        List<IElement> elements = GetAllElements().ToPooledList();

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
            sb.Append(Constants.TAGCLOSER);

            totalOffset += parsedData.Offset;
            lastPosition = funcPos;
            lastOffset = parsedData.Offset;
        }

        ListPool<IElement>.Shared.Return(elements);
        return $"<line-height={totalOffset}px>\n" + StringBuilderPool.Shared.ToStringReturn(sb);
    }
}