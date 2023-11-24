namespace RueI;

/*********\
*  /\_/\  *
* ( o.o ) *
*  > ^ <  *
\*********/

using System.Runtime.CompilerServices;
using eMEC;

/// <summary>
/// Represents the main class for RueI.
/// </summary>
public static class Main
{
    /// <summary>
    /// Gets the current version of RueI.
    /// </summary>
    public static readonly Version Version = new(1, 0, 0);

    private static bool isInit = false;

    static Main()
    {
        isInit = true;

        RoundRestarting.RoundRestart.OnRestartTriggered += EventHandler.OnRestart;
        PlayerRoles.PlayerRoleManager.OnServerRoleSet += EventHandler.OnServerRoleSet;

        ServerConsole.AddLog($"[Info] [RueI] Thank you for using RueI! Running v{Version}", ConsoleColor.Yellow);
        ServerConsole.AddLog("[Info] [RueI] RueI is completely open-source and licensed under CC0", ConsoleColor.Yellow);
        ServerConsole.AddLog("[Info] [RueI] https://github.com/Ruemena/RueI", ConsoleColor.Yellow);

        try
        {
            var harmony = new HarmonyLib.Harmony("com.example.patch");
        }
        catch(Exception)
        {
            ServerConsole.AddLog("[Warn] [RueI] Could not load Harmony patches.", ConsoleColor.Yellow);
        }
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
