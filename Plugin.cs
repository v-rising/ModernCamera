using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using UnhollowerRuntimeLib;

namespace ModernCamera
{
    [BepInPlugin("Travanti.ModernCamera", "Travanti-Inc-Modern-Camera", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public override void Load()
        {
            Logger = this.Log;

            Plugin.Logger.LogWarning("register");
            ClassInjector.RegisterTypeInIl2Cpp<ModernCamera>();

            Plugin.Logger.LogWarning("AC");
            AddComponent<ModernCamera>();

            //  RegisterTypeInIl2Cpp();

            Plugin.Logger.LogWarning("HARM");
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("Travanti.ModernCamera");
            harmony.PatchAll();
            // Plugin startup logic
            Log.LogInfo($"Plugin Travanti - Inc - Modern - Camera is loaded!");
        }
        public void RegisterTypeInIl2Cpp()
        {
            
        }
    }
}
