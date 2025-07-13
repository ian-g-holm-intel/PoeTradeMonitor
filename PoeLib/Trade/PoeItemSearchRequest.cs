using System;
using System.Collections.Generic;

namespace PoeLib.Trade;

public class Status
{
    public string option { get; set; } = "online";
}

public class Value
{
    public int? min { get; set; }
    public int? max { get; set; }

    public override bool Equals(object obj)
    {
        var value = obj as Value;
        return value != null &&
               min == value.min &&
               max == value.max;
    }

    public override int GetHashCode()
    {
        var hashCode = -897720056;
        hashCode = hashCode * -1521134295 + min.GetHashCode();
        hashCode = hashCode * -1521134295 + max.GetHashCode();
        return hashCode;
    }
}

public class StatFilter
{
    public string id { get; set; }
    public Value value { get; set; }
    public bool disabled { get; set; }

    public override bool Equals(object obj)
    {
        var filter = obj as StatFilter;
        return filter != null &&
               id == filter.id &&
               EqualityComparer<Value>.Default.Equals(value, filter.value) &&
               disabled == filter.disabled;
    }

    public override int GetHashCode()
    {
        var hashCode = 527693119;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
        hashCode = hashCode * -1521134295 + EqualityComparer<Value>.Default.GetHashCode(value);
        hashCode = hashCode * -1521134295 + disabled.GetHashCode();
        return hashCode;
    }
}

public class Stat
{
    public string type { get; set; }
    public List<StatFilter> filters { get; set; }
    public bool? disabled { get; set; }

    public override bool Equals(object obj)
    {
        var stat = obj as Stat;
        return stat != null &&
               type == stat.type &&
               EqualityComparer<List<StatFilter>>.Default.Equals(filters, stat.filters) &&
               disabled == stat.disabled;
    }

    public override int GetHashCode()
    {
        var hashCode = 1701581262;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<StatFilter>>.Default.GetHashCode(filters);
        hashCode = hashCode * -1521134295 + disabled.GetHashCode();
        return hashCode;
    }
}

public class ItemRarity
{
    public string option { get; set; }

    public override bool Equals(object obj)
    {
        var rarity = obj as ItemRarity;
        return rarity != null &&
               option == rarity.option;
    }

    public override int GetHashCode()
    {
        return 401477300 + EqualityComparer<string>.Default.GetHashCode(option);
    }
}

public class Links
{
    public int? b { get; set; }
    public int? g { get; set; }
    public int? r { get; set; }
    public int? w { get; set; }
    public int? max { get; set; }
    public int? min { get; set; }

    public override bool Equals(object obj)
    {
        var links = obj as Links;
        return links != null &&
               b == links.b &&
               g == links.g &&
               r == links.r &&
               w == links.w &&
               max == links.max &&
               min == links.min;
    }

    public override int GetHashCode()
    {
        var hashCode = 1293661102;
        hashCode = hashCode * -1521134295 + b.GetHashCode();
        hashCode = hashCode * -1521134295 + g.GetHashCode();
        hashCode = hashCode * -1521134295 + r.GetHashCode();
        hashCode = hashCode * -1521134295 + w.GetHashCode();
        hashCode = hashCode * -1521134295 + max.GetHashCode();
        hashCode = hashCode * -1521134295 + min.GetHashCode();
        return hashCode;
    }
}

public class Sockets
{
    public int? min { get; set; }
    public int? max { get; set; }

    public override bool Equals(object obj)
    {
        var sockets = obj as Sockets;
        return sockets != null &&
               min == sockets.min &&
               max == sockets.max;
    }

    public override int GetHashCode()
    {
        var hashCode = -897720056;
        hashCode = hashCode * -1521134295 + min.GetHashCode();
        hashCode = hashCode * -1521134295 + max.GetHashCode();
        return hashCode;
    }
}

public class Category
{
    public string option { get; set; }
}

public class Damage
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Crit
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Pdps
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Aps
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Dps
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Edps
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Ar
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Es
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Ev
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Block
{
    public int? min { get; set; }
    public int ?max { get; set; }
}

public class Lvl
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Dex
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Str
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Int
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class EmptyPrefix
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class EmptySuffix
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class MapTier
{
    public int? min { get; set; }
    public int? max { get; set; }

    public override bool Equals(object obj)
    {
        var tier = obj as MapTier;
        return tier != null &&
               min == tier.min &&
               max == tier.max;
    }

    public override int GetHashCode()
    {
        var hashCode = -897720056;
        hashCode = hashCode * -1521134295 + min.GetHashCode();
        hashCode = hashCode * -1521134295 + max.GetHashCode();
        return hashCode;
    }
}

public class MapIiq
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class MapShaped
{
    public bool option { get; set; }
}

public class MapSeries
{
    public bool option { get; set; }
}

public class MapPacksize
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class MapIir
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class MapElder
{
    public bool option { get; set; }
}

public class Quality
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class GemLevel
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class ShaperItem
{
    public bool option { get; set; }
}

public class AlternateArt
{
    public bool option { get; set; }
}

public class Corrupted
{
    public bool option { get; set; }

    public override bool Equals(object obj)
    {
        var corrupted = obj as Corrupted;
        return corrupted != null &&
               option == corrupted.option;
    }

    public override int GetHashCode()
    {
        return 401477300 + option.GetHashCode();
    }
}

public class FracturedItem
{
    public bool option { get; set; }

    public override bool Equals(object obj)
    {
        return obj is FracturedItem item &&
               option == item.option;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(option);
    }
}

public class Enchanted
{
    public bool option { get; set; }
}

public class TalismanTier
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Ilvl
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class GemLevelProgress
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class ElderItem
{
    public bool option { get; set; }
}

public class Identified
{
    public bool option { get; set; }
}

public class Crafted
{
    public bool option { get; set; }
}

public class Mirrored
{
    public bool option { get; set; }
}

public class DelveSockets
{
    public int? min { get; set; }
    public int? max { get; set; }
}

public class Account
{
    public string input { get; set; }
}

public class Indexed
{
    public bool option { get; set; }
}

public class SaleType
{
    public SaleTypeEnum option { get; set; }
}

public enum SaleTypeEnum
{
    priced,
    unpriced
}

public class Price
{
    public int? min { get; set; }
    public int? max { get; set; }
    public string option { get; set; }

    public override bool Equals(object obj)
    {
        var price = obj as Price;
        return price != null &&
               min == price.min &&
               max == price.max &&
               option == price.option;
    }

    public override int GetHashCode()
    {
        var hashCode = -897720056;
        hashCode = hashCode * -1521134295 + min.GetHashCode();
        hashCode = hashCode * -1521134295 + max.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(option);
        return hashCode;
    }
}

public class FilterTypes
{
    public Damage damage { get; set; }
    public Crit crit { get; set; }
    public Pdps pdps { get; set; }
    public Aps aps { get; set; }
    public Dps dps { get; set; }
    public Edps edps { get; set; }
    public Ar ar { get; set; }
    public Es es { get; set; }
    public Ev ev { get; set; }
    public Block block { get; set; }
    public ItemRarity rarity { get; set; }
    public Category category { get; set; }
    public Links links { get; set; }
    public Sockets sockets { get; set; }
    public Lvl lvl { get; set; }
    public Dex dex { get; set; }
    public Str str { get; set; }
    public Int @int { get; set; }
    public EmptyPrefix empty_prefix { get; set; }
    public EmptySuffix empty_suffix { get; set; }
    public MapTier map_tier { get; set; }
    public MapIiq map_iiq { get; set; }
    public MapShaped map_shaped { get; set; }
    public MapSeries map_series { get; set; }
    public MapPacksize map_packsize { get; set; }
    public MapIir map_iir { get; set; }
    public MapElder map_elder { get; set; }
    public Quality quality { get; set; }
    public GemLevel gem_level { get; set; }
    public ShaperItem shaper_item { get; set; }
    public AlternateArt alternate_art { get; set; }
    public Corrupted corrupted { get; set; }
    public FracturedItem fractured_item { get; set; }
    public Enchanted enchanted { get; set; }
    public TalismanTier talisman_tier { get; set; }
    public Ilvl ilvl { get; set; }
    public GemLevelProgress gem_level_progress { get; set; }
    public ElderItem elder_item { get; set; }
    public Identified identified { get; set; }
    public Crafted crafted { get; set; }
    public Mirrored mirrored { get; set; }
    public DelveSockets delve_sockets { get; set; }
    public Account account { get; set; }
    public Indexed indexed { get; set; }
    public SaleType sale_type { get; set; }
    public Price price { get; set; }

    public override bool Equals(object obj)
    {
        var types = obj as FilterTypes;
        return types != null &&
               EqualityComparer<ItemRarity>.Default.Equals(rarity, types.rarity) &&
               EqualityComparer<Links>.Default.Equals(links, types.links) &&
               EqualityComparer<Sockets>.Default.Equals(sockets, types.sockets) &&
               EqualityComparer<MapTier>.Default.Equals(map_tier, types.map_tier) &&
               EqualityComparer<Corrupted>.Default.Equals(corrupted, types.corrupted);
    }

    public override int GetHashCode()
    {
        var hashCode = 1772513850;
        hashCode = hashCode * -1521134295 + EqualityComparer<ItemRarity>.Default.GetHashCode(rarity);
        hashCode = hashCode * -1521134295 + EqualityComparer<Links>.Default.GetHashCode(links);
        hashCode = hashCode * -1521134295 + EqualityComparer<Sockets>.Default.GetHashCode(sockets);
        hashCode = hashCode * -1521134295 + EqualityComparer<MapTier>.Default.GetHashCode(map_tier);
        hashCode = hashCode * -1521134295 + EqualityComparer<Corrupted>.Default.GetHashCode(corrupted);
        return hashCode;
    }
}

public class TypeFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; }
}

public class WeaponFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; } 
}

public class ArmourFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; }
}

public class SocketFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; }

    public override bool Equals(object obj)
    {
        var filters = obj as SocketFilters;
        return filters != null &&
               EqualityComparer<FilterTypes>.Default.Equals(this.filters, filters.filters) &&
               disabled == filters.disabled;
    }

    public override int GetHashCode()
    {
        var hashCode = 834872625;
        hashCode = hashCode * -1521134295 + EqualityComparer<FilterTypes>.Default.GetHashCode(filters);
        hashCode = hashCode * -1521134295 + disabled.GetHashCode();
        return hashCode;
    }
}

public class ReqFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; }
}

public class ModFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; }
}

public class MapFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; }

    public override bool Equals(object obj)
    {
        var filters = obj as MapFilters;
        return filters != null &&
               EqualityComparer<FilterTypes>.Default.Equals(this.filters, filters.filters) &&
               disabled == filters.disabled;
    }

    public override int GetHashCode()
    {
        var hashCode = 834872625;
        hashCode = hashCode * -1521134295 + EqualityComparer<FilterTypes>.Default.GetHashCode(filters);
        hashCode = hashCode * -1521134295 + disabled.GetHashCode();
        return hashCode;
    }
}

public class MiscFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; }

    public override bool Equals(object obj)
    {
        var filters = obj as MiscFilters;
        return filters != null &&
               EqualityComparer<FilterTypes>.Default.Equals(this.filters, filters.filters) &&
               disabled == filters.disabled;
    }

    public override int GetHashCode()
    {
        var hashCode = 834872625;
        hashCode = hashCode * -1521134295 + EqualityComparer<FilterTypes>.Default.GetHashCode(filters);
        hashCode = hashCode * -1521134295 + disabled.GetHashCode();
        return hashCode;
    }
}

public class TradeFilters
{
    public FilterTypes filters { get; set; } = new FilterTypes();
    public bool disabled { get; set; }
}

public class Filters
{
    public TypeFilters type_filters { get; set; }
    public WeaponFilters weapon_filters { get; set; }
    public ArmourFilters armour_filters { get; set; }
    public SocketFilters socket_filters { get; set; }
    public ReqFilters req_filters { get; set; }
    public ModFilters mod_filters { get; set; }
    public MapFilters map_filters { get; set; }
    public MiscFilters misc_filters { get; set; }
    public TradeFilters trade_filters { get; set; }

    public override bool Equals(object obj)
    {
        var filters = obj as Filters;
        return filters != null &&
               EqualityComparer<TypeFilters>.Default.Equals(type_filters, filters.type_filters) &&
               EqualityComparer<SocketFilters>.Default.Equals(socket_filters, filters.socket_filters) &&
               EqualityComparer<MapFilters>.Default.Equals(map_filters, filters.map_filters) &&
               EqualityComparer<MiscFilters>.Default.Equals(misc_filters, filters.misc_filters);
    }

    public override int GetHashCode()
    {
        var hashCode = 1152422915;
        hashCode = hashCode * -1521134295 + EqualityComparer<TypeFilters>.Default.GetHashCode(type_filters);
        hashCode = hashCode * -1521134295 + EqualityComparer<SocketFilters>.Default.GetHashCode(socket_filters);
        hashCode = hashCode * -1521134295 + EqualityComparer<MapFilters>.Default.GetHashCode(map_filters);
        hashCode = hashCode * -1521134295 + EqualityComparer<MiscFilters>.Default.GetHashCode(misc_filters);
        return hashCode;
    }
}

public class Query
{
    public Status status { get; set; } = new Status();
    public string name { get; set; }
    public string type { get; set; }
    public string term { get; set; }
    public List<Stat> stats { get; set; } = new List<Stat>{new Stat{type = "and", filters = new List<StatFilter>()}};
    public Filters filters { get; set; }

    public override bool Equals(object obj)
    {
        var query = obj as Query;
        return query != null &&
               name == query.name &&
               type == query.type &&
               term == query.term &&
               EqualityComparer<List<Stat>>.Default.Equals(stats, query.stats) &&
               EqualityComparer<Filters>.Default.Equals(filters, query.filters);
    }

    public override int GetHashCode()
    {
        var hashCode = -1615781718;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(term);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<Stat>>.Default.GetHashCode(stats);
        hashCode = hashCode * -1521134295 + EqualityComparer<Filters>.Default.GetHashCode(filters);
        return hashCode;
    }
}

public class Sort
{
    public string price { get; set; } = "asc";
}

public class PoeItemSearchRequest
{
    public Query query { get; set; } = new Query();
    public Sort sort { get; set; } = new Sort();

    public override bool Equals(object obj)
    {
        var request = obj as PoeItemSearchRequest;
        return request != null &&
               EqualityComparer<Query>.Default.Equals(query, request.query);
    }

    public override int GetHashCode()
    {
        return -644595621 + EqualityComparer<Query>.Default.GetHashCode(query);
    }

    public string GetName()
    {
        var name = query.name ?? query.type ?? "";
        if (query.filters?.type_filters?.filters?.rarity?.option == "uniquefoil")
        {
            name = $"Foil {name}";
        }

        if (query.filters?.socket_filters?.filters?.links != null)
        {
            switch (query.filters.socket_filters?.filters?.links.min)
            {
                case 5:
                    name += ", 5L";
                    break;
                case 6:
                    name += ", 6L";
                    break;
            }
        }

        if (query.filters.map_filters?.filters?.map_tier != null)
        {
            name += $", MapTier: {query.filters.map_filters?.filters?.map_tier.min}";
        }

        if (query.stats != null && query.stats.Count > 0 && query.stats[0].filters.Count > 0)
        {
            switch (query.stats[0].filters[0].id)
            {
                case "explicit.stat_3527617737":
                    name += $", {(query.stats[0].filters[0].value.min > 1 ? query.stats[0].filters[0].value.min + " Jewel" : query.stats[0].filters[0].value.min + " Jewel")}";
                    break;
                case "explicit.stat_1085446536":
                    name += $", {(query.stats[0].filters[0].value.min > 1 ? query.stats[0].filters[0].value.min + " passive" : query.stats[0].filters[0].value.min + " passive")}";
                    break;
                case "explicit.stat_1753916791":
                    name += ", Maim";
                    break;
                case "explicit.stat_1114411822":
                    name += ", Poison";
                    break;
                case "explicit.stat_4058504226":
                    name += ", Bleeding";
                    break;
            }
        }

        return name;
    }

    public static PoeItemSearchRequest CreateBasicRequest(string name, bool nameIsType = false)
    {
        var itemName = nameIsType ? string.Empty : name;
        var itemType = nameIsType ? name : string.Empty;

        return new PoeItemSearchRequest
        {
            query = new Query
            {
                status = new Status
                {
                    option = "online"
                },
                name = !string.IsNullOrEmpty(itemName) ? itemName : null,
                type = !string.IsNullOrEmpty(itemType) ? itemType : null,
                stats = new List<Stat>
                    {
                        new Stat
                        {
                            type = "and",
                            filters = new(),
                            disabled = false
                        }
                    },
                filters = new Filters
                {
                    misc_filters = new MiscFilters
                    {
                        filters = new FilterTypes
                        {
                            corrupted = new Corrupted
                            {
                                option = false
                            }
                        },
                        disabled = false
                    },
                    trade_filters = new TradeFilters
                    {
                        filters = new FilterTypes
                        {
                            price = new Price
                            {
                                min = null,
                                max = null,
                                option = "divine"
                            }
                        },
                        disabled = false
                    }
                }
            },
            sort = new Sort
            {
                price = "asc"
            }
        };
    }
}
