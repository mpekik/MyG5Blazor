using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGConfigNDeviceTest
{
    public partial class Updater : Form
    {
        public Updater()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Updater frm1 = new Updater();
            label1.Left = (this.Size.Width - label1.Size.Width) / 2;
            label1.Top = (this.Size.Height - label1.Size.Height) / 2;
            await Task.Delay(1000);
            await CekUpdate();
        }
        private async Task CekUpdate()
        {
            await Task.Delay(1000);
            await DownloadUpdate();
        }
        private async Task DownloadUpdate()
        {
            await Task.Delay(1000);
            await DeleteOlder();
        }
        private async Task DeleteOlder()
        {
            await Task.Delay(1000);
            if (Directory.Exists(@"C:\MyGrapari\\Old"))
            {
                EmptyFolder(new DirectoryInfo(@"C:\MyGrapari\Old"));
            }
            await MoveOld();
        }
        private void EmptyFolder(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                EmptyFolder(subfolder);
            }
            Directory.Delete(directoryInfo.FullName);
        }
        private void MoveFile(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Name != "MyGConfigNDeviceTest.pdb" && file.Name != "MyGConfigNDeviceTest.exe.config" && file.Name != "MyGConfigNDeviceTest.exe" && file.Name!= "MyGUpdater.exe" && file.Name != "MyGApps.zip" && file.Name != "MyGUpdater.config")
                    File.Move(file.FullName, @"C:\MyGrapari\Old\" + file.Name);
            }
        }
        private async Task MoveOld()
        {
            await Task.Delay(1000);
            if(!Directory.Exists(@"C:\MyGrapari\Old"))
            {
                Directory.CreateDirectory(@"C:\MyGrapari\Old");
            }
            if (Directory.Exists(@"C:\MyGrapari\MyGApps"))
            {
                Directory.Move(@"C:\MyGrapari\MyGApps", @"C:\MyGrapari\Old\MyGApps");
            }
            if(Directory.Exists(@"C:\MyGrapari\wwwroot"))
            {
                Directory.Move(@"C:\MyGrapari\wwwroot", @"C:\MyGrapari\Old\wwwroot");         
            }
            MoveFile(new DirectoryInfo(@"C:\MyGrapari"));
            await ExtractUpdate();
        }
        private async Task ExtractUpdate()
        {
            await Task.Delay(1000);
            string zipPath = @"c:\MyGrapari\MyGApps.zip";
            string extractPath = @"c:\MyGrapari\";
            if(File.Exists(@"c:\MyGrapari\MyGApps.zip"))
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
            await RunApps();
        }
        
        private async Task RunApps()
        {
            await Task.Delay(1000);
            Process.Start(@"C:\MyGrapari\MyGApps\Run.vbs");
            this.Close();
        }
    }
}
