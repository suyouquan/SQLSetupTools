using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace SQLReg
{
    public partial class scanForm : Form
    {
        public static bool isCachedMode = false;
        public scanForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            InitProductList();
            InitCacheFiles();

        }
        private void InitCacheFiles()
        {
            List<string> cachedFiles = GetCachedFile();
            comboBox1.DataSource = cachedFiles;

            if (isCachedMode && cachedFiles.Count > 0)
            {
                txtSetupSrc.Enabled = btnBrowse.Enabled = false;
                radioButton1.Checked = false;
                radioButton2.Checked = true;
            }
            else
            {
                comboBox1.Enabled = false;
                txtSetupSrc.Enabled = btnBrowse.Enabled = true;
                radioButton1.Checked = true;
                radioButton2.Checked = false;


                if (cachedFiles.Count == 0)
                {

                    radioButton2.Enabled = false;

                }
            }
        }
        private void InitProductList()
        {

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = Path.Combine(path, "MetaData");
            if (!Directory.Exists(path))
            {
                Logger.LogMsg("[scanForm Init]Data folder doesn't exist:" + path);
                return;
            }
            string file = Path.Combine(path, "LatestSQLVersion.txt");
            if(!File.Exists(file))
            {
                Logger.LogMsg("[scanForm Init]File doesn't exist:" + file);
                return;
            }

            string[] content = File.ReadAllLines(file);
            foreach(string s in content)
            {
                if (string.IsNullOrEmpty(s)) continue;
                listBox1.Items.Add(s);
            }


            if (Controller.LastSQLSetupSource.Trim() != "") this.txtSetupSrc.Text = Controller.LastSQLSetupSource.Trim();

        }

        private List<string> GetCachedFile()
        {
            List<string> result = new List<string>();
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = Path.Combine(path, "Cache");
            if (!Directory.Exists(path))
            {
                Logger.LogMsg("[scanForm Init]Cache folder doesn't exist:" + path);
                return result;
            }

            string[] fileTypes = new string[] { "*.xml"};
            DirectoryInfo di = new DirectoryInfo(path);
            var xmlFiles = fileTypes.SelectMany(p => di.EnumerateFiles(p, SearchOption.AllDirectories)).OrderByDescending(fi=>fi.CreationTime);

          
            foreach (FileInfo f in xmlFiles)
            {
                try
                {


                    result.Add(Path.GetFileName(f.FullName));

                }
                catch (Exception ex)
                {
                    Logger.LogError("[GetCachedFile]:" + ex.Message);
                }

            }

            return result;


        }
        private void btnScan_Click(object sender, EventArgs e)
        {
            string folder = txtSetupSrc.Text.Trim();

            if (radioButton1.Checked == true)
            {
                if (folder == "")
                {
                    DialogResult r = MessageBox.Show
                        ("SQL setup media folder is not specified, the result could be incompleted.\nPlease click OK to Scan anyway, or Cancle to cancel this operation.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (r == DialogResult.OK)
                    {
                        //do nothing
                    }
                    else return;
                }
                else
                {
                    if (folder.Length <= 3)
                    {
                        DialogResult dr =
                            MessageBox.Show("The length of the SQL Setup media folder is too short. It should be at least 4 characters).", "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }
                    if (!Directory.Exists(folder))
                    {
                        DialogResult dr =
                        MessageBox.Show(folder + " doesnt exist!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }
                }


                //Now the folder is empty or valid folder
                Controller.cachedMetaDataFile = "";
                Controller.SQLSetupSource = folder;
             this.DialogResult = DialogResult.OK;
            }
            else //use cached file
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = Path.Combine(path, "Cache");

                Controller.cachedMetaDataFile =
                    Path.Combine(path, comboBox1.SelectedItem.ToString());
                //Now the folder is empty or valid folder
                Controller.SQLSetupSource =  "";
                this.DialogResult = DialogResult.OK;
            }
         
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new FolderBrowserDialog();
            string setupSource = "";
            while (true)
            {


                dialog.ShowNewFolderButton = false;
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //  MessageBox.Show("You selected: " + dialog.FileName);


                    if (dialog.SelectedPath.Length == 3)
                    {
                        DialogResult dr = MessageBox.Show("Root drive is not allowed, please choose a folder instead.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (dr == DialogResult.OK) continue;
                        else { dialog.SelectedPath = ""; break; }

                    }
                    else if (dialog.SelectedPath.Length > 3)
                    {
                        setupSource = dialog.SelectedPath; break;
                    }
                    else
                    {
                        setupSource = ""; break;
                    }
                }
                else { setupSource = ""; break; }
            } //while

            if (setupSource != "") txtSetupSrc.Text = setupSource;

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked==true)
            {
                txtSetupSrc.Enabled = true;
                btnBrowse.Enabled = true;
                comboBox1.Enabled = false;
                isCachedMode = false;
            }
            else
            {
                txtSetupSrc.Enabled = false;
                btnBrowse.Enabled = false;
                comboBox1.Enabled = true;
                isCachedMode = true;
            }


        }










    }
}
