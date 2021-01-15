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
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Management.Automation;

namespace CheckIsDigitallySignature
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        public void WriteToFile(string fileName, string status,string outputfileLocation)
        {
            outputfileLocation = outputfileLocation + @"/output.txt";
            if (!File.Exists(outputfileLocation))
            {
                File.Create(outputfileLocation).Dispose();          

            }
            
            File.AppendAllText(outputfileLocation, fileName+  " it's status " + status +Environment.NewLine);            

        }

        public void Process(string folderPath)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);//Assuming Test is your Folder
            string combine = "";
            if (radioButton1.Checked)
            {
                combine = "*.exe";
            }
            else {
                combine = "*.dll";
            }
            FileInfo[] Files = d.GetFiles(combine, SearchOption.AllDirectories); //Getting Text files
            string str = "";
            foreach (FileInfo file in Files)
            {
                if (alreadySigned(file.FullName))
                {
                   // WriteToFile(file.Name, "already signed ", outputpath);
                }
                else {
                    WriteToFile(file.Name, " it's not  signed ", outputpath);
                }
            }
        }
        string outputpath = "";
        string inputPath = "";
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                inputPath = folderDlg.SelectedPath;
                label1.Text = inputPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                outputpath = folderDlg.SelectedPath;
                label2.Text = outputpath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private static Boolean alreadySigned(string file)
        {
            try
            {
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                runspace.Open();

                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript("Get-AuthenticodeSignature \"" + file + "\"");

                Collection<PSObject> results = pipeline.Invoke();
                runspace.Close();
                Signature signature = results[0].BaseObject as Signature;
                return signature == null ? false : (signature.Status != SignatureStatus.NotSigned);
            }
            catch (Exception e)
            {
                throw new Exception("Error when trying to check if file is signed:" + file + " --> " + e.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process(inputPath);
            MessageBox.Show("It's completed,please check in output directory");
        }
    }
}
