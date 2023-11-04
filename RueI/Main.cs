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
            ServerConsole.AddLog("Running RueI :)");
            string path = Assembly.GetExecutingAssembly().Location;
            if (Path.GetDirectoryName(path) != "dependencies")
            {
                ServerConsole.AddLog("RueI is NOT in dependencies, this may cause plugin compatibility issues", ConsoleColor.DarkRed);
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
