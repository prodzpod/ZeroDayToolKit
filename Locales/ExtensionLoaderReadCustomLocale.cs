using Hacknet;
using Hacknet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace ZeroDayToolKit.Locales
{
    [HarmonyLib.HarmonyPatch(typeof(ExtensionLoader), nameof(ExtensionLoader.LoadNewExtensionSession))]
    public class ExtensionLoaderReadCustomLocale
    {
        public static List<string> LocaleKeys = [];
        public static List<string> LocaleKeys2 = [];
        public static Dictionary<string, string> GlobalLocales = [];
        public static Dictionary<string, string> ExtensionLocales = [];
        static void Prefix(ref ExtensionInfo info, object os_obj)
        {
            OS os = (OS)os_obj;
            ExtensionLocales.Clear();
            LocaleKeys.Clear();
            if (Directory.Exists(info.FolderPath + "/Locales"))
            {
                bool found = false;
                Hacknet.Utils.ActOnAllFilesRevursivley(info.FolderPath + "/Locales", filename =>
                {
                    if (!filename.EndsWith(".xml")) return;
                    Console.WriteLine("[ZeroDayToolKit] Reading Extension Locale " + filename);
                    LocaleActivatorReadCustomGlobals.LocaleAddXmlFile(filename, "default", true);
                    LocaleActivatorReadCustomGlobals.LocaleAddXmlFile(filename, "en-us", true);
                    found |= LocaleActivatorReadCustomGlobals.LocaleAddXmlFile(filename, Settings.ActiveLocale, true);
                });
                if (info.Language == "dynamic")
                {
                    if (found) info.Language = Settings.ActiveLocale;
                    else info.Language = "en-us";
                    Settings.ActiveLocale = info.Language;
                }
            }
            LocaleKeys.Sort((a, b) => b.Length - a.Length);
        }
    }
}
