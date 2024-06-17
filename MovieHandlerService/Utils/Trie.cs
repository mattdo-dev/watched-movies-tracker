namespace MovieHandlerService.Utils;

public class Trie
{
    private readonly TrieNode _root = new();

    public void Insert(string title)
    {
        var normalizedTitle = title.ToLower();

        for (var start = 0; start < normalizedTitle.Length; ++start)
        {
            var node = _root;
            for (var i = start; i < normalizedTitle.Length; ++i)
            {
                var ch = normalizedTitle[i];
                if (!node.Children.TryGetValue(ch, out var value))
                {
                    value = new TrieNode();
                    node.Children[ch] = value;
                }

                node = value;
            }

            node.IsWordTerminated = true;
            node.Titles.Add(title);
        }
    }

    public IEnumerable<string> Search(string query)
    {
        var node = _root;
        query = query.ToLower();

        foreach (var ch in query)
        {
            if (!node.Children.TryGetValue(ch, out var child)) return [];

            node = child;
        }

        return Traverse(node, []);
    }

    private static List<string> Traverse(TrieNode node, HashSet<string> results)
    {
        if (node.IsWordTerminated)
            foreach (var title in node.Titles)
                results.Add(title);

        foreach (var child in node.Children) Traverse(child.Value, results);

        return [..results];
    }
}