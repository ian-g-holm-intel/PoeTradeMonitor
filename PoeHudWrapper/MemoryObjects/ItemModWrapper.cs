using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.PoEMemory;
using ExileCore.Shared;

namespace PoeHudWrapper.MemoryObjects;
public class ItemModWrapper : RemoteMemoryObject
{
    private static readonly char[] Digits = "0123456789".ToCharArray();
    public static readonly int STRUCT_SIZE = 0x38;

    private string _rawName;
    private ModsDat.ModRecord _record;
    private string _translation;

    [Obsolete("Use Values instead")]
    public int Value1 => Values.Count > 0 ? Values[0] : 0;

    [Obsolete("Use Values instead")]
    public int Value2 => Values.Count > 1 ? Values[1] : 0;

    [Obsolete("Use Values instead")]
    public int Value3 => Values.Count > 2 ? Values[2] : 0;

    [Obsolete("Use Values instead")]
    public int Value4 => Values.Count > 3 ? Values[3] : 0;

    public List<int> Values
    {
        get
        {
            var start = M.Read<long>(Address);
            var end = M.Read<long>(Address + 0x8);
            var len = (end - start) / sizeof(long);

            if (len < 0 || len > 10)
                return new List<int>();

            return M.ReadStructsArray<int>(start, end, sizeof(int));
        }
    }

    public IntRange[] ValuesMinMax
    {
        get
        {
            if (_record == null)
                ReadRecord();

            return _record?.StatRange;
        }
    }

    public string RawName
    {
        get
        {
            if (_record == null)
                ReadRecord();

            return _rawName ?? string.Empty;
        }
    }

    public string Name
    {
        get
        {
            var rawName = RawName;
            return rawName.Replace("_", "").TrimEnd(Digits); // some mods have digit in the middle ie.: MapAtlasMapsHave20PercentQuality
        }
    }

    public string Group
    {
        get
        {
            if (_record == null)
                ReadRecord();

            return _record?.Group ?? string.Empty;
        }
    }

    public string DisplayName
    {
        get
        {
            if (_record == null)
                ReadRecord();

            return _record?.UserFriendlyName ?? string.Empty;
        }
    }

    public int Level
    {
        get
        {
            if (_record == null)
                ReadRecord();

            return _record?.MinLevel ?? 0;
        }
    }

    public ModsDat.ModRecord ModRecord
    {
        get
        {
            if (_record == null)
                ReadRecord();

            return _record;
        }
    }

    public string Translation => _translation ??= Translate();

    private string Translate()
    {
        var statFiles = new[]
        {
            GameWrapper.TheGame.Files.StatDescriptions,
            GameWrapper.TheGame.Files.HeistEquipmentStatDescriptions,
        };
        var statDictionary = ModRecord.StatNames.Zip(Values).ToDictionary(x => x.First.MatchingStat, x => x.Second);
        var description = statFiles[0].TranslateMod(statDictionary);
        foreach (var statDescriptionWrapper in statFiles.Skip(1))
        {
            if (description.StartsWith('<'))
            {
                var newDescription = statDescriptionWrapper.TranslateMod(statDictionary);
                if (!newDescription.StartsWith('<'))
                {
                    return newDescription;
                }
            }
            else
            {
                return description;
            }
        }

        return description;
    }

    private void ReadRecord()
    {
        var addr = M.Read<long>(Address + 0x28);
        _record = GameWrapper.TheGame.Files.Mods.GetModByAddress(addr);
        _rawName = _record?.Key;
    }

    public override string ToString()
    {
        var minMax = ValuesMinMax;

        var maxLength = Math.Min(Values.Count, minMax.Length); // values from memory can sometimes come with a 5th when its a combined mod?

        var enumerable = Values.Take(maxLength).Select((x, i) =>
        {
            var minMaxCur = ValuesMinMax[i];

            if (minMaxCur.Min == minMaxCur.Max)
                return x.ToString();

            return $"{x} [{minMaxCur.Min}-{minMaxCur.Max}]";
        });

        return $"{_rawName} ({string.Join(", ", enumerable)})";
    }
}