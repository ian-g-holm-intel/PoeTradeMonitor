using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Helpers;
using GameOffsets;
using MoreLinq;
using System.Numerics;

namespace PoeHudWrapper.MemoryObjects;

public class ElementWrapper : RemoteMemoryObject
{
    public const int OffsetBuffers = 0;
    private static readonly int ChildStartOffset = Extensions.GetOffset<ElementOffsets>(x => x.ChildStart);
    private static readonly int EntityOffset = Extensions.GetOffset<NormalInventoryItemOffsets>(x => x.Item);

    // dd id
    // dd (something zero)
    // 16 dup <128-bytes structure>
    // then the rest is
    private readonly List<ElementWrapper> _childrens = new List<ElementWrapper>();

    // public ElementWrapper Root => Elem.Root==0 ?null: GetObject<ElementWrapper>(M.Read<long>(Elem.Root+0xE8));
    private ElementWrapper _parent;
    private long childHashCache;

    public ElementOffsets Elem => Address == 0 ? default : M.Read<ElementOffsets>(Address);
    public bool IsValid => Elem.SelfPointer == Address;
    public ElementType Type => (ElementType)Elem.Type;

    //public int vTable => Elem.vTable;
    public long ChildCount => (Elem.ChildEnd - Elem.ChildStart) / 8;

    public bool IsVisibleLocal => Elem.Flags.HasFlag(ElementFlags.IsVisibleLocal); //3-rd bit: opened 1110, closed 0110
    public bool IsScrollable => Elem.Flags.HasFlag(ElementFlags.IsScrollable);
    public bool IsSelected => (Elem.IsSelected >> 5) == 0;
    public bool IsActive => Elem.Flags.HasFlag(ElementFlags.IsActive);
    public ElementWrapper Root => GameWrapper.TheGame.IngameState.UIRoot;
    public ElementWrapper Parent => Elem.Parent == 0 ? null : _parent ??= GetObject<ElementWrapper>(Elem.Parent);
    [Obsolete]
    public SharpDX.Vector2 Position => Elem.Position.ToSharpDx();
    public Vector2 PositionNum => Elem.Position;
    public float X => PositionNum.X;
    public float Y => PositionNum.Y;
    public Vector2 ScrollOffset => Elem.ScrollOffset;
    public ElementWrapper Tooltip => Address == 0 ? null : ReadObject<ElementWrapper>(Elem.Tooltip);
    public float Scale => Elem.Scale;
    public float Width => Elem.Size.X;
    public float Height => Elem.Size.Y;
    public bool isHighlighted => Elem.isHighlighted;
    public bool HasShinyHighlight => Elem.ShinyHighlightState != 0;
    public int TextSize => Elem.LabelTextSize;
    public SharpDX.ColorBGRA TextColor => Elem.LabelTextColor;
    public SharpDX.ColorBGRA BordColor => Elem.LabelBorderColor;
    public SharpDX.ColorBGRA BgColor => Elem.LabelBackgroundColor;

    public Entity Entity => ReadObject<Entity>(Address + EntityOffset);

    public System.Drawing.Point Center
    {
        get
        {
            var center = GetClientRect().Center;
            return new System.Drawing.Point(Convert.ToInt32(center.X), Convert.ToInt32(center.Y));
        }
    }

    public virtual string Text => GetText(Constants.DefaultMaxStringLength);
    public string TextureName => M.ReadStringU(M.Read<long>(Elem.TextureNamePtr));

    public string TextNoTags => GetTextWithNoTags(Constants.DefaultMaxStringLength);

    public string GetText(int maxLength) => ReplaceIconReferences(Elem.Text.ToString(M, maxLength));
    public string GetTextWithNoTags(int maxLength) => ReplaceIconReferences(Elem.TextNoTags.ToString(M, maxLength));

    private static string ReplaceIconReferences(string text)
    {
        if (!string.IsNullOrWhiteSpace(text)) return text.Replace("\u00A0\u00A0\u00A0\u00A0", "{{icon}}");
        return null;
    }

    public bool IsVisible
    {
        get
        {
            //998
            if (Address >= 1770350607106052 || Address <= 0) return false;
            return IsVisibleLocal && GetParentChain().All(current => current.IsVisibleLocal);
        }
    }

    public IList<ElementWrapper> Children => GetChildren<ElementWrapper>();
    public long ChildHash => Elem.Childs.GetHashCode();
    public ElementWrapper this[int index] => GetChildAtIndex(index);

    public int? IndexInParent => Parent?.Children.IndexOf(this);

    public string PathFromRoot
    {
        get
        {
            var parentChain = GetParentChain();
            if (parentChain.Count != 0)
            {
                parentChain.RemoveAt(parentChain.Count - 1);
                parentChain.Reverse();
            }

            parentChain.Add(this);
            var properties = GameWrapper.TheGame.IngameState.IngameUi.GetType()
                .GetProperties()
                .Where(p => typeof(ElementWrapper).IsAssignableFrom(p.PropertyType))
                .Where(p => p.GetIndexParameters().Length == 0)
                .Select(p => (property: p, (p.GetValue(GameWrapper.TheGame.IngameState.IngameUi) as ElementWrapper)?.Address))
                .Where(t => t.Address is not 0 or null)
                .ToLookup(x => x.Address, x => x.property.Name);

            return string.Join("->", parentChain.Select(x => properties[x.Address].ToList() switch
            {
                { Count: 0 } => x.IndexInParent.ToString(),
                { Count: 1 } single => $"({single.First()}){x.IndexInParent}",
                var many => $"({string.Join('/', many)}){x.IndexInParent}"
            }));
        }
    }

    protected List<ElementWrapper> GetChildren<T>() where T : ElementWrapper
    {
        var e = Elem;
        var childCount = ChildCount;
        if (Address == 0 || e.ChildStart == 0 || e.ChildEnd == 0 || childCount <= 0) return _childrens;

        if (childCount > 1000) return _childrens;
        var pointers = M.ReadPointersArray(e.ChildStart, e.ChildEnd);
        //can this even happen?
        if (pointers.Count != childCount) return _childrens;
        _childrens.Clear();
        _childrens.EnsureCapacity(pointers.Count);

        foreach (var pointer in pointers)
        {
            _childrens.Add(GetObject<ElementWrapper>(pointer));
        }

        childHashCache = ChildHash;
        return _childrens;
    }

    public List<T> GetChildrenAs<T>() where T : ElementWrapper, new()
    {
        var e = Elem;
        if (Address == 0 || e.ChildStart == 0 || e.ChildEnd == 0 || ChildCount <= 0) return new List<T>();

        var pointers = M.ReadPointersArray(e.ChildStart, e.ChildEnd);

        if (pointers.Count != ChildCount)
            return new List<T>();

        var results = new List<T>(pointers.Count);

        foreach (var pointer in pointers)
        {
            results.Add(GetObject<T>(pointer));
        }

        return results;
    }

    public List<ElementWrapper> GetParentChain()
    {
        var list = new List<ElementWrapper>();

        if (Address == 0)
            return list;

        var hashSet = new HashSet<ElementWrapper>();
        var root = Root;
        var parent = Parent;

        if (root == null || parent == null)
            return list;

        while (!hashSet.Contains(parent) && root.Address != parent.Address && parent.Address != 0 && hashSet.Count < 100)
        {
            list.Add(parent);
            hashSet.Add(parent);
            parent = parent.Parent;

            if (parent == null)
                break;
        }

        return list;
    }

    private Vector2 GetChainPos(Vector2 ownRawPosition)
    {
        var pos = Vector2.Zero;
        var rootScale = GameWrapper.TheGame.IngameState.UIRoot.Scale;
        var prevScale = 0f;
        var prevScroll = Vector2.Zero;

        var parentChain = GetParentChain();

        for (var i = parentChain.Count - 1; i >= 0; i--)
        {
            var current = parentChain[i];
            var scale = current.Scale / rootScale;
            pos += current.PositionNum * scale + (current.IsScrollable ? prevScale * prevScroll : Vector2.Zero);
            prevScale = scale;
            prevScroll = current.ScrollOffset;
        }

        pos += ownRawPosition * Scale / rootScale + (IsScrollable ? prevScale * prevScroll : Vector2.Zero);

        return pos;
    }

    public virtual SharpDX.RectangleF GetClientRect()
    {
        return GetClientRectWithCustomPosition(PositionNum);
    }

    public SharpDX.RectangleF GetClientRectWithCustomPosition(Vector2 position)
    {
        if (Address == 0) return SharpDX.RectangleF.Empty;
        var unscaledPos = GetChainPos(position);
        float width = GameWrapper.TheGame.IngameState.Camera.Width;
        float height = GameWrapper.TheGame.IngameState.Camera.Height;
        var ratioFixMult = width / height / 1.6f;
        var xScale = width / 2560f / ratioFixMult;
        var yScale = height / 1600f;

        //poe window limited to 12/5 (or whatever the new value is), element position gets offset by the blackbar width
        var xOffset = GameWrapper.TheGame.BlackBarSize;

        var rootScale = GameWrapper.TheGame.IngameState.UIRoot.Scale;
        var vec = unscaledPos * new Vector2(xScale, yScale);
        return new SharpDX.RectangleF(vec.X + xOffset, vec.Y,
            xScale * Width * Scale / rootScale,
            yScale * Height * Scale / rootScale);
    }

    public ElementWrapper FindChildRecursive(Func<ElementWrapper, bool> condition)
    {
        if (condition(this))
            return this;

        foreach (var child in Children)
        {
            var elem = child.FindChildRecursive(condition);

            if (elem != null)
                return elem;
        }

        return null;
    }

    public ElementWrapper FindChildRecursive(string text, bool contains = false)
    {
        return FindChildRecursive(elem => elem.Text == text || (contains && (elem.Text?.Contains(text) ?? false)));
    }

    public ElementWrapper GetChildFromIndices(params int[] indices)
    {
        var poe_UElement = this;

        foreach (var index in indices)
        {
            poe_UElement = poe_UElement.GetChildAtIndex(index);

            if (poe_UElement == null)
            {
                var str = "";
                indices.ForEach(i => str += $"[{i}] ");
                DebugWindow.LogMsg($"{nameof(ElementWrapper)} with index: {index} not found. Indices: {str}");
                return null;
            }

            if (poe_UElement.Address == 0)
            {
                var str = "";
                indices.ForEach(i => str += $"[{i}] ");
                DebugWindow.LogMsg($"{nameof(ElementWrapper)} with index: {index} 0 address. Indices: {str}");
                return GetObject<ElementWrapper>(0);
            }
        }

        return poe_UElement;
    }

    public ElementWrapper GetChildAtIndex(int index)
    {
        return index >= ChildCount ? null : GetObject<ElementWrapper>(M.Read<long>(Address + ChildStartOffset, index * 8));
    }
}