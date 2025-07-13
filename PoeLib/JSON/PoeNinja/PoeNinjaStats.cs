namespace PoeLib.JSON.PoeNinja;

public class PoeNinjaStats
{
    public int id { get; set; }
    public string next_change_id { get; set; }
    public long api_bytes_downloaded { get; set; }
    public long stash_tabs_processed { get; set; }
    public int api_calls { get; set; }
    public long character_bytes_downloaded { get; set; }
    public int character_api_calls { get; set; }
    public long ladder_bytes_downloaded { get; set; }
    public int ladder_api_calls { get; set; }
    public int pob_characters_calculated { get; set; }
}
