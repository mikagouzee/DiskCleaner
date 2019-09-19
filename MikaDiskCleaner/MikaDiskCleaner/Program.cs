using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace MikaDiskCleaner
{
    class Program
    {
        static List<FileInfo> existingFiles = new List<FileInfo>();

        static DirectoryInfo doublonFolder;
        static DirectoryInfo imagesFolder;
        static DirectoryInfo moviesFolder;
        static DirectoryInfo musicFolder;
        static DirectoryInfo documentsFolder;

        static List<string> imageExtensions;
        static List<string> musicExtensions;
        static List<string> movieExtensions;

        static void Main(string[] args)
        {
            SetupExtensions();

            Console.WriteLine("Hello Sir.");

            string folderPath = GetFolderPath();
            CreateDoublonsFolder(folderPath);

            //replace this with a get on argument
            DirectoryInfo root = new DirectoryInfo(@"C:\Users\mgo\Desktop\Folder\TestFolder");

            WalkDirectoryTree(root);
            Console.WriteLine("Exiting application");
            Console.ReadLine();
            Environment.Exit(0);
        }

        static void SetupExtensions()
        {
            imageExtensions = new List<string>
            {
                "png",
                "jpg",
                "svg",
                "gif"
            };

            musicExtensions = new List<string>
            {
                "mp3",
                "wav",
                "aiff"
            };

            movieExtensions = new List<string>
            {
                "avi",
                "mp4",
                "mov"
            };

        }

        static string GetFolderPath()
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

        static void CreateDoublonsFolder(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath))
            {
                doublonFolder = new DirectoryInfo(folderPath);
            }

            if (!doublonFolder.Exists)
            {
                doublonFolder.Create();

                imagesFolder = new DirectoryInfo(Path.Combine(doublonFolder.FullName, "images"));
                musicFolder = new DirectoryInfo(Path.Combine(doublonFolder.FullName, "music"));
                documentsFolder = new DirectoryInfo(Path.Combine(doublonFolder.FullName, "documents"));
                moviesFolder = new DirectoryInfo(Path.Combine(doublonFolder.FullName, "movies"));

                Console.WriteLine("Doublon folder created");
                return;
            }
            else
            {
                Console.WriteLine("Doublon folder already exists");

                imagesFolder = new DirectoryInfo(Path.Combine(doublonFolder.FullName, "images"));
                if (!imagesFolder.Exists)
                    imagesFolder.Create();

                musicFolder = new DirectoryInfo(Path.Combine(doublonFolder.FullName, "music"));
                if (!musicFolder.Exists)
                    musicFolder.Create();

                documentsFolder = new DirectoryInfo(Path.Combine(doublonFolder.FullName, "documents"));
                if (!documentsFolder.Exists)
                    documentsFolder.Create();

                moviesFolder = new DirectoryInfo(Path.Combine(doublonFolder.FullName, "movies"));
                if (!moviesFolder.Exists)
                    moviesFolder.Create();
            }

        }

        static void WalkDirectoryTree(DirectoryInfo root)
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

        static bool CompareFiles(FileInfo file)
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

                    MoveToDoublonsFolder(file);
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
    

        static void MoveToDoublonsFolder(FileInfo file)
        {
            Console.WriteLine($"Moving {file.Name} to doublon folder");

            var fileType = file.Name.Split(".").Last();

            string subfolderDestination = string.Empty;

            if (musicExtensions.Contains(fileType))
                subfolderDestination = musicFolder.FullName;
            else if (imageExtensions.Contains(fileType))
                subfolderDestination = imagesFolder.FullName;
            else if (movieExtensions.Contains(fileType))
                subfolderDestination = moviesFolder.FullName;
            else
                subfolderDestination = documentsFolder.FullName;

            File.Move(file.FullName, Path.Combine(subfolderDestination, file.Name));
        }

    }


}
