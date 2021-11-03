
using System;

namespace ReleaseUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            ProjectUpdater projectUpdater = new ProjectUpdater("Scraxtastic", "JogisWayToStreamingGod");
            string updateStatus = projectUpdater.Download(@"./update.zip", @"./update", @"./lastDownloadUrl.txt");
            if (updateStatus == "latest")
            {
                Console.WriteLine("Didn't update...");
            }
            else if (updateStatus.Contains("exception"))
            {
                Console.WriteLine("This should not have happened.");
            }
            else
            {
                Console.WriteLine("Updated to: ", updateStatus);
            }
        }

    }
}
