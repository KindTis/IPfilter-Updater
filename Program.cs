using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpfilterUpdater
{
	class Program
	{
		static void Main(string[] args)
		{
			string url = "http://upd.emule-security.org/ipfilter.zip";
			string fileName = "ipfilter.zip";

			IfExistingDelete(fileName);

			AsyncDonwloader downloader = new AsyncDonwloader(url, fileName);

			Console.WriteLine("Start Download: " + url);
			downloader.StartDownload();
			while (!downloader.IsDownloadComplete)
			{
				Thread.Sleep(500);
			}
			Console.WriteLine();
			Console.WriteLine("Download Complete: " + fileName);

			string bandizipDir = _GetBandizipPath();
			if (bandizipDir.Length == 0)
			{
				Console.WriteLine("Please Install Bandizip First");
				return;
			}

			IfExistingDelete("guarding.p2p");
			IfExistingDelete("ipfilter.dat");

			Console.WriteLine("Decompressing: ipfilter.zip");
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.CreateNoWindow = false;
			startInfo.UseShellExecute = false;
			startInfo.FileName = bandizipDir + "Bandizip.exe";
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.Arguments = "x ipfilter.zip";
			using (Process exeProcess = Process.Start(startInfo))
			{
				exeProcess.WaitForExit();
			}
			File.Move("guarding.p2p", "ipfilter.dat");
			Console.WriteLine("Setting Up Complete: ipfilter.dat");
			return;
		}

		static void IfExistingDelete(string fileName)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
				Console.WriteLine("Delete Previous " + fileName);
			}
		}

		static string _GetBandizipPath()
		{
			RegistryKey myKey = null;
			if (Environment.Is64BitOperatingSystem)
			{
				myKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			}
			else
			{
				myKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			}
			if (myKey == null)
			{
				Console.WriteLine("Registry Access Error!!");
			}
			RegistryKey bandizipKey = myKey.OpenSubKey("SOFTWARE\\Bandizip");
			if (bandizipKey == null)
			{
				return "";
			}
			return (string)bandizipKey.GetValue("ProgramFolder");
		}
	}
}
