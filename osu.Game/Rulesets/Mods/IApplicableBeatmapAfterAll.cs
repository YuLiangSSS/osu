// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Mods
{
    public interface IApplicableBeatmapAfterAll : IApplicableMod
    {
        void ApplyToFinalBeatmap(IBeatmap converted, IReadOnlyList<Mod> mods, Ruleset ruleset);
    }
}
