using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace AtlyssUIShortcuts
{
    [BepInPlugin("crpz.AtlyssUIShortcuts", "AtlyssUIShortcuts", "1.0.0")]
    public class AtlyssUIShortcuts : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger { get; private set; } = null;
        public static bool unlockedByUs = false;

        private void Awake()
        {
            Logger = base.Logger;
            //Harmony = new Harmony("crpz.AtlyssUIShortcuts");
            //Harmony.PatchAll();
            Logger.Log(LogLevel.Message, "hii :3");
        }

        private void Update()
        {
            if (Player._mainPlayer == null) return;
            if (Player._mainPlayer._currentGameCondition != GameCondition.IN_GAME) return;
            if (CameraFunction._current == null) return;

            if (unlockedByUs && CameraFunction._current._unlockedCamera != true)
            {
                Logger.Log(LogLevel.Message, "unlocking because we were forced to");
                unlockedByUs = false;
            }
            
            if (Input.GetKeyDown(KeyCode.LeftAlt) && CameraFunction._current._unlockedCamera == false)
            {
                Logger.Log(LogLevel.Message, "KeyDownEvent: LeftAlt");
                CameraFunction._current._unlockedCamera = true;
                unlockedByUs = true;
            } else if (Input.GetKeyUp(KeyCode.LeftAlt) && unlockedByUs)
            {
                Logger.Log(LogLevel.Message, "KeyUpEvent: LeftAlt");
                CameraFunction._current._unlockedCamera = false;
                unlockedByUs = false;
            }

            if (AtlyssNetworkManager._current == null || PartyUIManager._current == null) return;
            if (AtlyssNetworkManager._current._soloMode) return;
            if (PartyUIManager._current._partyObject != null) return;
            if (PartyUIManager._current._partyInviteElement.isEnabled == false) return;
            if (Input.GetKeyDown(KeyCode.Y)) Player._mainPlayer.Cmd_SetPartyInviteCondition(PartyInviteStatus.ACCEPTED);
            if (Input.GetKeyDown(KeyCode.N)) Player._mainPlayer.Cmd_SetPartyInviteCondition(PartyInviteStatus.DECLINED);

        }
    }
}
