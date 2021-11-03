using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace ReleaseUpdater
{
    class ProjectUpdater
    {

        public bool DownloadCompleted { get; private set; } = false;
        public bool ShallLog { get; set; } = true;
        public string WebsitePattern { get; } = @"<a href=""\/login\?return_to=(.*)""";
        public string FilePattern { get; } = @"<a href=""(.+\.zip)"" rel=""nofollow"">";

        private bool _shallMoveUp = false;
        private bool _firstRun = true;
        private bool _shallFinish = false;
        private object _locker = new object();
        private string _currentMessage = "";

        private string _username;
        private string _repo;


        /**
         * Since i didn't find a way to anonymously use the github api, i go to the latest release website and get the link
         * of the downloadable zip file.....
         * I would appreciate a PR with a better way. 
         */
        public ProjectUpdater(string username, string repo)
        {
            _username = username;
            _repo = repo;
        }
        private string GetNewestReleaseForProject(string latestUrlFile)
        {
            string latestContent = GetUri($"https://github.com/{_username}/{_repo}/releases/latest");
            Match latestWebsiteMatch = Regex.Match(latestContent, WebsitePattern, RegexOptions.Multiline);
            string latestUrl = latestWebsiteMatch.Groups[1].Value;
            string latestUrlDecoded = HttpUtility.UrlDecode(latestUrl);
            if (File.Exists(latestUrlFile) && File.ReadAllText(latestUrlFile) == latestUrlDecoded)
            {
                return "latest";
            }
            else
            {
                File.WriteAllText(latestUrlFile, latestUrlDecoded);
            }
            string latestUrlContent = GetUri(latestUrlDecoded);
            Match fileMatch = Regex.Match(latestUrlContent, FilePattern);
            string fileDownloadUrl = "https://github.com/" + fileMatch.Groups[1].Value;
            return fileDownloadUrl;
        }

        /**
         * Returns true, if download is 
         * Returns false, if download failed or file is already up2date
         * 
         */
        public string Download(string zipFileName, string extractedFolderName, string latestUrlFile)
        {
            try
            {
                string fileDownloadUrl = GetNewestReleaseForProject(latestUrlFile);
                if (fileDownloadUrl == "latest")
                {
                    return "latest";
                }
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Loading);
                webClient.DownloadFileAsync(new Uri(fileDownloadUrl), zipFileName);
                WaitForDownloadCompletion();
                ReplaceFiles(zipFileName, extractedFolderName);
                return "updated";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
        private void ReplaceFiles(string zipFileName, string extractedFolderName)
        {
            Console.WriteLine("extractedfolder: ", extractedFolderName);
            if (Directory.Exists(extractedFolderName))
                Directory.Delete(extractedFolderName, true);
            ZipFile.ExtractToDirectory(zipFileName, extractedFolderName);
        }
        private void MoveConsoleCursorUp()
        {
            if (Console.CursorTop > 0)
                Console.CursorTop--;
            Console.WriteLine();
            Console.CursorTop--;
            _shallMoveUp = false;
        }
        private void WaitForDownloadCompletion()
        {
            while (!DownloadCompleted)
            {
                if (ShallLog)
                {
                    lock (_locker)
                    {
                        if (_currentMessage != "")
                        {
                            if (_shallMoveUp)
                                MoveConsoleCursorUp();
                            Console.WriteLine(_currentMessage);
                            _currentMessage = "";
                        }
                    }
                    Thread.Sleep(100);
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
        }
        public string GetUri(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            while (!_shallFinish)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(100);
            lock (_locker)
            {
                if (e.Cancelled)
                {
                    _currentMessage = string.Format("Something went wrong...", e.Error);
                }
                else
                {
                    _currentMessage = "Download completed!";
                }
                DownloadCompleted = true;
            }
        }
        private void Loading(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (_locker)
            {
                if (_firstRun)
                    _firstRun = false;
                else
                {
                    _shallMoveUp = true;
                }
                long receivedBytes = e.BytesReceived;
                long totalBytes = e.TotalBytesToReceive;
                long percentage = (receivedBytes * 100 / totalBytes);
                if (percentage == 100 && receivedBytes == totalBytes)
                {
                    _shallFinish = true;
                }
                _currentMessage = string.Format("{0}/{1}Kb ({2}%)", receivedBytes / 1000, totalBytes / 1000, percentage);
            }
        }
    }
}
