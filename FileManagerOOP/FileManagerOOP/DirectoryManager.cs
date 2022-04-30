using System;
using System.IO;


namespace FileManagerOOP
{
    internal class DirectoryManager : IManager
    {
        /// <summary>
        /// Копирует существующий каталог в новый каталог
        /// </summary>
        /// <param name="sourcePath">Каталог для копирования</param>
        /// <param name="destPath">Каталог назначения</param>
        /// <returns>Статус выполнения</returns>
        public string Copy(string sourcePath, string destPath)
        {

            string status = "";

            string[] files = Directory.GetFiles(sourcePath);
            string[] directories = Directory.GetDirectories(sourcePath);

            //копирование файлов
            foreach (var file in files)
            {
                // Получаем имя файла
                string fName = file.Substring(sourcePath.Length + 1);
                try
                {
                    // копируем
                    File.Copy(file, Path.Combine(destPath, fName));
                }
                catch (Exception ex)
                {
                    status = ex.Message;
                    string errorMessage = "Copy. Src:" + sourcePath + " Dest:" + destPath + " ERROR: " + ex.Message;
                    ErrorMessage.WriteErrorToFile(errorMessage);
                    return status;
                }
            }

            //копирование директорий
            foreach (var pathSource in directories)
            {
                // получаем имя директории
                string dName = pathSource.Substring(sourcePath.Length + 1);
                string pathDest = Path.Combine(destPath, dName);
                try
                {
                    // создаем каталог
                    Directory.CreateDirectory(pathDest);
                }
                catch (Exception ex)
                {
                    status = ex.Message;
                    string errorMessage = "Copy. Src:" + sourcePath + " Dest:" + destPath + " ERROR: " + ex.Message;
                    ErrorMessage.WriteErrorToFile(errorMessage);
                    return status;
                }
                Copy(pathSource, pathDest);
            }

            status = "Копирование каталога завершено успешно.";
            return status;
        }

        /// <summary>
        /// Удаление каталога
        /// </summary>
        /// <param name="path">Каталог для удаления</param>
        /// <returns>Статус выполнения</returns>
        public string Remove(string path)
        {
            string status;
            try
            {
                Directory.Delete(path, recursive: true);
                status = "Удаление каталога завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Удаление завершено с ошибкой:" + ex.Message;
                string errorMessage = "Remove. Src:" + path + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }

        /// <summary>
        /// Создание каталога
        /// </summary>
        /// <param name="path">Каталог для создания</param>
        /// <returns>Статус выполнения</returns>
        public string Create(string path)
        {
            string status;
            try
            {
                Directory.CreateDirectory(path);
                status = "Создание каталога завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Создание каталога завершено с ошибкой:" + ex.Message;
                string errorMessage = "Create. Src:" + path + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }

        /// <summary>
        ///Перемещаем существующий каталог в новый каталог
        /// </summary>
        /// <param name="sourcePath">Каталог для перемещения</param>
        /// <param name="destPath">Каталог назначения</param>
        /// <returns>Статус выполнения</returns>
        public string Move(string sourcePath, string destPath)
        {
            string status = "";
            try
            {
                Directory.Move(sourcePath, destPath);
                status = "Перемещение каталога завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Перемещение каталога завершено с ошибкой:" + ex.Message;
                string errorMessage = "Move. Src:" + sourcePath + " Dest:" + destPath + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }

        /// <summary>
        ///Переименование каталога
        /// </summary>
        /// <param name="sourcePath">Каталог для переименования</param>
        /// <param name="destPath">Переименованный каталог</param>
        /// <returns>Статус выполнения</returns>
        public string Rename(string sourcePath, string destPath)
        {
            string status = "";
            try
            {
                Directory.Move(sourcePath, destPath);
                status = "Переименование каталога завершено успешно.";
            }
            catch (Exception ex)
            {
                status = $"Переименование каталога завершено с ошибкой:" + ex.Message;
                string errorMessage = "Rename. Src:" + sourcePath + " Dest:" + destPath + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return status;
        }
    }
}
