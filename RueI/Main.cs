namespace RueI;

using System.Reflection;

/*********\
*  /\_/\  *
* ( o.o ) *
*  > ^ <  *
\*********/

using System.Runtime.CompilerServices;

/// <summary>
/// Represents the main class for RueI.
/// </summary>
public static class Main
{
    /// <summary>
    /// Gets the current version of RueI.
    /// </summary>
    public static readonly Version Version = Assembly.GetAssembly(typeof(Main)).GetName().Version;

    private static bool isInit = false;

    static Main()
    {
        isInit = true;

        RoundRestarting.RoundRestart.OnRestartTriggered += EventHandler.OnRestart;
        PlayerRoles.PlayerRoleManager.OnServerRoleSet += EventHandler.OnServerRoleSet;

        if (!StartupArgs.Args.Contains("-noRMsg", StringComparison.OrdinalIgnoreCase)) // TODO: make this work
        {
            ServerConsole.AddLog($"[Info] [RueI] Thank you for using RueI! Running v{Version.ToString(3)}", ConsoleColor.Yellow);
        }

        try
        {
            var harmony = new HarmonyLib.Harmony("com.example.patch");
        }
        catch(Exception)
        {
            ServerConsole.AddLog("[Warn] [RueI] Could not load Harmony patches.", ConsoleColor.Yellow);
        }

        _ = CharacterLengths.Lengths.Count; // force static initializer
    }

    /// <summary>
    /// Ensures that RueI is properly initialized.
    /// </summary>
    public static void EnsureInit()
    {
        if (!isInit)
        {
            RuntimeHelpers.RunClassConstructor(typeof(Main).TypeHandle);
        }
    }
}
