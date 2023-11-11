using eMEC;
using System.Runtime.CompilerServices;

namespace RueI
{
    public static class Main
    {
        private static bool isInit = false;

        public static TaskPool RoundTaskPool { get; set; } = new();

        static Main()
        {
            isInit = true;

            RoundRestarting.RoundRestart.OnRestartTriggered += EventHandler.OnRestart;
            PlayerRoles.PlayerRoleManager.OnServerRoleSet += EventHandler.OnServerRoleSet;

            ServerConsole.AddLog("[Info] [RueI] Thank you for using RueI!", ConsoleColor.Yellow);
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
}
