using Xunit;
using System.Linq;
using ProjectBank.Infrastructure.Generator;

namespace Generator.Tests;

public class TagGeneratorTests
{

    [Fact]
    public void Basic_Input_Hello_World_Recive_Hello_World_From_Text()
    {
        string text = "hello and welcome to this beautiful world!";

        ITagGenerator trie = new TagGenerator();
        trie.Add("hello");
        trie.Add("world");
        trie.Build();

        string[] matches = trie.Find(text).ToArray();

        Assert.Equal(2, matches.Length);
        Assert.Equal("Hello", matches[0]);
        Assert.Equal("World", matches[1]);
    }

    [Fact]
    public void Find_Add_In_Capital_Letters_Find_Input_With_Lowercase_Letters()
    {
        string text = "I always types in caps";

        ITagGenerator trie = new TagGenerator();
        trie.Add("CAPS");
        trie.Build();

        string[] matches = trie.Find(text).ToArray();

        Assert.Equal("Caps", matches[0]);
    }

    [Fact]
    public void Adding_same_value_only_returns_one_value()
    {
        string text = "I always types in caps";

        ITagGenerator trie = new TagGenerator();
        trie.Add("caps");
        trie.Add("caps");
        trie.Build();

        string[] matches = trie.Find(text).ToArray();

        Assert.Equal("Caps", matches[0]);
    }

    [Fact]
    public void Find_Add_In_LowerCase_Find_With_Capital_Letters()
    {
        string text = "I always types in CAPS";

        ITagGenerator trie = new TagGenerator();
        trie.Add("caps");
        trie.Build();

        string[] matches = trie.Find(text).ToArray();

        Assert.Equal(1, matches.Length);
        Assert.Equal("Caps", matches[0]);
    }

    [Fact]
    public void Add_An_Array()
    {
        string text = "one two three four";

        ITagGenerator trie = new TagGenerator();
        trie.Add(new[] {"three", "four"});
        trie.Build();

        string[] matches = trie.Find(text).ToArray();

        Assert.Equal("Three", matches[0]);
        Assert.Equal("Four", matches[1]);
    }

}