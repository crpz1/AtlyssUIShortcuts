using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Object = System.Object;

namespace AtlyssUIShortcuts
{
    [BepInPlugin("crpz.AtlyssUIShortcuts", "AtlyssUIShortcuts", "1.0.0")]
    public class AtlyssUIShortcuts : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger { get; private set; } = null;

        private void Awake()
        {
            Logger = base.Logger;
            //Harmony = new Harmony("crpz.AtlyssUIShortcuts");
            //Harmony.PatchAll();
            Logger.Log(LogLevel.Message, "hii :3");
        }

        private void Update()
        {
            //Logger.Log(LogLevel.Message, Cursor.lockState);
            if (AtlyssNetworkManager._current == null || PartyUIManager._current == null) return;
            if (AtlyssNetworkManager._current._soloMode) return;
            if (PartyUIManager._current._partyObject != null) return;
            if (PartyUIManager._current._partyInviteElement.isEnabled == false) return;
            if (Input.GetKeyDown(KeyCode.Y)) Player._mainPlayer.Cmd_SetPartyInviteCondition(PartyInviteStatus.ACCEPTED);
            if (Input.GetKeyDown(KeyCode.N)) Player._mainPlayer.Cmd_SetPartyInviteCondition(PartyInviteStatus.DECLINED);

        }
    }
}
