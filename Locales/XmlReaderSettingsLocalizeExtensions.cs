using Hacknet;
using HarmonyLib;
using MonoMod.Cil;
using Pathfinder.Port;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ZeroDayToolKit.Locales
{

    [HarmonyLib.HarmonyPatch(typeof(XmlReaderSettings), "CreateReader", typeof(TextReader), typeof(string), typeof(XmlParserContext))]
    internal class XmlReaderSettingsLocalizeExtensions
    {
        static void Prefix(ref TextReader input, string baseUriString, XmlParserContext inputContext)
        {
            if (input == null) return;
            string replMe = input.ReadToEnd();
            input.Close();
            if (replMe.Contains("Language=\"dynamic\""))
            {
                StringBuilder res = new("", replMe.Length);
                bool mode = false;
                string key = "";
                for (var i = 0; i < replMe.Length; i++)
                {
                    replMe = replMe.Replace("Language=\"dynamic\"", "Language=\"" + Settings.ActiveLocale + "\"");
                    if (mode)
                    {
                        if (i < replMe.Length - 3 && replMe.Substring(i, 4) == "\\\\}}")
                        {
                            mode = false;
                            res.Append("\\");
                            key = "";
                            i += 3;
                        }
                        else if (i < replMe.Length - 2 && replMe.Substring(i, 3) == "\\}}")
                        {
                            key += "}}";
                            i += 2;
                        }
                        else if (i < replMe.Length - 1 && replMe.Substring(i, 2) == "}}")
                        {
                            mode = false;
                            res.Append(localizeThis(key));
                            key = "";
                            i++;
                        }
                        else key += replMe[i];
                    }
                    else
                    {
                        if (i < replMe.Length - 3 && replMe.Substring(i, 4) == "\\\\{{")
                        {
                            mode = true;
                            res.Append("\\");
                            i += 3;
                        }
                        else if (i < replMe.Length - 2 && replMe.Substring(i, 3) == "\\{{")
                        {
                            res.Append("{{");
                            i += 2;
                        }
                        else if (i < replMe.Length - 1 && replMe.Substring(i, 2) == "{{")
                        {
                            mode = true;
                            i++;
                        }
                        else res.Append(replMe[i]);
                    }
                }
                res.Append(localizeThis(key));
                input = new StringReader(res.ToString());
            }
            else input = new StringReader(replMe);
        }

        public static string localizeThis(string key)
        {
            if (key == "") return "";
            var ret = LocaleTerms.Loc(key); // complete overlaps (i.e. overrides) are covered below
            if (!key.Equals(ret)) return ret;
            foreach (string k in ExtensionLoaderReadCustomLocale.LocaleKeys) key = key.Replace(k, ExtensionLoaderReadCustomLocale.ExtensionLocales[k]);
            foreach (string k in ExtensionLoaderReadCustomLocale.LocaleKeys2) key = key.Replace(k, ExtensionLoaderReadCustomLocale.GlobalLocales[k]);
            return key;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(LocaleTerms), nameof(LocaleTerms.Loc))]
    public class LocalizeOverride
    {
        public static bool Prefix(string input, ref string __result)
        {
            if (ExtensionLoaderReadCustomLocale.ExtensionLocales.ContainsKey(input))
            {
                __result = ExtensionLoaderReadCustomLocale.ExtensionLocales[input];
                return false;
            }
            return true;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.write))]
    public class LocalizeOSWrites
    {
        static void Prefix(ref string text)
        {
            text = XmlReaderSettingsLocalizeExtensions.localizeThis(text);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.writeSingle))]
    public class LocalizeOSWriteSingles
    {
        static void Prefix(ref string text)
        {
            text = XmlReaderSettingsLocalizeExtensions.localizeThis(text);
        }
    }

    [HarmonyLib.HarmonyPatch]
    public class LocalizePortString
    {
        static void ILManipulator(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchLdstr("Port#: {0}")) && c.TryGotoNext(x => x.MatchStloc(out _))) c.EmitDelegate<Func<string, string>>(XmlReaderSettingsLocalizeExtensions.localizeThis);
            if (c.TryGotoNext(x => x.MatchLdstr(" - ")) && c.TryGotoNext(x => x.MatchStloc(out _))) c.EmitDelegate<Func<string, string>>(XmlReaderSettingsLocalizeExtensions.localizeThis);
        }

        static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(ComputerExtensions).GetNestedType("<>c", AccessTools.all)).Find(x => x.GetParameters().Length == 4);
        }
    }
}
