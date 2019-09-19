using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MikaDiskCleaner
{
    public class DiskCleaner
    {
        private List<FileInfo> existingFiles = new List<FileInfo>();
        private DirectoryInfo doublonFolder;

        private readonly FolderSorter _sorter;

        public DiskCleaner(FolderSorter sorter)
        {
            _sorter = sorter;
        }

        public void Run(string rootFolder)
        {
            CreateDoublonsFolder(GetFolderPath());

            DirectoryInfo root = new DirectoryInfo(rootFolder);

            if (root.Exists)
            {
                WalkDirectoryTree(root);
            }

            _sorter.SortFolder(doublonFolder);
        }

        private string GetFolderPath()
        {
            Console.WriteLine("Where would you want to save your files ?");
            var folderPath = Console.ReadLine();
            Console.WriteLine($"You want to save the files at : {folderPath}. {Environment.NewLine} Correct ?");
            var confirmation = Console.ReadLine();

            if (confirmation.ToLower() == "yes" || confirmation.ToLower() == "y")
            {
                return folderPath;
            }
            else
            {
                return GetFolderPath();
            }

        }

        private void CreateDoublonsFolder(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath))
            {
                doublonFolder = new DirectoryInfo(folderPath);
            }

            if (!doublonFolder.Exists)
            {
                doublonFolder.Create();

                _sorter.CreateSubFolders(doublonFolder);

                Console.WriteLine("Doublon folder created");
                return;
            }
            else
            {
                Console.WriteLine("Doublon folder already exists");

                _sorter.CreateSubFolders(doublonFolder);
            }
        }

        private void WalkDirectoryTree(DirectoryInfo root)
        {
            List<FileInfo> files = root.GetFiles("*.*").ToList();

            DirectoryInfo[] subDirs;

            if (files != null && files.Count > 0)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine($"{root.FullName} : {files.Count} files");

                foreach (var fileInfo in files)
                {
                    if (File.Exists(fileInfo.FullName))
                    {
                        CompareFiles(fileInfo);
                    }
                    else
                    {
                        Console.WriteLine($"{fileInfo.FullName} is a directory, we'll check it later.");
                    }
                }

                subDirs = root.GetDirectories();

                foreach (var dirInfo in subDirs)
                {
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        private bool CompareFiles(FileInfo file)
        {
            bool result = false;
            Console.WriteLine($"==== Checking if {file.Name} already exists : ====");
            byte[] file1 = File.ReadAllBytes(file.FullName);

            foreach (var item in existingFiles)
            {
                Console.WriteLine($"Comparing {file.Name} with {item.Name}. ");
                byte[] file2 = File.ReadAllBytes(item.FullName);

                if (file1.Length == file2.Length)
                {
                    for (int i = 0; i < file1.Length; i++)
                    {
                        if (file1[i] != file2[i])
                        {
                            result = false;
                        }
                    }

                    File.Move(file.FullName, Path.Combine(doublonFolder.FullName, file.Name));

                    result = true;
                }
            }


            if (existingFiles.Count <= 0 || !result)
            {
                Console.WriteLine($"{file.Name} was not registered yet.");
                existingFiles.Add(file);
                result = false;
            }

            Console.WriteLine("Finished comparing files");
            Console.WriteLine(Environment.NewLine);

            return result;

        }    

        
    }
}
