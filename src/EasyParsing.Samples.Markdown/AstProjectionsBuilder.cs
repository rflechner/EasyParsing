using System.Collections.Immutable;
using EasyParsing.Dsl.Linq;
using EasyParsing.Samples.Markdown.Ast;

namespace EasyParsing.Samples.Markdown;

internal static class AstProjectionsBuilder
{
    internal static ListItems BuildListTree(this IEnumerable<ListItem> items)
    {
        bool initialized = false;
        int minDepth = 0;
        var root = new Stack<ListItem>();
        var deep = new Stack<ListItem>();
                    
        foreach (var item in items)
        {
            if (!initialized)
            {
                minDepth = item.Depth;
                initialized = true;
                root.Push(item);
                continue;
            }

            // root
            if (item.Depth <= minDepth)
            {
                root.Push(item);
                deep.Clear();
                continue;
            }
                        
            if (!root.TryPeek(out var lastRoot)) throw new ArithmeticException("Stack is empty");

            // same level
            if (item.Depth == lastRoot.Depth)
            {
                root.Push(item);
                deep.Clear();
                continue;
            }

            // sub level under root starts
            if (item.Depth > lastRoot.Depth && deep.Count == 0)
            {
                lastRoot.NestedList.Push(item);
                deep.Push(lastRoot);
                continue;
            }

            if (!deep.TryPeek(out var currentLevelParent)) throw new ArithmeticException("Current level parent is null");

            // under level continues
            if (item.Depth > currentLevelParent.Depth && currentLevelParent.NestedList.TryPeek(out var lastNested) && lastNested.Depth < item.Depth)
            {
                lastNested.NestedList.Push(item);
                deep.Push(lastNested);
                continue;
            }
                        
            // under level starts
            if (item.Depth > currentLevelParent.Depth)
            {
                currentLevelParent.NestedList.Push(item);
                continue;
            }
                        
            // re-up level
            if (item.Depth <= currentLevelParent.Depth)
            {
                deep.Pop();
                            
                if (!deep.TryPeek(out currentLevelParent)) throw new ArithmeticException("Current level parent is null");
                            
                currentLevelParent.NestedList.Push(item);
            }
        }
                    
        return new ListItems(root.ToArray());
    }
    
    internal static IParser<MarkdownAst[]> MergeRawTextParts(this IParser<IEnumerable<MarkdownAst>> parser)
    {
        return parser.Aggregate(ImmutableList.Create<MarkdownAst>(),(acc, item) =>
        {
            if (acc.IsEmpty || acc[^1] is not RawText last || item is not RawText current) return acc.Add(item);
            
            var merge = new RawText($"{last.Content}{current.Content}");
            
            return acc.RemoveAt(acc.Count-1).Add(merge);
        }).Select(r => r.ToArray());
    }
}