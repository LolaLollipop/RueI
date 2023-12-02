# eMEC
eMEC is a wrapper for MEC that is used by RueI. it is absolutely not necessary to use it in your own plugins, but it does provide some utility.

eMEC provides a few notable advantages over the usual MEC. firstly, it is much higher level - you do not directly manipulate CoroutineHandles directly. instead, you use `TaskBases` (not to be confused with .NET Tasks). secondly, if RueI detects that it is *not* running in Unity, it will automatically swap over to using .NET tasks, which means that you can do unit tests on things that use `TaskBases` but also use them normally in scp:sl.
