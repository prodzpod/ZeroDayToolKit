using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Hacknet;
using Hacknet.Gui;
using Hacknet.Daemons.Helpers;
using Hacknet.Extensions;
using BepInEx;
using BepInEx.Hacknet;
using Pathfinder.Util;
using Pathfinder.Port;
using Pathfinder.Meta.Load;
using Pathfinder.Event.Saving;
using Pathfinder.Replacements;
using Pathfinder.Util.XML;

namespace HacknetPluginTemplate
{
    [BepInPlugin(ModGUID, ModName, ModVer)]
    public class Mod : HacknetPlugin
    {
        public const string ModGUID = "kr.o_r.prodzpod.ModName";
        public const string ModName = "ModName";
        public const string ModVer = "0.1.0";
        static public Random rnd;
        public ModCommands cmd = new ModCommands();

        public override bool Load()
        {
            rnd = new Random();
            PortManager.RegisterPort("backdoor", "Backdoor Connection", 0);
            PortManager.RegisterPort("mqtt", "MQTT Protocol", 1883); // IoT Devices
            PortManager.RegisterPort("ntp", "Network Time Protocol", 123); // Digital Clocks > Timezones > VPN
            PortManager.RegisterPort("docker", "Docker Plaintext API", 2375);
            PortManager.RegisterPort("telnet", "Telnet Protocol", 23); // obviously these ports aint done yet
            Pathfinder.Executable.ExecutableManager.RegisterExecutable<ModExecutibles.SSHSwiftEXE>("#SSH_SWIFT#");
            Pathfinder.Executable.ExecutableManager.RegisterExecutable<ModExecutibles.PacketHeaderInjectionEXE>("#PACKET_HEADER_INJECTION#");
            Pathfinder.Executable.ExecutableManager.RegisterExecutable<ModExecutibles.SQLTXCrasherEXE>("#SQL_TX_CRASHER#");
            Pathfinder.Executable.ExecutableManager.RegisterExecutable<ModExecutibles.PortBackdoorEXE>("#PORT_BACKDOOR#");
            Pathfinder.Command.CommandManager.RegisterCommand("mkdir", cmd.mkdir);
            Pathfinder.Command.CommandManager.RegisterCommand("touch", cmd.touch);
            Pathfinder.Command.CommandManager.RegisterCommand(">", cmd.sendIRC);
            Pathfinder.Action.ActionManager.RegisterAction<ModConditions.SAResetIRCDelay>("ResetIRCDelay");
            Pathfinder.Action.ActionManager.RegisterAction<ModConditions.SASetNumberOfChoices>("SetNumberOfChoices");
            Pathfinder.Action.ActionManager.RegisterAction<ModConditions.SARunCommand>("RunCommand");
            Pathfinder.Action.ActionManager.RegisterAction<ModConditions.SASetRAM>("SetRAM");
            Pathfinder.Action.ConditionManager.RegisterCondition<ModConditions.SCOnIRCMessageAny>("OnIRCMessageAny");
            Pathfinder.Action.ConditionManager.RegisterCondition<ModConditions.SCOnIRCMessage>("OnIRCMessage");
            Pathfinder.Action.ConditionManager.RegisterCondition<ModConditions.SCOnIRCMessageTone>("OnIRCMessageTone");
            Pathfinder.Action.ConditionManager.RegisterCondition<ModConditions.SCOnHostileActionTaken>("OnHostileActionTaken");
            Pathfinder.Action.ConditionManager.RegisterCondition<ModConditions.SCOnReboot>("OnReboot");
            Pathfinder.Action.ConditionManager.RegisterCondition<ModConditions.SCOnCrash>("OnCrash");
            Pathfinder.Action.ConditionManager.RegisterCondition<ModConditions.SCOnRebootCompleted>("OnRebootCompleted");
            SaveLoader.RegisterExecutor<ModPatches.NetworkExecutor>("HacknetSave.TraceV2", ParseOption.ParseInterior);
            Console.WriteLine("//////////////////////////////////// PRODZPOD");
            Console.WriteLine("/////////////////////////////////// ZERODAY");
            Console.WriteLine("////////////////////////////////// TOOLKIT");
            HarmonyInstance.PatchAll();
            return true;
        }
    }

    public class ModExecutibles
    {
        public class ModEXE : Pathfinder.Executable.BaseExecutable
        {
            public override string GetIdentifier() => IdentifierName;
            public int originPort = 0;
            public int port;
            public string startMessage = "";
            public string endMessage = "";
            public bool noProxy = true;
            public bool noHostile = false;
            public float life = 0f;
            public float runTime = 5f;
            public float exitTime = 0f;
            public bool done = false;
            public ModEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args) { }
            public void Init(int port, float runTime, int ramCost, string id, string startMessage, string endMessage) { Init(port, runTime, 0f, ramCost, id, startMessage, endMessage); }
            public void Init(int port, float runTime, float exitTime, int ramCost, string id, string startMessage, string endMessage)
            {
                originPort = port;
                this.runTime = runTime;
                this.exitTime = exitTime;
                this.ramCost = ramCost;
                IdentifierName = id;
                this.startMessage = startMessage;
                this.endMessage = endMessage;
            }

            public override void LoadContent()
            {
                base.LoadContent();
                Computer c = ComputerLookup.FindByIp(targetIP);
                port = c.GetDisplayPortNumberFromCodePort(originPort);
                if (noProxy && c.proxyActive)
                {
                    os.write("Proxy Active");
                    os.write("Execution failed");
                    needsRemoval = true;
                }
                else if (Args.Length <= 1)
                {
                    os.write("No port number Provided");
                    os.write("Execution failed");
                    needsRemoval = true;
                }
                else if (!int.TryParse(Args[1], out _) || int.Parse(Args[1]) != port || !ModUtils.isPortOpen(c, originPort))
                {
                    os.write("Target Port is Closed");
                    os.write("Execution failed");
                    needsRemoval = true;
                }
                else foreach (string line in startMessage.Split('\n')) os.write(line);
                if (!noHostile) Programs.getComputer(os, targetIP).hostileActionTaken();
            }
            public override void Update(float t)
            {
                if (!done && life >= runTime)
                {
                    done = true;
                    Programs.getComputer(os, targetIP).openPort(port, os.thisComputer.ip);
                    foreach (string line in endMessage.Split('\n')) os.write(line);
                }
                else if (!isExiting && life >= (runTime + exitTime)) isExiting = true;
                incrementLife(t);
                base.Update(t);
            }

            public virtual void incrementLife(float t)
            {
                life += t;
            }
            public override void Draw(float t)
            {
                base.Draw(t); drawTarget(); drawOutline();
            }
            public void drawLine(Vector2 origin, Vector2 dest, Color c)
            {
                ModUtils.drawLine(spriteBatch, origin, dest, c);
            }
        }

        public class SSHSwiftEXE : ModEXE
        {
            public SSHSwiftEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
            {
                Init(22, 4f, 200, ">S>.SSH.$w!f7.>S>", ">S> Let's do this. >S>", ">S> SSH? Swiftly Shafted. Thank me later. >S>");
            }

            public override void incrementLife(float t)
            {
                life += t * (float)Mod.rnd.NextDouble() * 2;
            }

            private const int WIDTH = 8;
            private const int HEIGHT = 8;
            public override void Draw(float t)
            {
                TextItem.doFontLabel(new Vector2(bounds.X + 2, bounds.Y + 14), done ? "And done." : "Sit back and watch it go.", GuiData.UITinyfont, new Color?(Utils.AddativeWhite * 0.8f * fade), bounds.Width - 6);
                Rectangle source = new Rectangle(0, 0, Bounds.Width / WIDTH - 4, Bounds.Height / HEIGHT - 8);
                for (int x = 0; x < WIDTH; x++)
                {
                    source.X = Bounds.X + 4 + (x * (Bounds.Width - 4) / WIDTH);
                    for (int y = 0; y < HEIGHT; y++) // grid
                    {
                        source.Y = Bounds.Y + 32 + (y * (Bounds.Height - 30) / HEIGHT);
                        Rectangle destination = source;
                        float progress = Math.Max(0f, 1f - ((1f - ((float)(x + y) / (WIDTH + HEIGHT))) * 0.5f + (life / runTime)));
                        if (Mod.rnd.NextDouble() < progress) progress = (float)Mod.rnd.NextDouble();
                        Color color = Color.Lerp(os.unlockedColor, os.brightLockedColor, progress);
                        int num = (int)(progress * 99);
                        float offsetx = (Bounds.Width * 0.5f / WIDTH) - ((0.5f + num.ToString().Length) * 3.75f);
                        float offsety = (Bounds.Height * 0.5f / HEIGHT) - 12f;
                        spriteBatch.Draw(Utils.white, destination, color * fade);
                        spriteBatch.DrawString(GuiData.UITinyfont, num.ToString(), new Vector2(source.X + offsetx, source.Y + offsety), Color.White * fade);
                    }
                }
                base.Draw(t);
            }
        }

        public class PacketHeaderInjectionEXE : ModEXE
        {
            public PacketHeaderInjectionEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
            {
                Init(80, 30f, 50, ":pkthdr::", "[pkthdr.exe] onLoad()::", "[pkthdr.exe] onComplete()::");
            }

            private const float PERIOD = 5f;
            public override void Draw(float t)
            {
                float offset = (life % PERIOD) / PERIOD;
                bool flip = life % (PERIOD * 2) < PERIOD;
                float temp = (1f - (life / runTime));
                int bound = (int)(120f * (1f - (temp * temp))) + 1;
                float unitWidth = Bounds.Width / bound;
                float unitHeight = Bounds.Height - 14;
                Vector2 pre = new Vector2(Bounds.X, Bounds.Y + 14f + (flip ? (offset * unitHeight) : ((1 - offset) * unitHeight)));
                Vector2 post;
                for (int i = 0; i < bound; i++)
                {
                    post = new Vector2(Bounds.X + ((i + 1) * unitWidth), Bounds.Y + 14f + (unitHeight * (float)Mod.rnd.NextDouble()));
                    drawLine(pre, post, os.defaultHighlightColor * fade * 0.5f);
                    pre = post;
                }
                pre = new Vector2(Bounds.X, Bounds.Y + 14f + (flip ? (offset * unitHeight) : ((1 - offset) * unitHeight)));
                post = new Vector2(Bounds.X + offset * unitWidth, Bounds.Y + 14f + (flip ? unitHeight : 0));
                for (int i = 0; i < bound; i++)
                {
                    drawLine(pre, post, Color.White * fade);
                    pre = post;
                    post = new Vector2(Bounds.X + (offset + i + 1f) * unitWidth, Bounds.Y + 14f + ((flip ^ (i % 2 == 0)) ? 0f : unitHeight));
                }
                drawLine(pre, new Vector2(Bounds.X + Bounds.Width, Bounds.Y + 14f + (flip ? ((1f - offset) * unitHeight) : (offset * unitHeight))), Color.White * fade);
                base.Draw(t);
            }
        }

        public class SQLTXCrasherEXE : ModEXE
        {
            public SQLTXCrasherEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
            {
                Init(1433, 10f, 10f, 240, "THE_LAST_RESORT", "" +
                    "================================\n" +
                    "//                                             \\\\\n" +
                    "\\\\      SQL_TX_Crasher v0.0.5       //\n" +
                    "//       a.k.a. THE LAST RESORT       \\\\\n" +
                    "\\\\                                             //\n" +
                    "================================", "" +
                    "================================\n" +
                    "//                                            \\\\\n" +
                    "\\\\           PROCESS                      //\n" +
                    "//                FINISHED                \\\\\n" +
                    "\\\\                                            //\n" +
                    "================================");
            }
            public override void Update(float t)
            {
                if (!done && life >= runTime)
                {
                    done = true;
                    Computer c = Programs.getComputer(os, targetIP);
                    c.openPort(port, os.thisComputer.ip);
                    foreach (string line in endMessage.Split('\n')) os.write(line);
                    Console.WriteLine(c.hasProxy.ToString() + ", " + c.firewall.ToString());
                    if (c.hasProxy)
                    {
                        c.proxyOverloadTicks = c.startingOverloadTicks;
                        c.proxyActive = true;
                        os.write("THREAT DETECTED: Proxy Reenabled");
                    }
                    if (c.firewall != null)
                    {
                        c.firewall.solution = Utils.FlipRandomChars(c.firewall.solution, 0.5);
                        c.firewall.solved = false;
                        c.firewall.resetSolutionProgress();
                        os.write("THREAT DETECTED: Firewall Reenabled");
                    }
                }
                base.Update(t);
            }

            public override void Draw(float t)
            {
                int minb = Math.Max(0, Math.Min(Bounds.Width, Bounds.Height - 14) / 2);
                Vector2 center = new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + 7 + (Bounds.Height / 2));
                Rectangle bound = new Rectangle(Bounds.X, Bounds.Y + 14, Bounds.Width, Bounds.Height - 14);
                spriteBatch.Draw(Utils.white, new Rectangle(Bounds.X, Bounds.Y + 7 + (Bounds.Height / 2) - 5, Bounds.Width, 10), (done ? os.unlockedColor : os.lockedColor) * 0.5f * fade);
                for (float q = 0.1f; q <= 1f; q += 0.1f)
                {
                    float angle = (float)Math.Sin((q * 4 + life) / 2.5f);
                    Color color = Color.Lerp(Color.White, done ? os.brightUnlockedColor : os.brightLockedColor, Math.Abs(angle)) * q * fade;
                    Color color2 = Color.Lerp(Color.White, done ? os.unlockedColor : os.lockedColor, Math.Abs(angle)) * q * fade;
                    Rectangle dest = new Rectangle((int)center.X, (int)center.Y, minb, minb);
                    spriteBatch.Draw(Utils.white, dest, new Rectangle?(), color, (float)Math.PI * angle, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
                    Vector2 top = new Vector2(center.X + (minb * (float)Math.Sin(Math.PI * angle)), center.Y - (minb * (float)Math.Cos(Math.PI * angle)));
                    Vector2 left = new Vector2(center.X - (minb * (float)Math.Cos(Math.PI * angle)), center.Y - (minb * (float)Math.Sin(Math.PI * angle)));
                    Vector2 bottom = new Vector2(center.X - (minb * (float)Math.Sin(Math.PI * angle)), center.Y + (minb * (float)Math.Cos(Math.PI * angle)));
                    Vector2 right = new Vector2(center.X + (minb * (float)Math.Cos(Math.PI * angle)), center.Y + (minb * (float)Math.Sin(Math.PI * angle)));
                    Vector2 a, b;
                    ModUtils.ClipLineSegmentsInRect(bound, top, left, out a, out b);
                    drawLine(a, b, color2);
                    ModUtils.ClipLineSegmentsInRect(bound, left, bottom, out a, out b);
                    drawLine(a, b, color2);
                    ModUtils.ClipLineSegmentsInRect(bound, bottom, right, out a, out b);
                    drawLine(a, b, color2);
                    ModUtils.ClipLineSegmentsInRect(bound, right, top, out a, out b);
                    drawLine(a, b, color2);
                }
                Rectangle midbar = new Rectangle(Bounds.X, Bounds.Y + 9 + (Bounds.Height / 3), Bounds.Width, Bounds.Height / 3);
                spriteBatch.Draw(Utils.white, midbar, Color.Black * 0.5f * fade);
                if ((!done && (Mod.rnd.NextDouble() < (life / runTime))) || (done && (Mod.rnd.NextDouble() > ((life - runTime) / exitTime))))
                    TextItem.doCenteredFontLabel(midbar, "-  " + (done ? "Completed." : "Crashing...") + "  -", GuiData.smallfont, Color.White);
                base.Draw(t);
            }
        }
    
        public class PortBackdoorEXE : ModEXE
        {
            public PortBackdoorEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
            {
                Init(0, 40f, 5f, 400, "thanks from /el/ <3", "TOP TEXT", "BOTTOM TEXT");
            }

            public override void LoadContent()
            {
                Computer c = ComputerLookup.FindByIp(targetIP);
                port = c.GetDisplayPortNumberFromCodePort(originPort);
                if (noProxy && c.proxyActive)
                {
                    os.write("Proxy Active");
                    os.write("Execution failed");
                    needsRemoval = true;
                }
                else if ((Args.Length > 1 && (!int.TryParse(Args[1], out _) || int.Parse(Args[1]) != port)) || (Args.Length <= 1 && port != 0))
                { // if no args given, use 65535
                    os.write("Target Port is Closed");
                    os.write("Execution failed");
                    needsRemoval = true;
                }
                else foreach (string line in startMessage.Split('\n')) os.write(line);
                if (!noHostile) Programs.getComputer(os, targetIP).hostileActionTaken();
            }
            public override void Update(float t)
            {
                if (!done && life >= runTime)
                {
                    Computer c = Programs.getComputer(os, targetIP);
                    done = true;
                    foreach (PortState s in c.GetAllPortStates()) Console.WriteLine(s.Record.DefaultPortNumber);
                    Console.WriteLine(ModUtils.isPortOpen(c, originPort));
                    if (!ModUtils.isPortOpen(c, originPort)) // now it has port 0
                        c.AddPort(PortManager.GetPortRecordFromNumber(originPort).CreateState(c, true));
                    c.openPort(port, os.thisComputer.ip);
                    foreach (PortState s in c.GetAllPortStates()) Console.WriteLine(s.Record.DefaultPortNumber);
                    foreach (string line in endMessage.Split('\n')) os.write(line);
                }
                base.Update(t);
            }

            public override void Draw(float t)
            {
                TextItem.doCenteredFontLabel(Bounds, "test graphic", GuiData.detailfont, Color.White * fade);
                base.Draw(t);
            }
        }
    }

    public class ModCommands
    {
        public void sendIRC(OS os, string[] args)
        {
            Computer c = ModUtils.getComputer(os);
            IRCSystem irc = ModUtils.getIRC(c);
            if (irc == null)
            {
                os.write("This computer does not have an IRC Daemon.");
                return;
            }
            string user = c.currentUser.name;
            if (user == null) user = os.SaveUserAccountName;
            irc.AddLog(user, string.Join(" ", args.Skip(1)));
            
        }

        // synced with / stolen from XMOD
        public void mkdir(OS os, string[] args)
        {
            Computer c = ModUtils.getComputer(os);
            if (!c.PlayerHasAdminPermissions()) os.write("Insufficient Permissions");
            else
            {
                if (args.Length < 1) os.write("You must input a valid folder name");
                else
                {
                    if (Programs.getCurrentFolder(os).searchForFolder(args[1]) == null)
                    {
                        string idLog = "@" + (int)OS.currentElapsedTime;
                        // @[time]_FileCreated:_by_[ip]_-_file:[filename]
                        string logFilename = idLog + "_FolderCreated:_by_" + os.thisComputer.ip + "_-_folder:" + args[1];
                        // @[time] FileCreated: by [ip] - file:[filename]
                        string logContent = idLog + " FolderCreated: by " + os.thisComputer.ip + " - folder:" + args[1];
                        c.getFolderFromPath("/log").files.Add(new FileEntry(logContent, logFilename));
                        Programs.getCurrentFolder(os).folders.Add(new Folder(args[1]));
                    }
                    else os.write("Folder already exists!");
                }
            }
        }

        public void touch(OS os, string[] args)
        {
            Computer c = ModUtils.getComputer(os);
            if (!c.PlayerHasAdminPermissions()) os.write("Insufficient Permissions");
            else
            {
                if (args.Length < 1) os.write("You must input the file name");
                else
                {
                    {
                        string final_filename = FileFilter(args[1], os);
                        string textFile;
                        if (args.Length < 2) textFile = "";
                        else textFile = string.Join(" ", args.Skip(1));
                        string idLog = "@" + (int)OS.currentElapsedTime;
                        Programs.getCurrentFolder(os).files.Add(new FileEntry(textFile, final_filename));
                        // @[time]_FileCreated:_by_[ip]_-_file:[filename]
                        string logFilename = idLog + "_FileCreated:_by_" + os.thisComputer.ip + "_-_file:" + final_filename;
                        // @[time] FileCreated: by [ip] - file:[filename]
                        string logContent = idLog + " FileCreated: by " + os.thisComputer.ip + " - file:" + final_filename;
                        c.getFolderFromPath("/log").files.Add(new FileEntry(logContent, logFilename));
                    }
                }
            }
        }
        public static string FileFilter(string filename, OS os)
        {
            int i_file;
            string filename_t = null;
            Folder actualFolder = Programs.getCurrentFolder(os);

            if (actualFolder.containsFile(filename))
            {
                if (!actualFolder.containsFile(filename + "(1)"))
                {
                    return filename + "(1)";
                }
                else
                {
                    i_file = 1;
                    while (actualFolder.containsFile(filename + "(" + i_file + ")"))
                    {
                        if (!actualFolder.containsFile(filename + "(" + (i_file + 1) + ")"))
                        {
                            i_file++;
                            filename_t = filename + "(" + i_file + ")";
                        }
                        else
                        {
                            i_file++;
                        }

                    }
                    return filename_t;
                }
            }
            else
            {
                return filename;
            }
        }
    }

    public class ModConditions
    {
        public static Dictionary<string, TimeSpan> times = new Dictionary<string, TimeSpan>();
        public static int choice = 3;

        public class SAResetIRCDelay : Pathfinder.Action.PathfinderAction
        {
            [XMLStorage]
            public string target;

            public override void Trigger(object os_obj)
            {
                OS os = (OS)os_obj;
                times[target] = os.lastGameTime.TotalGameTime;
            }
        }

        public class SASetNumberOfChoices : Pathfinder.Action.DelayablePathfinderAction
        {
            [XMLStorage]
            public int choices;

            public override void Trigger(OS os)
            {
                choice = choices;
            }
        }

        public class SASetRAM : Pathfinder.Action.DelayablePathfinderAction
        {
            [XMLStorage]
            public float ram;

            public override void Trigger(OS os)
            {
                os.ramAvaliable += (int)ram - os.totalRam;
                os.totalRam = (int)ram - (OS.TOP_BAR_HEIGHT + 2);
            }
        }

        public class SARunCommand : Pathfinder.Action.DelayablePathfinderAction
        {
            [XMLStorage]
            public string command;

            public override void Trigger(OS os)
            {
                os.runCommand(command);
            }
        }

        public class SCOnIRCMessageAny : Pathfinder.Action.PathfinderCondition
        {
            [XMLStorage]
            public string target = null;
            [XMLStorage]
            public string user = null;
            [XMLStorage]
            public string notUser = null;
            [XMLStorage]
            public float minDelay = 0.0f;
            [XMLStorage]
            public float maxDelay = float.MaxValue;
            [XMLStorage]
            public string requiredFlags;
            [XMLStorage]
            public string doesNotHaveFlags;

            public override bool Check(object os_obj)
            {
                OS os = (OS)os_obj;
                Computer c = ModUtils.getComputer(os);
                if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
                if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
                if (target != null && c != Programs.getComputer(os, target)) return false;
                if (user == "#PLAYERNAME#" && (c.currentUser.name != null && c.currentUser.name != os.SaveUserAccountName)) return false;
                if (!string.IsNullOrWhiteSpace(user) && c.currentUser.name != user) return false;
                if (notUser == "#PLAYERNAME#" && (c.currentUser.name == null || c.currentUser.name == os.SaveUserAccountName)) return false;
                if (!string.IsNullOrWhiteSpace(notUser) && c.currentUser.name == notUser) return false;
                if (times.ContainsKey(target))
                {
                    float delay = (float)(os.lastGameTime.TotalGameTime - times[target]).TotalSeconds;
                    if (minDelay > delay || delay > maxDelay) return false;
                }
                IRCSystem irc = ModUtils.getIRC(c);
                if (irc == null) return false;
                string[] args = os.terminal.lastRunCommand.Split(Utils.WhitespaceDelim, StringSplitOptions.RemoveEmptyEntries);
                return args[0] == ">";
            }
        }

        public static bool checkForWord(string msg, string parse)
        {
            string[] temp = parse.Split('&');
            List<string>[] words = new List<string>[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                words[i] = new List<string>(temp[i].Split('|'));
                bool found = false;
                foreach (string w in words[i]) if (msg.ToLower().Contains(w.ToLower().Trim())) found = true;
                if (!found) return false;
            }
            return true;
        }

        public class SCOnIRCMessage : SCOnIRCMessageAny
        {
            [XMLStorage]
            public string word = "";

            public override bool Check(object os_obj)
            {
                if (!base.Check(os_obj)) return false;
                OS os = (OS)os_obj;
                return checkForWord(os.terminal.lastRunCommand, word);
            }
        }

        public class SCOnIRCMessageTone : SCOnIRCMessageAny
        {
            [XMLStorage]
            public string tone = "";

            public override bool Check(object os_obj)
            {
                if (!base.Check(os_obj)) return false;
                OS os = (OS)os_obj;
                string msg = os.terminal.lastRunCommand;
                #region tone detection
                if (tone == "no" && checkForWord(msg, "no|na|eh|sorry|can't|cant|decline|no can|off")) return true; // early exit so "no can do" is "no"
                if (tone == "yes" && checkForWord(msg, "ye|yup|ok|alr|aight|lets|les|let's|leg|sure|wish me|got it|got this|cool|ready|can do|will|accept|bring|here i|here we")) return true;
                if (tone == "help" && checkForWord(msg, "stuck|can't|cant|what|hm|huh|?|not|help|aid|idea|how|hint|clue|doesn|nudge")) return true;
                if (tone == "hey" && checkForWord(msg, "guy|boy|girl|dude|folk|people|@channel|yall|hey|sup")) return true;
                if (tone == "1" && checkForWord(msg, "1|one|first|former")) return true;
                if (tone == "2" && checkForWord(msg, "2|two|second")) return true;
                if (tone == "3" && checkForWord(msg, "3|three|third")) return true;
                if (tone == "4" && checkForWord(msg, "4|four|fourth")) return true;
                if (tone == "5" && checkForWord(msg, "5|five|fifth")) return true;
                if (tone == "6" && checkForWord(msg, "6|six|sixth")) return true;
                if (tone == "7" && checkForWord(msg, "7|seven|seventh")) return true;
                if (tone == "8" && checkForWord(msg, "8|eight|eighth")) return true;
                if (tone == "9" && checkForWord(msg, "9|nine|ninth")) return true;
                if (tone == "10" && checkForWord(msg, "10|ten|tenth")) return true;
                if (tone == choice.ToString() && checkForWord(msg, "last|latter|bottom")) return true;
                if (tone == ((choice + 1) / 2).ToString() && checkForWord(msg, "any|whatever|random|middle|center")) return true;
                #endregion
                return false;
            }
        }

        public class SCOnHostileActionTaken : Pathfinder.Action.PathfinderCondition
        {
            [XMLStorage]
            public string requiredFlags;
            [XMLStorage]
            public string doesNotHaveFlags;
            [XMLStorage]
            public string targetComp = null;
            [XMLStorage]
            public string targetNetwork = null;

            public override bool Check(object os_obj)
            {
                OS os = (OS)os_obj;
                Computer c = ModUtils.getComputer(os);
                if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
                if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
                if (targetComp != null && c != Programs.getComputer(os, targetComp)) return false;
                if (targetNetwork != null && ModPatches.networks.ContainsKey(targetNetwork) && ModPatches.networks[targetNetwork].tail.Contains(c)) return false;
                return ModPatches.recentHostileActionTaken == c;
            }
        }

        public class SCOnReboot : Pathfinder.Action.PathfinderCondition
        {
            [XMLStorage]
            public string requiredFlags = null;
            [XMLStorage]
            public string doesNotHaveFlags = null;
            [XMLStorage]
            public string targetComp = null;
            [XMLStorage]
            public string targetNetwork = null;

            public override bool Check(object os_obj)
            {
                OS os = (OS)os_obj;
                Computer c = ModUtils.getComputer(os);
                if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
                if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
                if (targetComp != null && c != Programs.getComputer(os, targetComp)) return false;
                if (targetNetwork != null && ModPatches.networks.ContainsKey(targetNetwork) && ModPatches.networks[targetNetwork].tail.Contains(c)) return false;
                return ModPatches.recentReboot == c;
            }
        }

        public class SCOnCrash : Pathfinder.Action.PathfinderCondition
        {
            [XMLStorage]
            public string requiredFlags = null;
            [XMLStorage]
            public string doesNotHaveFlags = null;
            [XMLStorage]
            public string targetComp = null;
            [XMLStorage]
            public string targetNetwork = null;

            public override bool Check(object os_obj)
            {
                OS os = (OS)os_obj;
                Computer c = ModUtils.getComputer(os);
                if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
                if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
                if (targetComp != null && c != Programs.getComputer(os, targetComp)) return false;
                if (targetNetwork != null && ModPatches.networks.ContainsKey(targetNetwork) && ModPatches.networks[targetNetwork].tail.Contains(c)) return false;
                return ModPatches.recentCrash == c;
            }
        }

        public static bool hasLogOnSource(OS os, Computer c)
        {
            if (c == null) return false;
            if (TrackerCompleteSequence.CompShouldStartTrackerFromLogs(os, c, os.thisComputer.ip)) return true;
            return false;
        }
        public static bool isSourceIntact(OS os, Computer c)
        {
            if (c == null) return false;
            Folder sys = c.files.root.searchForFolder("sys");
            foreach (FileEntry file in sys.files) if (file.name == "netcfgx.dll" && file.data.Contains("0") && file.data.Contains("1")) return true;
            return false;
        }

        public class SCOnRebootCompleted : Pathfinder.Action.PathfinderCondition
        {
            [XMLStorage]
            public bool RequireLogsOnSource = false;
            [XMLStorage]
            public bool RequireSourceIntact = false;
            [XMLStorage]
            public string requiredFlags = null;
            [XMLStorage]
            public string doesNotHaveFlags = null;
            [XMLStorage]
            public string targetNetwork;

            public override bool Check(object os_obj)
            {
                OS os = (OS)os_obj;
                if (targetNetwork != null || !ModPatches.networks.ContainsKey(targetNetwork)) return false;
                if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
                if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
                List<Computer> c = ModPatches.networks[targetNetwork].tail;
                if (RequireLogsOnSource) foreach (Computer temp in c) if (hasLogOnSource(os, temp)) return false;
                if (RequireSourceIntact) foreach (Computer temp in c) if (isSourceIntact(os, temp)) return false;
                return ModPatches.recentRebootCompleted != ModPatches.networks[targetNetwork];
            }
        }
    }

    public class ModUtils
    {
        public static bool isPortOpen(Computer c, int id)
        {
            return c.GetPortState(PortManager.GetPortRecordFromNumber(id).Protocol) != null;
        }

        public static IRCSystem getIRC(Computer c)
        {
            IRCSystem irc = null;
            foreach (Daemon daemon in c.daemons)
            {
                if (daemon is DLCHubServer) irc = ((DLCHubServer)daemon).IRCSystem;
                else if (daemon is IRCDaemon) irc = ((IRCDaemon)daemon).System;
            }
            return irc;
        }

        public static Computer getComputer(OS os)
        {
            return os.connectedComp ?? os.thisComputer;
        }

        public static void drawLine(SpriteBatch spriteBatch, Vector2 origin, Vector2 dest, Color c)
        {
            float y = Vector2.Distance(origin, dest);
            float rotation = (float)Math.Atan2(dest.Y - (double)origin.Y, dest.X - (double)origin.X) + 4.712389f;
            spriteBatch.Draw(Utils.white, origin, new Rectangle?(), c, rotation, Vector2.Zero, new Vector2(1f, y), SpriteEffects.None, 0.5f);
        }

        public static float Ratio(float a1, float b1, float a2, float b2, float a3)
        {
            return b1 + ((a3 - a1) * (b1 - b2) / (a1 - a2));
        }

        public static void ClipLineSegmentsInRect(Rectangle bound, Vector2 left, Vector2 right, out Vector2 a, out Vector2 b)
        {
            a = left; b = right;
            bool aModified = false, bModified = false;
            if (a.X < bound.X)
            {
                aModified = true;
                a = new Vector2(bound.X, Ratio(left.X, left.Y, right.X, right.Y, bound.X));
            }
            if (a.X > (bound.X + bound.Width))
            {
                if (aModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                aModified = true;
                a = new Vector2(bound.X + bound.Width, Ratio(left.X, left.Y, right.X, right.Y, bound.X + bound.Width));
            }
            if (a.Y < bound.Y)
            {
                if (aModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                aModified = true;
                a = new Vector2(Ratio(left.Y, left.X, right.Y, right.X, bound.Y), bound.Y);
            }
            if (a.Y > (bound.Y + bound.Height))
            {
                if (aModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                a = new Vector2(Ratio(left.Y, left.X, right.Y, right.X, bound.Y + bound.Height), bound.Y + bound.Height);
            }
            if (b.X < bound.X)
            {
                bModified = true;
                b = new Vector2(bound.X, Ratio(left.X, left.Y, right.X, right.Y, bound.X));
            }
            if (b.X > (bound.X + bound.Width))
            {
                if (bModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                bModified = true;
                b = new Vector2(bound.X + bound.Width, Ratio(left.X, left.Y, right.X, right.Y, bound.X + bound.Width));
            }
            if (b.Y < bound.Y)
            {
                if (bModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                bModified = true;
                b = new Vector2(Ratio(left.Y, left.X, right.Y, right.X, bound.Y), bound.Y);
            }
            if (b.Y > (bound.Y + bound.Height))
            {
                if (bModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                b = new Vector2(Ratio(left.Y, left.X, right.Y, right.X, bound.Y + bound.Height), bound.Y + bound.Height);
            }
        }
    }

    public class ModPatches
    {
        public class Network
        {
            public Computer head;
            public List<Computer> tail = new List<Computer>();
            public float traceTime = -1;
            public float rebootTime = -1;
            public NetworkTrigger onStart;
            public NetworkTrigger onCrash;
            public NetworkTrigger onComplete;
            public AfterCompleteTrigger afterComplete;
        }

        public static bool doesNetworkHaveLogsLeft(OS os, Network network)
        {
            foreach (Computer temp in network.tail) if (ModConditions.hasLogOnSource(os, temp)) return true;
            return false;
        }

        public static bool doesNetworkHaveSourceIntact(OS os, Network network)
        {
            foreach (Computer temp in network.tail) if (ModConditions.isSourceIntact(os, temp)) return true;
            return false;
        }

        public class NetworkTrigger
        {
            public OS os;
            public Network network;
            public Computer source;
            public string action;
            public float delay = 0f;
            public string delayHost;
            public bool requireLogs = false;
            public bool sourceIntact = false;
            public virtual void Start(OS os, Network network, Computer source)
            {
                this.os = os;
                this.network = network;
                this.source = source;
            }

            public void Trigger()
            {
                Console.WriteLine("Something Triggered");
                if (requireLogs && !doesNetworkHaveLogsLeft(os, network)) return;
                if (sourceIntact && !doesNetworkHaveSourceIntact(os, network)) return;
                SAAddConditionalActions action = new SAAddConditionalActions();
                action.Filepath = this.action;
                action.DelayHost = delayHost ?? source.idName;
                action.Delay = delay;
                DelayableActionSystem.FindDelayableActionSystemOnComputer(Programs.getComputer(os, delayHost)).AddAction(action, delay);
            }
        }

        public class AfterCompleteTrigger : NetworkTrigger
        {
            public int every = 1;
            public int offAfter = -1;
            public int _offAfter;

            public override void Start(OS os, Network network, Computer source)
            {
                base.Start(os, network, source);
                _offAfter = offAfter;
            }

            public bool TryTrigger()
            {
                if (connections % every != 0) return true;
                if (requireLogs && !doesNetworkHaveLogsLeft(os, network)) return false;
                if (sourceIntact && !doesNetworkHaveSourceIntact(os, network)) return false;
                if (_offAfter == 0) return false;
                Trigger();
                _offAfter--;
                return true;
            }
        }

        public class TraceV2Tracker
        {
            public OS os;
            public Network network;
            public Computer source;
            public float initialTimer = 0f;
            public float startTimer = 0f;
            public float timer = 0f;
            public float lastFrameTime;
            public byte active = 0; // 2 = active, 1 = rebooting, 0 = none
            public SpriteFont font;
            public SoundEffect beep;
            public SoundEffect BreakSound;
            public Color color;
            public string prefix;
            public List<TraceKillExe.PointImpactEffect> ImpactEffects = new List<TraceKillExe.PointImpactEffect>();
            public Texture2D circle;

            public void Start(OS os, Network network, Computer source)
            {
                this.os = os;
                this.network = network;
                this.source = source;
                color = new Color(0, 170, 170);
                BreakSound = os.content.Load<SoundEffect>("SFX/DoomShock");
                lastFrameTime = 0f;
                startTimer = network.traceTime;
                timer = network.traceTime;
                active = 2;
                os.warningFlash();
                Console.WriteLine("TraceV2 Start, Time: " + timer);
                Console.WriteLine("Warning flash");
                prefix = "TRACK :";
                if (network.onStart != null)
                {
                    network.onStart.Start(os, network, source);
                    network.onStart.Trigger();
                }
            }
            public void Stop()
            {
                active = 0;
                network = null;
            }

            public void Update(float t)
            {
                UpdateImpactEffects(t);
                if (active == 0) return;
                timer -= t * (Settings.AllTraceTimeSlowed ? 0.55f : 1f) * os.traceTracker.trackSpeedFactor;
                if (active == 2)
                {
                    if (timer <= 0f)
                    {
                        active = 0;
                        timer = 0f;
                        os.timerExpired();
                    }
                }
                else if (timer <= 0f) RebootComplete();
                float percent = timer / startTimer * 100.0f;
                float beepPeriod = percent < 45.0f ? (percent < 15.0f ? 1f : 5f) : 10f;
                if (percent % beepPeriod > lastFrameTime % beepPeriod)
                {
                    TraceTracker.beep.Play(0.5f, 0.0f, 0.0f);
                    os.warningFlash();
                }
                lastFrameTime = percent;
            }

            public void RebootHead()
            {
                active = 1;
                timer = network.rebootTime;
                startTimer = network.rebootTime;
                color = new Color(128, 128, 128);
                lastFrameTime = 0f;
                prefix = "RETRACE :";
                if (network.onCrash != null)
                {
                    network.onCrash.Start(os, network, source);
                    network.onCrash.Trigger();
                }
            }

            public void RebootComplete()
            {
                recentRebootCompleted = network;
                foreach (Computer c in network.tail)
                {
                    os.netMap.visibleNodes.Remove(os.netMap.nodes.IndexOf(c));
                    ImpactEffects.Add(new TraceKillExe.PointImpactEffect()
                    {
                        location = c.getScreenSpacePosition(),
                        scaleModifier = (float)(3.0 + (c.securityLevel > 2 ? 1.0 : 0.0)),
                        cne = new ConnectedNodeEffect(os, true),
                        timeEnabled = 0.0f,
                        HasHighlightCircle = true
                    });
                    c.adminIP = c.ip;
                    c.GetAllPortStates().ForEach(x => c.closePort(x.Record.Protocol, os.thisComputer.ip));
                }
                BreakSound.Play();
                if (network.onComplete != null)
                {
                    network.onComplete.Start(os, network, source);
                    network.onComplete.Trigger();
                }
                if (network.afterComplete != null)
                {
                    network.afterComplete.Start(os, network, source);
                    afterCompleteTriggers.Add(network.afterComplete);
                }
                if (network.tail.Contains(os.connectedComp)) Programs.disconnect(new string[0], os);
                Stop();
            }

            public void Draw(SpriteBatch sb)
            {
                //DrawImpactEffects(sb, ImpactEffects);
                if (active == 0) return;
                string text = (timer / startTimer * 100.0).ToString("00.00");
                Vector2 vector2 = TraceTracker.font.MeasureString(text);
                Vector2 position = new Vector2(10f, sb.GraphicsDevice.Viewport.Height - vector2.Y);
                if (os.traceTracker.active) position.Y -= vector2.Y + 14f; // display both if both are present
                sb.DrawString(TraceTracker.font, text, position, color);
                position.Y -= 25f;
                sb.DrawString(TraceTracker.font, prefix, position, color, 0.0f, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0.5f);
            }

            public void UpdateImpactEffects(float t)
            {
                for (int index = 0; index < ImpactEffects.Count; ++index)
                {
                    TraceKillExe.PointImpactEffect impactEffect = ImpactEffects[index];
                    impactEffect.timeEnabled += t;
                    if (impactEffect.timeEnabled > 5f)
                    {
                        ImpactEffects.RemoveAt(index);
                        --index;
                    }
                    else ImpactEffects[index] = impactEffect;
                }
            }

            public void DrawImpactEffects(SpriteBatch sb, List<TraceKillExe.PointImpactEffect> Effects)
            {
                foreach (TraceKillExe.PointImpactEffect effect in Effects)
                {
                    Color color = Color.Lerp(Utils.AddativeWhite, Utils.AddativeRed, (float)(0.600000023841858 + 0.400000005960464 * (double)Utils.LCG.NextFloatScaled())) * (float)(0.600000023841858 + 0.400000005960464 * (double)Utils.LCG.NextFloatScaled());
                    Vector2 location = effect.location;
                    float num1 = Utils.QuadraticOutCurve(effect.timeEnabled / DLCIntroExe.NodeImpactEffectTransInTime);
                    float num2 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(effect.timeEnabled / (DLCIntroExe.NodeImpactEffectTransInTime + DLCIntroExe.NodeImpactEffectTransOutTime)));
                    float num3 = Utils.QuadraticOutCurve((effect.timeEnabled - DLCIntroExe.NodeImpactEffectTransInTime) / DLCIntroExe.NodeImpactEffectTransOutTime);
                    effect.cne.color = color * num1;
                    effect.cne.ScaleFactor = num2 * effect.scaleModifier;
                    if (effect.timeEnabled > DLCIntroExe.NodeImpactEffectTransInTime)
                        effect.cne.color = color * (1f - num3);
                    if (num1 >= 0.0f && effect.HasHighlightCircle)
                        sb.Draw(circle, location, new Rectangle?(), color * (float)(1.0 - (double)num1 - ((double)num3 >= 0.0 ? 1.0 - (double)num3 : 0.0)), 0.0f, new Vector2(circle.Width / 2f, circle.Height / 2f), (num1 / circle.Width * 60f), SpriteEffects.None, 0.7f);
                    effect.cne.draw(sb, location);
                }
            }
        }

        public static Dictionary<string, Network> networks = new Dictionary<string, Network>();
        public static Dictionary<string, List<string>> postLoadComputerCache = new Dictionary<string, List<string>>();
        public static List<AfterCompleteTrigger> afterCompleteTriggers = new List<AfterCompleteTrigger>();
        public static TraceV2Tracker tracker = new TraceV2Tracker();
        public static Computer recentHostileActionTaken = null;
        public static Computer recentReboot = null;
        public static Computer recentCrash = null;
        public static Network recentRebootCompleted = null;
        public static int connections = 0;

        [HarmonyLib.HarmonyPatch(typeof(Programs), nameof(Programs.connect))] // connections+, reset recent&#&
        class PatchConnect
        {
            static void Postfix(OS os)
            {
                PatchDisconnect.Postfix(os);
                // nulling everything so unrelated hacking from ages ago dont trigger new conditions
                connections++;
                for (int i = 0; i < afterCompleteTriggers.Count; i++)
                {
                    AfterCompleteTrigger trigger = afterCompleteTriggers[i];
                    if (!trigger.TryTrigger())
                    {
                        afterCompleteTriggers.Remove(trigger);
                        i--;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Programs), nameof(Programs.disconnect))] // reset recent&#&
        class PatchDisconnect
        {
            public static void Postfix(OS os)
            {
                recentReboot = null;
                recentRebootCompleted = null;
                recentHostileActionTaken = null;
                // nulling everything so unrelated hacking from ages ago dont trigger new conditions
                if (tracker.active == 1) {
                    bool tracked = false;
                    foreach (Computer c in tracker.network.tail) if (ModConditions.hasLogOnSource(os, c)) tracked = true;
                    if (!tracked) tracker.RebootComplete();
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Computer), nameof(Computer.reboot))] // set recentreboot
        class PatchReboot
        {
            static void Postfix(Computer __instance)
            {
                recentReboot = __instance;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Computer), nameof(Computer.crash))] // set recentreboot & recentcrash, snooze tracev2 if possible
        class PatchCrash
        {
            static void Postfix(Computer __instance)
            {
                recentReboot = __instance;
                recentCrash = __instance;
                if (tracker.network?.head == __instance) tracker.RebootHead();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Computer), nameof(Computer.hostileActionTaken))] // set recenthostile, activate tracev2 if possible
        class PatchHostileActionTaken
        {
            static void Postfix(Computer __instance)
            {
                recentHostileActionTaken = __instance.os.connectedComp;
                foreach (Network network in networks.Values) if (network.tail.Contains(__instance) && tracker.network != network) tracker.Start(__instance.os, network, __instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.Update))] // hook tracker update
        class PatchOSUpdate
        {
            static void Postfix(OS __instance, GameTime gameTime)
            {
                float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (__instance.canRunContent && __instance.isLoaded) tracker.Update(totalSeconds);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.drawModules))] // hook tracker draw
        class PatchDrawModules
        {
            static void Postfix()
            {
                tracker.Draw(GuiData.spriteBatch);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(TrackerCompleteSequence), nameof(TrackerCompleteSequence.CompShouldStartTrackerFromLogs))] // stricter log control
        class PatchCompShouldStartTrackerFromLogs
        {
            static bool Prefix(object osobj, Computer c, string targetIP, ref bool __result)
            {
                OS os = (OS)osobj;
                Folder log = c.files.root.searchForFolder("log");
                if (targetIP == null) targetIP = os.thisComputer.ip;
                foreach (FileEntry file in log.files)
                {
                    string data = file.data;
                    if (data.Contains(targetIP) && !data.Contains("Connection") && !data.Contains("Disconnected"))
                    {
                        __result = true;
                        return false;
                    }
                }
                __result = false;
                return false; // skips redundant log check, will return true if i see mods that do postfixes
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.LoadContent))] // reset data
        class PatchOSLoadContent
        {
            static void Prefix(OS __instance)
            {
                if (__instance.canRunContent)
                {
                    networks.Clear();
                    postLoadComputerCache.Clear();
                    tracker.Stop();
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ExtensionLoader), nameof(ExtensionLoader.LoadNewExtensionSession))] // load from extension to os
        class PatchLoadNewExtensionSession
        {
            static void Prefix(ExtensionInfo info, object os_obj)
            {
                OS os = (OS)os_obj;
                if (Directory.Exists(info.FolderPath + "/Networks"))
                {
                    Utils.ActOnAllFilesRevursivley(info.FolderPath + "/Networks", filename =>
                    {
                    if (!filename.EndsWith(".xml")) return;
                    Console.WriteLine("Reading " + filename);
                    XmlReader rdr = XmlReader.Create(File.OpenRead(LocalizedFileLoader.GetLocalizedFilepath(filename)));
                    while (rdr.Name != "TraceV2")
                    {
                        rdr.Read();
                        if (rdr.EOF) return;
                    }
                    Network network = new Network();
                    string name;
                    if (rdr.MoveToAttribute("name"))
                    {
                        name = rdr.ReadContentAsString();
                        networks[name] = network;
                        postLoadComputerCache[name] = new List<string>();
                    }
                    else return;
                    if (rdr.MoveToAttribute("head")) postLoadComputerCache[name].Add(rdr.ReadContentAsString());
                    else return;
                    rdr.Read();
                    while (rdr.Name != "TraceV2")
                    {
                        if (rdr.Name.ToLower().Equals("trace") && rdr.MoveToAttribute("time")) network.traceTime = rdr.ReadContentAsFloat();
                        if (rdr.Name.ToLower().Equals("reboot") && rdr.MoveToAttribute("time")) network.rebootTime = rdr.ReadContentAsFloat();
                        if (rdr.Name.ToLower().Equals("computer") && rdr.MoveToAttribute("name")) postLoadComputerCache[name].Add(rdr.ReadContentAsString());
                        if (rdr.Name.ToLower().Equals("onstart") && rdr.MoveToAttribute("action"))
                        {
                            NetworkTrigger onStart = new NetworkTrigger();
                            onStart.action = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("RequireLogsOnSource")) onStart.requireLogs = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("RequireSourceIntact")) onStart.sourceIntact = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("DelayHost")) onStart.delayHost = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("Delay")) onStart.delay = rdr.ReadContentAsFloat();
                            network.onStart = onStart;
                        }
                        if (rdr.Name.ToLower().Equals("oncrash") && rdr.MoveToAttribute("action"))
                        {
                            NetworkTrigger onCrash = new NetworkTrigger();
                            onCrash.action = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("RequireLogsOnSource")) onCrash.requireLogs = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("RequireSourceIntact")) onCrash.sourceIntact = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("DelayHost")) onCrash.delayHost = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("Delay")) onCrash.delay = rdr.ReadContentAsFloat();
                            network.onCrash = onCrash;
                        }
                        if (rdr.Name.ToLower().Equals("oncomplete") && rdr.MoveToAttribute("action"))
                        {
                            NetworkTrigger onComplete = new NetworkTrigger();
                            onComplete.action = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("RequireLogsOnSource")) onComplete.requireLogs = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("RequireSourceIntact")) onComplete.sourceIntact = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("DelayHost")) onComplete.delayHost = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("Delay")) onComplete.delay = rdr.ReadContentAsFloat();
                            network.onComplete = onComplete;
                        }
                        if (rdr.Name.ToLower().Equals("aftercomplete") && rdr.MoveToAttribute("action"))
                        {
                            AfterCompleteTrigger afterComplete = new AfterCompleteTrigger();
                            afterComplete.action = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("RequireLogsOnSource")) afterComplete.requireLogs = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("RequireSourceIntact")) afterComplete.sourceIntact = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("DelayHost")) afterComplete.delayHost = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("Delay")) afterComplete.delay = rdr.ReadContentAsFloat();
                            if (rdr.MoveToAttribute("every")) afterComplete.every = rdr.ReadContentAsInt();
                            if (rdr.MoveToAttribute("offAfter")) afterComplete.every = rdr.ReadContentAsInt();
                            network.afterComplete = afterComplete;
                        }
                        if (rdr.EOF) return;
                        rdr.Read();
                    }
                    rdr.Close();
                    ComputerLoader.postAllLoadedActions += () =>
                    {
                        if (postLoadComputerCache.Count != 0) foreach (string key in postLoadComputerCache.Keys)
                        {
                            var value = postLoadComputerCache[key];
                            networks[key].head = Programs.getComputer(ComputerLoader.os, value[0]);
                            foreach (string id in value) networks[key].tail.Add(Programs.getComputer(ComputerLoader.os, id));
                        }
                    };
                    });
                }
            }
        }
    
        [Event] // save from os to save
        public static void SaveNetworkHandler(SaveEvent e)
        {
            foreach (string key in networks.Keys)
            {
                Network value = networks[key];
                XElement network = new XElement("TraceV2");
                network.SetAttributeValue("name", key);
                network.SetAttributeValue("head", value.tail[0].idName);
                XElement trace = new XElement("trace");
                trace.SetAttributeValue("time", value.traceTime);
                network.Add(trace);
                XElement reboot = new XElement("reboot");
                reboot.SetAttributeValue("time", value.rebootTime);
                network.Add(reboot);
                if (value.onStart != null)
                {
                    XElement onStart = new XElement("onStart");
                    onStart.SetAttributeValue("action", value.onStart.action);
                    if (value.onStart.requireLogs) onStart.SetAttributeValue("RequireLogsOnSource", true);
                    if (value.onStart.sourceIntact) onStart.SetAttributeValue("RequireSourceIntact", true);
                    if (value.onStart.delay != 0f) onStart.SetAttributeValue("Delay", value.onStart.delay);
                    if (value.onStart.delayHost != null) onStart.SetAttributeValue("DelayHost", value.onStart.delayHost);
                    network.Add(onStart);
                }
                if (value.onCrash != null)
                {
                    XElement onCrash = new XElement("onCrash");
                    onCrash.SetAttributeValue("action", value.onCrash.action);
                    if (value.onCrash.requireLogs) onCrash.SetAttributeValue("RequireLogsOnSource", true);
                    if (value.onCrash.sourceIntact) onCrash.SetAttributeValue("RequireSourceIntact", true);
                    if (value.onCrash.delay != 0f) onCrash.SetAttributeValue("Delay", value.onCrash.delay);
                    if (value.onCrash.delayHost != null) onCrash.SetAttributeValue("DelayHost", value.onCrash.delayHost);
                    network.Add(onCrash);
                }
                if (value.onComplete != null)
                {
                    XElement onComplete = new XElement("onComplete");
                    onComplete.SetAttributeValue("action", value.onComplete.action);
                    if (value.onComplete.requireLogs) onComplete.SetAttributeValue("RequireLogsOnSource", true);
                    if (value.onComplete.sourceIntact) onComplete.SetAttributeValue("RequireSourceIntact", true);
                    if (value.onComplete.delay != 0f) onComplete.SetAttributeValue("Delay", value.onComplete.delay);
                    if (value.onComplete.delayHost != null) onComplete.SetAttributeValue("DelayHost", value.onComplete.delayHost);
                    network.Add(onComplete);
                }
                if (value.afterComplete != null)
                {
                    XElement afterComplete = new XElement("afterComplete");
                    afterComplete.SetAttributeValue("action", value.afterComplete.action);
                    if (value.afterComplete.requireLogs) afterComplete.SetAttributeValue("RequireLogsOnSource", true);
                    if (value.afterComplete.sourceIntact) afterComplete.SetAttributeValue("RequireSourceIntact", true);
                    if (value.afterComplete.delay != 0f) afterComplete.SetAttributeValue("Delay", value.afterComplete.delay);
                    if (value.afterComplete.delayHost != null) afterComplete.SetAttributeValue("DelayHost", value.afterComplete.delayHost);
                    if (value.afterComplete.every != 1) afterComplete.SetAttributeValue("every", value.afterComplete.every);
                    if (value.afterComplete.offAfter != -1) afterComplete.SetAttributeValue("offAfter", value.afterComplete.offAfter);
                    network.Add(afterComplete);
                }
                for (int i = 1; i < value.tail.Count; i++)
                {
                    XElement comp = new XElement("Computer");
                    comp.SetAttributeValue("name", value.tail[i].idName);
                    network.Add(comp);
                }
                e.Save.Add(network);
            }
        }

        // load from save to os
        public class NetworkExecutor : SaveLoader.SaveExecutor 
        {
            public override void Execute(EventExecutor exec, ElementInfo info)
            {
                Console.WriteLine(info.Name + " > " + info.Attributes.ContainsKey("name"));
                if (!info.Attributes.ContainsKey("name")) return;
                Network network = new Network();
                networks[info.Attributes["name"]] = network;
                network.head = Programs.getComputer(Os, info.Attributes["head"]);
                network.tail.Add(network.head);
                string attr;
                foreach (ElementInfo child in info.Children)
                {
                    switch (child.Name)
                    {
                        case "trace":
                            network.traceTime = float.Parse(child.Attributes["time"]);
                            break;
                        case "reboot":
                            network.rebootTime = float.Parse(child.Attributes["time"]);
                            break;
                        case "Computer":
                            network.tail.Add(Programs.getComputer(Os, child.Attributes["name"]));
                            break;
                        case "onStart":
                            network.onStart = new NetworkTrigger();
                            network.onStart.action = child.Attributes["action"];
                            if (child.Attributes.TryGetValue("RequireLogsOnSource", out attr)) network.onStart.requireLogs = bool.Parse(attr);
                            if (child.Attributes.TryGetValue("RequireSourceIntact", out attr)) network.onStart.sourceIntact = bool.Parse(attr);
                            if (child.Attributes.TryGetValue("Delay", out attr)) network.onStart.delay = float.Parse(attr);
                            if (child.Attributes.TryGetValue("DelayHost", out attr)) network.onStart.delayHost = attr;
                            break;
                        case "onCrash":
                            network.onCrash = new NetworkTrigger();
                            network.onCrash.action = child.Attributes["action"];
                            if (child.Attributes.TryGetValue("RequireLogsOnSource", out attr)) network.onCrash.requireLogs = bool.Parse(attr);
                            if (child.Attributes.TryGetValue("RequireSourceIntact", out attr)) network.onCrash.sourceIntact = bool.Parse(attr);
                            if (child.Attributes.TryGetValue("Delay", out attr)) network.onCrash.delay = float.Parse(attr);
                            if (child.Attributes.TryGetValue("DelayHost", out attr)) network.onCrash.delayHost = attr;
                            break;
                        case "onComplete":
                            network.onComplete = new NetworkTrigger();
                            network.onComplete.action = child.Attributes["action"];
                            if (child.Attributes.TryGetValue("RequireLogsOnSource", out attr)) network.onComplete.requireLogs = bool.Parse(attr);
                            if (child.Attributes.TryGetValue("RequireSourceIntact", out attr)) network.onComplete.sourceIntact = bool.Parse(attr);
                            if (child.Attributes.TryGetValue("Delay", out attr)) network.onComplete.delay = float.Parse(attr);
                            if (child.Attributes.TryGetValue("DelayHost", out attr)) network.onComplete.delayHost = attr;
                            break;
                        case "afterComplete":
                            network.afterComplete = new AfterCompleteTrigger();
                            network.afterComplete.action = child.Attributes["action"];
                            if (child.Attributes.TryGetValue("RequireLogsOnSource", out attr)) network.afterComplete.requireLogs = bool.Parse(attr);
                            if (child.Attributes.TryGetValue("RequireSourceIntact", out attr)) network.afterComplete.sourceIntact = bool.Parse(attr);
                            if (child.Attributes.TryGetValue("Delay", out attr)) network.afterComplete.delay = float.Parse(attr);
                            if (child.Attributes.TryGetValue("DelayHost", out attr)) network.afterComplete.delayHost = attr;
                            if (child.Attributes.TryGetValue("every", out attr)) network.afterComplete.every = int.Parse(attr);
                            if (child.Attributes.TryGetValue("offAfter", out attr)) network.afterComplete.offAfter = int.Parse(attr);
                            break;
                    }
                }
            }
        }
    }
}