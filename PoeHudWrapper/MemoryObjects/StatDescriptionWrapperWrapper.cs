using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Interfaces;
using Newtonsoft.Json;

namespace PoeHudWrapper.MemoryObjects;

public record ModTranslationReplacerInput(GameStat Stat, int RawValue, float ConvertedValue, string ConvertedValueString);

public partial class StatDescriptionWrapperWrapper : UniversalFileWrapperWrapper<StatDescription>
{
    public StatDescriptionWrapperWrapper(IMemory mem, Func<string, long> address, string fileName) : base(mem, () => address(fileName))
    {
        CheckCache();
        StaticFileName = fileName;
    }

    public string StaticFileName { get; }
    protected override long RecordLength => 8;
    protected override int NumberOfRecords => (int)((LastRecord - FirstRecord) / RecordLength);
    protected override int? ArrayPointerStride => null;

    private readonly List<CachedStatDescription> _cachedDescriptions = [];
    private bool _cachedStatDescriptionsAreValid = true;
    private readonly ConcurrentDictionary<HashSet<GameStat>, List<CachedStatDescription>> _statsByStatSet = new(HashSet<GameStat>.CreateSetComparer());
    private List<CachedStatDescription> _descriptionsFromFile;

    private List<CachedStatDescription> DescriptionsFromFile
    {
        get
        {
            if (_descriptionsFromFile != null) return _descriptionsFromFile;

            var path = CacheFilePath;
            if (File.Exists(path))
            {
                try
                {
                    var text = File.ReadAllText(path);
                    _descriptionsFromFile = JsonConvert.DeserializeObject<List<CachedStatDescription>>(text);
                    return _descriptionsFromFile;
                }
                catch (Exception ex)
                {
                    return _descriptionsFromFile = [];
                }
            }

            return null;
        }
    }

    public string GetSourcePath([CallerFilePath] string path = "")
    {
        return path;
    }

    private string CacheFilePath
    {
        get
        {
            var sourcePath = GetSourcePath();
            var directoryPath = Path.GetDirectoryName(sourcePath);
            var path = Path.GetFullPath(Path.Join(directoryPath, @"..\..\ExileCore2\ExileCore2\Data", "StatDescriptions", $"{Path.GetFileNameWithoutExtension(StaticFileName)}.json"));
            return path;
        }
    }

    private void UpdateFileDescriptions()
    {
        File.WriteAllText(CacheFilePath, JsonConvert.SerializeObject(_cachedDescriptions, Formatting.Indented));
        _descriptionsFromFile = null;
    }

    public string TranslateMod(IReadOnlyDictionary<GameStat, int> values, int? sectionIndexOverride, Func<ModTranslationReplacerInput, string> replacer)
    {
        var statSet = values.Keys.ToHashSet();
        var entries = _statsByStatSet.GetOrAdd(statSet, s =>
        {
            List<CachedStatDescription> description = null;
            if (_cachedStatDescriptionsAreValid && GameWrapper.TheGame.CoreSettings.TryLoadingStatDescriptionsFromGameData)
            {
                description = _cachedDescriptions.Where(x => s.IsSupersetOf(x.Stats)).ToList();
            }

            if (description == null || description.Count == 0)
            {
                description = DescriptionsFromFile?.Where(x => s.IsSupersetOf(x.Stats)).ToList();
            }

            return description;
        });
        if (entries == null || entries.Count == 0)
        {
            _statsByStatSet.Remove(statSet, out _);
            return $"<unknown {string.Join(", ", values.Select(x => $"{x.Key}:{x.Value}"))}>";
        }

        var results = new List<string>();
        foreach (var entry in entries)
        {
            var sortedStatValues = entry.Stats.Select(x => values[x]).ToList();
            var section = sectionIndexOverride != null
                ? entry.Sections.ElementAtOrDefault(sectionIndexOverride.Value)
                : entry.Sections.FirstOrDefault(x => x.StatRanges.Zip(sortedStatValues, (range, value) => value >= range.Min && value <= range.Max).All(c => c));
            if (section == null)
            {
                results.Add($"<unknownSection {entry} {string.Join(", ", values.Select(x => $"{x.Key}:{x.Value}"))}>");
                continue;
            }

            var r = StringTemplateRegex();
            var replaced = r.Replace(section.String, m =>
            {
                var statIndex = m.Groups["id"].ValueSpan.Length == 0 ? 0 : int.Parse(m.Groups["id"].ValueSpan);
                if (section.StatConversionTypes.Count <= statIndex)
                {
                    return $"<Missing StatIndex {statIndex} ({section.StatConversionTypes.Count})>";
                }

                if (sortedStatValues.Count <= statIndex)
                {
                    return $"<Missing Stat {statIndex} ({sortedStatValues.Count})>";
                }

                var rawValue = sortedStatValues[statIndex];
                var convertedStatValue = StatTranslationUtils.ConvertStat(section.StatConversionTypes[statIndex], rawValue);
                var str = convertedStatValue.ToString(CultureInfo.CurrentCulture);
                if (convertedStatValue > 0 && m.Groups["format"].ValueSpan.StartsWith("+"))
                {
                    str = $"+{str}";
                }

                return replacer == null ? str : replacer(new ModTranslationReplacerInput(entry.Stats[statIndex], rawValue, convertedStatValue, str));
            });
            results.Add(replaced);
        }

        return string.Join("\n", results.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public string TranslateMod(IReadOnlyCollection<GameStat> stats, int sectionIndexOverride, Func<GameStat, string> replacer)
    {
        return TranslateMod(stats.ToDictionary(x => x, _ => 0), sectionIndexOverride, x => replacer(x.Stat));
    }

    public string TranslateMod(IReadOnlyDictionary<GameStat, int> values) => TranslateMod(values, null);
    public string TranslateMod(IReadOnlyDictionary<GameStat, int> values, int? sectionIndexOverride) => TranslateMod(values, sectionIndexOverride, null);

    protected override void EntryAdded(long addr, StatDescription entry)
    {
        if (!GameWrapper.TheGame.CoreSettings.TryLoadingStatDescriptionsFromGameData)
        {
            return;
        }

        var cachedStatDescription = new CachedStatDescription(entry.Stats,
            entry.Sections.Select(x => new CachedStatDescriptionSection(x.StatRanges, x.StatConversionTypes, x.String)).ToList());
        _cachedDescriptions.Add(cachedStatDescription);
        if (_cachedStatDescriptionsAreValid)
        {
            _cachedStatDescriptionsAreValid =
                cachedStatDescription.Stats.Any() &&
                cachedStatDescription.Sections.Any() &&
                cachedStatDescription.Sections
                    .All(x => x.StatRanges.Any() &&
                              x.StatConversionTypes.Any() &&
                              x.StatConversionTypes.Count == x.StatRanges.Count);
        }
    }

    protected override void OnReload()
    {
        _cachedDescriptions.Clear();
    }

    [GeneratedRegex(@"{(?<id>\d*)(:(?<format>[^}]*))?}", RegexOptions.Compiled | RegexOptions.ExplicitCapture)]
    private static partial Regex StringTemplateRegex();
}