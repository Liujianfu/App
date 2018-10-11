using System;
using System.IO;
using EvvMobile.Database.FileStorages;
using EvvMobile.Ios.FileStorages;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace EvvMobile.Ios.FileStorages
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}