using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FileManagerOOP
{
 

    internal class Program
    {
        static void Main(string[] args)
        {
            
            int choiceString = 0;
            int currentPage = 1;
            int stringsOnPage = 15;
            string rootPath = "";
            // создание файла для записи директории
            string path = Path.Combine(Environment.CurrentDirectory, "directory.xml");
            if (!File.Exists(path))
            {
                rootPath = "C:\\";
            }
            else
            {
                XmlSerializer xs = new XmlSerializer(rootPath.GetType());
                using (FileStream xmlLoad = File.Open(path, FileMode.Open))
                {
                    // десериализация
                    rootPath = xs.Deserialize(xmlLoad) as string;
                    if (string.IsNullOrEmpty(rootPath))
                    {
                        rootPath = "C:\\";
                    }
                }
            }
           

            string status = "";
            //список для хранения истории команд
            List<string> historyCmd = new List<string>();
            int historyCounter = 0;
            Creator fm = new Creator(rootPath,  Config.LevelMax);
            ConsoleKeyInfo userChoice;
            StringBuilder cmd = new StringBuilder();
            UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, "");

            do
            {

                userChoice = Console.ReadKey();
                switch (userChoice.Key)
                {
                    case ConsoleKey.DownArrow:
                        {
                            if ((((currentPage - 1) * stringsOnPage) + choiceString) < fm.Count() - 1)
                            {
                                if (choiceString == stringsOnPage - 1)
                                {
                                    currentPage++;
                                    choiceString = 0;
                                    UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, "");
                                }
                                else
                                {
                                    choiceString++;
                                    UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, "");
                                }
                            }
                            break;

                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (choiceString == 0 && currentPage == 1)
                            {
                                break;
                            }
                            else
                            {
                                if (choiceString == 0)
                                {
                                    currentPage--;
                                    choiceString = stringsOnPage - 1;
                                    UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, "");
                                }
                                else
                                {
                                    choiceString--;
                                    UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, "");
                                }
                                break;
                            }
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            if (currentPage == 1)
                            {
                                break;
                            }
                            else
                            {
                                currentPage--;
                                choiceString = 0;
                                UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, status);
                            }
                            break;

                        }
                    case ConsoleKey.RightArrow:
                        {
                            //проверка на последнюю страницу
                            if (currentPage == Math.Ceiling(fm.Count() / (stringsOnPage * 1.0)))
                            {
                                break;
                            }
                            else
                            {
                                currentPage++;
                                choiceString = 0;
                                UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, status);
                            }
                            break;

                        }
                    case ConsoleKey.Backspace:
                        {
                            if (cmd.Length > 0)
                            {
                                cmd.Remove(cmd.Length - 1, 1);
                                UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, "");
                                break;
                            }
                            Console.CursorLeft = 18;
                            break;
                        }
                    case ConsoleKey.Enter:
                        {
                            //вход в активный каталог, если командная строка пуста
                            if (cmd.Length == 0)
                            {
                                if (fm.ObjectsFileSystem[choiceString].Type == ObjectFileSystemType.Catalog)
                                {
                                    rootPath = fm.ObjectsFileSystem[choiceString + (currentPage - 1) * stringsOnPage].AbsPath;
                                    fm.CreateNewObjectsFileSystem(rootPath);
                                    choiceString = 0;
                                    currentPage = 1;
                                    UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, status);
                                }

                            }
                            //а если не пустая, то выполнение командной строки
                            else
                            {
                                SplitedCommand splCommand = new SplitedCommand();
                                splCommand = Commander.SplitCmd(cmd.ToString());
                                if (splCommand.isValid == true)
                                {
                                    status = Commander.ExecuteCommand(splCommand, fm);
                                    if (splCommand.cmd == "ls")
                                    {
                                        if(splCommand.page == 0)
                                        {
                                            currentPage = 1;
                                        }
                                        else currentPage =  splCommand.page;
                                        choiceString = 0;
                                    }

                                }
                                else
                                {
                                    status = splCommand.errorMessage;
                                }
                                if (historyCmd.Count == 10)
                                {
                                    historyCmd.RemoveAt(0);
                                }
                                historyCmd.Add(cmd.ToString());
                                cmd.Clear();
                                UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, status);
                            }
                            break;

                        }

                    case ConsoleKey.PageUp:
                        {

                            if (historyCounter > 0)
                            {
                                cmd.Clear();
                                historyCounter--;
                                cmd.Append(historyCmd[historyCounter]);
                                UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, status);
                            }
                            else
                            {
                                Console.CursorLeft--;
                            }
                            break;
                        }

                    case ConsoleKey.PageDown:
                        {

                            if (historyCounter < historyCmd.Count)
                            {
                                cmd.Clear();
                                cmd.Append(historyCmd[historyCounter]);
                                historyCounter++;
                                UI.PrintScreen(fm, cmd, choiceString, currentPage, stringsOnPage, status);
                            }
                            else
                            {
                                Console.CursorLeft--;
                            }
                            break;
                        }

                    default: cmd.Append(userChoice.KeyChar); break;

                }
            } while (userChoice.Key != ConsoleKey.Escape);
            //завершающие операции            
            using (FileStream stream = File.Create(path))
            {
                // cериализация
                XmlSerializer xs = new XmlSerializer(rootPath.GetType());
                xs.Serialize(stream, fm.ParentDictionary);
            }

        }
    }
    
}
