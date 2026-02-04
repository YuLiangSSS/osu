// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Mania.Mods.YuLiangSSSMods
{
    public class ManiaModLNColor : Mod, IApplicableToBeatmap
    {
        public override string Name => "LN Color";

        public override string Acronym => "LC";

        public override LocalisableString Description => "Change holding LN color.";

        public override ModType Type => ModType.Fun;

        public override double ScoreMultiplier => 1;

        [SettingSource("LN Color R", "Change R of LN color (0~255).", SettingControlType = typeof(ColorNumberBox))]
        public Bindable<int?> LNColorR { get; } = new Bindable<int?>();

        [SettingSource("LN Color G", "Change G of LN color (0~255).", SettingControlType = typeof(ColorNumberBox))]
        public Bindable<int?> LNColorG { get; } = new Bindable<int?>();

        [SettingSource("LN Color B", "Change B of LN color (0~255).", SettingControlType = typeof(ColorNumberBox))]
        public Bindable<int?> LNColorB { get; } = new Bindable<int?>();

        [SettingSource("LN Color A", "Change A of LN color (0~255).", SettingControlType = typeof(ColorNumberBox))]
        public Bindable<int?> LNColorA { get; } = new Bindable<int?>();

        [SettingSource("Fade Duration", "Change the fade duration(ms) of LN.", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> FadeDuration { get; } = new Bindable<int?>();

        public static bool IsActivated = false;
        public static byte R = 0;
        public static byte G = 0;
        public static byte B = 0;
        public static byte A = 0;
        public static int FadeDur = 0;

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            IsActivated = true;

            LNColorR.BindValueChanged(r =>
            {
                checkColor(LNColorR, out R);
            }, true);

            LNColorG.BindValueChanged(g =>
            {
                checkColor(LNColorG, out G);
            }, true);

            LNColorB.BindValueChanged(b =>
            {
                checkColor(LNColorB, out B);
            }, true);

            LNColorA.BindValueChanged(a =>
            {
                checkColor(LNColorA, out A);
            }, true);

            FadeDuration.BindValueChanged(fd =>
            {
                if (fd.NewValue is not null && fd.NewValue > 0)
                {
                    FadeDur = (int)fd.NewValue;
                }
                else
                {
                    FadeDur = 0;
                }
            }, true);
        }

        public override void ResetSettingsToDefaults()
        {
            base.ResetSettingsToDefaults();

            IsActivated = false;
        }

        private void checkColor(Bindable<int?> color, out byte byteColor)
        {
            if (color.Value > 255)
            {
                color.Value = 255;
                byteColor = 255;
            }
            else if (color.Value < 0)
            {
                color.Value = 0;
                byteColor = 0;
            }
            else if (color.Value is not null)
            {
                byteColor = (byte)color.Value;
            }
            else
            {
                byteColor = 0;
            }
        }
    }
}
