using System;
using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ZeroDayToolKit.Utils;
using Pathfinder.Executable;
using System.Security.Policy;

namespace ZeroDayToolKit.Executibles
{
    public class TeleSmoothTalkEXE : ZeroDayEXE
    {
        public static int[] RAMCosts = [121, 81, 54, 36, 24];
        public static float[] RunTimes = [170.5f, 85, 42, 20, 8];
        public TeleSmoothTalkEXE parent = null;
        public TeleSmoothTalkEXE child = null;
        public bool failed = false;
        public int index = 0;
        public TeleSmoothTalkEXE() : this(0) { }
        public TeleSmoothTalkEXE(int i) : base()
        {
            index = i;
            Init(23, RunTimes[index], 180 - RunTimes[index], RAMCosts.Sum(), "process: " + PID, "[teleSMOOTH] Starting task", ""); 
        }
        public override void OnInitialize()
        {
            ramCost = RAMCosts[index];
            base.OnInitialize();
            if (!needsRemoval && index < RAMCosts.Length - 1)
            {
                TeleSmoothTalkEXE child = new(index + 1) { parent = this };
                child.Assign(bounds, os, Args);
                os.AddGameExecutable(child);
            }
        }
        public override void OnSuccess()
        {
            if (failed) return;
            if (parent == null) base.OnSuccess(); 
            else done = true;
        }
        public override void IncrementLife(float t) { if (!failed) base.IncrementLife(t); }
        public override void OnCompleteKilled()
        {
            base.OnCompleteKilled();
            if (failed) return;
            if (done && child == null)
            {
                var p = parent;
                while (p != null) { p.Lifetime = p.runTime - ((p.runTime - p.Lifetime) / 3); p = p.parent; }
                if (parent != null) parent.child = null;
            }
            else
            {
                var p = parent;
                while (p != null) { p.failed = true; p = p.parent; }
                p = child;
                while (p != null) { p.failed = true; p = p.child; }
            }
        }

        public float drawT = Hacknet.Utils.rand(1);
        public float drawTMult = Hacknet.Utils.rand(.2f) + .9f;
        public List<int> expos = [];
        public override void Draw(float t)
        {
            base.Draw(t);
            var go = !done && !failed;
            if (go) drawT += t * drawTMult;
            Color color = failed ? os.brightLockedColor : (done ? os.brightUnlockedColor : (index % 2 == 0 ? os.unlockedColor : os.shellColor));
            var time = Math.Ceiling(drawT);
            if (expos.Count < time)
            {
                while (expos.Count < time) expos.Add(expos.Count);
                expos = [..expos.Shuffle()];
            }
            if (!go && t % 1 == 0)
                spriteBatch.Draw(Hacknet.Utils.white, new Rectangle(Bounds.X, Bounds.Y + 14, Bounds.Width, Bounds.Height - 14), new Rectangle?(), color, 0, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
            else for (int i = 0; i < time; i++)
            {
                var width = Bounds.Width / time;
                var height = Math.Pow(drawT % 1, expos[i] + 1) * (Bounds.Height - 14);
                Rectangle rect = new((int)(Bounds.X + (width * (i + .5f))), (int)(Bounds.Y + 14 + (Bounds.Height - 14 - (height / 2))), (int)(width), (int)(height));
                spriteBatch.Draw(Hacknet.Utils.white, rect, new Rectangle?(), color, 0, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
            }
        }
    }
}
