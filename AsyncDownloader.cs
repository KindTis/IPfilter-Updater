using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading;

namespace IpfilterUpdater
{
	public class AsyncDonwloader
	{
		public bool IsDownloadComplete { get; set; }

		private ProgressBar mProgressBar;
		private string mURL;
		private string mFileName;

		public AsyncDonwloader(string url, string filename)
		{
			mURL = url;
			mFileName = filename;
		}

		public void StartDownload()
		{
			IsDownloadComplete = false;
			mProgressBar = new ProgressBar();
			Thread thread = new Thread(() =>
			{
				using (var client = new WebClient())
				{
					try
					{
						client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
						client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
						client.DownloadFileAsync(new Uri(mURL), mFileName);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			});
			thread.Start();
		}
		private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			double progress = ((double)e.BytesReceived) / ((double)e.TotalBytesToReceive);
			mProgressBar.Report(progress);
		}
		private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			IsDownloadComplete = true;
		}
	}
}