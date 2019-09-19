using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MikaDiskCleaner
{
    public class FolderSorter
    {
        private DirectoryInfo imagesFolder;
        private DirectoryInfo moviesFolder;
        private DirectoryInfo musicFolder;
        private DirectoryInfo documentsFolder;

        private List<string> imageExtensions;
        private List<string> musicExtensions;
        private List<string> movieExtensions;

        private readonly IConfiguration _config;

        public FolderSorter(IConfiguration config)
        {
            _config = config;
        }

        public void CreateSubFolders(DirectoryInfo root)
        {
            imagesFolder = new DirectoryInfo(Path.Combine(root.FullName, "images"));
            if (!imagesFolder.Exists)
                imagesFolder.Create();
            musicFolder = new DirectoryInfo(Path.Combine(root.FullName, "music"));
            if (!musicFolder.Exists)
                musicFolder.Create();

            documentsFolder = new DirectoryInfo(Path.Combine(root.FullName, "documents"));
            if (!documentsFolder.Exists)
                documentsFolder.Create();

            moviesFolder = new DirectoryInfo(Path.Combine(root.FullName, "movies"));
            if (!moviesFolder.Exists)
                moviesFolder.Create();
        }

        public void SortFolder(DirectoryInfo toSort)
        {
            SetupExtensions();

            if (!toSort.Exists)
            {
                toSort.Create();
                CreateSubFolders(toSort);
            }

            foreach (var file in toSort.GetFiles())
            {
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

        private void SetupExtensions()
        {
            var image = _config.GetValue<string>("imageExtensions");
            imageExtensions = image.Split(",").ToList();

            movieExtensions = _config.GetValue<string>("movieExtensions").Split(",").ToList();

            musicExtensions = _config.GetValue<string>("musicExtensions").Split(",").ToList();

        }


    }
}
