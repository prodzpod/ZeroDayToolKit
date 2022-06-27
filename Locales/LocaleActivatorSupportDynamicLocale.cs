using Hacknet;
using Hacknet.Localization;

namespace ZeroDayToolKit.Locales
{
    [HarmonyLib.HarmonyPatch(typeof(LocaleActivator), nameof(LocaleActivator.ActivateLocale))]
    public class LocaleActivatorSupportDynamicLocale
    {
        static void Prefix(ref string localeCode)
        {
            if (localeCode == "dynamic") localeCode = Settings.ActiveLocale;
        }
    }
}
