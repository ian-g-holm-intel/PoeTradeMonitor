using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.PoEMemory.FilesInMemory.Harvest;
using ExileCore.PoEMemory.FilesInMemory.Ultimatum;
using ExileCore.Shared.Static;

namespace PoeHudWrapper.MemoryObjects;

public partial class FilesContainerWrapper
{
    private BaseItemTypes _baseItemTypes;
    private UniversalFileWrapperWrapper<ClientString> _clientString;
    private StatDescriptionWrapper _statDescriptions;
    private ModsDat _mods;
    private StatsDat _stats;
    private TagsDat _tags;
    private ItemVisualIdentities _itemVisualIdentities;
    private UniqueItemDescriptions _uniqueItemDescriptions;
    private UniversalFileWrapperWrapper<WordEntry> _word;
    private LabyrinthTrials _labyrinthTrials;
    private MonsterVarieties _monsterVarieties;
    private PassiveSkills _passiveSkills;
    private PropheciesDat _prophecies;
    private Quests _quests;
    private QuestStates _questStates;
    private QuestFlagsDat _questFlags;
    private UniversalFileWrapperWrapper<MinimapIconDat> _minimapIcons;
    private UniversalFileWrapperWrapper<ArchnemesisMod> _archnemesisMods;
    private UniversalFileWrapperWrapper<LakeRoom> _lakeRooms;
    private UniversalFileWrapperWrapper<StampChoice> _stampChoices;
    private UniversalFileWrapperWrapper<BlightTowerDat> _blightTowers;
    private UniversalFileWrapperWrapper<HarvestSeed> _harvestSeeds;
    private UniversalFileWrapperWrapper<UltimatumModifier> _ultimatumModifiers;
    private UniversalFileWrapperWrapper<ChestRecord> _chests;
    private UniversalFileWrapperWrapper<QuestReward> _questRewards;
    private UniversalFileWrapperWrapper<QuestRewardOffer> _questRewardOffers;
    private UniversalFileWrapperWrapper<Character> _characters;
    private UniversalFileWrapperWrapper<GrantedEffectPerLevel> _grantedEffectsPerLevel;
    private UniversalFileWrapperWrapper<GrantedEffect> _grantedEffects;
    private UniversalFileWrapperWrapper<SkillGemDat> _skillGems;
    private UniversalFileWrapperWrapper<GemEffect> _gemEffects;
    private UniversalFileWrapperWrapper<BuffVisual> _buffVisuals;
    private UniversalFileWrapperWrapper<BuffDefinition> _buffDefinitions;
    private UniversalFileWrapperWrapper<MapPin> _mapPins;
    private WorldAreasWrapper _worldAreas;
    private UniversalFileWrapperWrapper<AtlasPrimordialAltarChoice> _atlasPrimordialAltarChoices;
    private UniversalFileWrapperWrapper<AtlasPrimordialAltarChoiceType> _atlasPrimordialAltarChoiceTypes;
    private StatDescriptionWrapper _primordialAltarStatDescriptions;
    private UniversalFileWrapperWrapper<CurrencyExchangeEntry> _currencyExchange;
    private UniversalFileWrapperWrapper<CurrencyExchangeCategory> _currencyExchangeCategories;
    private UniversalFileWrapperWrapper<TinctureDat> _tinctures;
    private UniversalFileWrapperWrapper<GroundEffectDat> _groundEffects;
    private UniversalFileWrapperWrapper<GroundEffectTypeDat> _groundEffectTypes;
    private UniversalFileWrapperWrapper<MiscAnimatedArtVariation> _miscAnimatedArtVariations;
    private MiscAnimatedList _miscAnimated;
    private UniversalFileWrapperWrapper<SkillArtVariation> _skillArtVariations;
    public ItemClasses ItemClasses { get; }

    public BaseItemTypes BaseItemTypes =>
        _baseItemTypes ??= new BaseItemTypes(_memory, () => FindFile("Data/BaseItemTypes.dat"));

    public UniversalFileWrapperWrapper<ClientString> ClientStrings => _clientString ??= new UniversalFileWrapperWrapper<ClientString>(_memory, () => FindFile("Data/ClientStrings.dat"));
    public StatDescriptionWrapper StatDescriptions => _statDescriptions ??= new StatDescriptionWrapper(_memory, FindFile, "Metadata/StatDescriptions/stat_descriptions.txt");
    public StatDescriptionWrapper HeistEquipmentStatDescriptions => _statDescriptions ??= new StatDescriptionWrapper(_memory, FindFile, "Metadata/StatDescriptions/heist_equipment_stat_descriptions.txt");
    public StatDescriptionWrapper PrimordialAltarStatDescriptions => _primordialAltarStatDescriptions ??= new StatDescriptionWrapper(_memory, FindFile, "Metadata/StatDescriptions/primordial_altar_stat_descriptions.txt");

    public ModsDat Mods => _mods ??= new ModsDat(_memory, () => FindFile("Data/Mods.dat"), Stats, Tags);
    public StatsDat Stats => _stats ??= new StatsDat(_memory, () => FindFile("Data/Stats.dat"));
    public TagsDat Tags => _tags ??= new TagsDat(_memory, () => FindFile("Data/Tags.dat"));
    public WorldAreasWrapper WorldAreas => _worldAreas ??= new WorldAreasWrapper(_memory, () => FindFile("Data/WorldAreas.dat"));
    public PassiveSkills PassiveSkills => _passiveSkills ??= new PassiveSkills(_memory, () => FindFile("Data/PassiveSkills.dat"));
    public LabyrinthTrials LabyrinthTrials => _labyrinthTrials ??= new LabyrinthTrials(_memory, () => FindFile("Data/LabyrinthTrials.dat"));
    public Quests Quests => _quests ??= new Quests(_memory, () => FindFile("Data/Quest.dat"));
    public QuestStates QuestStates => _questStates ??= new QuestStates(_memory, () => FindFile("Data/QuestStates.dat"));
    public QuestFlagsDat QuestFlags => _questFlags ??= new QuestFlagsDat(_memory, () => FindFile("Data/QuestFlags.dat"));
    public UniversalFileWrapperWrapper<QuestReward> QuestRewards => _questRewards ??= new UniversalFileWrapperWrapper<QuestReward>(_memory, () => FindFile("Data/QuestRewards.dat"));

    public UniversalFileWrapperWrapper<QuestRewardOffer> QuestRewardOffers =>
        _questRewardOffers ??= new UniversalFileWrapperWrapper<QuestRewardOffer>(_memory, () => FindFile("Data/QuestRewardOffers.dat"));

    public UniversalFileWrapperWrapper<MinimapIconDat> MinimapIcons => _minimapIcons ??= new UniversalFileWrapperWrapper<MinimapIconDat>(_memory, () => FindFile("Data/MinimapIcons.dat"));
    public UniversalFileWrapperWrapper<MapPin> MapPins => _mapPins ??= new UniversalFileWrapperWrapper<MapPin>(_memory, () => FindFile("Data/MapPins.dat"));
    public UniversalFileWrapperWrapper<Character> Characters => _characters ??= new UniversalFileWrapperWrapper<Character>(_memory, () => FindFile("Data/Characters.dat"));
    public MonsterVarieties MonsterVarieties => _monsterVarieties ??= new MonsterVarieties(_memory, () => FindFile("Data/MonsterVarieties.dat"));
    public PropheciesDat Prophecies => _prophecies ??= new PropheciesDat(_memory, () => FindFile("Data/Prophecies.dat"));
    public ItemVisualIdentities ItemVisualIdentities => _itemVisualIdentities ??= new ItemVisualIdentities(_memory, () => FindFile("Data/ItemVisualIdentity.dat"));
    public UniqueItemDescriptions UniqueItemDescriptions => _uniqueItemDescriptions ??= new UniqueItemDescriptions(_memory, () => FindFile("Data/UniqueStashLayout.dat"));
    public UniversalFileWrapperWrapper<WordEntry> Words => _word ??= new UniversalFileWrapperWrapper<WordEntry>(_memory, () => FindFile("Data/Words.dat"));

    public UniversalFileWrapperWrapper<ArchnemesisMod> ArchnemesisMods =>
        _archnemesisMods ??= new UniversalFileWrapperWrapper<ArchnemesisMod>(_memory, () => FindFile("Data/ArchnemesisMods.dat"));

    public UniversalFileWrapperWrapper<LakeRoom> LakeRooms => _lakeRooms ??= new UniversalFileWrapperWrapper<LakeRoom>(_memory, () => FindFile("Data/LakeRooms.dat"));
    public UniversalFileWrapperWrapper<StampChoice> StampChoices => _stampChoices ??= new UniversalFileWrapperWrapper<StampChoice>(_memory, () => FindFile("Data/StampChoice.dat"));
    public UniversalFileWrapperWrapper<BlightTowerDat> BlightTowers => _blightTowers ??= new UniversalFileWrapperWrapper<BlightTowerDat>(_memory, () => FindFile("Data/BlightTowers.dat"));
    public UniversalFileWrapperWrapper<HarvestSeed> HarvestSeeds => _harvestSeeds ??= new UniversalFileWrapperWrapper<HarvestSeed>(_memory, () => FindFile("Data/HarvestSeeds.dat"));
    public UniversalFileWrapperWrapper<CurrencyExchangeEntry> CurrencyExchange => _currencyExchange ??= new UniversalFileWrapperWrapper<CurrencyExchangeEntry>(_memory, () => FindFile("Data/CurrencyExchange.dat"));
    public UniversalFileWrapperWrapper<CurrencyExchangeCategory> CurrencyExchangeCategories => _currencyExchangeCategories ??= new UniversalFileWrapperWrapper<CurrencyExchangeCategory>(_memory, () => FindFile("Data/CurrencyExchangeCategories.dat"));

    public UniversalFileWrapperWrapper<UltimatumModifier> UltimatumModifiers =>
        _ultimatumModifiers ??= new UniversalFileWrapperWrapper<UltimatumModifier>(_memory, () => FindFile("Data/UltimatumModifiers.dat"));

    public UniversalFileWrapperWrapper<GrantedEffectPerLevel> GrantedEffectsPerLevel =>
        _grantedEffectsPerLevel ??= new UniversalFileWrapperWrapper<GrantedEffectPerLevel>(_memory, () => FindFile("Data/GrantedEffectsPerLevel.dat"));

    public UniversalFileWrapperWrapper<GrantedEffect> GrantedEffects => _grantedEffects ??= new UniversalFileWrapperWrapper<GrantedEffect>(_memory, () => FindFile("Data/GrantedEffects.dat"));
    public UniversalFileWrapperWrapper<SkillGemDat> SkillGems => _skillGems ??= new UniversalFileWrapperWrapper<SkillGemDat>(_memory, () => FindFile("Data/SkillGems.dat"));
    public UniversalFileWrapperWrapper<GemEffect> GemEffects => _gemEffects ??= new UniversalFileWrapperWrapper<GemEffect>(_memory, () => FindFile("Data/GemEffects.dat"));

    public UniversalFileWrapperWrapper<BuffDefinition> BuffDefinitions =>
        _buffDefinitions ??= new UniversalFileWrapperWrapper<BuffDefinition>(_memory, () => FindFile("Data/BuffDefinitions.dat"));

    public UniversalFileWrapperWrapper<BuffVisual> BuffVisuals => _buffVisuals ??= new UniversalFileWrapperWrapper<BuffVisual>(_memory, () => FindFile("Data/BuffVisuals.dat"));

    public UniversalFileWrapperWrapper<ChestRecord> Chests => _chests ??= new UniversalFileWrapperWrapper<ChestRecord>(_memory, () => FindFile("Data/Chests.dat"));

    public UniversalFileWrapperWrapper<AtlasPrimordialAltarChoice> AtlasPrimordialAltarChoices => _atlasPrimordialAltarChoices ??=
        new UniversalFileWrapperWrapper<AtlasPrimordialAltarChoice>(_memory, () => FindFile("Data/AtlasPrimordialAltarChoices.dat"));

    public UniversalFileWrapperWrapper<AtlasPrimordialAltarChoiceType> AtlasPrimordialAltarChoiceTypes => _atlasPrimordialAltarChoiceTypes ??=
        new UniversalFileWrapperWrapper<AtlasPrimordialAltarChoiceType>(_memory, () => FindFile("Data/AtlasPrimordialAltarChoiceTypes.dat"));

    public UniversalFileWrapperWrapper<TinctureDat> Tinctures => _tinctures ??= new UniversalFileWrapperWrapper<TinctureDat>(_memory, () => FindFile("Data/Tinctures.dat"));
    public UniversalFileWrapperWrapper<GroundEffectDat> GroundEffects => _groundEffects ??= new UniversalFileWrapperWrapper<GroundEffectDat>(_memory, () => FindFile("Data/GroundEffects.dat"));
    public UniversalFileWrapperWrapper<GroundEffectTypeDat> GroundEffectTypes => _groundEffectTypes ??= new UniversalFileWrapperWrapper<GroundEffectTypeDat>(_memory, () => FindFile("Data/GroundEffectTypes.dat"));
    public UniversalFileWrapperWrapper<MiscAnimatedArtVariation> MiscAnimatedArtVariations => _miscAnimatedArtVariations ??= new UniversalFileWrapperWrapper<MiscAnimatedArtVariation>(_memory, () => FindFile("Data/MiscAnimatedArtVariations.dat"));
    public UniversalFileWrapperWrapper<SkillArtVariation> SkillArtVariations => _skillArtVariations ??= new UniversalFileWrapperWrapper<SkillArtVariation>(_memory, () => FindFile("Data/SkillArtVariations.dat"));
    public MiscAnimatedList MiscAnimated => _miscAnimated ??= new MiscAnimatedList(_memory, () => FindFile("Data/MiscAnimated.dat"));
}