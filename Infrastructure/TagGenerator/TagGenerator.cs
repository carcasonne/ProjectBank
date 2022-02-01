using System.Collections;
using System.Collections.Generic;
using AhoCorasick;
using System;

namespace ProjectBank.Infrastructure.Generator;

public interface ITagGenerator
{
    public void Add(string s);
    public void Add(IEnumerable<string> strings);
    public void Build();
    public IEnumerable<string> Find(string text);
}

public class TagGenerator : ITagGenerator
{
    private Trie t;

    public TagGenerator() => t = new Trie();


    public void Add(string s) {
        s = s.ToLower();
        t.Add(s);
    }

    public void Build() { 
        t.Build();
    }

    public void Add(IEnumerable<string> strings)
    {
        foreach (string s in strings)
        {
            Add(s);
        }
    }

    public IEnumerable<string> Find(string text) {
        text = text.ToLower();

        var rawtags = t.Find(text).Distinct();
        List<string> tags = new List<string>();

        foreach (var tag in rawtags)
        {
            var lowerCaseTag = tag.ToCharArray();
            char[] upperCaseTag = new char[tag.Length];
            upperCaseTag[0] = lowerCaseTag[0].ToString().ToUpper().ToCharArray()[0];
            var counter = 0;

            foreach (var letter in lowerCaseTag)
            {
                if (counter != 0) 
                {
                    upperCaseTag[counter] = letter;
                }
                counter++;
            }
            tags.Add(new string(upperCaseTag));
        }

        return tags;

    }
}