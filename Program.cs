
using System;
using System.IO;

namespace ReleaseUpdater
{
    class Program
    {
        public const string JogiGamePath = "./Jogigame/";
        public const string JogiGameFileName = "JogisWayToStreamingGod.exe";
        public const string Username = "Scraxtastic";
        public const string GitRepo = "JogisWayToStreamingGod";
        public const string OldFolderName = "./Jogigame_old";
        public const string ZipFileName = "./JogiGameUpdate.zip";
        public const string ExtractedFolderName = "./JogiGame_update";
        public const string latestUrlFile = "./JogiLastDownloadedVersion.txt";


        static void Main(string[] args)
        {
            Console.WriteLine("Starting Updater...");

            ProjectUpdater projectUpdater = new ProjectUpdater(Username, GitRepo);
            string updateStatus = projectUpdater.DownloadAndReplace(JogiGamePath, OldFolderName, ZipFileName, ExtractedFolderName, latestUrlFile);

            Console.WriteLine(updateStatus);
            Console.WriteLine("Finished Updater.");
            Console.WriteLine("Starting Jogigame...");

            System.Diagnostics.Process.Start(Path.Combine(JogiGamePath, JogiGameFileName));
        }

    }
}
