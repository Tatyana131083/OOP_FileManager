namespace FileManagerOOP
{
    internal interface IManager
    {
        public string Copy(string sourcePath, string destPath);
        public string Remove(string path);
        public string Move(string sourcePath, string destPath);
        public string Rename(string sourcePath, string destPath);
        public string Create(string path);
    }
}
