using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace AutomatedRedditImageDownloaderClient
{
    public class Program
    {
        private static string _localDownloadLocation = @".\Images";
        private static string _blacklistUrl = @"http://earth.tyleryoungs.com/Blacklist";
        private static string _whiteListUrl = @"http://earth.tyleryoungs.com/Whitelist";
        private static string _serverDownloadLocation = @"http://earth.tyleryoungs.com/Pictures/";

        public static void Main(string[] args)
        {
            var imagesAlreadyDownloaded = GetImagesAlreadyDownloaded();
            AddWhitelistedImages(imagesAlreadyDownloaded);
            RemoveBlacklistedImages(imagesAlreadyDownloaded);
        }

        private static List<string> GetFileNamesFromSource(string url)
        {
            var client = new WebClient();
            var whiteListHtml = client.DownloadString(url);
            return whiteListHtml.Split(',').ToList();
        }

        private static List<string> GetImagesAlreadyDownloaded()
        {
            if (!Directory.Exists(_localDownloadLocation))
            {
                Directory.CreateDirectory(_localDownloadLocation);
            }

            return (new DirectoryInfo(_localDownloadLocation)).GetFiles("*.jpg").Select(x => x.Name).ToList();
        }

        private static void AddWhitelistedImages(List<string> imagesAlreadyDownloaded)
        {
            var whiteListNamesFromSource = GetFileNamesFromSource(_whiteListUrl);

            foreach (var name in whiteListNamesFromSource)
            {
                if (!imagesAlreadyDownloaded.Contains(name) && !string.IsNullOrEmpty(name))
                {
                    var localPath = Path.Combine(_localDownloadLocation, name);
                    var serverPath = Path.Combine(_serverDownloadLocation, name);
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            Console.WriteLine($"Downloading image {name}");
                            client.DownloadFile(new Uri(serverPath), localPath);
                        }
                    }
                    catch(Exception)
                    {
                        Console.WriteLine($"Error downloading {name}");
                    }
                }
            }
        }

        private static void RemoveBlacklistedImages(List<string> imagesAlreadyDownloaded)
        {
            var blackListNamesFromSource = GetFileNamesFromSource(_blacklistUrl);

            foreach (var name in blackListNamesFromSource)
            {
                if (imagesAlreadyDownloaded.Contains(name))
                {
                    var path = Path.Combine(_localDownloadLocation, name);
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Error deleting image {name} from {path}");
                    }
                }
            }
        }
    }
}
