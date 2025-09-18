using Hacknet;
using Hacknet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDayToolKit.Locales
{
    [HarmonyLib.HarmonyPatch(typeof(LocaleTerms), nameof(LocaleTerms.Loc))]
    public class ExtensionLocaleSupport
    {
        public static void Postfix(string input, ref string __result)
        {
            if (ExtensionLoaderReadCustomLocale.ExtensionLocales.TryGetValue(input, out var a)) __result = a;
            return;
        }
        public static bool Contains(string input)
        {
            return ExtensionLoaderReadCustomLocale.ExtensionLocales.ContainsKey(input) || LocaleTerms.ActiveTerms.ContainsKey(input);
        }
    }
}
