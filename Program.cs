using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CS_Exam_01_Translator
{
    public class ForeignWord
    {
        public string origWord;
        public List<string> transl;
        public ForeignWord() : this("") { }
        public ForeignWord(string word)
        {
            origWord = word;
            transl = new List<string>();
        }
        public void PrintTraslation()
        {
            Console.WriteLine($"Перевод слова:");
            foreach (string word in transl) { Console.WriteLine($"\t\t{word}"); }
        }
        public override string ToString()
        {
            string str = $"Английский:\n\t\"{origWord}\"\nРусский:\n";
            foreach (string word in transl) { str += "\t" + word + "\n"; }
            return str;
        }
    }

    class Translator
    {
        string fileName;
        public List<string> Dictionaries;
        public List<ForeignWord> Vocabulary;
        public Translator()
        {
            Vocabulary = new List<ForeignWord>();
        }
        public void LoadVocabulary(string vocabu)
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<ForeignWord>));
            try
            {
                using (Stream fStream = File.OpenRead($"{vocabu}.xml"))
                {
                    Vocabulary = (List<ForeignWord>)xmlFormat.Deserialize(fStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public int SearchWord(string forWord)
        {
            for (int i = 0; i < Vocabulary.Count; i++)
            {
                if (forWord.ToLower() == Vocabulary[i].origWord.ToLower())
                {
                    return i;
                }
            }
            return -1;
        }
        public void SaveVocabulary()
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<ForeignWord>));
            try
            {
                using (Stream fStream = File.Create($"{fileName}.xml"))
                {
                    xmlFormat.Serialize(fStream, Vocabulary);
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
            int userInput = 0;
            Console.WriteLine("\tГлавное меню");
            Console.WriteLine("    Выберите действие:\n");
            Console.WriteLine("1 - Создать новый словарь");
            Console.WriteLine("2 - Редактировать существующий словарь");
            Console.WriteLine("3 - Искать перевод");
            userInput = Int32.Parse(Console.ReadLine());

            switch (userInput)
            {
                case 3:
                    int dictChoice;
                    Console.WriteLine("\tПереводчик");
                    Console.WriteLine("    Выберите словарь:\n");
                    Console.WriteLine("1 - Англо-Русский");
                    //Console.WriteLine("2 - Русско-Английский");
                    dictChoice = Int32.Parse(Console.ReadLine());

                    switch (dictChoice)
                    {
                        case 1:
                            Translator trans = new Translator();
                            trans.LoadVocabulary("EN-RU"); // hardcoded name wont allow additional user created dictionaries

                            Console.Write("Введите слово:  ");
                            string srchWord = Console.ReadLine();
                            int ind = trans.SearchWord(srchWord);
                            if (ind != -1)
                                trans.Vocabulary[ind].PrintTraslation();
                            else
                                Console.WriteLine("В словаре нет такого слова");

                            Console.WriteLine("Что дальше?:");
                            Console.WriteLine("1 - Назад");
                            Console.WriteLine("2 - Ввести новое слово");
                            Console.WriteLine("3 - Выгрузить результат в файл");
                            
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }

            //Translator trans = new Translator();
            //trans.LoadVocabulary("test");
            //int ind = trans.SearchWord("orange");
            //trans.Vocabulary[ind].PrintTraslation();

            Console.ReadLine();
        }
    }
}
