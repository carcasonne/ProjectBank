// Copyright (c) 2013 Pēteris Ņikiforovs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections;

namespace AhoCorasick;

    //The Trie that will return keywords found in text 
    public class Trie : Trie<string>
    {
       // Adds string to the trie
        public void Add(string s)
        {
            s.ToLower();
            Add(s, s);
        }
    }

    public class Trie<TValue> : Trie<char, TValue>
    {
    }

    public class Trie<T, TValue>
    {
        //Root node of the trie
        private readonly Node<T, TValue> root = new Node<T, TValue>();

       //Adds word to the trie 
        public void Add(IEnumerable<T> word, TValue value)
        {
            // start at the root
            var node = root;

            // build a branch for the word, one letter at a time
            // if a letter node doesn't exist, add it
            foreach (T c in word)
            {
                var child = node[c];

                if (child == null)
                    child = node[c] = new Node<T, TValue>(c, node);

                node = child;
            }

            // Add the value that will be returned when this word is found in a text
            node.Values.Add(value);
        }


        //Build up fail and fall links in the rie
        public void Build()
        {
            // construction is done using breadth-first-search
            var queue = new Queue<Node<T, TValue>>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                // visit children
                foreach (var child in node)
                    queue.Enqueue(child);

                // fail link of root is root
                if (node == root)
                {
                    root.Fail = root;
                    continue;
                }

                var fail = node.Parent.Fail;

                while (fail[node.Word] == null && fail != root)
                    fail = fail.Fail;

                node.Fail = fail[node.Word] ?? root;
                if (node.Fail == node) 
                    node.Fail = root;
            }
        }

        //Find all added keywords in the text
        public IEnumerable<TValue> Find(IEnumerable<T> text)
        {
            var node = root;

            foreach (T c in text)
            {
                while (node[c] == null && node != root)
                    node = node.Fail;

                node = node[c] ?? root;

                for (var t = node; t != root; t = t.Fail)
                {
                    foreach (TValue value in t.Values)
                        yield return value;
                }
            }
        }

        private class Node<TNode, TNodeValue> : IEnumerable<Node<TNode, TNodeValue>>
    {
            private readonly TNode word;
            private readonly Node<TNode, TNodeValue> parent;
            private readonly Dictionary<TNode, Node<TNode, TNodeValue>> children = new Dictionary<TNode, Node<TNode, TNodeValue>>();
            private readonly List<TNodeValue> values = new List<TNodeValue>();

            //Constructer for the root node
            public Node()
            {
            }

            //Constructer for a node with a word
            public Node(TNode word, Node<TNode, TNodeValue> parent)
            {
                this.word = word;
                this.parent = parent;
            }

            //Word or letter for the node
            public TNode Word
            {
                get { return word; }
            }

            public Node<TNode, TNodeValue> Parent
            {
                get { return parent; }
            }

            public Node<TNode, TNodeValue> Fail
            {
                get;
                set;
            }

            //Child nodes
            public Node<TNode, TNodeValue> this[TNode c]
            {
                get { return children.ContainsKey(c) ? children[c] : null; }
                set { children[c] = value; }
            }

            //End values
            public List<TNodeValue> Values
            {
                get { return values; }
            }

            //inherit
            public IEnumerator<Node<TNode, TNodeValue>> GetEnumerator()
            {
                return children.Values.GetEnumerator();
            }

            //inherit
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            //inherit
            public override string ToString()
            {
                return Word.ToString();
            }
        }
    }
