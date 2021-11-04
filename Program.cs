
using System;

namespace ReleaseUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            string jogiGamePath = @"./Jogigame/";
            string jogiGameFileName = "JogisWayToStreamingGod.exe";
            Console.WriteLine("Starting Updater...");
            ProjectUpdater projectUpdater = new ProjectUpdater("Scraxtastic", "JogisWayToStreamingGod");
            string updateStatus = projectUpdater.DownloadAndReplace(jogiGamePath, @"./Jogigame_old",@"./JogiGameUpdate.zip", @"./JogiGame_update", @"./JogiLastDownloadedVersion.txt");
            Console.WriteLine(updateStatus);
            Console.WriteLine("Finished Updater.");
            Console.WriteLine("Starting Jogigame...");
            System.Diagnostics.Process.Start(jogiGamePath + jogiGameFileName);
        }

    }
}
