using ExileCore.PoEMemory;
using ExileCore.Shared.Interfaces;

namespace PoeHudWrapper.MemoryObjects;

public class UniversalFileWrapperWrapper<RecordType> : FileInMemory where RecordType : RemoteMemoryObject, new()
{
    public UniversalFileWrapperWrapper(IMemory mem, Func<long> address) : base(mem, address)
    {
    }

    public bool ExcludeZeroAddresses { get; set; } = false;

    //We mark this fields as private coz we don't allow to read them directly dut to optimisation. Use EntriesList and methods instead.
    protected Dictionary<long, RecordType> EntriesAddressDictionary { get; set; } = new Dictionary<long, RecordType>();
    protected List<RecordType> CachedEntriesList { get; set; } = [];

    public List<RecordType> EntriesList
    {
        get
        {
            CheckCache();
            return CachedEntriesList;
        }
    }

    public RecordType GetByAddress(long address)
    {
        CheckCache();
        EntriesAddressDictionary.TryGetValue(address, out var result);
        return result;
    }

    public RecordType GetByAddressOrReload(long address)
    {
        CheckCache();
        if (EntriesAddressDictionary.TryGetValue(address, out var result) || address == 0)
        {
            return result;
        }

        Reload();
        return EntriesAddressDictionary.GetValueOrDefault(address);
    }

    public void ReloadIfEmptyOrZero()
    {
        if (EntriesList.All(x => x.Address == 0))
        {
            Reload();
        }
    }

    public void ReloadIfEmpty()
    {
        if (EntriesList.Count == 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        GameWrapper.TheGame.Files.LoadFiles();
        ReloadCore();
    }

    private void ReloadCore()
    {
        EntriesAddressDictionary.Clear();
        CachedEntriesList.Clear();
        Address = FAddressCache.Value;
        OnReload();
        CheckCache();
    }

    public void CheckCache()
    {
        var currentAddress = FAddressCache.Value;
        if (currentAddress != 0 && currentAddress != Address)
        {
            ReloadCore();
        }

        if (EntriesAddressDictionary.Count != 0)
            return;

        try
        {
            foreach (var addr in RecordAddresses().Where(x => !ExcludeZeroAddresses || x != 0))
            {
                if (!EntriesAddressDictionary.ContainsKey(addr))
                {
                    var r = GameWrapper.TheGame.GetObject<RecordType>(addr);
                    EntriesAddressDictionary.Add(addr, r);
                    CachedEntriesList.Add(r);
                    EntryAdded(addr, r);
                }
            }
        }
        finally
        {
            OnAllEntriesAdded();
        }
    }

    protected virtual void EntryAdded(long addr, RecordType entry)
    {
    }

    protected virtual void OnReload()
    {
    }

    protected virtual void OnAllEntriesAdded()
    {
    }
}