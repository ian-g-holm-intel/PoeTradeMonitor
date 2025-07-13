using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;

namespace PoeHudWrapper.MemoryObjects;

public class IngameDataWrapper : RemoteMemoryObject
{
    private const double TileHeightFinalMultiplier = 125 / 16.0;
    private static readonly int EntitiesCountOffset = Extensions.GetOffset<IngameDataOffsets>(x => x.EntitiesCount);

    private NativePtrArray cacheStats;
    private Dictionary<GameStat, int> _mapStats = new Dictionary<GameStat, int>();
    private Dictionary<GameStat, int> _mapStatsVisible = new Dictionary<GameStat, int>();

    public IngameDataOffsets DataStruct => M.Read<IngameDataOffsets>(Address);
    public long EntitiesCount => M.Read<long>(Address + EntitiesCountOffset);
    public WorldAreaWrapper CurrentArea => GameWrapper.TheGame.Files.WorldAreas.GetByAddress(DataStruct.CurrentArea);
    public int CurrentAreaLevel => DataStruct.CurrentAreaLevel;
    public uint CurrentAreaHash => DataStruct.CurrentAreaHash;
    public Entity LocalPlayer => GetObject<Entity>(DataStruct.LocalPlayer);
    public ServerDataWrapper ServerData => GetObject<ServerDataWrapper>(DataStruct.ServerData);
    public EntityListWrapper EntityList => GetObject<EntityListWrapper>(DataStruct.EntityList);
    public List<EffectEnvironment> EffectEnvironments => M.ReadRMOStdVector<EffectEnvironment>(M.Read<IngameDataOffsets>(Address).EffectEnvironments, Unsafe.SizeOf<EnvironmentOffsets>()).ToList();
    public EnvironmentData EnvironmentData => GetObjectStatic<EnvironmentData>(M.Read<IngameDataOffsets>(Address).EnvironmentDataPtr);
    public LabyrinthData LabyrinthData => DataStruct.LabDataPtr is not 0 and var ptr ? GetObject<LabyrinthData>(ptr) : null;
    public TimeSpan TimeInMap => TimeSinceZoneIn + TimeInMapBeforeZoneIn; // these only work when using the MTX
    public TimeSpan TimeSinceZoneIn => DataStruct.ZoneInQPC == 0 ? TimeSpan.Zero : TimeSpan.FromMilliseconds(Stopwatch.GetTimestamp() * 1000 / Stopwatch.Frequency - DataStruct.ZoneInQPC);
    public TimeSpan TimeInMapBeforeZoneIn => TimeSpan.FromMilliseconds(DataStruct.MillisecondsSpentInMapBeforeZoneIn);
    public TerrainData Terrain => DataStruct.Terrain;
    public Vector2i AreaDimensions => new Vector2i(Terrain.BytesPerRow * 2, (int)(Terrain.LayerMelee.Size / Terrain.BytesPerRow));
    public int[][] RawPathfindingData => GetTerrainLayerData(Terrain.LayerMelee);
    public int[][] RawTerrainTargetingData => GetTerrainLayerData(Terrain.LayerRanged);
    public float[][] RawTerrainHeightData => GetTerrainHeight();
    public byte[] AzmeriDataArray1 => M.ReadStdVector<byte>(DataStruct.AzmeriDataArray1);
    public Vector2i AzmeriZoneTileDimensions => DataStruct.AzmeriZoneTileDimensions;
    public Vector2 AzmeriZoneMinBorder => DataStruct.AzmeriZoneMinBorder;
    public Vector2 AzmeriZoneMaxBorder => DataStruct.AzmeriZoneMaxBorder;
    public bool IsInsideAzmeriZone => DataStruct.IsInsideAzmeriZone > 0;

    private static T GetArrayValueAt<T>(Vector2 gridPosition, T[][] array)
    {
        var y = Math.Clamp((int)gridPosition.Y, 0, array.Length - 1);
        var row = array[y];
        var x = Math.Clamp((int)gridPosition.X, 0, row.Length - 1);
        return row[x];
    }

    public float GetTerrainHeightAt(Vector2 gridPosition) => GetArrayValueAt(gridPosition, RawTerrainHeightData);
    public int GetPathfindingValueAt(Vector2 gridPosition) => GetArrayValueAt(gridPosition, RawPathfindingData);
    public int GetTerrainTargetingValueAt(Vector2 gridPosition) => GetArrayValueAt(gridPosition, RawTerrainTargetingData);

    public Vector2 GetGridScreenPosition(Vector2 gridPosition)
    {
        return GetWorldScreenPosition(ToWorldWithTerrainHeight(gridPosition));
    }

    public Vector2 GetWorldScreenPosition(Vector3 worldPosition)
    {
        return GameWrapper.TheGame.IngameState.Camera.WorldToScreen(worldPosition);
    }

    public Vector2 GetGridMapScreenPosition(Vector2 gridPosition)
    {
        return GetGridMapScreenPosition(gridPosition, gridPosition);
    }

    public Vector2 GetGridMapScreenPosition(Vector2 gridPosition, Vector2 terrainHeightGridPos)
    {
        var largeMap = GameWrapper.TheGame.IngameState.IngameUi.Map.LargeMap;
        return largeMap.MapCenter + TranslateGridDeltaToMapDelta(
            gridPosition - LocalPlayer.GridPosNum,
            GetTerrainHeightAt(terrainHeightGridPos) - (LocalPlayer.GetComponent<Render>()?.Z ?? 0));
    }

    public Vector2 TranslateGridDeltaToMapDelta(Vector2 delta, float deltaZ)
    {
        deltaZ *= PoeMapExtension.WorldToGridConversion; //z is normally "world" units, translate to grid
        var largeMap = GameWrapper.TheGame.IngameState.IngameUi.Map.LargeMap;
        return largeMap.MapScale * new Vector2((delta.X - delta.Y) * SubMap.CameraAngleCos, (deltaZ - (delta.X + delta.Y)) * SubMap.CameraAngleSin);
    }

    public Vector3 ToWorldWithTerrainHeight(Vector2 gridPosition)
    {
        return new Vector3(gridPosition.GridToWorld(), GetTerrainHeightAt(gridPosition));
    }

    public Vector3 ToWorldWithTerrainHeight(Vector3 worldPosition)
    {
        return new Vector3(worldPosition.Xy(), GetTerrainHeightAt(worldPosition.WorldToGrid()));
    }

    private int[][] GetTerrainLayerData(NativePtrArray terrainLayer)
    {
        var mapTextureData = M.ReadStdVector<byte>(terrainLayer);
        var bytesPerRow = Terrain.BytesPerRow;
        var totalRows = mapTextureData.Length / bytesPerRow;
        var processedTerrainData = new int[totalRows][];
        var xSize = bytesPerRow * 2;
        for (var i = 0; i < totalRows; i++)
        {
            processedTerrainData[i] = new int[xSize];
        }

        Parallel.For(0, totalRows, y =>
        {
            for (var x = 0; x < xSize; x += 2)
            {
                var dateElement = mapTextureData[y * bytesPerRow + x / 2];
                for (var xs = 0; xs < 2; xs++)
                {
                    var rawPathType = dateElement >> 4 * xs & 15;
                    processedTerrainData[y][x + xs] = rawPathType;
                }
            }
        });

        return processedTerrainData;
    }

    private float[][] GetTerrainHeight()
    {
        var rotationSelector = GameWrapper.TheGame.TerrainRotationSelector;
        var rotationHelper = GameWrapper.TheGame.TerrainRotationHelper;
        var terrainMetadata = Terrain;
        var tileData = M.ReadStdVector<TileStructure>(terrainMetadata.TgtArray);
        var tileHeightCache = tileData.Select(x => x.SubTileDetailsPtr)
            .Distinct()
            .AsParallel()
            .Select(addr => new
            {
                addr,
                data = M.ReadStdVector<sbyte>(M.Read<SubTileStructure>(addr).SubTileHeight)
            })
            .ToDictionary(x => x.addr, x => x.data);
        var gridSizeX = terrainMetadata.NumCols * PoeMapExtension.TileToGridConversion;
        var toExclusive = terrainMetadata.NumRows * PoeMapExtension.TileToGridConversion;
        var result = new float[toExclusive][];
        Parallel.For(0, toExclusive, y =>
        {
            result[y] = new float[gridSizeX];
            for (var x = 0; x < gridSizeX; ++x)
            {
                var tileDataIndex = y / PoeMapExtension.TileToGridConversion * terrainMetadata.NumCols + x / PoeMapExtension.TileToGridConversion;
                if (tileDataIndex < 0 ||
                    tileDataIndex >= tileData.Length)
                {
                    DebugWindow.LogError($"Tile data array length is {tileData.Length}, index was {tileDataIndex}");
                    result[y][x] = 0;
                    continue;
                }

                var tileStructure = tileData[tileDataIndex];
                var tileHeightArray = tileHeightCache[tileStructure.SubTileDetailsPtr];
                var tileHeight = 0;
                if (tileHeightArray.Length == 1)
                {
                    tileHeight = tileHeightArray[0];
                }
                else if (tileHeightArray.Length != 0)
                {
                    var gridX = x % PoeMapExtension.TileToGridConversion;
                    var gridY = y % PoeMapExtension.TileToGridConversion;
                    var maxCoordInTile = PoeMapExtension.TileToGridConversion - 1;
                    int[] coordHelperArray =
                    {
                        maxCoordInTile - gridX,
                        gridX,
                        maxCoordInTile - gridY,
                        gridY
                    };
                    var rotationIndex = rotationSelector[tileStructure.RotationSelector] * 3;
                    int axisSwitch = rotationHelper[rotationIndex];
                    int smallAxisFlip = rotationHelper[rotationIndex + 1];
                    int largeAxisFlip = rotationHelper[rotationIndex + 2];
                    var smallIndex = coordHelperArray[axisSwitch * 2 + smallAxisFlip];
                    var index = coordHelperArray[largeAxisFlip + (1 - axisSwitch) * 2] * PoeMapExtension.TileToGridConversion + smallIndex;
                    tileHeight = GetTileHeightFromPackedArray(tileHeightArray, index);
                }

                result[y][x] = -(float)((tileStructure.TileHeight * terrainMetadata.TileHeightMultiplier + tileHeight) * TileHeightFinalMultiplier);
            }
        });
        return result;
    }

    private static int GetTileHeightFromPackedArray(sbyte[] tileHeightArray, int index)
    {
        var (shift, add, and1, mult, and2, unpack) = tileHeightArray.Length switch
        {
            0x45 => (3, 2, 7, 1, 1, true),
            0x89 => (2, 4, 3, 2, 3, true),
            0x119 => (1, 0x10, 1, 4, 0xf, true),
            _ => default,
        };
        if (!unpack)
        {
            if (index < 0 || index >= tileHeightArray.Length)
            {
                DebugWindow.LogError($"Tile height array length is {tileHeightArray.Length}, index (0) was {index}");
            }
            else
            {
                return tileHeightArray[index];
            }
        }

        var intermediateIndex = (index >> shift) + add;
        if (intermediateIndex < 0 || intermediateIndex >= tileHeightArray.Length)
        {
            DebugWindow.LogError($"Tile height array length is {tileHeightArray.Length}, index (1) was {intermediateIndex}");
        }
        else
        {
            var heightByte = (byte)tileHeightArray[intermediateIndex];
            var finalIndex = (heightByte >> ((index & and1) * mult)) & and2;
            if (finalIndex < 0 || finalIndex >= tileHeightArray.Length)
            {
                DebugWindow.LogError($"Tile height array length is {tileHeightArray.Length}, index (2) was {intermediateIndex}, {finalIndex}");
            }
            else
            {
                return tileHeightArray[finalIndex];
            }
        }

        return 0;
    }

    public Dictionary<GameStat, int> GetMapStats(NativePtrArray cacheStruct, ref Dictionary<GameStat, int> statsDictionary)
    {
        if (cacheStats.Equals(cacheStruct)) return statsDictionary;

        var statPtrStart = cacheStruct.First;
        var statPtrEnd = cacheStruct.Last;
        var totalStats = (int)(statPtrEnd - statPtrStart);

        if (totalStats / 8 > 200)
            return null;

        var statArray = M.ReadStdVector<(GameStat stat, int value)>(cacheStruct);
        cacheStats = cacheStruct;
        statsDictionary = statArray.ToDictionary(x => x.stat, x => x.value);

        return statsDictionary;
    }

    public Dictionary<GameStat, int> MapStats => GetMapStats(DataStruct.MapStats, ref _mapStats);

    public Dictionary<GameStat, int> MapStatsVisible => GetMapStats(DataStruct.MapStatsVisible, ref _mapStatsVisible);

    public IList<PortalObject> TownPortals
    {
        get
        {
            var portalPtrStart = M.Read<long>(Address + 0x938);
            var portalPtrEnd = M.Read<long>(Address + 0x940);

            return M.ReadStructsArray<PortalObject>(portalPtrStart, portalPtrEnd, PortalObject.StructSize, GameWrapper.TheGame);
        }
    }

    public class PortalObject : RemoteMemoryObject
    {
        public const int StructSize = 0x38;
        public long Id => M.Read<long>(Address);

        public string PlayerOwner => NativeStringReader.ReadString(Address + 8, M);
        public WorldAreaWrapper Area => GameWrapper.TheGame.Files.WorldAreas.GetAreaByWorldId(M.Read<int>(Address + 0x2C));

        public override string ToString()
        {
            return $"{PlayerOwner} => {Area.Name}";
        }
    }
}