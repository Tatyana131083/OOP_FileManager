﻿
namespace FileManagerOOP
{
    enum ObjectFileSystemType
    {
        File,
        Catalog
    }
    internal class ObjectFileSystem
    {
        readonly string _name;
        readonly string _absPath;
        readonly ObjectFileSystemType _type;
        readonly long _size;
        readonly string _extension = string.Empty;
        readonly string _creationTime = string.Empty;
        readonly int _level;


        public ObjectFileSystem(string name, ObjectFileSystemType type, string creationTime, int level, long size, string extension, string absPath)
        {
            _name = name;
            _absPath = absPath;
            _type = type;
            _size = size;
            _extension = extension;
            _creationTime = creationTime;
            _level = level;

        }
        public ObjectFileSystem(string name, ObjectFileSystemType type, string creationTime, int level, string absPath)
        {
            _name = name;
            _absPath = absPath;
            _type = type;
            _creationTime = creationTime;
            _level = level;

        }

        public string Name { get { return _name; } }
        public string AbsPath { get { return _absPath; } }
        public ObjectFileSystemType Type { get { return _type; } }
        public double Size { get { return _size; } }
        public string Extension { get { return _extension; } }
        public string CreationTime { get { return _creationTime; } }
        public int Level { get { return _level; } }

    }
}
