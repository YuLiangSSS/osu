// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Screens.SelectV2
{
    public class CircleSizeItemInfo
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
        public float? CsValue { get; set; }
        public bool IsDefault { get; set; }
    }

    public static class CircleSizeItemID
    {
        private const int mania_ruleset_id = 3;

        public static readonly List<CircleSizeItemInfo> ALL = new List<CircleSizeItemInfo>
        {
            new CircleSizeItemInfo { Id = "All", DisplayName = "All", IsDefault = true },
            new CircleSizeItemInfo { Id = "CS1", DisplayName = "1", CsValue = 1 },
            new CircleSizeItemInfo { Id = "CS2", DisplayName = "2", CsValue = 2 },
            new CircleSizeItemInfo { Id = "CS3", DisplayName = "3", CsValue = 3 },
            new CircleSizeItemInfo { Id = "CS4", DisplayName = "4", CsValue = 4 },
            new CircleSizeItemInfo { Id = "CS5", DisplayName = "5", CsValue = 5 },
            new CircleSizeItemInfo { Id = "CS6", DisplayName = "6", CsValue = 6 },
            new CircleSizeItemInfo { Id = "CS7", DisplayName = "7", CsValue = 7 },
            new CircleSizeItemInfo { Id = "CS8", DisplayName = "8", CsValue = 8 },
            new CircleSizeItemInfo { Id = "CS9", DisplayName = "9", CsValue = 9 },
            new CircleSizeItemInfo { Id = "CS10", DisplayName = "10", CsValue = 10 },
            new CircleSizeItemInfo { Id = "CS12", DisplayName = "12", CsValue = 12 },
            new CircleSizeItemInfo { Id = "CS14", DisplayName = "14", CsValue = 14 },
            new CircleSizeItemInfo { Id = "CS16", DisplayName = "16", CsValue = 16 },
            new CircleSizeItemInfo { Id = "CS18", DisplayName = "18", CsValue = 18 },
        };

        public static List<CircleSizeItemInfo> GetModesForRuleset(int rulesetId)
        {
            if (rulesetId == mania_ruleset_id)
                return ALL.Where(m => m.CsValue == null || m.CsValue >= 4).ToList();

            return ALL.Where(m => m.CsValue == null || m.CsValue <= 12).ToList();
        }

        public static CircleSizeItemInfo? GetById(string id) => ALL.FirstOrDefault(m => m.Id == id);
    }

    public class CircleSizeFilter
    {
        public HashSet<string> SelectedModeID { get; } = new HashSet<string> { "All" };

        public event Action<bool>? SelectionChanged;

        public void SetSelection(HashSet<string> modeIds)
        {
            var newSet = new HashSet<string>();
            if (modeIds.Count == 0 || modeIds.Contains("All"))
                newSet.Add("All");
            else
                newSet.UnionWith(modeIds);

            SelectedModeID.Clear();
            SelectedModeID.UnionWith(newSet);
            SelectionChanged?.Invoke(true);
        }
    }
}
