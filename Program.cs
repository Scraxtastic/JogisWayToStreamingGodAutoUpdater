
using System;

namespace ReleaseUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Updater...");
            ProjectUpdater projectUpdater = new ProjectUpdater("Scraxtastic", "JogisWayToStreamingGod");
            string updateStatus = projectUpdater.DownloadAndReplace(@"./Jogigame", @"./Jogigame_old",@"./JogiGameUpdate.zip", @"./JogiGame_update", @"./JogiLastDownloadedVersion.txt");
            Console.WriteLine(updateStatus);
            Console.WriteLine("Finished Updater.");
        }

    }
}
