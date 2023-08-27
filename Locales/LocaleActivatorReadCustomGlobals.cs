using Hacknet;
using Hacknet.Gui;
using Hacknet.Localization;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace ZeroDayToolKit.Locales
{
    [HarmonyPatch(typeof(LocaleActivator), nameof(LocaleActivator.ActivateLocale))]
    public class LocaleActivatorReadCustomGlobals
    {
        internal static void ILManipulator(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(MoveType.After, x => x.MatchCall(AccessTools.Method(typeof(LocaleTerms), nameof(LocaleTerms.ReadInTerms))), x => x.MatchNop(), x => x.MatchNop(), x => x.MatchNop(), x => x.MatchNop());
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Action<string>>((localeCode) =>
            {
                if (Directory.Exists("locales/Custom"))
                {
                    ExtensionLoaderReadCustomLocale.GlobalLocales.Clear();
                    ExtensionLoaderReadCustomLocale.LocaleKeys2.Clear();
                    foreach (var file in Directory.GetFiles("locales/Custom"))
                    {
                        Console.WriteLine("[ZeroDayToolKit] Reading Global Locale " + file);
                        LocaleAddXmlFile(file, localeCode);
                    }
                    ExtensionLoaderReadCustomLocale.LocaleKeys2.Sort((a, b) => b.Length - a.Length);
                }
            });
        }

        public static bool LocaleAddXmlFile(string file, string localeCode, bool toExtension = false)
        {
            bool ret = false;
            XmlReader rdr = XmlReader.Create(File.OpenRead(LocalizedFileLoader.GetLocalizedFilepath(file)));
            while (rdr.Name != localeCode && rdr.Name != "default")
            {
                rdr.Read();
                if (rdr.EOF) { rdr.Close(); return false; }
            }
            rdr.Read();
            while (rdr.Name != localeCode && rdr.Name != "default")
            {
                if (rdr.Name.ToLower().Equals("l") && rdr.MoveToAttribute("key"))
                {
                    string key = rdr.ReadContentAsString();
                    bool exact = false;
                    if (rdr.MoveToAttribute("exact")) exact = rdr.ReadContentAsBoolean();
                    rdr.MoveToContent();
                    string value = rdr.ReadElementContentAsString();
                    if (toExtension)
                    {
                        if (ExtensionLoaderReadCustomLocale.ExtensionLocales.ContainsKey(key)) ExtensionLoaderReadCustomLocale.ExtensionLocales[key] = value;
                        else
                        {
                            if (!exact) ExtensionLoaderReadCustomLocale.LocaleKeys.Add(key);
                            ExtensionLoaderReadCustomLocale.ExtensionLocales.Add(key, value);
                        }
                    }
                    else
                    {
                        if (LocaleTerms.ActiveTerms.ContainsKey(key)) LocaleTerms.ActiveTerms[key] = value;
                        else LocaleTerms.ActiveTerms.Add(key, value);
                        if (!ExtensionLoaderReadCustomLocale.GlobalLocales.ContainsKey(key))
                        {
                            if (!exact) ExtensionLoaderReadCustomLocale.LocaleKeys2.Add(key);
                            ExtensionLoaderReadCustomLocale.GlobalLocales.Add(key, value);
                        }
                    }
                    ret = true;
                }
                if (rdr.EOF) { rdr.Close(); return ret; }
                rdr.Read();
            }
            rdr.Close();
            return ret;
        }

        [HarmonyPatch(typeof(SelectableTextList), nameof(SelectableTextList.doFancyList))]
        public class LocalizeDropdown
        {
            public static void Prefix(ref string[] text)
            {
                text = text.Select(x => XmlReaderSettingsLocalizeExtensions.localizeThis(x)).ToArray();
            }
        }
    }
}
