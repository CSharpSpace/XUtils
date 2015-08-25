using System;
using System.Collections.Generic;
using XUtils.Net.FTP;
namespace FTPTest
{
	internal class Program
	{
		private static long size;
		private static void Main(string[] args)
		{
			try
			{
				string hostname = "ftp.symantec.com";
				int port = 21;
				string username = "anonymous";
				string password = "anonymous@gmail.com";
				FTP fTP = new FTP();
				fTP.ConnectionMode = ConnectionMode.Passive;
				fTP.OnServerResponse += new ServerResponseEventHendler(Program.ftp_OnServerResponse);
				fTP.OnClientCommand += new ClientCommandEventHendler(Program.ftp1_OnClientCommand);
				fTP.OnSpeed += new SpeedEventHendler(Program.home_OnSpeed);
				fTP.OnFileTransfer += new FileTransferEventHendler(Program.home_OnFileTransfer);
				fTP.Open(hostname, port);
				fTP.Login(username, password);
				List<ContentItemInformation> list = new List<ContentItemInformation>();
				fTP.GetDirectoryContent("/", ref list);
				foreach (ContentItemInformation current in list)
				{
					Console.WriteLine("{4,-10} {0,-20} {1,-20} {2,10} {3}", new object[]
					{
						current.Created,
						current.LastChange,
						current.Size,
						current.Name,
						current.ItemType
					});
				}
				fTP.LogOut();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		private static void home_OnFileTransfer(int totalTransferedBytes)
		{
		}
		private static void home_OnSpeed(FTP ftp)
		{
			Console.Title = ftp.Speed(SpeedPerSecond.KiB).ToString();
		}
		private static void ftp1_OnClientCommand(string command)
		{
			Console.Write(command);
		}
		private static void ftp_OnServerResponse(string response)
		{
			Console.WriteLine(response);
		}
	}
}
