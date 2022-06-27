using Hacknet;
using Hacknet.Localization;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.IO;
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
                    foreach (var file in Directory.GetFiles("locales/Custom"))
                    {
                        Console.WriteLine("[ZeroDayToolKit] Reading Global Locale " + file);
                        LocaleAddXmlFile(file, localeCode);
                    }
                }
            });
        }

        public static bool LocaleAddXmlFile(string file, string localeCode, bool toExtension = false)
        {
            bool ret = false;
            XmlReader rdr = XmlReader.Create(File.OpenRead(LocalizedFileLoader.GetLocalizedFilepath(file)));
            while (rdr.Name != localeCode)
            {
                rdr.Read();
                if (rdr.EOF) { rdr.Close(); return false; }
            }
            rdr.Read();
            while (rdr.Name != localeCode)
            {
                if (rdr.Name.ToLower().Equals("l") && rdr.MoveToAttribute("key"))
                {
                    string key = rdr.ReadContentAsString();
                    rdr.MoveToContent();
                    string value = rdr.ReadElementContentAsString();
                    if (toExtension)
                    {
                        if (ExtensionLoaderReadCustomLocale.ExtensionLocales.ContainsKey(key)) ExtensionLoaderReadCustomLocale.ExtensionLocales[key] = value;
                        else ExtensionLoaderReadCustomLocale.ExtensionLocales.Add(key, value);
                    }
                    else
                    {
                        if (LocaleTerms.ActiveTerms.ContainsKey(key)) LocaleTerms.ActiveTerms[key] = value;
                        else LocaleTerms.ActiveTerms.Add(key, value);
                    }
                    ret = true;
                }
                if (rdr.EOF) { rdr.Close(); return ret; }
                rdr.Read();
            }
            rdr.Close();
            return ret;
        }
    }
}
