using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CS_Exam_01_Translator
{
    public class EnglishWord
    {
        public string EnglWord; // rename to originalWord or something (target)
        public List<string> RuTransl;  // translations
        public EnglishWord() : this("") { }
        public EnglishWord(string enWord)
        {
            EnglWord = enWord;
            RuTransl = new List<string>();
        }
        public override string ToString()
        {
            string str = $"Английский:\n\t\"{EnglWord}\"\nРусский:\n";
            foreach (string ruWord in RuTransl) { str += "\t" + ruWord + "\n"; }
            return str;
        }
    }

    class Translator
    {
        // move from here to main maybe (or menu class)
        public string fromWhat;
        public string toWhat;
        //
        public List<EnglishWord> Vocabulary;
        public Translator()
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<EnglishWord>));
            try
            {
                using (Stream fStream = File.OpenRead("test.xml"))
                {
                    Vocabulary = (List<EnglishWord>)xmlFormat.Deserialize(fStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            EnglishWord apple = new EnglishWord("Apple");
            apple.RuTransl.Add("Яблоко");

            EnglishWord orange = new EnglishWord("Orange");
            orange.RuTransl.Add("Оранжевый");
            orange.RuTransl.Add("Апельсин");

            List<EnglishWord> englishWords = new List<EnglishWord> { apple, orange };

            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<EnglishWord>));

            try
            {
                /*using (Stream fStream = File.Create("test.xml"))
                {
                    xmlFormat.Serialize(fStream, englishWords);
                }*/

                List<EnglishWord> translatorDatabase = null;
                using (Stream fStream = File.OpenRead("test.xml"))
                {
                    translatorDatabase = (List<EnglishWord>)xmlFormat.Deserialize(fStream);
                }

                string searchedWord = "orange";

                foreach (EnglishWord word in translatorDatabase)
                {
                    if (word.EnglWord.ToLower() == searchedWord.ToLower())
                    {
                        foreach (string transl in word.RuTransl)
                        {
                            Console.WriteLine(transl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //Console.WriteLine(apple);
            //Console.WriteLine(orange);

            Console.ReadLine();
        }
    }
}
