namespace RueI;

using System.Runtime.CompilerServices;
using eMEC;

public static class Main
{
    private static bool isInit = false;

    public static TaskPool RoundTaskPool { get; set; } = new();

    public static Version Version = new(1, 0, 0);

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

    public static void EnsureInit()
    {
        if (!isInit)
        {
            RuntimeHelpers.RunClassConstructor(typeof(Main).TypeHandle);
        }
    }
}
