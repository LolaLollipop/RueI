# RueI
a ui library (ish) for scp sl
## example usage
```csharp
PlayerDisplay display = new(ev.Player);
SetElement element = new(-500, zIndex: 10, "example text");
display.Update();
```
## limitations
voffsets will break this (they're not replaced for performance reasons), and linebreaks caused by overflows will also break it
