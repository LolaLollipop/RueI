namespace RueI.Patches
{
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using Hints;
    using Mirror;
    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    public static class HintPatch
    {
        public static void HandleAnonymousHint(NetworkConnection connection, Hint hint)
        {
            if (ReferenceHub.TryGetHub(connection.identity.gameObject, out ReferenceHub hub))
            {
                
            }
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindLastIndex(instruction =>
                instruction.opcode == OpCodes.Callvirt
                && (MethodInfo)instruction.operand == Method(typeof(NetworkConnection), nameof(NetworkConnection.Send)));

            index -= 1;

            Label returnLabel = generator.DefineLabel();

            CodeInstruction[] collection =
            {
                    new CodeInstruction(OpCodes.Ldloc_0), // NetworkConnection
                    new(OpCodes.Ldarg_1), // Hint
                    new(OpCodes.Call, Method(typeof(HintPatch), nameof(HandleAnonymousHint))),
            };

            newInstructions.InsertRange(index, collection);

            foreach (CodeInstruction instruction in newInstructions)
            {
                yield return instruction;
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
