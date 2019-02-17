using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Merg_Videos_GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach(var file in openFileDialog1.FileNames)
                {
                    lstFiles.Items.Add(file);
                    File.Copy(file, $"Temp\\{Path.GetFileName(file)}");
                }
            }
        }
        void runCmdCommand(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {command}";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.FileNames != null)
            {
                if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

                var outputPath = saveFileDialog1.FileName;
                string fn = "";
                string[] reolution = comboBox1.Text.Split('x');

                using (var stw = new StreamWriter("videos.txt"))
                    foreach(var fileName in openFileDialog1.FileNames)
                    {
                      fn = Path.GetFileName(fileName);
                        stw.WriteLine($"file 'Temp\\{Path.GetFileNameWithoutExtension(fn)}output{Path.GetExtension(fn)}'");
                        runCmdCommand($"ffmpeg -i Temp\\{fn}  -c:v libx264 -preset {comboBox2.Text}  -r 60 -c:a aac -ar 48000 -b:a 160k  Temp\\{Path.GetFileNameWithoutExtension(fn)}output{Path.GetExtension(fn)}");
                    }

                runCmdCommand($"ffmpeg -f concat -safe 0 -i videos.txt -c copy {outputPath}");

                Directory.Delete($"{AppDomain.CurrentDomain.BaseDirectory}\\Temp",true);
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}\\Temp");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 5;

            if(!File.Exists("ffmpeg.exe"))
                MessageBox.Show("FFMPEG Not found , you need to add that in root directory","Error");
        }
    }
}
