using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Hacknet;
using Pathfinder.Port;

namespace ZeroDayToolKit.TraceV2
{
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
            Network.recentRebootCompleted = network;
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
                Network.afterCompleteTriggers.Add(network.afterComplete);
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
                Color color = Color.Lerp(Hacknet.Utils.AddativeWhite, Hacknet.Utils.AddativeRed, (float)(0.600000023841858 + 0.400000005960464 * (double)Hacknet.Utils.LCG.NextFloatScaled())) * (float)(0.600000023841858 + 0.400000005960464 * (double)Hacknet.Utils.LCG.NextFloatScaled());
                Vector2 location = effect.location;
                float num1 = Hacknet.Utils.QuadraticOutCurve(effect.timeEnabled / DLCIntroExe.NodeImpactEffectTransInTime);
                float num2 = Hacknet.Utils.QuadraticOutCurve(Hacknet.Utils.QuadraticOutCurve(effect.timeEnabled / (DLCIntroExe.NodeImpactEffectTransInTime + DLCIntroExe.NodeImpactEffectTransOutTime)));
                float num3 = Hacknet.Utils.QuadraticOutCurve((effect.timeEnabled - DLCIntroExe.NodeImpactEffectTransInTime) / DLCIntroExe.NodeImpactEffectTransOutTime);
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
}
