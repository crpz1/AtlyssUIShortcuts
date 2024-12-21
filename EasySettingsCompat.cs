using Nessie.ATLYSS.EasySettings;

namespace AtlyssUIShortcuts
{
    public static class EasySettingsCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("EasySettings");
                }

                return (bool)_enabled;
            }
        }

        public static void SetupSettings()
        {
            Settings.OnInitialized.AddListener(AddSettings);
            Settings.OnApplySettings.AddListener(() => { AtlyssUIShortcuts.instance.Config.Save(); });
        }

        private static void AddSettings()
        {
            SettingsTab tab = Settings.ModTab;
            tab.AddHeader("UIShortcuts");

            tab.AddToggle(AtlyssUIShortcuts.UnlockCursorConfig);
            tab.AddToggle(AtlyssUIShortcuts.InviteConfig);
            tab.AddToggle(AtlyssUIShortcuts.WorldPortalScrollConfig);

            tab.AddSpace();

            tab.AddKeyButton(AtlyssUIShortcuts.UnlockCursorBind);
            tab.AddKeyButton(AtlyssUIShortcuts.InviteAcceptBind);
            tab.AddKeyButton(AtlyssUIShortcuts.InviteDeclineBind);
        }
    }
}