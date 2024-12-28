using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace AtlyssUIShortcuts
{
    [BepInPlugin("crpz.AtlyssUIShortcuts", "AtlyssUIShortcuts", "1.2.0")]
    [BepInDependency("EasySettings", BepInDependency.DependencyFlags.SoftDependency)]
    public class AtlyssUIShortcuts : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger { get; private set; } = null;

        internal static ConfigEntry<bool> UnlockCursorConfig;
        internal static ConfigEntry<bool> InviteConfig;
        internal static ConfigEntry<bool> WorldPortalScrollConfig;
        internal static ConfigEntry<KeyCode> UnlockCursorBind;
        internal static ConfigEntry<KeyCode> InviteAcceptBind;
        internal static ConfigEntry<KeyCode> InviteDeclineBind;

        public static bool unlockedByUs = false;
        [CanBeNull] internal static AtlyssUIShortcuts instance;

        private void Awake()
        {
            Logger = base.Logger;
            //Harmony = new Harmony("crpz.AtlyssUIShortcuts");
            //Harmony.PatchAll();
            InitConfig();
            instance = this;

            if (EasySettingsCompat.enabled)
            {
                EasySettingsCompat.SetupSettings();
            }

            Logger.Log(LogLevel.Message, "hii :3");
        }

        private void Update()
        {
            if (Player._mainPlayer == null) return;
            if (Player._mainPlayer._currentGameCondition != GameCondition.IN_GAME) return;
            if (CameraFunction._current == null) return;

            if (UnlockCursorConfig.Value)
            {
                if (unlockedByUs && CameraFunction._current._unlockedCamera != true)
                {
                    Logger.Log(LogLevel.Message, "unlocking because we were forced to");
                    unlockedByUs = false;
                }

                if (Input.GetKeyDown(UnlockCursorBind.Value) && CameraFunction._current._unlockedCamera == false)
                {
                    Logger.Log(LogLevel.Message,
                        "KeyDownEvent: " + NameOfKey(UnlockCursorBind.Value));
                    CameraFunction._current._unlockedCamera = true;
                    unlockedByUs = true;
                }
                else if (Input.GetKeyUp(UnlockCursorBind.Value) && unlockedByUs)
                {
                    Logger.Log(LogLevel.Message,
                        "KeyUpEvent: " + NameOfKey(UnlockCursorBind.Value));
                    CameraFunction._current._unlockedCamera = false;
                    unlockedByUs = false;
                }
            }

            HandleWorldPortalScroll();

            if (InviteConfig.Value == false) return;
            if (AtlyssNetworkManager._current == null || PartyUIManager._current == null) return;
            if (AtlyssNetworkManager._current._soloMode) return;
            if (PartyUIManager._current._partyObject != null) return;
            if (PartyUIManager._current._partyInviteElement.isEnabled == false) return;

            if (!PartyUIManager._current._acceptInviteButton.GetComponentInChildren<TextMeshProUGUI>().text
                    .Contains(NameOfKey(InviteAcceptBind.Value)))
            {
                PartyUIManager._current._acceptInviteButton.GetComponentInChildren<TextMeshProUGUI>().text +=
                    String.Format("({})", NameOfKey(InviteAcceptBind.Value));
                PartyUIManager._current._declineInviteButton.GetComponentInChildren<TextMeshProUGUI>().text +=
                    String.Format("({})", NameOfKey(InviteDeclineBind.Value));
            }
            
            if (Input.GetKeyDown(InviteAcceptBind.Value)) Player._mainPlayer.Cmd_SetPartyInviteCondition(PartyInviteStatus.ACCEPTED);
            if (Input.GetKeyDown(InviteDeclineBind.Value)) Player._mainPlayer.Cmd_SetPartyInviteCondition(PartyInviteStatus.DECLINED);

        }

        private void HandleWorldPortalScroll()
        {
            if (WorldPortalScrollConfig.Value == false) return;
            if (ZoneSelectionManager._current == null) return;
            ZoneSelectionManager zsm = ZoneSelectionManager._current;
            if (!zsm._isOpen) return;

            if (Input.mouseScrollDelta.y < 0)
            {
                int nextIndex = zsm._worldPortal._selectedPortalEntry + 1;
                if (nextIndex > zsm._worldPortal._worldPortalSlots.Length - 1) return;
                if (!Player._mainPlayer._waypointAttunements.Contains(zsm._worldPortal._worldPortalSlots[nextIndex]
                        ._mapLockID)) return;
                zsm._zoneSelectPrefabs[nextIndex].GetComponent<ZoneSelectionEntry>().Select_Entry();
            } else if (Input.mouseScrollDelta.y > 0)
            {
                if (zsm._worldPortal._selectedPortalEntry == 0) return;
                zsm._zoneSelectPrefabs[zsm._worldPortal._selectedPortalEntry - 1].GetComponent<ZoneSelectionEntry>().Select_Entry();
            }
        }

        private void InitConfig()
        {
            ConfigDefinition unlockCursorToggleDefinition = new ConfigDefinition("Features", "UnlockCursorEnabled");
            ConfigDescription unlockCursorToggleDescription = new ConfigDescription("Decline Invite");
            UnlockCursorConfig = Config.Bind(unlockCursorToggleDefinition, true, unlockCursorToggleDescription);

            ConfigDefinition inviteToggleDefinition = new ConfigDefinition("Features", "InviteBindEnable");
            ConfigDescription inviteToggleDescription = new ConfigDescription("Decline Invite");
            InviteConfig = Config.Bind(inviteToggleDefinition, true, inviteToggleDescription);

            ConfigDefinition worldPortalScrollDefinition = new ConfigDefinition("Features", "WorldPortalScrollEnabled");
            ConfigDescription worldPortalScrollDescription = new ConfigDescription("Decline Invite");
            WorldPortalScrollConfig = Config.Bind(worldPortalScrollDefinition, true, worldPortalScrollDescription);

            ConfigDefinition unlockCursorDefinition = new ConfigDefinition("Keybinds", "UnlockCursorKeybind");
            ConfigDescription unlockCursorDescription = new ConfigDescription("Unlock Mouse");
            UnlockCursorBind = Config.Bind(unlockCursorDefinition, KeyCode.LeftAlt, unlockCursorDescription);

            ConfigDefinition inviteAcceptDefinition = new ConfigDefinition("Keybinds", "InviteAcceptKeybind");
            ConfigDescription inviteAcceptDescription = new ConfigDescription("Accept Invite");
            InviteAcceptBind = Config.Bind(inviteAcceptDefinition, KeyCode.Y, inviteAcceptDescription);

            ConfigDefinition inviteDeclineDefinition = new ConfigDefinition("Keybinds", "InviteDeclineKeybind");
            ConfigDescription inviteDeclineDescription = new ConfigDescription("Decline Invite");
            InviteDeclineBind = Config.Bind(inviteDeclineDefinition, KeyCode.N, inviteDeclineDescription);
        }

        private string NameOfKey(KeyCode key)
        {
            return Enum.GetName(typeof(KeyCode), key);
        }
    }
}
