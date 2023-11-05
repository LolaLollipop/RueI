using eMEC;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

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
