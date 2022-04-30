using System.IO;
using System.Text.RegularExpressions;


namespace FileManagerOOP
{
    //структура для хранения введенной команды
    struct SplitedCommand
    {
        public string cmd;
        public string pathSrc;
        public string pathDst;
        public int page;
        public bool isValid;
        public string errorMessage;
        public ObjectFileSystemType type;

    }

    static internal class Commander
    {

        /// <summary>
        /// Парсинг и верификация заданной команды
        /// </summary>
        /// <param name="cmd">Строка команды</param>
        /// <returns>Структуру с командой</returns>
        static public SplitedCommand SplitCmd(string cmd)
        {
            SplitedCommand command = new SplitedCommand();
            command.isValid = true;
            //проверяем первый параметр
            cmd = GetFirstParam(cmd, ref command);
            //проверяем второй параметр
            if (command.isValid)
            {
                cmd = GetSecondParam(cmd, ref command);
            }
            //проверяем третий параметр
            if (command.isValid)
            {
                cmd = GetThirdParam(cmd, ref command);
            }

            //если что-то осталось
            if (command.isValid)
            {
                if (!string.IsNullOrEmpty(cmd))
                {
                    command.errorMessage = "Ошибка. Неверное число параметров.";
                    string errorMessage = "Split. ERROR: Uncorrect command";
                    ErrorMessage.WriteErrorToFile(errorMessage);
                    command.isValid = false;
                }
            }
            
            return command;

        }

        /// <summary>
        /// Парсинг и верификация 1 параметра команды
        /// </summary>
        /// <param name="cmd">Строка с командой</param>
        /// <param name="commandStruct">Структура с верифицированной командой</param>
        /// <returns>Строка команды без 1 параметра</returns>
        static private string GetFirstParam(string cmd, ref SplitedCommand commandStruct)
        {
            string pattern1param = @"^cp|ls|rm|mv|rn|cr|fp|st ";
            if (Regex.IsMatch(cmd, pattern1param))
            {
                commandStruct.cmd = Regex.Match(cmd, pattern1param).ToString().Trim();
                cmd = Regex.Replace(cmd, pattern1param, "");
            }
            else
            {
                commandStruct.errorMessage = "Ошибка. Введена неверная команда.";
                commandStruct.isValid = false;
                string errorMessage = "Split. ERROR:Uncorrect command";
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return cmd;
        }

        /// <summary>
        /// Парсинг и верификация 2 параметра команды
        /// </summary>
        /// <param name="cmd">Строка с командой</param>
        /// <param name="commandStruct">Структура с верифицированной командой</param>
        /// <returns>Строка команды без 2 параметра</returns>
        static private string GetSecondParam(string cmd, ref SplitedCommand commandStruct)
        {
            Regex innerRegex = new Regex("\"[^\"]*\"");
            if (innerRegex.IsMatch(cmd))
            {
                commandStruct.pathSrc = innerRegex.Match(cmd).ToString().Replace("\"", "");
                cmd = innerRegex.Replace(cmd, "", 1);
                cmd = cmd.Trim();
                //проверка корректности
                if (Path.HasExtension(commandStruct.pathSrc))
                {
                    if (!File.Exists(commandStruct.pathSrc) && commandStruct.cmd != "cr")
                    {
                        commandStruct.errorMessage = "Ошибка. Данного файла не существует.";
                        string errorMessage = "Split. ERROR: File don`t exist";
                        ErrorMessage.WriteErrorToFile(errorMessage);
                        commandStruct.isValid = false;
                    }
                    else
                    {
                        commandStruct.type = ObjectFileSystemType.File;
                    }
                }
                //если каталог...
                else
                {
                    if (!Directory.Exists(commandStruct.pathSrc) && commandStruct.cmd != "cr")
                    {
                        commandStruct.errorMessage = "Ошибка. Данного каталога не существует.";
                        string errorMessage = "Split. ERROR: Directory don`t exist";
                        ErrorMessage.WriteErrorToFile(errorMessage);
                        commandStruct.isValid = false;
                    }
                    else
                    {
                        commandStruct.type = ObjectFileSystemType.Catalog;
                    }
                }
            }
            else
            {
                commandStruct.errorMessage = "Ошибка. Введен неверный второй параметр.";
                commandStruct.isValid = false;
                string errorMessage = "Split. ERROR: Uncorrect command";
                ErrorMessage.WriteErrorToFile(errorMessage);
            }
            return cmd;
        }

        /// <summary>
        /// Парсинг и верификация 3 параметра команды
        /// </summary>
        /// <param name="cmd">Строка с командой</param>
        /// <param name="commandStruct">Структура с верифицированной командой</param>
        /// <returns>Строка команды без 3 параметра</returns>
        static private string GetThirdParam(string cmd, ref SplitedCommand commandStruct)
        {
            Regex innerRegex = new Regex("\"[^\"]*\"");
            //случай если путь к каталогу или файлу
            if (innerRegex.IsMatch(cmd) && 
                (((commandStruct.cmd == "cp" || commandStruct.cmd == "mv" || commandStruct.cmd == "rn")
                && commandStruct.type == ObjectFileSystemType.Catalog) 
                || ((commandStruct.cmd == "mv" || commandStruct.cmd == "rn")
                && commandStruct.type == ObjectFileSystemType.File)
                || commandStruct.cmd == "fp"))
            {
                commandStruct.pathDst = innerRegex.Match(cmd).ToString().Replace("\"", "");
                cmd = innerRegex.Replace(cmd, "");
                cmd = cmd.Trim();
            }
            else
            //случай педжинга
            {
                string pattern3param = @"^-p\d+";
                if (Regex.IsMatch(cmd, pattern3param) && commandStruct.cmd == "ls")
                {
                    string param = Regex.Match(cmd, pattern3param).ToString().Replace("-p", "");
                    if (!int.TryParse(param, out commandStruct.page))
                    {
                        commandStruct.errorMessage = "Ошибка. Превышение допустимого значения страницы.";
                        commandStruct.isValid = false;
                        string errorMessage = "Split. ERROR: Uncorrect command";
                        ErrorMessage.WriteErrorToFile(errorMessage);
                    }
                    cmd = Regex.Replace(cmd, pattern3param, "");
                    cmd = cmd.Trim();
                }
                else
                {
                    if (((commandStruct.cmd == "cp" && commandStruct.type == ObjectFileSystemType.File) || commandStruct.cmd == "rm"
                        || commandStruct.cmd == "cr" || commandStruct.cmd == "ls" || commandStruct.cmd == "st")
                       && string.IsNullOrEmpty(cmd.ToString()))
                    {
                        commandStruct.isValid = true;
                        return cmd;
                    }
                    commandStruct.errorMessage = "Ошибка. Введен неверный третий параметр.";
                    string errorMessage = "Split. ERROR: Uncorrect command";
                    ErrorMessage.WriteErrorToFile(errorMessage);
                    commandStruct.isValid = false;
                }
            }
            return cmd;
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="splCommand">Структура с командой</param>
        /// <param name="creator">Класс с объектами файловой системы</param>
        /// <returns>Статус завершения команды</returns>
        static public string ExecuteCommand(SplitedCommand splCommand, Creator creator)
        {
            string status = string.Empty;
            switch (splCommand.cmd)
            {
                case "ls":
                    {
                        if (splCommand.type == ObjectFileSystemType.Catalog)
                        {
                            creator.CreateNewObjectsFileSystem(splCommand.pathSrc);
                            return "Успешно";
                        }
                        //если попытка открыть файл вместо каталога
                        else
                        {
                            string errorMessage = "Execute. ERROR:Uncorrect command";
                            ErrorMessage.WriteErrorToFile(errorMessage);
                            return "Введите правильный путь к каталогу";
                        }
                    }
                case "cp":
                    {
                        if (splCommand.type == ObjectFileSystemType.File)
                        {
                            if (string.IsNullOrEmpty(splCommand.pathDst))
                            {

                                splCommand.pathDst = Path.Combine(Path.GetDirectoryName(splCommand.pathSrc), (Path.GetFileNameWithoutExtension(splCommand.pathSrc) + "_copy"
                                    + Path.GetExtension(splCommand.pathSrc)));
                            }
                            FileManager fileManager = new FileManager();
                            status = fileManager.Copy(splCommand.pathSrc, splCommand.pathDst);
                            return status;
                        }
                        else
                        {
                            if (!Directory.Exists(splCommand.pathDst))
                            {
                                Directory.CreateDirectory(splCommand.pathDst);
                            }
                            DirectoryManager directoryManager = new DirectoryManager();
                            status = directoryManager.Copy(splCommand.pathSrc, splCommand.pathDst);
                            return status;
                        }
                    }
                case "rm":
                    {
                        if (splCommand.type == ObjectFileSystemType.File)
                        {
                            FileManager fileManager = new FileManager();
                            status = fileManager.Remove(splCommand.pathSrc);
                            return status;
                        }
                        else
                        {
                            DirectoryManager directoryManager = new DirectoryManager();
                            status = directoryManager.Remove(splCommand.pathSrc);
                            return status;
                        }
                    }
                case "mv":
                    {
                        if (splCommand.type == ObjectFileSystemType.File)
                        {
                            FileManager fileManager = new FileManager();
                            status = fileManager.Move(splCommand.pathSrc, splCommand.pathDst);
                            return status;
                        }
                        else
                        {
                            DirectoryManager directoryManager = new DirectoryManager();
                            status = directoryManager.Move(splCommand.pathSrc, splCommand.pathDst);
                            return status;
                        }
                    }
                case "rn":
                    {
                        if (splCommand.type == ObjectFileSystemType.File)
                        {
                            FileManager fileManager = new FileManager();
                            status = fileManager.Rename(splCommand.pathSrc, splCommand.pathDst);
                            return status;
                        }
                        else
                        {
                            DirectoryManager directoryManager = new DirectoryManager();
                            status = directoryManager.Rename(splCommand.pathSrc, splCommand.pathDst);
                            return status;
                        }
                    }
                case "cr":
                    {
                        if (splCommand.type == ObjectFileSystemType.File)
                        {
                            FileManager fileManager = new FileManager();
                            status = fileManager.Create(splCommand.pathSrc);
                            return status;
                        }
                        else
                        {
                            DirectoryManager directoryManager = new DirectoryManager();
                            status = directoryManager.Create(splCommand.pathSrc);
                            return status;
                        }
                    }
                case "fp":
                    {
                        if (splCommand.type == ObjectFileSystemType.Catalog)
                        {
                            creator.FindFilesByPatterns(splCommand.pathSrc, splCommand.pathDst);
                            return "Успешно";
                        }
                        //если попытка открыть файл вместо каталога
                        else
                        {
                            string errorMessage = "Execute. ERROR:Uncorrect command";
                            ErrorMessage.WriteErrorToFile(errorMessage);
                            return "Введите правильный путь к каталогу";
                        }
                    }
                case "st":
                    {
                        if (splCommand.type == ObjectFileSystemType.File)
                        {
                            FileManager fileManager = new FileManager();
                            status = fileManager.FileInfo(splCommand.pathSrc);
                            return status;
                        }
                        else
                        {
                            string errorMessage = "Execute. ERROR:Uncorrect command";
                            ErrorMessage.WriteErrorToFile(errorMessage);
                            return "Введите правильный путь к файлу";
                        }
                    }
            }
            return status;
        }


    }
}
