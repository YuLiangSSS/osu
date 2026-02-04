// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Filter;
using osu.Game.Rulesets.Mania.Beatmaps;
using osu.Game.Rulesets.Mania.Configuration;
using osu.Game.Rulesets.Mania.Difficulty;
using osu.Game.Rulesets.Mania.Edit;
using osu.Game.Rulesets.Mania.Edit.Setup;
using osu.Game.Rulesets.Mania.Mods;
using osu.Game.Rulesets.Mania.Mods.YuLiangSSSMods;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mania.Replays;
using osu.Game.Rulesets.Mania.Scoring;
using osu.Game.Rulesets.Mania.Skinning.Argon;
using osu.Game.Rulesets.Mania.Skinning.Default;
using osu.Game.Rulesets.Mania.Skinning.Legacy;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Scoring.Legacy;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Edit.Setup;
using osu.Game.Screens.Ranking.Statistics;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Mania
{
    public class ManiaRuleset : Ruleset, ILegacyRuleset
    {
        /// <summary>
        /// The maximum number of supported keys in a single stage.
        /// </summary>
        public const int MAX_STAGE_KEYS = 10;

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod>? mods = null) => new DrawableManiaRuleset(this, beatmap, mods);

        public override ScoreProcessor CreateScoreProcessor() => new ManiaScoreProcessor();

        public override HealthProcessor CreateHealthProcessor(double drainStartTime) => new ManiaHealthProcessor(drainStartTime);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new ManiaBeatmapConverter(beatmap, this);

        public override PerformanceCalculator CreatePerformanceCalculator() => new ManiaPerformanceCalculator();

        public const string SHORT_NAME = "mania";

        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;

        public override HitObjectComposer CreateHitObjectComposer() => new ManiaHitObjectComposer(this);

        public override IBeatmapVerifier CreateBeatmapVerifier() => new ManiaBeatmapVerifier();

        public override ISkin? CreateSkinTransformer(ISkin skin, IBeatmap beatmap)
        {
            switch (skin)
            {
                case TrianglesSkin:
                    return new ManiaTrianglesSkinTransformer(skin, beatmap);

                case ArgonSkin:
                    return new ManiaArgonSkinTransformer(skin, beatmap);

                case DefaultLegacySkin:
                case RetroSkin:
                    return new ManiaClassicSkinTransformer(skin, beatmap);

                case LegacySkin:
                    return new ManiaLegacySkinTransformer(skin, beatmap);
            }

            return null;
        }

        public override IEnumerable<Mod> ConvertFromLegacyMods(LegacyMods mods)
        {
            if (mods.HasFlag(LegacyMods.Nightcore))
                yield return new ManiaModNightcore();
            else if (mods.HasFlag(LegacyMods.DoubleTime))
                yield return new ManiaModDoubleTime();

            if (mods.HasFlag(LegacyMods.Perfect))
                yield return new ManiaModPerfect();
            else if (mods.HasFlag(LegacyMods.SuddenDeath))
                yield return new ManiaModSuddenDeath();

            if (mods.HasFlag(LegacyMods.Cinema))
                yield return new ManiaModCinema();
            else if (mods.HasFlag(LegacyMods.Autoplay))
                yield return new ManiaModAutoplay();

            if (mods.HasFlag(LegacyMods.Easy))
                yield return new ManiaModEasy();

            if (mods.HasFlag(LegacyMods.FadeIn))
                yield return new ManiaModFadeIn();

            if (mods.HasFlag(LegacyMods.Flashlight))
                yield return new ManiaModFlashlight();

            if (mods.HasFlag(LegacyMods.HalfTime))
                yield return new ManiaModHalfTime();

            if (mods.HasFlag(LegacyMods.HardRock))
                yield return new ManiaModHardRock();

            if (mods.HasFlag(LegacyMods.Hidden))
                yield return new ManiaModHidden();

            if (mods.HasFlag(LegacyMods.Key1))
                yield return new ManiaModKey1();

            if (mods.HasFlag(LegacyMods.Key2))
                yield return new ManiaModKey2();

            if (mods.HasFlag(LegacyMods.Key3))
                yield return new ManiaModKey3();

            if (mods.HasFlag(LegacyMods.Key4))
                yield return new ManiaModKey4();

            if (mods.HasFlag(LegacyMods.Key5))
                yield return new ManiaModKey5();

            if (mods.HasFlag(LegacyMods.Key6))
                yield return new ManiaModKey6();

            if (mods.HasFlag(LegacyMods.Key7))
                yield return new ManiaModKey7();

            if (mods.HasFlag(LegacyMods.Key8))
                yield return new ManiaModKey8();

            if (mods.HasFlag(LegacyMods.Key9))
                yield return new ManiaModKey9();

            if (mods.HasFlag(LegacyMods.KeyCoop))
                yield return new ManiaModDualStages();

            if (mods.HasFlag(LegacyMods.NoFail))
                yield return new ManiaModNoFail();

            if (mods.HasFlag(LegacyMods.Random))
                yield return new ManiaModRandom();

            if (mods.HasFlag(LegacyMods.Mirror))
                yield return new ManiaModMirror();

            if (mods.HasFlag(LegacyMods.ScoreV2))
                yield return new ManiaModScoreV2();
        }

        public override LegacyMods ConvertToLegacyMods(Mod[] mods)
        {
            var value = base.ConvertToLegacyMods(mods);

            foreach (var mod in mods)
            {
                switch (mod)
                {
                    case ManiaModKey1:
                        value |= LegacyMods.Key1;
                        break;

                    case ManiaModKey2:
                        value |= LegacyMods.Key2;
                        break;

                    case ManiaModKey3:
                        value |= LegacyMods.Key3;
                        break;

                    case ManiaModKey4:
                        value |= LegacyMods.Key4;
                        break;

                    case ManiaModKey5:
                        value |= LegacyMods.Key5;
                        break;

                    case ManiaModKey6:
                        value |= LegacyMods.Key6;
                        break;

                    case ManiaModKey7:
                        value |= LegacyMods.Key7;
                        break;

                    case ManiaModKey8:
                        value |= LegacyMods.Key8;
                        break;

                    case ManiaModKey9:
                        value |= LegacyMods.Key9;
                        break;

                    case ManiaModDualStages:
                        value |= LegacyMods.KeyCoop;
                        break;

                    case ManiaModFadeIn:
                        value |= LegacyMods.FadeIn;
                        value &= ~LegacyMods.Hidden; // this is toggled on in the base call due to inheritance, but we don't want that.
                        break;

                    case ManiaModMirror:
                        value |= LegacyMods.Mirror;
                        break;

                    case ManiaModRandom:
                        value |= LegacyMods.Random;
                        break;
                }
            }

            return value;
        }

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new Mod[]
                    {
                        new ManiaModEasy(),
                        new ManiaModNoFail(),
                        new MultiMod(new ManiaModHalfTime(), new ManiaModDaycore()),
                        new ManiaModNoRelease(),
                    };

                case ModType.DifficultyIncrease:
                    return new Mod[]
                    {
                        new ManiaModHardRock(),
                        new MultiMod(new ManiaModSuddenDeath(), new ManiaModPerfect()),
                        new MultiMod(new ManiaModDoubleTime(), new ManiaModNightcore()),
                        new MultiMod(new ManiaModFadeIn(), new ManiaModHidden(), new ManiaModCover()),
                        new ManiaModFlashlight(),
                        new ModAccuracyChallenge(),
                    };

                case ModType.Conversion:
                    return new Mod[]
                    {
                        new ManiaModAdjust(),
                        new StarRatingRebirth(),
                        new ManiaModNtoMAnother(),
                        new ManiaModNtoM(),
                        new ManiaModDuplicate(),
                        new ManiaModDoublePlay(),
                        new ManiaModNoteAdjust(),
                        new ManiaModLNTransformer(),
                        new ManiaModLNLongShortAddition(),
                        new ManiaModLNDoubleDistribution(),
                        new ManiaModLNSimplify(),
                        new ManiaModJackAdjust(),
                        new ManiaModDeleteSpace(),
                        new ManiaModCleaner(),
                        new ManiaModRandom(),
                        new ManiaModDualStages(),
                        new ManiaModMirror(),
                        new ManiaModDifficultyAdjust(),
                        new ManiaModClassic(),
                        new ManiaModInvert(),
                        new ManiaModConstantSpeed(),
                        new ManiaModHoldOff(),
                        new MultiMod(
                            new ManiaModKey1(),
                            new ManiaModKey2(),
                            new ManiaModKey3(),
                            new ManiaModKey4(),
                            new ManiaModKey5(),
                            new ManiaModKey6(),
                            new ManiaModKey7(),
                            new ManiaModKey8(),
                            new ManiaModKey9(),
                            new ManiaModKey10()
                        ),
                    };

                case ModType.Automation:
                    return new Mod[]
                    {
                        new MultiMod(new ManiaModAutoplay(), new ManiaModCinema()),
                    };

                case ModType.Fun:
                    return new Mod[]
                    {
                        new ManiaModAccuracyAdaptive(),
                        new ManiaModHealthAdaptive(),
                        new ManiaModO2Judgement(),
                        new ManiaModO2Health(),
                        new ManiaModLNColor(),
                        new ManiaModNiceBPM(),
                        new ManiaModGracer(),
                        new ManiaModNewJudgement(),
                        new ManiaModJudgmentsAdjust(),
                        new ManiaModRemedy(),
                        new ManiaModPlayfieldTransformation(),
                        new ManiaModLNJudgementAdjust(),
                        new ManiaModReleaseAdjust(),
                        new MultiMod(new ModWindUp(), new ModWindDown()),
                        new ManiaModMuted(),
                        new ModAdaptiveSpeed()
                    };

                case ModType.System:
                    return new Mod[]
                    {
                        new ManiaModScoreV2(),
                    };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override string Description => "osu!mania";

        public override string ShortName => SHORT_NAME;

        public override string PlayingVerb => "Smashing keys";

        public override Drawable CreateIcon() => new SpriteIcon { Icon = OsuIcon.RulesetMania };

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new ManiaDifficultyCalculator(RulesetInfo, beatmap);

        public int LegacyID => 3;

        public ILegacyScoreSimulator CreateLegacyScoreSimulator() => new ManiaLegacyScoreSimulator();

        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new ManiaReplayFrame();

        public override IRulesetConfigManager CreateConfig(SettingsStore? settings) => new ManiaRulesetConfigManager(settings, RulesetInfo);

        public override RulesetSettingsSubsection CreateSettings() => new ManiaSettingsSubsection(this);

        public override IEnumerable<int> AvailableVariants
        {
            get
            {
                for (int i = 1; i <= MAX_STAGE_KEYS; i++)
                    yield return (int)PlayfieldType.Single + i;
                for (int i = 2; i <= MAX_STAGE_KEYS * 2; i += 2)
                    yield return (int)PlayfieldType.Dual + i;
            }
        }

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0)
        {
            switch (getPlayfieldType(variant))
            {
                case PlayfieldType.Single:
                    return new SingleStageVariantGenerator(variant).GenerateMappings();

                case PlayfieldType.Dual:
                    return new DualStageVariantGenerator(getDualStageKeyCount(variant)).GenerateMappings();
            }

            return Array.Empty<KeyBinding>();
        }

        public override LocalisableString GetVariantName(int variant)
        {
            switch (getPlayfieldType(variant))
            {
                default:
                    return $"{variant}K";

                case PlayfieldType.Dual:
                {
                    int keys = getDualStageKeyCount(variant);
                    return $"{keys}K + {keys}K";
                }
            }
        }

        /// <summary>
        /// Finds the number of keys for each stage in a <see cref="PlayfieldType.Dual"/> variant.
        /// </summary>
        /// <param name="variant">The variant.</param>
        private int getDualStageKeyCount(int variant) => (variant - (int)PlayfieldType.Dual) / 2;

        /// <summary>
        /// Finds the <see cref="PlayfieldType"/> that corresponds to a variant value.
        /// </summary>
        /// <param name="variant">The variant value.</param>
        /// <returns>The <see cref="PlayfieldType"/> that corresponds to <paramref name="variant"/>.</returns>
        private PlayfieldType getPlayfieldType(int variant)
        {
            return (PlayfieldType)Enum.GetValues(typeof(PlayfieldType)).Cast<int>().OrderDescending().First(v => variant >= v);
        }

        protected override IEnumerable<HitResult> GetValidHitResults()
        {
            return new[]
            {
                HitResult.Perfect,
                HitResult.Great,
                HitResult.Good,
                HitResult.Ok,
                HitResult.Meh,

                // HitResult.SmallBonus is used for awarding perfect bonus score but is not included here as
                // it would be a bit redundant to show this to the user.
            };
        }

        public
            (
                int allLate,
                int allEarly,
                int allCount,
                int allPerfectCount,
                int allGreatCount,
                int allGoodCount,
                int allOkCount,
                int allMehCount,
                int allMissCount,
                int allPerfectCountLate,
                int allPerfectCountEarly,
                int allGreatCountLate,
                int allGreatCountEarly,
                int allGoodCountLate,
                int allGoodCountEarly,
                int allOkCountLate,
                int allOkCountEarly,
                int allMehCountLate,
                int allMehCountEarly,
                int allMissCountLate,
                int allMissCountEarly
            ) GetJudgementCounts(IEnumerable<HitEvent> hitEvents)
        {
            int allPerfectCount = hitEvents.Count(h => h.Result == HitResult.Perfect);
            int allGreatCount = hitEvents.Count(h => h.Result == HitResult.Great);
            int allGoodCount = hitEvents.Count(h => h.Result == HitResult.Good);
            int allOkCount = hitEvents.Count(h => h.Result == HitResult.Ok);
            int allMehCount = hitEvents.Count(h => h.Result == HitResult.Meh);
            int allMissCount = hitEvents.Count(h => h.Result == HitResult.Miss);
            int allPerfectCountLate = hitEvents.Count(h => h.Result == HitResult.Perfect && h.TimeOffset > 0);
            int allPerfectCountEarly = hitEvents.Count(h => h.Result == HitResult.Perfect && h.TimeOffset <= 0);
            int allGreatCountLate = hitEvents.Count(h => h.Result == HitResult.Great && h.TimeOffset > 0);
            int allGreatCountEarly = hitEvents.Count(h => h.Result == HitResult.Great && h.TimeOffset <= 0);
            int allGoodCountLate = hitEvents.Count(h => h.Result == HitResult.Good && h.TimeOffset > 0);
            int allGoodCountEarly = hitEvents.Count(h => h.Result == HitResult.Good && h.TimeOffset <= 0);
            int allOkCountLate = hitEvents.Count(h => h.Result == HitResult.Ok && h.TimeOffset > 0);
            int allOkCountEarly = hitEvents.Count(h => h.Result == HitResult.Ok && h.TimeOffset <= 0);
            int allMehCountLate = hitEvents.Count(h => h.Result == HitResult.Meh && h.TimeOffset > 0);
            int allMehCountEarly = hitEvents.Count(h => h.Result == HitResult.Meh && h.TimeOffset <= 0);
            int allMissCountLate = hitEvents.Count(h => h.Result == HitResult.Miss && h.TimeOffset > 0);
            int allMissCountEarly = hitEvents.Count(h => h.Result == HitResult.Miss && h.TimeOffset <= 0);

            int allLate = allPerfectCountLate + allGreatCountLate + allGoodCountLate + allOkCountLate + allMehCountLate + allMissCountLate;
            int allEarly = allPerfectCountEarly + allGreatCountEarly + allGoodCountEarly + allOkCountEarly + allMehCountEarly + allMissCountEarly;
            int allCount = allLate + allEarly;

            return
                (
                    allLate,
                    allEarly,
                    allCount,
                    allPerfectCount,
                    allGreatCount,
                    allGoodCount,
                    allOkCount,
                    allMehCount,
                    allMissCount,
                    allPerfectCountLate,
                    allPerfectCountEarly,
                    allGreatCountLate,
                    allGreatCountEarly,
                    allGoodCountLate,
                    allGoodCountEarly,
                    allOkCountLate,
                    allOkCountEarly,
                    allMehCountLate,
                    allMehCountEarly,
                    allMissCountLate,
                    allMissCountEarly
                );
        }

        public void WriteJudgement
            (
                List<StatisticItem> itemList,
                string name,
                (
                    int allLate,
                    int allEarly,
                    int allCount,
                    int allPerfectCount,
                    int allGreatCount,
                    int allGoodCount,
                    int allOkCount,
                    int allMehCount,
                    int allMissCount,
                    int allPerfectCountLate,
                    int allPerfectCountEarly,
                    int allGreatCountLate,
                    int allGreatCountEarly,
                    int allGoodCountLate,
                    int allGoodCountEarly,
                    int allOkCountLate,
                    int allOkCountEarly,
                    int allMehCountLate,
                    int allMehCountEarly,
                    int allMissCountLate,
                    int allMissCountEarly
                ) judgement
            )
        {
            var totalColor = Color4Extensions.FromHex(@"ff8c00");
            var perfectColor = Color4Extensions.FromHex(@"99eeff");
            var greatColor = Color4Extensions.FromHex(@"66ccff");
            var goodColor = Color4Extensions.FromHex(@"b3d944");
            var okColor = Color4Extensions.FromHex(@"88b300");
            var mehColor = Color4Extensions.FromHex(@"ffcc22");
            var missColor = Color4Extensions.FromHex(@"ed1121");

            itemList.Add(new StatisticItem(name, () => new SimpleStatisticTable(3, new SimpleStatisticItem[]
            {
                new JudgementsItem(judgement.allCount.ToString(), "Total", totalColor),
                new JudgementsItem(judgement.allLate.ToString(), "Total (Late)", ColourInfo.GradientVertical(Colour4.White, totalColor)),
                new JudgementsItem(judgement.allEarly.ToString(), "Total (Early)", ColourInfo.GradientVertical(totalColor, Colour4.White)),
                new JudgementsItem(judgement.allPerfectCount.ToString(), "Perfect", perfectColor),
                new JudgementsItem(judgement.allPerfectCountLate.ToString(), "Perfect (Late)", ColourInfo.GradientVertical(Colour4.White, perfectColor)),
                new JudgementsItem(judgement.allPerfectCountEarly.ToString(), "Perfect (Early)", ColourInfo.GradientVertical(perfectColor, Colour4.White)),
                new JudgementsItem(judgement.allGreatCount.ToString(), "Great", greatColor),
                new JudgementsItem(judgement.allGreatCountLate.ToString(), "Great (Late)", ColourInfo.GradientVertical(Colour4.White, greatColor)),
                new JudgementsItem(judgement.allGreatCountEarly.ToString(), "Great (Early)", ColourInfo.GradientVertical(greatColor, Colour4.White)),
                new JudgementsItem(judgement.allGoodCount.ToString(), "Good", goodColor),
                new JudgementsItem(judgement.allGoodCountLate.ToString(), "Good (Late)", ColourInfo.GradientVertical(Colour4.White, goodColor)),
                new JudgementsItem(judgement.allGoodCountEarly.ToString(), "Good (Early)", ColourInfo.GradientVertical(goodColor, Colour4.White)),
                new JudgementsItem(judgement.allOkCount.ToString(), "Ok", okColor),
                new JudgementsItem(judgement.allOkCountLate.ToString(), "Ok (Late)", ColourInfo.GradientVertical(Colour4.White, okColor)),
                new JudgementsItem(judgement.allOkCountEarly.ToString(), "Ok (Early)", ColourInfo.GradientVertical(okColor, Colour4.White)),
                new JudgementsItem(judgement.allMehCount.ToString(), "Meh", mehColor),
                new JudgementsItem(judgement.allMehCountLate.ToString(), "Meh (Late)", ColourInfo.GradientVertical(Colour4.White, mehColor)),
                new JudgementsItem(judgement.allMehCountEarly.ToString(), "Meh (Early)", ColourInfo.GradientVertical(mehColor, Colour4.White)),
                new JudgementsItem(judgement.allMissCount.ToString(), "Miss", missColor),
                new JudgementsItem(judgement.allMissCountLate.ToString(), "Miss (Late)", ColourInfo.GradientVertical(Colour4.White, missColor)),
                new JudgementsItem(judgement.allMissCountEarly.ToString(), "Miss (Early)", ColourInfo.GradientVertical(missColor, Colour4.White)),
            }), true));
        }

        public override StatisticItem[] CreateStatisticsForScore(ScoreInfo score, IBeatmap playableBeatmap)
        {
            var hitWindows = new ManiaHitWindows();

            hitWindows.SetDifficulty(playableBeatmap.Difficulty.OverallDifficulty);

            foreach (var mod in score.Mods)
            {
                if (mod is ManiaModAdjust adjust)
                {
                    if (adjust.CustomHitRange.Value)
                    {
                        DifficultyRange[] ranges =
                        {
                            new DifficultyRange(adjust.PerfectHit.Value, adjust.PerfectHit.Value, adjust.PerfectHit.Value),
                            new DifficultyRange(adjust.GreatHit.Value, adjust.GreatHit.Value, adjust.GreatHit.Value),
                            new DifficultyRange(adjust.GoodHit.Value, adjust.GoodHit.Value, adjust.GoodHit.Value),
                            new DifficultyRange(adjust.OkHit.Value, adjust.OkHit.Value, adjust.OkHit.Value),
                            new DifficultyRange(adjust.MehHit.Value, adjust.MehHit.Value, adjust.MehHit.Value),
                            new DifficultyRange(adjust.MissHit.Value, adjust.MissHit.Value, adjust.MissHit.Value),
                        };
                        hitWindows.SetDifficulty(0);
                        hitWindows.SetSpecialDifficultyRange(ranges);
                    }
                }
            }

            var itemList = new List<StatisticItem>
            {
                new StatisticItem("Performance Breakdown", () => new PerformanceBreakdownChart(score, playableBeatmap)
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y
                }),
                new StatisticItem("Hit Distribution Dot", () => new HitEventTimingDistributionDot(score.HitEvents, hitWindows)
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 300
                }, true),
                new StatisticItem("Timing Distribution", () => new HitEventTimingDistributionGraph(score.HitEvents)
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 250
                }, true),
            };

            int allPerfectCount = score.HitEvents.Count(h => h.Result == HitResult.Perfect);
            int allGreatCount = score.HitEvents.Count(h => h.Result == HitResult.Great);
            int allGoodCount = score.HitEvents.Count(h => h.Result == HitResult.Good);
            int allOkCount = score.HitEvents.Count(h => h.Result == HitResult.Ok);
            int allMehCount = score.HitEvents.Count(h => h.Result == HitResult.Meh);
            int allMissCount = score.HitEvents.Count(h => h.Result == HitResult.Miss);
            int allPerfectCountLate = score.HitEvents.Count(h => h.Result == HitResult.Perfect && h.TimeOffset > 0);
            int allPerfectCountEarly = score.HitEvents.Count(h => h.Result == HitResult.Perfect && h.TimeOffset <= 0);
            int allGreatCountLate = score.HitEvents.Count(h => h.Result == HitResult.Great && h.TimeOffset > 0);
            int allGreatCountEarly = score.HitEvents.Count(h => h.Result == HitResult.Great && h.TimeOffset <= 0);
            int allGoodCountLate = score.HitEvents.Count(h => h.Result == HitResult.Good && h.TimeOffset > 0);
            int allGoodCountEarly = score.HitEvents.Count(h => h.Result == HitResult.Good && h.TimeOffset <= 0);
            int allOkCountLate = score.HitEvents.Count(h => h.Result == HitResult.Ok && h.TimeOffset > 0);
            int allOkCountEarly = score.HitEvents.Count(h => h.Result == HitResult.Ok && h.TimeOffset <= 0);
            int allMehCountLate = score.HitEvents.Count(h => h.Result == HitResult.Meh && h.TimeOffset > 0);
            int allMehCountEarly = score.HitEvents.Count(h => h.Result == HitResult.Meh && h.TimeOffset <= 0);
            int allMissCountLate = score.HitEvents.Count(h => h.Result == HitResult.Miss && h.TimeOffset > 0);
            int allMissCountEarly = score.HitEvents.Count(h => h.Result == HitResult.Miss && h.TimeOffset <= 0);

            int allLate = allPerfectCountLate + allGreatCountLate + allGoodCountLate + allOkCountLate + allMehCountLate + allMissCountLate;
            int allEarly = allPerfectCountEarly + allGreatCountEarly + allGoodCountEarly + allOkCountEarly + allMehCountEarly + allMissCountEarly;
            int allCount = allLate + allEarly;

            var rgNote = score.HitEvents.Where(h => h.HitObject is Note && h.HitObject is not HeadNote && h.HitObject is not TailNote).ToList();
            var lnHead = score.HitEvents.Where(h => h.HitObject is HeadNote).ToList();
            var lnTail = score.HitEvents.Where(h => h.HitObject is TailNote).ToList();

            var allJudge = GetJudgementCounts(score.HitEvents);
            var noteJudge = GetJudgementCounts(rgNote);
            var headJudge = GetJudgementCounts(lnHead);
            var tailJudge = GetJudgementCounts(lnTail);

            itemList.Add(new StatisticItem("All Statistics", () => new SimpleStatisticTable(2, new SimpleStatisticItem[]
            {
                new AverageHitError(score.HitEvents),
                new UnstableRate(score.HitEvents),
            }), true));


            #region Score V1 Calculation

            double PerfectRange = 16 + 0.5;
            double GreatRange = 34 + 0.5;
            double GoodRange = 67 + 0.5;
            double OkRange = 97 + 0.5;
            double MehRange = 121 + 0.5;
            double MissRange = 158 + 0.5;

            double[] HeadOffsets = new double[18];
            double MaxPoints = 0;
            double TotalPoints = 0;

            double TotalMultiplier = hitWindows.SpeedMultiplier / hitWindows.DifficultyMultiplier;

            void calculateRange(double od)
            {
                double invertedOd = 10 - od;

                // Do not use +0.5.
                //PerfectRange = Math.Floor(16 * TotalMultiplier) + 0.5;
                //GreatRange = Math.Floor((34 + 3 * invertedOd)) * TotalMultiplier + 0.5;
                //GoodRange = Math.Floor((67 + 3 * invertedOd)) * TotalMultiplier + 0.5;
                //OkRange = Math.Floor((97 + 3 * invertedOd)) * TotalMultiplier + 0.5;
                //MehRange = Math.Floor((121 + 3 * invertedOd)) * TotalMultiplier + 0.5;
                //MissRange = Math.Floor((158 + 3 * invertedOd)) * TotalMultiplier + 0.5;

                PerfectRange = Math.Floor(16 * TotalMultiplier) + 0.5;
                GreatRange = Math.Floor((34 + 3 * invertedOd)) * TotalMultiplier + 0.5;
                GoodRange = Math.Floor((67 + 3 * invertedOd)) * TotalMultiplier + 0.5;
                OkRange = Math.Floor((97 + 3 * invertedOd)) * TotalMultiplier + 0.5;
                MehRange = Math.Floor((121 + 3 * invertedOd)) * TotalMultiplier + 0.5;
                MissRange = Math.Floor((158 + 3 * invertedOd)) * TotalMultiplier + 0.5;
            }

            double od = 0.0;
            bool odFlag = true;
            foreach (Mod mod in score.Mods)
            {
                if (mod is ManiaModAdjust adjust)
                {
                    if (adjust.OverallDifficulty.Value is not null && adjust.CustomOD.Value)
                    {
                        od = (float)adjust.OverallDifficulty.Value;
                        odFlag = false;
                        break;
                    }
                }
            }

            if (odFlag)
            {
                od = playableBeatmap.Difficulty.OverallDifficulty;
            }

            calculateRange(od);

            HitResult getResultByOffset(double offset) =>
            offset <= PerfectRange ? HitResult.Perfect :
            offset <= GreatRange ? HitResult.Great :
            offset <= GoodRange ? HitResult.Good :
            offset <= OkRange ? HitResult.Ok :
            offset <= MehRange ? HitResult.Meh :
            offset <= MissRange ? HitResult.Miss :
            HitResult.Miss;

            double getLNScore(double head, double tail)
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
                    {
                        return score;
                    }
                }

                return 0;
            }

            foreach (var hit in score.HitEvents)
            {
                double offset = Math.Abs(hit.TimeOffset);
                var result = getResultByOffset(offset);
                var hitObject = (ManiaHitObject)hit.HitObject;
                if (hitObject is HeadNote)
                {
                    HeadOffsets[hitObject.Column] = offset;
                }
                else if (hitObject is TailNote)
                {
                    MaxPoints += 300;
                    TotalPoints += getLNScore(HeadOffsets[hitObject.Column], offset);
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
            }

            itemList.Add(new StatisticItem("Score Accuracy  OD: " + od.ToString("0.0"), () => new SimpleStatisticTable(2, new SimpleStatisticItem[]
            {
                new JudgementsItem((TotalPoints / MaxPoints * 100.0).ToString("0.00") + "%", "Score v1", Colour4.White),
                new JudgementsItem((score.Accuracy * 100.0).ToString("0.00") + "%", "Score v2", Colour4.White),
            }), true));

            #endregion


            WriteJudgement(itemList, "All Judgement", allJudge);
            WriteJudgement(itemList, "Note Judgement", noteJudge);
            WriteJudgement(itemList, "LN Head Judgement", headJudge);
            WriteJudgement(itemList, "LN Tail Judgement", tailJudge);

            itemList.Add(new StatisticItem("Timing Distribution By Column", () => new HitEventTimingDistributionGraphByColumn(score.HitEvents, ((ManiaBeatmap)playableBeatmap).TotalColumns)
            {
                RelativeSizeAxes = Axes.X,
            }, true));

            return itemList.ToArray();
        }

        /// <seealso cref="ManiaHitWindows"/>
        public override BeatmapDifficulty GetAdjustedDisplayDifficulty(IBeatmapInfo beatmapInfo, IReadOnlyCollection<Mod> mods)
        {
            BeatmapDifficulty adjustedDifficulty = base.GetAdjustedDisplayDifficulty(beatmapInfo, mods);

            // notably, in mania, hit windows are designed to be independent of track playback rate (see `ManiaHitWindows.SpeedMultiplier`).
            // *however*, to not make matters *too* simple, mania Hard Rock and Easy differ from all other rulesets
            // in that they apply multipliers *to hit window durations directly* rather than to the Overall Difficulty attribute itself.
            // because the duration of hit window durations as a function of OD is not a linear function,
            // this means that multiplying the OD is *not* the same thing as multiplying the hit window duration.
            // in fact, the second operation is *much* harsher and will produce values much farther outside of normal operating range
            // (even negative in the case of Easy).
            // stable handles this wrong on song select and just assumes that it can handle mania EZ / HR the same way as all other rulesets.

            double perfectHitWindow = IBeatmapDifficultyInfo.DifficultyRange(adjustedDifficulty.OverallDifficulty, ManiaHitWindows.PERFECT_WINDOW_RANGE);

            if (mods.Any(m => m is ManiaModHardRock))
                perfectHitWindow /= ManiaModHardRock.HIT_WINDOW_DIFFICULTY_MULTIPLIER;
            else if (mods.Any(m => m is ManiaModEasy))
                perfectHitWindow /= ManiaModEasy.HIT_WINDOW_DIFFICULTY_MULTIPLIER;

            adjustedDifficulty.OverallDifficulty = (float)IBeatmapDifficultyInfo.InverseDifficultyRange(perfectHitWindow, ManiaHitWindows.PERFECT_WINDOW_RANGE);
            adjustedDifficulty.CircleSize = ManiaBeatmapConverter.GetColumnCount(LegacyBeatmapConversionDifficultyInfo.FromBeatmapInfo(beatmapInfo), mods);

            return adjustedDifficulty;
        }

        public override IEnumerable<RulesetBeatmapAttribute> GetBeatmapAttributesForDisplay(IBeatmapInfo beatmapInfo, IReadOnlyCollection<Mod> mods)
        {
            // a special touch-up of key count is required to the original difficulty, since key conversion mods are not `IApplicableToDifficulty`
            var originalDifficulty = new BeatmapDifficulty(beatmapInfo.Difficulty)
            {
                CircleSize = ManiaBeatmapConverter.GetColumnCount(LegacyBeatmapConversionDifficultyInfo.FromBeatmapInfo(beatmapInfo), [])
            };
            var adjustedDifficulty = GetAdjustedDisplayDifficulty(beatmapInfo, mods);
            var colours = new OsuColour();

            yield return new RulesetBeatmapAttribute(SongSelectStrings.KeyCount, @"KC", originalDifficulty.CircleSize, adjustedDifficulty.CircleSize, 18)
            {
                Description = "Affects the number of key columns on the playfield."
            };

            var hitWindows = new ManiaHitWindows();
            hitWindows.SetDifficulty(adjustedDifficulty.OverallDifficulty);
            hitWindows.IsConvert = !beatmapInfo.Ruleset.Equals(RulesetInfo);
            hitWindows.ClassicModActive = mods.Any(m => m is ManiaModClassic);
            yield return new RulesetBeatmapAttribute(SongSelectStrings.Accuracy, @"OD", originalDifficulty.OverallDifficulty, adjustedDifficulty.OverallDifficulty, 10)
            {
                Description = "Affects timing requirements for notes.",
                AdditionalMetrics = hitWindows.GetAllAvailableWindows()
                                              .Reverse()
                                              .Select(window => new RulesetBeatmapAttribute.AdditionalMetric(
                                                  $"{window.result.GetDescription().ToUpperInvariant()} hit window",
                                                  LocalisableString.Interpolate($@"±{hitWindows.WindowFor(window.result):0.##} ms"),
                                                  colours.ForHitResult(window.result)
                                              )).ToArray()
            };

            yield return new RulesetBeatmapAttribute(SongSelectStrings.HPDrain, @"HP", originalDifficulty.DrainRate, adjustedDifficulty.DrainRate, 10)
            {
                Description = "Affects the harshness of health drain and the health penalties for missing."
            };
        }

        public override IRulesetFilterCriteria CreateRulesetFilterCriteria()
        {
            return new ManiaFilterCriteria();
        }

        public override IEnumerable<Drawable> CreateEditorSetupSections() =>
        [
            new MetadataSection(),
            new ManiaDifficultySection(),
            new ResourcesSection(),
            new DesignSection(),
        ];

        public int GetKeyCount(IBeatmapInfo beatmapInfo, IReadOnlyList<Mod>? mods = null)
            => ManiaBeatmapConverter.GetColumnCount(LegacyBeatmapConversionDifficultyInfo.FromBeatmapInfo(beatmapInfo), mods);
    }

    public enum PlayfieldType
    {
        /// <summary>
        /// Columns are grouped into a single stage.
        /// Number of columns in this stage lies at (item - Single).
        /// </summary>
        Single = 0,

        /// <summary>
        /// Columns are grouped into two stages.
        /// Overall number of columns lies at (item - Dual), further computation is required for
        /// number of columns in each individual stage.
        /// </summary>
        Dual = 1000,
    }
}
