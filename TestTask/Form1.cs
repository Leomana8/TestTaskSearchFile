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
using Forms = System.Windows.Forms;
using System.Windows.Forms;

namespace TestTask
{
    public partial class Form1 : Form
    {
        Term criteria;
        string saveFile = "term.xml";
        CancellationTokenSource cts;
        Stopwatch timer;
        Forms.Timer timeTick;
        SearchFiles searchingFile;

        public Form1()
        {
            InitializeComponent();           
            timer = new Stopwatch();
            timeTick = new Forms.Timer();
            timeTick.Tick += new System.EventHandler(this.timer_Tick);
            timeTick.Start();

            criteria = new Term();
            criteria.LoadTermFile(saveFile);
            tbFolder.Text = criteria.StartFolder;
            tbPattern.Text = criteria.Pattern;
            tbText.Text = criteria.Text;
        }
        
        private void bBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        public void GetTerm()
        {
            criteria.StartFolder = tbFolder.Text;
            criteria.Pattern = tbPattern.Text;
            criteria.Text = tbText.Text;
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            
            if (bOk.Text == "Стоп")
            {
                bOk.Text = "Поиск";
                cts.Cancel();
                timer.Stop();
            }
            else
            {
                timer.Reset();
                tbFolder.ForeColor = Color.Black;
                if ( !Directory.Exists(tbFolder.Text) )
                {
                    tbFolder.ForeColor = Color.Red;
                    return;
                }
                GetTerm();
                criteria.SaveTermFile(saveFile);
                treeView.NewTree( new DirectoryInfo(criteria.StartFolder) );
                cts = new CancellationTokenSource();
                Search();
                bOk.Text = "Стоп";
            }
        }
      
        private async void Search()
        {
            searchingFile = new SearchFiles();
            searchingFile.FileFound += new SearchFiles.FileFoundDelegate(FileFounded);
            searchingFile.AnyFileFound += new SearchFiles.FileFoundDelegate(CurFileFounded);
            timer.Start();
            await searchingFile.Find(criteria, cts.Token);
            if(searchingFile.CountFiles == 0)
                MessageBox.Show("Файлы не найдены");
            bOk.Text = "Поиск";
        }

        public void FileFounded(FileInfo file)
        {
            treeView.AddFile(file);
        }

        public void CurFileFounded(FileInfo file)
        {
            if(file != null)
                label_nameFile.Text = file.Name;
            label_count.Text = searchingFile.CountFiles.ToString();                 
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            label_time.Text = timer.Elapsed.ToString(@"hh\:mm\:ss"); 
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cts.Cancel();
            GetTerm();
            criteria.SaveTermFile(saveFile);
        }
    }
}
