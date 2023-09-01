using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace RightClickLinkSettlementTracker
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Messenger.Prefix("RightClickLinkSettlementTracker");
            new Harmony("mod.bannerlord.RightClickLinkSettlementTracker").PatchAll(Assembly.GetExecutingAssembly());
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            Messenger.Log("Mod loaded", Messenger.Level.Success);
        }
    }
}
