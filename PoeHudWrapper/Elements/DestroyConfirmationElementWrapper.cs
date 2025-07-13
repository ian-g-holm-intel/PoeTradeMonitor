using PoeHudWrapper.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper.Elements;

public class DestroyConfirmationElementWrapper : ElementWrapper
{
    public Point KeepButtonLocation => GetChildFromIndices(0, 0, 3, 1).Center;
}
