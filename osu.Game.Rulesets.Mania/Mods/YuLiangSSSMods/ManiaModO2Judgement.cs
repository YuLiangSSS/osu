// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Threading;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mania.Beatmaps;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mania.Objects.Drawables;
using osu.Game.Rulesets.Mania.Scoring;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.SelectV2;

namespace osu.Game.Rulesets.Mania.Mods.YuLiangSSSMods
{
    public partial class ManiaModO2Judgement : Mod, IApplicableToDifficulty, IApplicableAfterBeatmapConversion, IApplicableToDrawableRuleset<ManiaHitObject>, IApplicableBeatmapAfterAll
    {
        public const double COOL = 7500.0;
        public const double GOOD = 22500.0;
        public const double BAD = 31250.0;
        public const int MAX_PILL = 5;

        public static double CoolRange => COOL / FinalBPM;
        public static double GoodRange => GOOD / FinalBPM;
        public static double BadRange => BAD / FinalBPM;

        public static double FinalBPM => BPMAdjust.Value ?? ConvertedBPM ?? NowBeatmapBPM;

        // Return as tuple to avoid repeated array allocations.
        public static (double cool, double good, double bad) GetRangeAtTime(double time)
        {
            double bpm = GetBPMAtTime(time);
            if (bpm <= 0) bpm = NowBeatmapBPM;
            return (COOL / bpm, GOOD / bpm, BAD / bpm);
        }

        private static double coolDescription = 0;
        private static double goodDescription = 0;
        private static double badDescription = 0;

        public static double GetCoolRangeAtTime(double time)
        {
            var (c, _, _) = GetRangeAtTime(time);
            coolDescription = c;
            return c;
        }

        public static double GetGoodRangeAtTime(double time)
        {
            var (_, g, _) = GetRangeAtTime(time);
            goodDescription = g;
            return g;
        }

        public static double GetBadRangeAtTime(double time)
        {
            var (_, _, b) = GetRangeAtTime(time);
            badDescription = b;
            return b;
        }

        public static double GetBPMAtTime(double time) => ControlPoints?.TimingPointAt(time).BPM ?? NowBeatmapBPM;

        // MISS

        public const double DEFAULT_BPM = 160;

        public static BindableInt Pill = new();
        public static BindableInt CoolCombo = new();
        public static Bindable<int?> BPMAdjust = new();
        public static bool PillActivated;
        public static ManiaHitWindows Windows = new ManiaHitWindows();
        public static double? ConvertedBPM;
        public static ControlPointInfo? ControlPoints;
        public static bool IsActivated;

        public override string Name => "O2JAM Judgement";

        public override string Acronym => "OJ";

        public override LocalisableString Description => "Judgement System for O2JAM players.";

        public override double ScoreMultiplier => 1.0;

        public override ModType Type => ModType.Fun;

        public ManiaHitWindows HitWindows { get; set; } = new ManiaHitWindows();

        public override IEnumerable<(LocalisableString setting, LocalisableString value)> SettingDescription
        {
            get
            {
                if (BPM.Value is not null)
                {
                    yield return ("BPM", BPM.Value.Value.ToString());
                }
                yield return ("Cool Range", $"{Math.Round(coolDescription, 2)} ms");
                yield return ("Good Range", $"{Math.Round(goodDescription, 2)} ms");
                yield return ("Bad Range", $"{Math.Round(badDescription, 2)} ms");
                if (PillMode.Value)
                {
                    yield return ("Pill", "On");
                }
            }
        }

        [SettingSource("BPM", "Adjust BPM for HitWindows.", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> BPM { get; set; } = new Bindable<int?>();

        [SettingSource("Pill Switch", "Use O2JAM pill function.")]
        public BindableBool PillMode { get; set; } = new BindableBool(true);

        public static double NowBeatmapBPM
        {
            get
            {
                if (BeatmapTitleWedge.SelectedWorkingBeatmap is not null)
                {
                    return BeatmapTitleWedge.SelectedWorkingBeatmap.BeatmapInfo.BPM;
                }
                else
                {
                    return DEFAULT_BPM;
                }
            }
        }

        public void ApplyToDrawableRuleset(DrawableRuleset<ManiaHitObject> drawableRuleset)
        {
            if (drawableRuleset is not DrawableManiaRuleset maniaRuleset)
                return;

            foreach (var stage in maniaRuleset.Playfield.Stages)
            {
                foreach (var column in stage.Columns)
                {
                    column.RegisterPool<O2Note, O2DrawableNote>(10, 50);
                    column.RegisterPool<O2HeadNote, O2DrawableHoldNoteHead>(10, 50);
                    column.RegisterPool<O2TailNote, O2DrawableHoldNoteTail>(10, 50);
                }
            }
        }

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            if (beatmap is not ManiaBeatmap maniaBeatmap)
                return;

            var converted = new List<ManiaHitObject>(maniaBeatmap.HitObjects.Count);
            foreach (var obj in maniaBeatmap.HitObjects)
            {
                converted.Add(obj switch
                {
                    Note n => new O2Note(n),
                    HoldNote h => new O2HoldNote(h),
                    _ => obj,
                });
            }

            maniaBeatmap.HitObjects = converted;
        }

        public void ApplyToDifficulty(BeatmapDifficulty difficulty)
        {
            BPM.CopyTo(BPMAdjust);
            Pill.Value = 0;
            PillActivated = PillMode.Value;
            HitWindows.SetSpecialDifficultyRange(CoolRange, CoolRange, GoodRange, GoodRange, BadRange, BadRange);
            Windows = HitWindows;
            IsActivated = true;
        }

        public override void ResetSettingsToDefaults()
        {
            base.ResetSettingsToDefaults();
            HitWindows.ResetRange();
            ControlPoints = null;
            IsActivated = false;
        }

        public void ApplyToFinalBeatmap(IBeatmap converted, IReadOnlyList<Mod> mods, Ruleset ruleset)
        {
            foreach (var mod in mods)
            {
                if (mod is ManiaModAdjust adjust)
                {
                    ConvertedBPM = NowBeatmapBPM * adjust.SpeedChange.Value;
                    HitWindows.SetSpecialDifficultyRange(CoolRange, CoolRange, GoodRange, GoodRange, BadRange, BadRange);
                    Windows = HitWindows;
                }
            }
            ControlPoints = converted.ControlPointInfo;
        }

        public sealed partial class O2DrawableNote : DrawableNote
        {
            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (userTriggered)
                {
                    bool flowControl = O2Helper.PillCheck(timeOffset, Time.Current, out var result, this);
                    if (result != HitResult.None)
                    {
                        ApplyResult(result);
                        return;
                    }
                    if (!flowControl)
                    {
                        return;
                    }
                }
                base.CheckForResult(userTriggered, timeOffset);
            }
        }

        public sealed partial class O2DrawableHoldNoteHead : DrawableHoldNoteHead
        {
            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (userTriggered)
                {
                    bool flowControl = O2Helper.PillCheck(timeOffset, Time.Current, out var result, this);
                    if (result != HitResult.None)
                    {
                        ApplyResult(result);
                        return;
                    }
                    if (!flowControl)
                    {
                        return;
                    }
                }
                base.CheckForResult(userTriggered, timeOffset);
            }
        }

        public sealed partial class O2DrawableHoldNoteTail : DrawableHoldNoteTail
        {
            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (userTriggered)
                {
                    bool flowControl = O2Helper.PillCheck(timeOffset, Time.Current, out var result, this);
                    if (result != HitResult.None)
                    {
                        // ApplyResult(GetCappedResult(result));
                        ApplyResult(result);
                        return;
                    }
                    if (!flowControl)
                    {
                        return;
                    }
                }
                base.CheckForResult(userTriggered, timeOffset * TailNote.RELEASE_WINDOW_LENIENCE);
            }
        }

        public static class O2Helper
        {
            public static bool PillCheck(double timeOffset, double currentTime, out HitResult result, DrawableManiaHitObject obj)
            {
                // Determine if this tail has a combo break (hold broken or head not hit).
                var tail = obj as O2DrawableHoldNoteTail;
                bool hasComboBreak = tail is not null && (!tail.HoldNote.Head.IsHit || tail.HoldNote.Body.HasHoldBreak);

                var (cool, good, bad) = GetRangeAtTime(currentTime);
                // update description values for UI display
                coolDescription = cool;
                goodDescription = good;
                badDescription = bad;

                Windows.SetSpecialDifficultyRange(cool, cool, good, good, bad, bad);
                result = Windows.ResultFor(timeOffset);

                if (!PillActivated)
                    return true;

                if (hasComboBreak && tail is not null)
                {
                    result = HitResult.Miss;
                    return false;
                }

                if (result == HitResult.Perfect)
                {
                    // increment cool combo and grant pills periodically
                    CoolCombo.Value++;
                    if (CoolCombo.Value >= 15)
                    {
                        CoolCombo.Value -= 15;
                        if (Pill.Value < MAX_PILL)
                            Pill.Value++;
                    }
                    return true;
                }

                if (result == HitResult.Good)
                {
                    CoolCombo.Value = 0;
                    return true;
                }

                if (result != HitResult.Miss && result != HitResult.None)
                {
                    CoolCombo.Value = 0;
                    if (Pill.Value > 0)
                    {
                        Pill.Value--;
                        result = HitResult.Perfect;
                        return false;
                    }
                }

                return true;
            }
        }

        private sealed class O2Note : Note
        {
            public O2Note(Note note)
            {
                StartTime = note.StartTime;
                Column = note.Column;
                Samples = note.Samples;
            }

            protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
            {
            }
        }

        private sealed class O2HeadNote : HeadNote { }

        private sealed class O2TailNote : TailNote
        {
            public override double MaximumJudgementOffset => base.MaximumJudgementOffset / RELEASE_WINDOW_LENIENCE;
        }

        public sealed class O2HoldNote : HoldNote
        {
            public O2HoldNote(HoldNote hold)
            {
                StartTime = hold.StartTime;
                Duration = hold.Duration;
                Column = hold.Column;
                NodeSamples = hold.NodeSamples;
            }

            protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
            {

                AddNested(Head = new O2HeadNote
                {
                    StartTime = StartTime,
                    Column = Column,
                    Samples = GetNodeSamples(0),
                });

                AddNested(Tail = new O2TailNote
                {
                    StartTime = EndTime,
                    Column = Column,
                    Samples = GetNodeSamples((NodeSamples?.Count - 1) ?? 1),
                });

                AddNested(Body = new HoldNoteBody
                {
                    StartTime = StartTime,
                    Column = Column,
                });
            }

            public override double MaximumJudgementOffset => base.MaximumJudgementOffset / TailNote.RELEASE_WINDOW_LENIENCE;
        }
    }
}

