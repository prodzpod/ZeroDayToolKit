using Hacknet;
using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ZeroDayToolKit.Compat.Stuxnet;
using ZeroDayToolKit.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(ComputerLoader), nameof(ComputerLoader.filter))]
    public class ImageFile
    {
        public static Dictionary<string, string> Binaries = [];
        public static Dictionary<string, Texture2D> Textures = [];
        public static void Postfix(ref string __result)
            { __result = new Regex("#0DTK_IMAGE:[^#]+#").Replace(__result, m => GetFile(m.Captures[0].Value.Substring("#0DTK_IMAGE:".Length, m.Captures[0].Value.Length - "#0DTK_IMAGE:#".Length).Trim())); }
        public static string GetFile(string path)
        {
            if (!Binaries.ContainsKey(path))
            {
                Random _r = Hacknet.Utils.random;
                Hacknet.Utils.random = new Random(path.GetHashCode());
                ZeroDayToolKit.Instance.Log.LogInfo($"Loading image for {path} ({path.GetHashCode()})");
                Binaries[path] = Computer.generateBinaryString(500);
                Hacknet.Utils.random = _r;
            }
            return Binaries[path];
        }
        public static void DrawImage(string path, Rectangle rect, SpriteBatch sb)
        {
            if (!Textures.ContainsKey(path))
            {
                using var stream = File.OpenRead(Hacknet.Utils.GetFileLoadPrefix() + path);
                Textures[path] = Texture2D.FromStream(Game1.singleton.GraphicsDevice, stream);
            }
            Hacknet.Utils.DrawSpriteAspectCorrect(rect, sb, Textures[path], Color.White);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(DisplayModule), nameof(DisplayModule.doCatDisplay))]
    public class ImageCatDisplay
    {
        public static void ILManipulator(ILContext il)
        {
            ILCursor c = new(il);
            int rect = -1;
            c.GotoNext(x => x.MatchLdsfld(typeof(GuiData), nameof(GuiData.tmpRect)), x => x.MatchStloc(out rect));
            c.GotoNext(x => x.MatchRet());
            var ret = c.MarkLabel();
            c.GotoPrev(x => x.MatchLdfld<DisplayModule>(nameof(DisplayModule.LastDisplayedFileFolder)));
            ILLabel start = default;
            c.GotoNext(x => x.MatchBrtrue(out start));
            c.GotoLabel(start);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<DisplayModule, bool>>(self => ImageFile.Binaries.Values.Any(x => x == Unspace(self.os.displayCache)));
            var end = c.MarkLabel(); c.MoveBeforeLabels();
            c.Emit(OpCodes.Brfalse, end);
            c.Emit(OpCodes.Ldarg_0); // DisplayModule
            c.Emit(OpCodes.Ldloc, rect); // rect
            c.EmitDelegate<Action<DisplayModule, Rectangle>>((self, rect) =>
            {
                var path = ImageFile.Binaries.Keys.First(x => ImageFile.Binaries[x] == Unspace(self.os.displayCache));
                ImageFile.DrawImage(path, new Rectangle(self.x, self.y, self.bounds.Width - 70, self.bounds.Height), GuiData.spriteBatch);
                var dest = new Rectangle(rect.X + 4, rect.Y + 55, rect.Width - 6, rect.Height - 55 - 2);
                self.y += 70;
                self.catTextRegion.Panel.ScrollDown = 0;
                self.catTextRegion.Panel.PanelHeight = dest.Height;
                self.catTextRegion.Panel.NumberOfPanels = 1;
                self.catTextRegion.Panel.Draw((idx, dest, sb) => ImageFile.DrawImage(path, dest, sb), self.spriteBatch, dest);
            });
            c.Emit(OpCodes.Br, ret);
        }
        public static string Unspace(string str) => new Regex(@"[\s\n\r]+").Replace(str, "");
    }

    [HarmonyLib.HarmonyPatch(typeof(IRCSystem), nameof(IRCSystem.DrawLogEntry))]
    public class ImageIRCDisplay
    {
        public static int MAX_IMAGE_SIDE = 300;
        public static int Width = 0;
        public static int Height = 0;
        public static SpriteBatch SpriteBatch;
        public static void ILManipulator(ILContext il)
        {
            ILCursor c = new(il);
            int lineHeight = -1, sb = -1;
            c.GotoNext(x => x.MatchLdfld<Rectangle>(nameof(Rectangle.Height)), x => x.MatchLdarg(out lineHeight), x => x.MatchSub());
            c.GotoNext(x => x.MatchCallOrCallvirt<SpriteBatch>(nameof(SpriteBatch.Draw)));
            c.GotoPrev(x => x.MatchLdarg(out sb));
            c.Index = 0;
            c.Emit(OpCodes.Ldarg_1);
            c.Emit(OpCodes.Ldarg, lineHeight);
            c.Emit(OpCodes.Ldarg, sb);
            c.EmitDelegate<Func<IRCSystem.IRCLogEntry, int, SpriteBatch, int>>((log, val, sb) =>
            {
                if (string.IsNullOrEmpty(log.Message)) log.Message = "";
                if (!log.Message.StartsWith("!ATTACHMENT:")) return val;
                var args = log.Message.Substring("!ATTACHMENT:".Length).Split(["#%#"], StringSplitOptions.None);
                if (args[0] != "image") return val;
                var path = args[2];
                if (!ImageFile.Textures.ContainsKey(path))
                {
                    using var stream = File.OpenRead(Hacknet.Utils.GetFileLoadPrefix() + path);
                    ImageFile.Textures[path] = Texture2D.FromStream(Game1.singleton.GraphicsDevice, stream);
                }
                var width = ImageFile.Textures[path].Width;
                var height = ImageFile.Textures[path].Height;
                if (width >= height) { Width = MAX_IMAGE_SIDE; Height = Width * height / width; }
                else { Height = MAX_IMAGE_SIDE; Width = Height * width / height; }
                SpriteBatch = sb;
                return Math.Max(val, Height);
            });
            c.Emit(OpCodes.Starg, lineHeight);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(AttachmentRenderer), nameof(AttachmentRenderer.RenderAttachment))]
    public class ImageIRCAttachment
    {
        public static bool Prefix(string data, object osObj, Vector2 dpos, int startingButtonIndex, SoundEffect buttonSound, bool __result)
        {
            OS os = (OS)osObj;
            string[] args = data.Split(["#%#"], StringSplitOptions.RemoveEmptyEntries);
            if (args[0] == "file")
            {
                Vector2 labelSize = TextItem.doMeasuredTinyLabel(dpos, LocaleTerms.Loc(StuxnetCompat.IsRadioFile(args[2]) ? "AUDIO" : "FILE") + " : " + args[1], null);
                if (Button.doButton(802009 + startingButtonIndex, (int)(dpos.X + labelSize.X + 5f), (int)dpos.Y, 20, 17, "+", null))
                {
                    DownloadFile(os, args[1], args[2]);
                    if (StuxnetCompat.IsRadioFile(args[2])) StuxnetCompat.InstallRadio(os, args[2]);
                    if (buttonSound != null && !Settings.soundDisabled) buttonSound.Play();
                }
                return false;   
            }
            else if (args[0] == "image")
            {
                ImageFile.DrawImage(args[2], new Rectangle((int)dpos.X, (int)dpos.Y - ImageIRCDisplay.Height + 17, ImageIRCDisplay.Width, ImageIRCDisplay.Height), ImageIRCDisplay.SpriteBatch);
                if (Button.doButton(803009 + startingButtonIndex, (int)(dpos.X + ImageIRCDisplay.Width + 5f), (int)dpos.Y, 20, 17, "+", null))
                {
                    DownloadFile(os, args[1], ImageFile.GetFile(args[2]));
                    if (buttonSound != null && !Settings.soundDisabled) buttonSound.Play();
                }
                return false;
            }
            return true;
        }

        public static void DownloadFile(OS os, string name, string data)
        {
            Folder home = os.thisComputer.files.root.searchForFolder("home");
            name = ComUtils.getNoDupeFileName(name, home);
            os.write("Copying Remote File " + name + "\nto Local Folder /home");
            os.write(".");
            for (int i = 0; i < Math.Min(data.Length / 200, 20); i++)
            {
                os.writeSingle(".");
                Thread.Sleep(200);  
            }
            home.files.Add(new FileEntry(data, name));
            os.write("Transfer Complete\n");
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(MemoryForensicsExe), nameof(MemoryForensicsExe.RenderResultsDisplayMainState))]
    public class ImageMemDumps
    {
        public static List<KeyValuePair<Rectangle, int>> ButtonsToDraw = [];
        public static void ILManipulator(ILContext il) 
        {
            ILCursor c = new(il);
            c.EmitDelegate(ButtonsToDraw.Clear);
            int _rect2 = -1, _rect = -1, _i = -1;
            c.GotoNext(x => x.MatchLdstr("No Valid Matches Found"));
            c.GotoPrev(x => x.MatchLdloc(out _rect2));
            c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(Hacknet.Utils), nameof(Hacknet.Utils.DrawSpriteAspectCorrect)));
            c.GotoPrev(x => x.MatchStfld<Rectangle>(nameof(Rectangle.Height)));
            c.GotoNext(x => x.MatchLdloc(out _rect));
            c.GotoNext(x => x.MatchLdcI4(1), x => x.MatchAdd(), x => x.MatchStloc(out _i));
            c.GotoPrev(MoveType.After, x => x.MatchCallOrCallvirt(typeof(Hacknet.Utils), nameof(Hacknet.Utils.DrawSpriteAspectCorrect)));
            c.Emit(OpCodes.Ldloc, _rect);
            c.Emit(OpCodes.Ldloc, _i);
            c.EmitDelegate<Action<Rectangle, int>>((rect, i) => ButtonsToDraw.Add(new(rect, i)));
            c.GotoNext(x => x.MatchCallOrCallvirt(typeof(ScrollablePanel), nameof(ScrollablePanel.endPanel)));
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, _rect2);
            // janky copyover
            c.EmitDelegate<Action<MemoryForensicsExe, Rectangle>>((self, rect) =>
            {
                var mouse = GuiData.getMousePoint();
                // ZeroDayToolKit.Instance.Log.LogInfo($"{mouse.X}, {mouse.Y}");
                foreach (var b in ButtonsToDraw)
                {
                    var id = 381023909 + b.Value + 1;
                    Button.drawModernButton(id, b.Key.X + 8, b.Key.Y + 8, 60, 30, "Download", null, Hacknet.Utils.white);
                    if (GuiData.hot == id && !GuiData.blockingInput && GuiData.active == id && (GuiData.mouseLeftUp() || GuiData.mouse.LeftButton == ButtonState.Released))
                    {
                        ImageIRCAttachment.DownloadFile(self.os, self.filenameLoaded + "_image_" + b.Value + (self.OutputData[b.Value].EndsWith(".png") ? ".png" : ".jpg"), ImageFile.GetFile(self.OutputData[b.Value]));
                        GuiData.active = -1;
                    }
                    Rectangle tmpRect = GuiData.tmpRect;
                    tmpRect.X = b.Key.X + 8 + rect.X;
                    tmpRect.Y = b.Key.Y + 8 + rect.Y;
                    tmpRect.Width = 60;
                    tmpRect.Height = 30;
                    // ZeroDayToolKit.Instance.Log.LogInfo($"Button: {tmpRect.X} ~ {tmpRect.X + tmpRect.Width}, {tmpRect.Y} ~ {tmpRect.Y + tmpRect.Height}");
                    if (tmpRect.Contains(mouse) && !GuiData.blockingInput)
                    {
                        GuiData.hot = id;
                        if (GuiData.isMouseLeftDown() && (!Button.DisableIfAnotherIsActive || GuiData.active == -1)) GuiData.active = id;
                    }
                    else
                    {
                        if (GuiData.hot == id) GuiData.hot = -1;
                        if (GuiData.isMouseLeftDown() && GuiData.active == id && GuiData.active == id) GuiData.active = -1;
                    }
                }
            });
        }
    }
} 
