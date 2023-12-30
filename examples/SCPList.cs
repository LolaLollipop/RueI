namespace SCPList
{
    /*
    This shows using RueI to create a simple SCP list *EXILED* plugin.
    */
    using System.Text;
    using System.Drawing;

    using PlayerRoles;

    using Exiled.API.Features;
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;

    using RueI.Extensions;
    using RueI.Elements;
    using RueI.Extensions.HintBuilding;
    using RueI.Displays;
    using RueI.Parsing.Enums;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        
        public bool Debug { get; set; } = false;
    }

    public class SCPList : Plugin<Config>
    {
        public override string Name => "SCPList";

        public static DynamicElement MyElement { get; } = new(GetContent, 900);
        
        public static AutoElement AutoElement { get; } = AutoElement.Create(Roles.Scps, MyElement).UpdateEvery(TimeSpan.FromSeconds(0.7));

        public override void OnEnabled()
        {
            RueI.RueIMain.EnsureInit(); // make sure to always call this!
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            AutoElement.Disable();
            base.OnDisabled();
        }

        private static string GetContent()
        {
            StringBuilder sb = new StringBuilder()
                .SetSize(65, MeasurementUnit.Percentage)
                .SetAlignment(HintBuilding.AlignStyle.Right);

            foreach (Player player in Player.Get(Side.Scp))
            {
                if (player.Role == RoleTypeId.Scp0492)
                {
                    continue;
                }

                string scpName = player.Role.Type switch
                {
                    RoleTypeId.Scp173 => "SCP-173",
                    RoleTypeId.Scp106 => "SCP-106",
                    RoleTypeId.Scp096 => "SCP-096",
                    RoleTypeId.Scp049 => "SCP-049",
                    RoleTypeId.Scp079 => "SCP-079",
                    RoleTypeId.Scp939 => "SCP-939",
                    RoleTypeId.Scp3114 => "SCP-3114",
                    _ => "???",
                };

                float health = UnityEngine.Mathf.Ceil(player.Health);
                float max = player.MaxHealth;
                float percentage = Clamp(health / player.MaxHealth, 0, 1);

                // the less health they have, the more red their hp will show
                Color healthColor = Color.FromArgb((int)(255 * (1 - percentage)), (int)(255 * percentage), 0);

                // SCP-??? : 500/500 HP
                sb.SetColor(Color.FromArgb(255, 50, 50))
                  .SetBold()
                  .Append($"{scpName} : ")
                  .CloseColor()
                  .CloseBold()
                  .SetColor(healthColor)
                  .Append(health)
                  .Append('/')
                  .Append(max)
                  .Append(" HP")
                  .CloseColor()
                  .AddLinebreak();
                Log.Info(sb.ToString());
            }

            return sb.ToString();
        }

        private static float Clamp(float value, float min, float max) => Math.Max(Math.Min(value, min), max);
    }
}
