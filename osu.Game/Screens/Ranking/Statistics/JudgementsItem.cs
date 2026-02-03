using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Localisation;

namespace osu.Game.Screens.Ranking.Statistics
{
    public partial class JudgementsItem : SimpleStatisticItem<string>
    {
        public JudgementsItem(string display, string name = "Count", ColourInfo? colour = null) : base(name)
        {
            Value = display;
            Colour = colour ?? Colour4.White;
        }

        protected override LocalisableString DisplayValue(string value)
        {
            return value ?? "N/A";
        }
    }
}
