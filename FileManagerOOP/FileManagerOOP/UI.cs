using System;
using System.Collections.Generic;
using System.Text;


namespace FileManagerOOP
{
    static internal class UI
    {


        /// <summary>
        /// Вывод области с каталогами и файлами
        /// </summary>
        /// <param name="objectsFileSystem">Структура с объектами файловой системы</param>
        /// <param name="choiceString">Активная строка</param>
        /// <param name="currentPage">Страница</param>
        /// <param name="stringsOnPage">Кол-во строк на странице</param>
        static private void PrintDirectory(List<ObjectFileSystem> objectsFileSystem, int choiceString, int currentPage, int stringsOnPage)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            int index = 0;
            int startPosition = (currentPage - 1) * stringsOnPage;
            //проверка индекса
            int endPosition = startPosition + stringsOnPage;
            int emptyString = 0;
            if (endPosition > objectsFileSystem.Count)
            {
                emptyString = stringsOnPage;
                stringsOnPage = objectsFileSystem.Count % stringsOnPage;
                emptyString -= stringsOnPage;
            }
            PrintLine();
            Console.WriteLine($"{" ",Config.Indent}{"Имя",-Config.WidthClmnName}{"Дата создания",-Config.WidthClmnData}{"Тип",-Config.WidthClmnDType}{"Размер",-Config.WidthClmnSize}{"Расширение",-Config.WidthClmnEx}");
            PrintLine();
            foreach (ObjectFileSystem obj in objectsFileSystem.GetRange(startPosition, stringsOnPage))
            {
                int count = Math.Abs(obj.Level - Config.LevelMax + 1);
                int widthName = Config.WidthClmnName - count * 4;

                StringBuilder objName = new StringBuilder();
                for (int i = 0; i < count * 4; i++)
                {
                    objName.Append(" ");
                }
                //обрезание имени под колонку
                objName.Append(obj.Name.Length <= widthName - 4 ? obj.Name : obj.Name.Substring(0, widthName - 4));
                if (choiceString == index)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
                if (index == 0 && currentPage == 0)
                {
                    Console.WriteLine($"{ "   :",-Config.WidthClmnName}");
                }
                else if (obj.Type == ObjectFileSystemType.Catalog)
                {
                    Console.WriteLine($"{" ",Config.Indent}{objName,-Config.WidthClmnName}{obj.CreationTime,-Config.WidthClmnData}{"Каталог"}");
                }
                else
                {
                    Console.WriteLine($"{" ",Config.Indent}{objName,-Config.WidthClmnName}{obj.CreationTime,-Config.WidthClmnData}{"Файл",-Config.WidthClmnDType}{obj.Size,-Config.WidthClmnSize}{obj.Extension}");
                }

                if (choiceString == index)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                }
                index++;
            }
            while (emptyString > 0)
            {
                Console.WriteLine();
                emptyString--;
            }
            PrintLine();
        }

        /// <summary>
        /// Вывол горизонтальной линии
        /// </summary>
        static private void PrintLine()
        {
            int count = Config.Indent + Config.WidthClmnName + Config.WidthClmnData + Config.WidthClmnDType + Config.WidthClmnSize + Config.WidthClmnEx;
            while (count > 0)
            {
                Console.Write("-");
                count--;
            }
            Console.WriteLine();
        }


        /// <summary>
        /// Вывод области с командной строкой
        /// </summary>
        /// <param name="cmd">Вводимая командная строка</param>
        static void PrintCmd(StringBuilder cmd)
        {
            if (cmd.Length == 0)
            {
                Console.Write("Командная строка: ");
            }
            else
            {
                Console.Write("Командная строка: ");
                Console.Write(cmd);
            }
        }

        /// <summary>
        /// Вывод статуса исполнения команд
        /// </summary>
        /// <param name="status">Статус команды</param>
        static private void PrintStatus(string status)
        {
            Console.Write("Статус выполнения: ");
            Console.WriteLine(status);
            PrintLine();
        }

        /// <summary>
        /// Вывод информации о каталоге
        /// </summary>
        /// <param name="fm">Класс, где хранится информация об объектах файловой системы</param>
        static private void PrintInfo(Creator fm)
        {
            Console.WriteLine($"Краткая информация: ");
            Console.WriteLine($"Кол-во подкаталогов: {fm.CountDictionaries}");
            Console.WriteLine($"Кол-во файлов: {fm.CountFiles}   {fm.Size} Б");
            int i = 0;
            foreach (var current in fm.Extension.Keys)
            {
                if (i > 5)
                {
                    Console.WriteLine();
                    i = 0;
                }
                Console.Write($"    {fm.Extension[current]} {current}    |   ");
                i++;
            }
            Console.WriteLine();
            PrintLine();
        }

        /// <summary>
        /// Вывод на консоль
        /// </summary>
        /// <param name="fm">Класс, где хранится информация об объектах файловой системы</param>
        /// <param name="cmd">Командная строка</param>
        /// <param name="choiceString">Активная строка</param>
        /// <param name="currentPage">Текущая страница</param>
        /// <param name="stringsOnPage">Количество строк на странице</param>
        /// <param name="status">Статус</param>
        static public void PrintScreen(Creator fm, StringBuilder cmd, int choiceString, int currentPage, int stringsOnPage, string status)
        {
            PrintDirectory(fm.ObjectsFileSystem, choiceString, currentPage, stringsOnPage);
            PrintInfo(fm);
            PrintStatus(status);
            PrintCmd(cmd);
        }


    }
}
