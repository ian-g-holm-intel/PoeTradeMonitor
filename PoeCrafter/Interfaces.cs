using System.Threading.Tasks;

namespace PoeCrafter;

/// <summary>
/// Interface for crafting operations in Path of Exile item modification.
/// </summary>
public interface ICrafter
{
    /// <summary>
    /// Executes the crafting process asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous crafting operation.</returns>
    Task Craft();
}
