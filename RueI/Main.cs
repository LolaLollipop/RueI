using eMEC;

namespace RueI
{
    public class Main
    {
        public TaskPool RoundTaskPool { get; set; } = new();

        static Main()
        {
            RoundRestarting.RoundRestart.OnRestartTriggered += EventHandler.OnRestart;
            PlayerRoles.PlayerRoleManager.OnServerRoleSet += EventHandler.OnServerRoleSet;
        }
    }
}
