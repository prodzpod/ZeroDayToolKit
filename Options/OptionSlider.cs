using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Pathfinder.GUI;
using Pathfinder.Options;
using System;

namespace ZeroDayToolKit.Options
{
    public class OptionSlider : Option
    {
        public float Value;
        public int ButtonID = PFButton.GetNextID();
        public override int SizeX => 210;
        public override int SizeY => 30;

        public OptionSlider(string name, string description = "", float defVal = 0.5f) : base(name, description)
        {
            Value = defVal;
        }

        public override void Draw(int x, int y)
        {
            TextItem.doLabel(new Vector2(x, y), Name, new Color?(), 200f);
            Value = SliderBar.doSliderBar(ButtonID, x, y + 30, SizeX, SizeY, 1f, 0f, Value, 0.001f);
            TextItem.doSmallLabel(new Vector2(x + 32, y + 30), Description, new Color?());
        }
    }
}
