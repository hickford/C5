using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace C5.UserGuideExamples
{
    /// <summary>
    /// C5 example: anagrams 2004-08-08, 2004-11-16, 2013-09-01
    /// Compile with csc /r:C5.dll Anagrams.cs 
    /// </summary>
    public class Anagrams
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var words = args.Length == 2 ? ReadFileWords(args[0], int.Parse(args[1])) : args;

            // foreach (string s in FirstAnagramOnly(words)) 
            //   Console.WriteLine(s);
            //   Console.WriteLine("===");

            var stopwatch = Stopwatch.StartNew();
            const bool unsequenced = true;
            var classes = AnagramClasses(words, unsequenced);
            var count = 0;
            foreach (var anagramClass in classes)
            {
                count++;
                foreach (var anagram in anagramClass)
                {
                    Console.Write(anagram + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("{0} non-trivial anagram classes", count);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }

        /// <summary>
        /// Read at most maximumNumberOfWords words from filename
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="maximumNumberOfWords"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadFileWords(string filename, int maximumNumberOfWords)
        {
            var delimiter = new Regex("[^a-zæøåA-ZÆØÅ0-9-]+");

            using (var reader = new StreamReader(filename, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine() ?? string.Empty;

                    foreach (var word in delimiter.Split(line))
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            yield return word.ToLower();
                        }
                    }
                    if (--maximumNumberOfWords == 0)
                    {
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        /// From an anagram point of view, a word is just a bag of characters.  
        /// So an anagram class is represented as TreeBag{char} which permits fast equality comparison 
        /// -- we shall use them as elements of hash sets or keys in hash maps.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static TreeBag<char> AnagramClass(string word)
        {
            var anagram = new TreeBag<char>(Comparer<char>.Default, EqualityComparer<char>.Default);

            anagram.AddAll(word.ToCharArray());

            return anagram;
        }

        /// <summary>
        /// Given a sequence of strings, return only the first member of each anagram class.
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static IEnumerable<string> FirstAnagramOnly(System.Collections.Generic.IEnumerable<string> words)
        {
            IEqualityComparer<TreeBag<char>> comparer = UnsequencedCollectionEqualityComparer<TreeBag<char>, char>.Default;

            var anagrams = new HashSet<TreeBag<char>>(comparer);

            foreach (var word in words)
            {
                var anagram = AnagramClass(word);

                if (!anagrams.Contains(anagram))
                {
                    anagrams.Add(anagram);

                    yield return word;
                }
            }
        }

        /// <summary>
        /// Given a sequence of strings, return all non-trivial anagram classes. 
        /// Should use a *sequenced* equalityComparer on a TreeBag{char},
        /// obviously: after all, characters can be sorted by ASCII code.  
        /// On 347,000 distinct Danish words this takes 70 cpu seconds, 180 MB memory, 
        /// and 263 wall-clock seconds (due to swapping).
        /// Using a TreeBag{char} and a sequenced equalityComparer takes 82 cpu seconds
        /// and 180 MB RAM to find the 26,058 anagram classes among 347,000
        /// distinct words.
        /// Using an unsequenced equalityComparer on TreeBag{char} or HashBag{char}
        /// makes it criminally slow: at least 1200 cpu seconds.  This must
        /// be because many bags get the same hash code, so that there are
        /// many collisions.  But exactly how the unsequenced equalityComparer works is
        /// not clear ... or is it because unsequenced equality is slow?
        /// </summary>
        /// <param name="words"></param>
        /// <param name="unsequenced"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<string>> AnagramClasses(IEnumerable<string> words, bool unsequenced)
        {
            IEqualityComparer<TreeBag<char>> comparer;
            
            if (unsequenced)
            {
                comparer = UnsequencedCollectionEqualityComparer<TreeBag<char>, char>.Default;
            }
            else
            {
                comparer = SequencedCollectionEqualityComparer<TreeBag<char>, char>.Default;
            }

            IDictionary<TreeBag<char>, TreeSet<string>> classes = new HashDictionary<TreeBag<char>, TreeSet<string>>(comparer);

            foreach (var word in words)
            {
                var anagram = AnagramClass(word);

                TreeSet<string> anagramClass;

                if (!classes.Find(ref anagram, out anagramClass))
                {
                    classes[anagram] = anagramClass = new TreeSet<string>();
                }
                anagramClass.Add(word);
            }

            foreach (var anagramClass in classes.Values)
            {
                if (anagramClass.Count > 1)
                {
                    yield return anagramClass;
                }
            }
        }
    }
}