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
    /// Compile with csc /r:C5.dll AnagramHashBag.cs 
    /// </summary>
    public class AnagramHashBag
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            var strings = args.Length == 2 ? ReadFileWords(args[0], int.Parse(args[1])) : args;

            // foreach (string word in FirstAnagramOnly(words)) 
            //   Console.WriteLine(word);
            //   Console.WriteLine("===");
            var sw = Stopwatch.StartNew();
            var classes = AnagramClasses(strings);
            var count = 0;
            foreach (var anagramClass in classes)
            {
                count++;
                foreach (var s in anagramClass)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("{0} non-trivial anagram classes", count);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
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
        /// So an anagram class is represented as HashBag{char} which permits fast equality comparison 
        /// -- we shall use them as elements of hash sets or keys in hash maps.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static HashBag<char> AnagramClass(string word)
        {
            var anagram = new HashBag<char>();
            
            anagram.AddAll(word.ToCharArray());
            
            return anagram;
        }

        /// <summary>
        /// Given a sequence of words, return only the first member of each anagram class.
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static IEnumerable<string> FirstAnagramOnly(IEnumerable<string> words)
        {
            var anagrams = new HashSet<HashBag<char>>();
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
        /// Given a sequence of words, return all non-trivial anagram classes.  
        /// Using HashBag{char} and an unsequenced equality comparer, this performs as
        /// follows on 1600 MHz Mobile P4 and .Net 2.0 beta 1 (wall-clock time):
        /// 50,000 words, 2,822 classes:    2.0 sec
        /// 100,000 words, 5,593 classes:   4.3 sec
        /// 200,000 words, 11,705 classes:  8.8 sec
        /// 300,000 words, 20,396 classes:  52.0 sec includes swapping
        /// 347,165 words, 24,428 classes: 146.0 sec includes swapping
        /// The maximal memory consumption is less than 180 MB.
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<string>> AnagramClasses(IEnumerable<String> words)
        {
            IDictionary<HashBag<char>, TreeSet<String>> classes = new HashDictionary<HashBag<char>, TreeSet<string>>();
            
            foreach (var word in words)
            {
                var anagram = AnagramClass(word);
                TreeSet<string> anagramClass;
                if (!classes.Find(ref anagram, out anagramClass))
                {
                    classes[anagram] = anagramClass = new TreeSet<String>();
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
