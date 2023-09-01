using System;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.ObjectSystem;

namespace RightClickLinkSettlementTracker
{
    [HarmonyPatch(typeof(EncyclopediaManager), "GoToLink", new Type[] { typeof(string), typeof(string) })]
    internal class ShouldToggleTrackSettlement
    {
        private static bool Prefix(string pageType, string stringID)
        {
            bool rightClicked = Input.IsReleased(InputKey.RightMouseButton);

            if (!rightClicked || !pageType.Equals("Settlement"))
            {
                return true;
            }

            // Get the encyclopedia page and object
            EncyclopediaPage encyclopediaPage2 = Campaign.Current.EncyclopediaManager.GetEncyclopediaPages().FirstOrDefault((EncyclopediaPage e) => e.GetIdentifierNames().Contains(pageType));
			MBObjectBase @object = encyclopediaPage2.GetObject(pageType, stringID);

            if (encyclopediaPage2 == null || !encyclopediaPage2.IsValidEncyclopediaItem(@object))
            {
                return true;
            }

            string settlementName = @object.GetName().ToString();
            Settlement settlement = Settlement.Find(stringID);

            // Make sure we're right clicking on a settlement, otherwise let the original method handle it
            if (settlement == null)
            {
                return true;
            }

            executeTrack(settlement);

            return false;
        }

        private static void executeTrack(Settlement settlement)
        {
            bool IsVisualTrackerSelected = Campaign.Current.VisualTrackerManager.CheckTracked(settlement);

            if (!IsVisualTrackerSelected)
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(settlement);
				IsVisualTrackerSelected = true;
                Messenger.Log("Tracking " + settlement.Name.ToString(), Messenger.Level.Warning);
			}
			else
			{
				Campaign.Current.VisualTrackerManager.RemoveTrackedObject(settlement, false);
				IsVisualTrackerSelected = false;
                Messenger.Log("Untracking " + settlement.Name.ToString(), Messenger.Level.Warning);
			}

			Game.Current.EventManager.TriggerEvent<PlayerToggleTrackSettlementFromEncyclopediaEvent>(new PlayerToggleTrackSettlementFromEncyclopediaEvent(settlement, IsVisualTrackerSelected));
        }
    }
}