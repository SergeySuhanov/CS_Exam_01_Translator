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
        public void AddTranslation(string s)
        {
            foreach (string translation in transl)
            {
                if (s.ToLower() == translation.ToLower())
                {
                    Console.WriteLine("Такой перевод уже есть!");
                    return;
                }
            }
            transl.Add(s);
            Console.WriteLine("Слово добавлено");
        }
        public void PrintTraslation()
        {
            Console.WriteLine($"Перевод слова:");
            for (int i = 0; i < transl.Count; i++)
            {
                Console.WriteLine($"\t{i+ 1} -\t{transl[i]}");
            }
            Console.WriteLine();
        }
        /*public override string ToString()
        {
            string str = $"Английский:\n\t\"{origWord}\"\nРусский:\n";
            foreach (string word in transl) { str += "\t" + word + "\n"; }
            return str;
        }*/
    }

    class Translator
    {
        public string FilePath;
        public string[] Dictionaries;
        public List<ForeignWord> Vocabulary;    // FIX IT: Vocabulary doesn't clear, copied into new vocabulary
        public Translator()
        {
            Vocabulary = new List<ForeignWord>();
        }
        public void GetDictionariesList()
        {
            string dictFolder = Directory.GetCurrentDirectory() + "\\dictionaries";
            if (!Directory.Exists(dictFolder))
            {
                Directory.CreateDirectory(dictFolder);
            }
            Dictionaries = Directory.GetFiles(dictFolder);
            string fileName;
            for (int i = 0; i < Dictionaries.Length; i++)
            {
                fileName = Dictionaries[i].Substring(dictFolder.Length + 1); // get rid of full path name
                fileName = fileName.Substring(0, fileName.Length - 4);       // get rid of .xml suffix 
                Console.WriteLine($"{i + 1} - {fileName}");
            }
        }
        public void LoadVocabulary()
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<ForeignWord>));
            try
            {
                using (Stream fStream = File.OpenRead(FilePath))
                {
                    Vocabulary = (List<ForeignWord>)xmlFormat.Deserialize(fStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void EditOriginalWord(string newOrigWord, int index)
        {
            for (int i = 0; i < Vocabulary.Count; i++)
            {
                if ((newOrigWord == Vocabulary[i].origWord) && (i != index))
                {
                    Console.WriteLine($"Новое слово совпадает с другим в словаре");
                    return;
                }
            }
            this.Vocabulary[index].origWord = newOrigWord;
            Console.WriteLine("Слово успешно изменено");
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
                using (Stream fStream = File.Create(FilePath))
                {
                    xmlFormat.Serialize(fStream, Vocabulary);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void CreateVocabulary(string transName)    // FIX IT: Code repeat with ? or find two another similar functions in Translator
        {
            foreach (string existingTrans in this.Dictionaries)
            {
                if (transName == existingTrans)
                {
                    Console.WriteLine("Словарь с таким именем уже существует!");
                    return;
                }
            }

            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<ForeignWord>));
            try
            {
                string dictFolder = Directory.GetCurrentDirectory() + "\\dictionaries";
                if (!Directory.Exists(dictFolder))
                {
                    Directory.CreateDirectory(dictFolder);
                }
                using (Stream fStream = File.Create($"{dictFolder}\\{transName}.xml"))
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
            Translator trans = new Translator();

            while (true)
            {
                Console.Clear();
                int userInput = 0;
                Console.WriteLine("\tГлавное меню");
                Console.WriteLine("    Выберите действие:\n");
                Console.WriteLine("1 - Создать новый словарь");
                Console.WriteLine("2 - Редактировать существующий словарь");
                Console.WriteLine("3 - Искать перевод");
                userInput = Int32.Parse(Console.ReadLine());

                bool inMenuLvl2 = true;
                switch (userInput)
                {
                    case 1:    // Create New Vocabulary
                        Console.Clear();
                        Console.WriteLine("Уже существующие словари:");
                        trans.GetDictionariesList();
                        Console.WriteLine("Укажите тип словаря (пример: Русско-Английский)");
                        string transName = Console.ReadLine();
                        trans.CreateVocabulary(transName);
                        break;
                    case 2:    // Edit Vocabulary
                        while (inMenuLvl2)
                        {
                            Console.Clear();
                            int dictChoice;
                            Console.WriteLine("\tРедактор словарей");
                            Console.WriteLine("    Выберите словарь:\n");
                            Console.WriteLine("0 - Назад в предыдущее меню");
                            trans.GetDictionariesList();
                            dictChoice = Int32.Parse(Console.ReadLine());

                            if (dictChoice == 0)
                            {
                                inMenuLvl2 = false;
                                continue;
                            }
                            else if (dictChoice <= trans.Dictionaries.Length)
                            {
                                trans.FilePath = trans.Dictionaries[dictChoice - 1];
                                trans.LoadVocabulary();

                                bool inMenuLvl3 = true;
                                while (inMenuLvl3)
                                {
                                    Console.Clear();
                                    int editChoice; 
                                    Console.WriteLine("    Что редактировать:\n");
                                    Console.WriteLine("0 - Выйти в предыдущее меню");
                                    Console.WriteLine("1 - Добавить новое слово");
                                    Console.WriteLine("2 - Редактировать слово или перевод");
                                    Console.WriteLine("3 - Удалить слово или перевод");
                                    editChoice = Int32.Parse(Console.ReadLine());

                                    string srchWord = "";
                                    int ind = -1;

                                    switch (editChoice)
                                    {
                                        case 0:    // Back
                                            inMenuLvl3 = false;
                                            break;
                                        case 1:    // Add New Word
                                            Console.Write("Введите слово:  ");
                                            srchWord = Console.ReadLine();
                                            ind = trans.SearchWord(srchWord);
                                            if (ind != -1)
                                            {
                                                Console.WriteLine("В словаре уже есть такое слово!");
                                                trans.Vocabulary[ind].PrintTraslation();
                                            }  
                                            else
                                            {
                                                ForeignWord newWord = new ForeignWord(srchWord);

                                                bool enteringNewWord = true;
                                                while (enteringNewWord)
                                                {
                                                    Console.Write("Введите перевод слова: ");
                                                    string newTransl = Console.ReadLine();
                                                    newWord.AddTranslation(newTransl);

                                                    int nextNewWord;
                                                    Console.WriteLine("\n\tЧто дальше?:");
                                                    Console.WriteLine("0 - Назад (+ сохранить изменения)");
                                                    Console.WriteLine("1 - Ввести дополнительный перевод");
                                                    nextNewWord = Int32.Parse(Console.ReadLine());

                                                    switch (nextNewWord)
                                                    {
                                                        case 0:
                                                            trans.Vocabulary.Add(newWord);
                                                            trans.SaveVocabulary();
                                                            enteringNewWord = false;
                                                            break;
                                                        case 1:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            break;
                                        case 2:    // Edit Existing Word
                                            Console.Clear();
                                            Console.Write("Введите слово:  ");
                                            srchWord = Console.ReadLine();
                                            ind = trans.SearchWord(srchWord);
                                            if (ind != -1)
                                            {
                                                Console.WriteLine("В словаре есть такое слово!");
                                                trans.Vocabulary[ind].PrintTraslation();
                                                Console.WriteLine("Что нужно изменить:");
                                                Console.WriteLine("0 - Ничего");
                                                Console.WriteLine("1 - Слово");
                                                Console.WriteLine("2 - Перевод");
                                                int wordEdit = Int32.Parse(Console.ReadLine());

                                                switch (wordEdit)
                                                {
                                                    case 0:
                                                        break;
                                                    case 1:
                                                        Console.WriteLine("Введите новое слово: ");
                                                        string newOrigWord = Console.ReadLine();
                                                        trans.EditOriginalWord(newOrigWord, ind);
                                                        trans.SaveVocabulary();
                                                        break;
                                                    case 2:
                                                        trans.Vocabulary[ind].PrintTraslation();
                                                        Console.WriteLine("Какое слово изменить: ");
                                                        int translInd = Int32.Parse(Console.ReadLine()) - 1;
                                                        Console.WriteLine("Напишите новый вариант перевода: ");
                                                        string newTransl = Console.ReadLine();
                                                        trans.Vocabulary[ind].transl[translInd] = newTransl;
                                                        trans.SaveVocabulary();
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            else
                                                Console.WriteLine("В словаре нет такого слова!");
                                            break;
                                        case 3:    // Delete Word
                                            Console.Clear();
                                            Console.Write("Введите слово:  ");
                                            srchWord = Console.ReadLine();
                                            ind = trans.SearchWord(srchWord);
                                            if (ind != -1)
                                            {
                                                Console.WriteLine("В словаре есть такое слово!");
                                                trans.Vocabulary[ind].PrintTraslation();
                                                Console.WriteLine("Что нужно удалить:");
                                                Console.WriteLine("0 - Ничего");
                                                Console.WriteLine("1 - Слово целиком");
                                                Console.WriteLine("2 - Вариант перевода");
                                                int wordDel = Int32.Parse(Console.ReadLine());

                                                switch (wordDel)
                                                {
                                                    case 0:
                                                        break;
                                                    case 1:    // Delete Whole Word
                                                        trans.Vocabulary.RemoveAt(ind);
                                                        Console.WriteLine($"Слово {srchWord} удалено: ") ;
                                                        trans.SaveVocabulary();
                                                        break;
                                                    case 2:    // Delete Translation
                                                        trans.Vocabulary[ind].PrintTraslation();
                                                        if (trans.Vocabulary[ind].transl.Count > 1)
                                                        {
                                                            Console.WriteLine("Какое слово удалить: ");
                                                            int translInd = Int32.Parse(Console.ReadLine()) - 1;
                                                            trans.Vocabulary[ind].transl.RemoveAt(translInd);
                                                            Console.WriteLine($"Вариант перевода удалён");
                                                            trans.SaveVocabulary();
                                                        }
                                                        else
                                                            Console.WriteLine("Нельзя удалить единственный вариант перевода!");
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                                break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case 3:    // Search Word Translation
                        while (inMenuLvl2)
                        {
                            Console.Clear();
                            int dictChoice;
                            Console.WriteLine("\tПереводчик");
                            Console.WriteLine("    Выберите словарь:\n");
                            Console.WriteLine("0 - Назад в предыдущее меню");
                            trans.GetDictionariesList();
                            dictChoice = Int32.Parse(Console.ReadLine());

                            if (dictChoice == 0)
                            {
                                inMenuLvl2 = false;
                                continue;
                            }
                            else if (dictChoice <= trans.Dictionaries.Length)
                            {
                                trans.FilePath = trans.Dictionaries[dictChoice - 1];
                                trans.LoadVocabulary();

                                bool inMenuLvl3 = true;
                                while (inMenuLvl3)
                                {
                                    Console.Clear();
                                    Console.Write("Введите слово:  ");
                                    string srchWord = Console.ReadLine();
                                    int ind = trans.SearchWord(srchWord);
                                    if (ind != -1)
                                        trans.Vocabulary[ind].PrintTraslation();
                                    else
                                        Console.WriteLine("В словаре нет такого слова");

                                    int nextAction;
                                    Console.WriteLine("Что дальше?:");
                                    Console.WriteLine("0 - Назад");
                                    Console.WriteLine("1 - Ввести новое слово");
                                    Console.WriteLine("2 - Выгрузить результат в файл");
                                    nextAction = Int32.Parse(Console.ReadLine());
                                    switch (nextAction)
                                    {
                                        case 0:
                                            inMenuLvl3 = false;
                                            break;
                                        case 1:
                                            break;
                                        case 2:    // FIX IT: Code almost fully repeated
                                            XmlSerializer xmlFormat = new XmlSerializer(typeof(ForeignWord));
                                            try
                                            {
                                                string dictFolder = Directory.GetCurrentDirectory() + "\\saved_searches";
                                                if (!Directory.Exists(dictFolder))
                                                {
                                                    Directory.CreateDirectory(dictFolder);
                                                }
                                                using (Stream fStream = File.Create($"{dictFolder}\\{srchWord}.xml"))
                                                {
                                                    xmlFormat.Serialize(fStream, trans.Vocabulary[ind]);
                                                    Console.WriteLine($"Слово было сохранено в файл {srchWord}.xml");
                                                    inMenuLvl3 = false;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            } 
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
