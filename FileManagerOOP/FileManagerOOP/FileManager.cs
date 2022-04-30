using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FileManagerOOP
{
    internal class FileManager: IManager
    {

        /// <summary>
        /// Копирует существующий файл в новый файл
        /// </summary>
        /// <param name="sourcePath">Файл для копирования</param>
        /// <param name="destPath">Файл назначения</param>
        /// <returns></returns>
        public string Copy(string sourcePath, string destPath)
        {
            string status = "";
            try
            {
                File.Copy(sourcePath, destPath, overwrite: true);
                status = "Копирование файла завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Копирование файла завершено с ошибкой:" + ex.Message;
                string errorMessage = "Copy. Src:" + sourcePath + " Dest:" + destPath + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="path">Файл для удаления</param>
        /// <returns></returns>
        public string Remove(string path)
        {
            string status = "";
            try
            {
                File.Delete(path);
                status = "Удаление файла завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Удаление файла завершено с ошибкой:" + ex.Message;
                string errorMessage = "Remove. Src:" + path + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }

        /// <summary>
        /// Создание файла
        /// </summary>
        /// <param name="path">Файл для создания</param>
        /// <returns>Статус выполнения</returns>
        public string Create(string path)
        {
            string status;
            try
            {
                File.Create(path);
                status = "Создание файла завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Создание файла завершено с ошибкой:" + ex.Message;
                string errorMessage = "Create. Src:" + path + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }

        /// <summary>
        ///Перемещаем существующий файл
        /// </summary>
        /// <param name="sourcePath">Файл для перемещения</param>
        /// <param name="destPath">Файл назначения</param>
        /// <returns>Статус выполнения</returns>
        public string Move(string sourcePath, string destPath)
        {
            string status = "";
            try
            {
                File.Move(sourcePath, destPath);
                status = "Перемещение файла завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Перемещение файла завершено с ошибкой:" + ex.Message;
                string errorMessage = "Move. Src:" + sourcePath + " Dest:" + destPath + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }

        /// <summary>
        ///Переименование файла
        /// </summary>
        /// <param name="sourcePath">Файл для переименования</param>
        /// <param name="destPath">Переименованный файл</param>
        /// <returns>Статус выполнения</returns>
        public string Rename(string sourcePath, string destPath)
        {
            string status = "";
            try
            {
                File.Move(sourcePath, destPath);
                status = "Переименование файла завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Переименование файла завершено с ошибкой:" + ex.Message;
                string errorMessage = "Rename. Src:" + sourcePath + " Dest:" + destPath + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }

        /// <summary>
        /// Поиск по маске (с поиском по подпапкам)
        /// </summary>
        /// <param name="path">Каталог, с которого начинается поиск</param>
        /// <param name="pattern">Шаблон</param>
        /// <param name="fileEntries">Найденные по шаблону файлы</param>
        public void FindFiles(string path, string pattern, List<string> fileEntries)
        {
            try
            {
                foreach (string file in Directory.GetFiles(path, pattern))
                {
                    fileEntries.Add(file);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Search. Src:" + path + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
          
            try
            {
                foreach (string subdirectory in Directory.GetDirectories(path))
                {
                    FindFiles(subdirectory, pattern, fileEntries);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Search. Src:" + path + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
               
        }

        /// <summary>
        /// Сбор информации по текстовому файлу (кол-во строк, слов, абзацев)
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Результат в строке</returns>
        public string FileInfo(string path)
        {

            int[] stat = new int[3];
            string[] arrayWords;
            Regex regParagraph = new Regex(@"^\t|^\s");
            using (StreamReader reader = new StreamReader(path))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    //строки
                    stat[0]++;
                    //слова
                    arrayWords = line.Split(' ');
                    stat[1] += arrayWords.Length;
                    //абзац определяется из начала строки
                    //наличием пробелов или \t
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (regParagraph.IsMatch(line))
                        {
                            stat[2]++;
                        }                       
                    }
                    line = reader.ReadLine();
                }
            }
            return $"{path}\nКол-во строк: {stat[0]},  Кол-во слов: {stat[1]}\nКол-во абзацев: {stat[2]}\n";
        }
    }
}
