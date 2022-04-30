using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FileManagerOOP
{
    internal class Creator
    {
        //хранение данных о файлах и каталогах
        private List<ObjectFileSystem> _objectsFileSystem;
        private Dictionary<string, int> _extension;
        private int _countFiles;
        private int _countDictionaries;
        private string _parentDictionary;
        private long _size;
        private int _depth;



        public Creator(string currentDirectory, int depth)
        {
            _objectsFileSystem = new List<ObjectFileSystem>();
            //первый элемент - родительский каталог
            if (currentDirectory == "C:\\")
            {
                _objectsFileSystem.Add(new ObjectFileSystem(":", ObjectFileSystemType.Catalog, "", -1, currentDirectory));
            }
            else
            {
                DirectoryInfo df = new DirectoryInfo(currentDirectory);
                _objectsFileSystem.Add(new ObjectFileSystem(":", ObjectFileSystemType.Catalog, "", -1, df.Parent.FullName));
            }
            _extension = new Dictionary<string, int>();
            _depth = depth;
            _parentDictionary = currentDirectory;
            _size = 0;
            SearchDirectory(currentDirectory, _objectsFileSystem, depth);
        }

        public List<ObjectFileSystem> ObjectsFileSystem { get { return _objectsFileSystem; } }
        public int CountFiles { get { return _countFiles; } }
        public int CountDictionaries { get { return _countDictionaries; } }
        public int Depth { get { return _depth; } }
        public Dictionary<string, int> Extension { get { return _extension; } }
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string ParentDictionary { get { return _parentDictionary; } }

        /// <summary>
        /// Количество объектов 
        /// </summary>
        /// <returns>Количество объектов</returns>
        public int Count()
        {
            return _objectsFileSystem.Count;
        }

        /// <summary>
        /// Заполнение двухуровневой структуры объектов файловой системы
        /// </summary>
        /// <param name="currentDirectory">Корневой каталог</param>
        /// <param name="objectsFileSystem">Структура с объектами файловой системы</param>
        /// <param name="depth">Глубина (в нашем примере 2, двухуровневая)</param>
        private void SearchDirectory(string currentDirectory, List<ObjectFileSystem> objectsFileSystem, int depth)
        {
            //глубина рекурсии промотра каталогов
            depth--;
            if (depth < 0)
            {
                return;
            }

            try
            {
                foreach (var directory in new DirectoryInfo(currentDirectory).GetDirectories())
                {
                    //сохраняем информацию заданной глубины на вывод на экран
                    objectsFileSystem.Add(new ObjectFileSystem(directory.Name, ObjectFileSystemType.Catalog, directory.CreationTime.ToString(), depth, directory.FullName));
                    //обход всей рекурсии для сбора информации о количестве файлов
                    SearchDirectory(directory.FullName, objectsFileSystem, depth);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Search. Src:" + currentDirectory + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            try
            {
                //собираем информацию по родительскому каталогу
                if (currentDirectory == _parentDictionary)
                {
                    string[] directories = Directory.GetDirectories(currentDirectory);
                    _countDictionaries = directories.Count();
                    string[] files = Directory.GetFiles(currentDirectory);
                    _countFiles = files.Count();
                    List<string> fileEntries = new List<string>(files);
                    FullExtensions(fileEntries);
                }

                foreach (var file in new DirectoryInfo(currentDirectory).GetFiles())
                {
                    objectsFileSystem.Add(new ObjectFileSystem(file.Name, ObjectFileSystemType.File, file.CreationTime.ToString(), depth, file.Length, file.Extension, currentDirectory));
                    if (currentDirectory == _parentDictionary)
                    {
                        Size += file.Length;
                    }

                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Search. Src:" + currentDirectory + " ERROR: " + ex.Message;
                ErrorMessage.WriteErrorToFile(errorMessage);
            }

        }

        /// <summary>
        /// метод пересоздает новый список, от родительского каталога
        /// </summary>
        /// <param name="currentDirectory"></param>
        public void CreateNewObjectsFileSystem(string currentDirectory)
        {
            _objectsFileSystem.Clear();
            _extension.Clear();
            _countFiles = 0;
            _countDictionaries = 0;
            _size = 0;
            _parentDictionary = currentDirectory;
            if (currentDirectory == "C:\\")
            {
                _objectsFileSystem.Add(new ObjectFileSystem(":", ObjectFileSystemType.Catalog, "", -1, currentDirectory));
            }
            else
            {
                DirectoryInfo df = new DirectoryInfo(currentDirectory);
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.Name == currentDirectory)
                    {
                        _objectsFileSystem.Add(new ObjectFileSystem(":", ObjectFileSystemType.Catalog, "", -1, currentDirectory));
                    }
                }
                if (_objectsFileSystem.Count == 0)
                {
                    _objectsFileSystem.Add(new ObjectFileSystem(":", ObjectFileSystemType.Catalog, "", -1, df.Parent.FullName));
                }
               
            }
            SearchDirectory(currentDirectory, _objectsFileSystem, Depth);
        }

        /// <summary>
        /// Создание списка файлов по шаблону
        /// </summary>
        /// <param name="currentDirectory">Родительский каталог</param>
        /// <param name="pattern">Шаблон</param>
        public void FindFilesByPatterns(string currentDirectory, string pattern)
        {
            _objectsFileSystem.Clear();
            _extension.Clear();
            _countFiles = 0;
            _countDictionaries = 0;
            _size = 0;
            _parentDictionary = currentDirectory;
            List<string> fileEntries = new List<string>();
            FileManager fileManager = new FileManager();
            fileManager.FindFiles(currentDirectory, pattern, fileEntries);
            foreach(string file in fileEntries)
            {
                FileInfo fi = new FileInfo(file);
                _objectsFileSystem.Add(new ObjectFileSystem(fi.Name, ObjectFileSystemType.File, fi.CreationTime.ToString(), 1, fi.Length, fi.Extension, fi.Directory.ToString()));
                Size += file.Length;
            }
            _countFiles = fileEntries.Count();
            FullExtensions(fileEntries);
        }

        /// <summary>
        /// Расчет количества файлов по расширениям
        /// </summary>
        /// <param name="fileEntries">Список файлов</param>
        private void FullExtensions(List<string> fileEntries)
        {
            //находим все расширения в каталоге
            var extensions =
               from file in fileEntries
               group file by Path.GetExtension(file);
            //собираем информацию по расширениям
            foreach (var extension in extensions)
            {
                if (_extension.ContainsKey(extension.Key))
                {
                    _extension[extension.Key] += extension.Count();
                }
                else
                {
                    _extension[extension.Key] = extension.Count();
                }
            }
        }
    }
}
