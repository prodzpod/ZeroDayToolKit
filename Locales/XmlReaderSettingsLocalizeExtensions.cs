using Hacknet;
using System;
using System.IO;
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
            string res = "";
            bool mode = false;
            string key = "";
            for (var i = 0; i < replMe.Length; i++)
            {
                if (mode)
                {
                    if (i < replMe.Length - 3 && replMe.Substring(i, 4) == "\\\\}}")
                    {
                        key += "\\";
                        mode = false;
                        res += localizeThis(key);
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
                        res += localizeThis(key);
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
                        res += "\\";
                        i += 3;
                    }
                    else if (i < replMe.Length - 2 && replMe.Substring(i, 3) == "\\{{")
                    {
                        res += "{{";
                        i += 2;
                    }
                    else if (i < replMe.Length - 1 && replMe.Substring(i, 2) == "{{")
                    {
                        mode = true;
                        i++;
                    }
                    else res += replMe[i];
                }
            }
            res += localizeThis(key);
            input = new StringReader(res);
        }

        public static string localizeThis(string key)
        {
            if (key == "") return "";
            if (ExtensionLoaderReadCustomLocale.ExtensionLocales.ContainsKey(key)) return ExtensionLoaderReadCustomLocale.ExtensionLocales[key];
            return LocaleTerms.Loc(key);
        }
    }
}
