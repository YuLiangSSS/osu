// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mania.Mods.YuLiangSSSMods;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mania.Scoring;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Screens.Play.HUD.HitErrorMeters;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mania.Skinning.Argon
{
    [Cached]
    public partial class ScoreV1Accuracy : HitErrorMeter, ISerialisableDrawable
    {
        public double PerfectRange = 16 + 0.5;
        public double GreatRange = 34 + 0.5;
        public double GoodRange = 67 + 0.5;
        public double OkRange = 97 + 0.5;
        public double MehRange = 121 + 0.5;
        public double MissRange = 158 + 0.5;

        public ManiaHitWindows ManiaHitWindows => (ManiaHitWindows)HitWindows;
        public double TotalMultiplier => ManiaHitWindows.SpeedMultiplier / ManiaHitWindows.DifficultyMultiplier;

        public void CalculateRange(double od)
        {
            double invertedOd = 10 - od;

            PerfectRange = Math.Floor(16 * TotalMultiplier) + 0.5;
            GreatRange = Math.Floor((34 + 3 * invertedOd)) * TotalMultiplier + 0.5;
            GoodRange = Math.Floor((67 + 3 * invertedOd)) * TotalMultiplier + 0.5;
            OkRange = Math.Floor((97 + 3 * invertedOd)) * TotalMultiplier + 0.5;
            MehRange = Math.Floor((121 + 3 * invertedOd)) * TotalMultiplier + 0.5;
            MissRange = Math.Floor((158 + 3 * invertedOd)) * TotalMultiplier + 0.5;
        }

        public double CurrentAccuracy = 100.0;

        public double MaxPoints = 0;
        public double TotalPoints = 0;

        public double[] HeadOffsets = new double[18];

        [Resolved]
        private IBindable<IReadOnlyList<Mod>> mods { get; set; } = null!;

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; } = null!;

        public string AccuracyString => CurrentAccuracy.ToString("0.00") + "%";

        public Container AccuracyContainer = null!;
        public OsuSpriteText AccuracyText = null!;

        public ScoreV1Accuracy()
        {
            AutoSizeAxes = Axes.Both;
        }

        public override void Clear()
        {
        }

        protected void UpdateDisplay()
        {
            UpdateAccuracy();
            AccuracyText.Text = AccuracyString;
        }

        public void UpdateAccuracy()
        {
            if (MaxPoints == 0)
            {
                CurrentAccuracy = 100.0;
                return;
            }

            CurrentAccuracy = TotalPoints / MaxPoints * 100.0;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new Container
            {
                Width = 130,
                Height = 20,
                Margin = new MarginPadding(2),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            AccuracyText = new OsuSpriteText
                            {
                                Font = OsuFont.Numeric.With(size: 20),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Color4.White,
                            }
                        }
                    }
                }
            };

            AccuracyText.Text = AccuracyString;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            CalculateRange(beatmap.Value.BeatmapInfo.Difficulty.OverallDifficulty);

            beatmap.BindValueChanged(value =>
            {
                Reset(value.NewValue);
            }, true);
        }

        protected void Reset(WorkingBeatmap working)
        {
            HeadOffsets = new double[18];
            MaxPoints = 0;
            TotalPoints = 0;
            double od = double.NaN;
            foreach (Mod mod in mods.Value)
            {
                if (mod is ManiaModAdjust adjust)
                {
                    od = adjust.OverallDifficulty.Value ?? 0;
                }
            }

            if (double.IsNaN(od))
            {
                od = working.BeatmapInfo.Difficulty.OverallDifficulty;
            }

            CalculateRange(od);
        }

        protected override void OnNewJudgement(JudgementResult judgement)
        {
            double offset = Math.Abs(judgement.TimeOffset);
            var result = GetResultByOffset(offset);
            var hitObject = (ManiaHitObject)judgement.HitObject;
            if (hitObject is HeadNote)
            {
                HeadOffsets[hitObject.Column] = offset;
            }
            else if (hitObject is TailNote)
            {
                MaxPoints += 300;
                TotalPoints += GetLNScore(HeadOffsets[hitObject.Column], offset);
                HeadOffsets[hitObject.Column] = 0;
            }
            else if (hitObject is Note)
            {
                MaxPoints += 300;
                TotalPoints += result switch
                {
                    HitResult.Perfect => 300,
                    HitResult.Great => 300,
                    HitResult.Good => 200,
                    HitResult.Ok => 100,
                    HitResult.Meh => 50,
                    HitResult.Miss => 0,
                    _ => 0
                };
            }
            else return;

            UpdateDisplay();
        }

        protected HitResult GetResultByOffset(double offset) =>
            offset <= PerfectRange ? HitResult.Perfect :
            offset <= GreatRange ? HitResult.Great :
            offset <= GoodRange ? HitResult.Good :
            offset <= OkRange ? HitResult.Ok :
            offset <= MehRange ? HitResult.Meh :
            offset <= MissRange ? HitResult.Miss :
            HitResult.Miss;

        public double GetLNScore(double head, double tail)
        {
            double combined = head + tail;

            (double range, double headFactor, double combinedFactor, double score)[] rules = new[]
            {
                (PerfectRange, 1.2, 2.4, 300.0),
                (GreatRange, 1.1, 2.2, 300),
                (GoodRange, 1.0, 2.0, 200),
                (OkRange, 1.0, 2.0, 100),
                (MehRange, 1.0, 2.0, 50),
            };

            foreach (var (range, headFactor, combinedFactor, score) in rules)
            {
                if (head <= range * headFactor && combined <= range * combinedFactor)
                    return score;
            }

            return 0;
        }
    }
}
