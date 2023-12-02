# Using the Scheduler
RueI comes with the [Scheduler](../api/RueI.Displays.Scheduling.Scheduler), and every display has one of them. the main purpose of the Scheduler is to provide an easy way to update the display at a certain time, and to provide a way to batch updates together to prevent getting ratelimited.

normally, the hints system comes with a 0.5 second ratelimit. if you send a hint (which updating the display does), you cannot send another one for 0.5 seconds[\*](#asterisk). this can easily create problems. for example, what if you update the display in 0.3 seconds and 0.5 seconds? normally, the first update would show, but then the second update would not show up until in 0.8 seconds.

this might not seem like the biggest problem, but it can make things look weird because of the delay. instead, what RueI will do is batch those two operations together, averaging the times. so, in that example, it will update the display in 0.4 seconds (assuming they both have the same priority).

## How to do it
scheduling a job is pretty easy. first, you have to get the [DisplayCore](../api/RueI.Displays.DisplayCore) of a player, and then access the `DisplayCore.Scheduler` property. then you can use the `Scheduler.Schedule` method to schedule jobs. here's an example:
```csharp
// assuming hub is previously defined as a ReferenceHub
SetElement element = new(300, "Hello!!");
DisplayCore core = DisplayCore.Get(hub);
Display display = new(core);
core.Scheduler.Schedule(TimeSpan.FromSeconds(5), () => display.Elements.Add(element));
```
notice how you do not have to explicitly call `display.Update()` or `core.Update()`. this is because, once RueI performs a batch job, it automatically updates the display. updating the display in a batch job doesn't do anything as the DisplayCore ignores all update requests when it is performing a batch job. so, this does nothing:
```csharp
core.Scheduler.Schedule(TimeSpan.FromSeconds(5), () => core.Update());
```
## Canceling a job
often, you're going to want to cancel a job that is ongoing. however, RueI does not return a reference to a job that you can then use to cancel it. instead, you use a [JobToken](../api/RueI.Displays.Scheduling.JobToken). you can then pass a JobToken in the `Scheduler.Schedule()` method. the benefit of this is that since you can declare a static JobToken singleton, you can ensure that there is only ever one instance of a certain job. here's an example:
```csharp
public static JobToken HelloToken { get; } = new();

public void UpdateDisplay(ReferenceHub hub) {
    SetElement element = new(300, "Hello!!");
    DisplayCore core = DisplayCore.Get(hub);
    Display display = new(core);

    core.Scheduler.KillJob(HelloToken);
    core.Scheduler.Schedule(TimeSpan.FromSeconds(5), () => display.Elements.Add(element));
}
```
with this, you won't ever have to worry about having multiple instances of the same job.


<a name="asterisk">\* it is more complicated than this, technically. however, RueI enforces a 0.525 second ratelimit, so you won't notice it.</a>
