using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using AmaroK86.MassEffect3;


namespace ME3Patch
{
    public partial class Form1 : Form
    {
        DLCBase dlcBase = null;
        DLCEditor dlcEditor = null;
        int std = 1;
        int stdd;
        public static bool bgm;
        [DllImport("user32.dll")]
        public static extern int ShowScrollBar(IntPtr hWnd, int iBar, int bShow);

        const int SB_HORZ = 0;
        const int SB_VERT = 1;



        protected override void WndProc(ref   Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                //   User   clicked   close   button   
                return;
            }
            ShowScrollBar(m.HWnd, SB_HORZ, 0);
            base.WndProc(ref   m);
        }
        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = listView1.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }
        public Form1()
        {
            InitializeComponent();
            this.Text = String.Format("质量效应3玩家汉化安装程序  版本：{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("将退出汉化，是否继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                File.Delete(System.IO.Path.GetTempPath() + "bgm1.wma");
                File.Delete(System.IO.Path.GetTempPath() + "bgm2.wma");
                fun.logger("用户选择退出程序", false, System.Windows.Forms.Application.StartupPath);
                fun.dellog(System.Windows.Forms.Application.StartupPath + "\\zh-installlog.log", Program.switcher);
                this.Close();
                System.Environment.Exit(System.Environment.ExitCode);
            } 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                       
            if (Program.flag)
                System.Environment.Exit(System.Environment.ExitCode);
            for (int i = 0; i < Program.filecode.Count+1; i++)
            {
                if (i < 4)
                    this.listView1.Items.Add(fun.codetrans(i), fun.codetrans(i), 0);
                else if(i==4)
                    this.listView1.Items.Add("DLC", "DLC", 0);
                else
                    this.listView1.Items.Add(fun.codetrans(i-1), fun.codetrans(i-1), 0);
            }
            fun.Play(std);
            fun.logger("启动检测线程", false, System.Windows.Forms.Application.StartupPath);
            backgroundWorker1.RunWorkerAsync();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Form1.bgm)
            {
                fun.Pause(std);
                this.button1.Image = global::ME3Patch.Properties.Resources.stop;
                
            }
            else
            {
                fun.Play(std);
                this.button1.Image = global::ME3Patch.Properties.Resources.play;
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Program.fristrun = true;
            AboutBox1 fm2 = new AboutBox1();
            fm2.ShowDialog();
        }
        #region 检测线程
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           StartPosition: string temp = System.IO.Path.GetTempPath();
            fun.SaveRecordFile(temp, "bgm2", ".wma");
            string path; int status;
            fun.logger("开始校验文件", false, System.Windows.Forms.Application.StartupPath);this.Invoke(new MethodInvoker(delegate() 
                    {this.label6.Hide();}));
            #region MD5计算
            for (int FileID = 0; FileID < Program.filecode.Count+1; FileID++)
            {
                this.Invoke(new MethodInvoker(delegate() 
                    {
                        
                    this.progressBar1.Value = 0; this.progressBar2.Maximum = Program.filecode.Count+1;
                    if (FileID < 4)
                    {
                        listView1.Items[fun.codetrans(FileID)].SubItems.Add("校验中...");
                        this.listView1.Items[FileID].UseItemStyleForSubItems = false;
                        listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                    }
                    else if (FileID > 4)
                    {
                        listView1.Items[fun.codetrans(FileID - 1)].SubItems.Add("校验中...");
                        listView1.Items[FileID].EnsureVisible();
                        this.listView1.Items[FileID].UseItemStyleForSubItems = false;
                        listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                    }
                    Application.DoEvents(); 
                    }));
                if (FileID == 4)
                    continue;
                if(FileID<4)
                    path = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(FileID)] + IniFiles.ini.INIGetStringValue(Program.IniPath,Convert.ToString(FileID),"Filename",null);
                else path = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(FileID-1)]+"Default.sfar";
                if (!File.Exists(path))
                {
                    if (FileID < 4)
                    {
                        this.Invoke(new MethodInvoker(delegate()
                   {
                       listView1.Items[FileID].SubItems[1].Text = "核心文件缺失，修复中...";
                       this.listView1.Items[FileID].UseItemStyleForSubItems = false;
                       listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                       Application.DoEvents();
                   }));
                        fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), null, 1, FileID);
                        this.Invoke(new MethodInvoker(delegate()
                   {
                       listView1.Items[FileID].SubItems[1].Text = " 重新校验中...";
                       Application.DoEvents();
                   }));
                    }
                    else
                    {
                        Program.filehash.Add(Convert.ToString(FileID), "");
                        this.Invoke(new MethodInvoker(delegate() { this.progressBar2.Value = FileID; this.label4.Text = Convert.ToString(Math.Ceiling((FileID + 1) / Convert.ToDouble(Program.filecode.Count) * 100)) + "%"; Application.DoEvents(); }));
                        if (FileID > 4)
                        {
                            Program.filestatus.Add(Convert.ToString(FileID - 1), Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(FileID)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile)));
                            status = Convert.ToInt32(Program.filestatus[Convert.ToString(FileID - 1)].ToString());
                        }
                        else
                        {
                            Program.filestatus.Add(Convert.ToString(FileID), Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(FileID)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile)));
                            status = Convert.ToInt32(Program.filestatus[Convert.ToString(FileID)].ToString());
                        }
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            if (FileID != 4)
                            {
                                listView1.Items[FileID].SubItems[1].Text = fun.codestatustrans(status);
                                this.listView1.Items[FileID].UseItemStyleForSubItems = false;
                                if (status == 1 || status == 6)
                                    listView1.Items[FileID].SubItems[1].ForeColor = Color.Black;
                                else if (status == 2 || status == 4)
                                    listView1.Items[FileID].SubItems[1].ForeColor = Color.Green;
                                else if (status == 3 || status == 5)
                                    listView1.Items[FileID].SubItems[1].ForeColor = Color.Brown;
                                else listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                            }
                        }));
                        continue;
                    }
                }
                Program.filehash.Add(Convert.ToString(FileID),fun.AnyscMD5(path,progressBar1,label3));
                #endregion
                if (FileID > 4)
                {
                    Program.filestatus.Add(Convert.ToString(FileID - 1), Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(FileID)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile)));
                    status = Convert.ToInt32(Program.filestatus[Convert.ToString(FileID-1)].ToString());
                    #region DLC修复
                    if (status == 8)
                    {
                        for (int t = 1; t < 4; t++)
                        {
                            if (status != 8)
                                break;
                            fun.logger("文件" + Convert.ToString(FileID) + "损坏，第" + Convert.ToString(t) + "次尝试修复中...", false, System.Windows.Forms.Application.StartupPath);
                            this.Invoke(new MethodInvoker(delegate()
                            {
                                listView1.Items[FileID].SubItems[1].Text = "损坏，第" + Convert.ToString(t) + "次尝试修复中...";
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                                progressBar1.Maximum = 100;
                                Application.DoEvents();
                            }));
                            dlcBase = new DLCBase(path);
                            dlcEditor = new DLCEditor(dlcBase);
                            string file = IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(FileID-1), "FileName", null);
                            string[] arr1=file.Split(';');
                            foreach (string replacefile in arr1)
                            {
                                fun.fileupdate(Path.GetDirectoryName(path), replacefile, null, -1, FileID-1);
                                dlcEditor.setReplaceFile(Program.filepath[Convert.ToString(FileID-1)].ToString().Replace('\\','/')+replacefile, Path.GetDirectoryName(path)+"\\"+replacefile);
                            }
                            string oldSfar = dlcBase.fileName;
                            string newSfar = oldSfar + ".tmp";
                            backgroundWorkerEditFile.RunWorkerAsync(new object[1] { newSfar });
                            while (backgroundWorkerEditFile.IsBusy)
                            {
                                // Keep UI messages moving, so the form remains 
                                // responsive during the asynchronous operation.
                                if (backgroundWorkerEditFile.CancellationPending)
                                    return;
                                else
                                    Application.DoEvents();
                            }
                            File.Delete(oldSfar);
                            foreach (string replacefile in arr1)
                            {
                                File.Delete(Path.GetDirectoryName(path) + "\\" + replacefile);
                            }
                            File.Move(newSfar, oldSfar);
                            this.Invoke(new MethodInvoker(delegate()
                            {
                                listView1.Items[FileID].SubItems[1].Text = "重新校验中...";
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                                Application.DoEvents();
                            }));
                            Program.filehash[Convert.ToString(FileID)] = fun.AnyscMD5(path, progressBar1, label3);
                            Program.filestatus[Convert.ToString(FileID-1)] = Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(FileID)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile));
                            status = Convert.ToInt32(Program.filestatus[Convert.ToString(FileID-1)].ToString());

                        }
                        if (status == 8)
                        {
                            MessageBox.Show("修复失败，请将位于" + System.Windows.Forms.Application.StartupPath + "\\zh-installlog.log 这一日志文件发送到clfqsa@gmail.com,便于确定问题所在。感谢支持！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            System.Environment.Exit(0);
                        }
                    }
                    #endregion
                    #region DLC升级
                    if (status == 3 || status == 5||((status==1||status==6)&&(Convert.ToInt32(Program.filestatus["1"].ToString())!=1)))
                    {
                        fun.logger("开始升级文件" + Convert.ToString(FileID), false, System.Windows.Forms.Application.StartupPath);
                        if (!Program.updst)
                        {
                            if (bgm)
                            {
                                fun.Pause(std);
                                std = 2;
                                fun.Play(std);
                            }
                            else std = 2;
                        }                 
                        Program.updst = true;
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            listView1.Items[FileID].SubItems[1].Text = "升级中...";
                            listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                            progressBar1.Maximum = 100;
                            progressBar1.Value = 0;
                            Application.DoEvents();
                        }));
                        
                        dlcBase = new DLCBase(path);
                        dlcEditor = new DLCEditor(dlcBase);
                        string file = IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(FileID - 1), "FileName", null);
                        string[] arr1 = file.Split(';');
                        foreach (string replacefile in arr1)
                        {
                            fun.fileupdate(Path.GetDirectoryName(path), replacefile, IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(FileID - 1), "Type", null), Convert.ToInt32(Program.filestatus["1"].ToString()), FileID - 1);
                            dlcEditor.setReplaceFile(Program.filepath[Convert.ToString(FileID - 1)].ToString().Replace('\\', '/') + replacefile, Path.GetDirectoryName(path) + "\\" + replacefile);
                        }
                        string oldSfar = dlcBase.fileName;
                        string newSfar = oldSfar + ".tmp";
                        backgroundWorkerEditFile.RunWorkerAsync(new object[1] { newSfar });
                        while (backgroundWorkerEditFile.IsBusy)
                        {
                            // Keep UI messages moving, so the form remains 
                            // responsive during the asynchronous operation.
                            if (backgroundWorkerEditFile.CancellationPending)
                                return;
                            else
                                Application.DoEvents();
                        }
                        File.Delete(oldSfar);
                        foreach (string replacefile in arr1)
                        {
                            File.Delete(Path.GetDirectoryName(path) + "\\" + replacefile);
                        }
                        File.Move(newSfar, oldSfar);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            listView1.Items[FileID].SubItems[1].Text = "重新校验中...";
                            listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                            Application.DoEvents();
                        }));
                        Program.filehash[Convert.ToString(FileID)] = fun.AnyscMD5(path, progressBar1, label3);
                        Program.filestatus[Convert.ToString(FileID - 1)] = Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(FileID)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile));
                        status = Convert.ToInt32(Program.filestatus[Convert.ToString(FileID - 1)].ToString());
                    }
                    #endregion
                    
                }
                else
                {
                    Program.filestatus.Add(Convert.ToString(FileID), Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(FileID)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile)));
                    status = Convert.ToInt32(Program.filestatus[Convert.ToString(FileID)].ToString());
                    #region 核心文件修复
                    if (status == 8 && FileID < 4)
                    {
                        for (int t = 1; t < 4; t++)
                        {
                            
                            if (status != 8)
                                break;
                            fun.logger("文件" + Convert.ToString(FileID) + "损坏，第" + Convert.ToString(t) + "次尝试修复中...", false, System.Windows.Forms.Application.StartupPath);
                            this.Invoke(new MethodInvoker(delegate()
                                {
                                    listView1.Items[FileID].SubItems[1].Text = "损坏，第"+Convert.ToString(t)+"次尝试修复中...";
                                    listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                                    Application.DoEvents();
                                }));
                            fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), null, 1, FileID);
                            this.Invoke(new MethodInvoker(delegate()
                            {
                                listView1.Items[FileID].SubItems[1].Text = "重新校验中...";
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                                Application.DoEvents();
                            }));
                            Program.filehash[Convert.ToString(FileID)] = fun.AnyscMD5(path, progressBar1, label3);
                            Program.filestatus[Convert.ToString(FileID)] = Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(FileID)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile));
                            status = Convert.ToInt32(Program.filestatus[Convert.ToString(FileID)].ToString());

                        }
                        if (status == 8)
                        {
                            MessageBox.Show("修复失败，请将位于" + System.Windows.Forms.Application.StartupPath + "\\zh-installlog.log 这一日志文件发送到clfqsa@gmail.com,便于确定问题所在。感谢支持！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            System.Environment.Exit(0);
                        }
                    }
                    #endregion
                    #region 核心文件升级
                    if (status == 3 || status == 5)
                    {
                        fun.logger("开始升级文件" + Convert.ToString(FileID), false, System.Windows.Forms.Application.StartupPath);
                        Program.updst = true;
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            listView1.Items[FileID].SubItems[1].Text = "升级中...";
                            listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                            Application.DoEvents();
                        }));
                        if (FileID < 3)
                            fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(FileID), "Type", null), status, FileID);
                        else
                        {
                            fun.fileupdate(Path.GetDirectoryName(path), "Data.pat","COM", status, FileID);
                            VPatch4cs.DoPatch.Patch(Path.GetDirectoryName(path) + "\\Data.pat", path, Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".tmp", progressBar1, label3, FileID);
                            File.Delete(path);
                            File.Move(Path.GetFileNameWithoutExtension(path) + ".tmp", path);
                        }
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            listView1.Items[FileID].SubItems[1].Text = "重新校验中...";
                            listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                            Application.DoEvents();
                        }));
                        Program.filehash[Convert.ToString(FileID)] = fun.AnyscMD5(path, progressBar1, label3);
                        Program.filestatus[Convert.ToString(FileID)] = Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(FileID)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile));
                        status = Convert.ToInt32(Program.filestatus[Convert.ToString(FileID)].ToString());
                    }
                    #endregion
                   
                }

                this.Invoke(new MethodInvoker(delegate() 
                    {
                        if (FileID > 4)
                        {
                            listView1.Items[FileID].SubItems[1].Text = fun.codestatustrans(status);
                            this.listView1.Items[FileID].UseItemStyleForSubItems = false;
                            if (status == 1 || status == 6)
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Black;
                            else if (status == 2 || status == 4)
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Green;
                            else if (status == 3 || status == 5)
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Brown;
                            else listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                        }
                        else if (FileID < 4)
                        {
                            listView1.Items[FileID].SubItems[1].Text = fun.codestatustrans(status);
                            this.listView1.Items[FileID].UseItemStyleForSubItems = false;
                            if (status == 1 || status == 6)
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Black;
                            else if (status == 2 || status == 4)
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Green;
                            else if (status == 3 || status == 5)
                                listView1.Items[FileID].SubItems[1].ForeColor = Color.Brown;
                            else listView1.Items[FileID].SubItems[1].ForeColor = Color.Red;
                            if (status == 1 && FileID == 1)
                            {
                                this.button2.Text = "开始汉化(&S)";
                                this.radioButton1.Checked = true;
                            }
                            else if ((status == 2 || status == 4) && FileID == 1)
                            {
                                this.button2.Text = "开始还原(&S)";
                                if (status == 2)
                                    this.radioButton1.Checked = true;
                                if (status == 4)
                                    this.radioButton2.Checked = true;
                                this.groupBox3.Enabled = false;
                            }
                        }
                        this.progressBar2.Value = FileID ; 
                        this.label4.Text = Convert.ToString(Math.Ceiling((FileID) / Convert.ToDouble(Program.filecode.Count) * 100)) + "%"; 
                        Application.DoEvents(); 
                    }));

            }
               
            #region 输出文件状态信息
            StreamWriter sw;
            sw = File.AppendText(System.Windows.Forms.Application.StartupPath + "\\zh-installlog.log");
            sw.WriteLine(DateTime.Now + " 文件状态已确认，内部信息代码如下：");
            for (int i = 0; i < Program.filestatus.Count; i++)
            {
                sw.WriteLine("                   " + "文件：" + Convert.ToString(i) + "  " + "状态：" + Program.filestatus[Convert.ToString(i)].ToString());
            }
            sw.Close();
            #endregion

        }

        
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Program.updst)
            {
                MessageBox.Show("已成功升级为汉化" + "V" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "版！我是薛帕德指挥官，这是我在神堡最喜欢的汉化版本！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fun.logger("升级结束，程序退出", false, System.Windows.Forms.Application.StartupPath);
                System.Environment.Exit(0);
            }
            else this.Invoke(new MethodInvoker(delegate() { this.groupBox2.Visible = false; this.groupBox4.Visible = true; this.groupBox3.Visible = true;  Application.DoEvents(); }));
            
        }
        #endregion
        #region DLC处理线程
        private void backgroundWorkerEditFile_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            //extracting arguments
            object[] args = e.Argument as object[];

            string newSfar = (string)args[0];
            dlcEditor.Execute(newSfar, worker);
        }

        private void backgroundWorkerEditFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
                    {
                        progressBar1.Maximum = 100;
                        progressBar1.Value = e.ProgressPercentage;
                        label3.Text = Convert.ToString(e.ProgressPercentage) + "%";
                    }));
        }
        #endregion 

        private void button2_Click(object sender, EventArgs e)
        {
            string verion="",opt="";
            if (this.radioButton1.Checked)
            {
                verion = this.radioButton1.Text;
                opt = "安装";
                Program.ChoseVer = 2;
            }
            if (this.radioButton2.Checked)
            {
                verion = this.radioButton2.Text;
                opt = "安装";
                Program.ChoseVer = 4;
            }
            if (this.groupBox3.Enabled == false)
            {
                Program.ChoseVer = -1;
                opt = "卸载";
                
            }

            if (MessageBox.Show("您选择"+opt+"玩家汉化"+verion+",确认请单击“确定”，如想更换或退出，请单击“取消”。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                if (bgm)
                {
                    fun.Pause(std);
                    std = 2;
                    fun.Play(std);
                }
                else std = 2;
                this.groupBox3.Enabled = false;
                this.progressBar2.Maximum = Program.filestatus.Count;
                this.progressBar2.Value = 0;
                this.progressBar1.Value = 0;
                this.label3.Text = "";
                this.label4.Text = "";
                this.groupBox2.Visible = true; this.groupBox4.Visible = false;
                this.backgroundWorker2.RunWorkerAsync();
            }

        }
        #region 安装线程
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            fun.logger("辅助线程启动", false, System.Windows.Forms.Application.StartupPath);
            int i, status, updatecode, recode;
            for (i = 0; i < Program.filestatus.Count; i++)
            {
                status = Convert.ToInt32(Program.filestatus[Convert.ToString(i)].ToString());
                fun.logger("开始处理文件#" + Convert.ToString(i), false, System.Windows.Forms.Application.StartupPath);
                string path = "";
                if (i < 4)
                    path = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(i)] + IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Filename", null);
                else path = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(i)] + "Default.sfar";
                if (i < 4)
                {
                    if (listView1.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i].SubItems[1].ForeColor = Color.Red; }));
                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i].SubItems[1].Text = "处理中..."; this.progressBar2.Maximum = 100; progressBar1.Maximum = 100; }));
                    }
                    if (i > 1)
                    {
                        if (status == 1)
                        {
                            fun.fileupdate(Path.GetDirectoryName(path), "Data.pat", "COM", status, i);
                            recode = VPatch4cs.DoPatch.Patch(Path.GetDirectoryName(path) + "\\Data.pat", path, Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".tmp", progressBar1, label3, i);
                            if (recode == 0)
                            {
                                updatecode = Program.ChoseVer;
                            }
                            else updatecode = recode;
                        }
                        else updatecode = fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                    }
                    else
                    {
                        recode=fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                         if (recode == 1)
                         {
                             updatecode = Program.ChoseVer;
                         }
                         else updatecode = recode;
                    }
                    if (listView1.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i].SubItems[1].Text = fun.fileupdatcode(updatecode); listView1.Items[i].EnsureVisible(); }));
                    }

                    if (updatecode == 1 || updatecode == -1||updatecode==2||updatecode==4)
                    {
                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i].SubItems[1].ForeColor = Color.Green; }));
                    }
                    else
                    {
                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i].SubItems[1].ForeColor = Color.Red; }));
                    }
                }
                else
                {
                    if (listView1.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i + 1].SubItems[1].ForeColor = Color.Red; }));
                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i + 1].SubItems[1].Text = "处理中..."; listView1.Items[i + 1].EnsureVisible(); }));
                    }
                    if (status >=7)
                    {

                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i + 1].SubItems[1].Text = fun.fileupdatcode(status); listView1.Items[i + 1].EnsureVisible(); }));
                        this.backgroundWorker2.ReportProgress(Convert.ToInt32(Math.Ceiling((((double)(i + 1) / (double)Program.filestatus.Count)) * 100)));
                        fun.logger("处理完成，结果：" + fun.fileupdatcode(status), false, System.Windows.Forms.Application.StartupPath);
                        continue;

                    }
                    else
                    {
                        if (Program.ChoseVer==-1)
                        {
                            fun.fileupdate(Path.GetDirectoryName(path), "Data.pat", IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                            recode = VPatch4cs.DoPatch.Patch(Path.GetDirectoryName(path) + "\\Data.pat", path, Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".tmp", progressBar1, label3,i+1);
                            if (recode == 0)
                                updatecode = 1;
                            else updatecode = recode;
                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(delegate() { this.progressBar1.Value = 0; this.label3.Text = "0%"; }));
                            dlcBase = new DLCBase(path);
                            dlcEditor = new DLCEditor(dlcBase);
                            string file = IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "FileName", null);
                            string[] arr1 = file.Split(';');
                            foreach (string replacefile in arr1)
                            {
                                fun.fileupdate(Path.GetDirectoryName(path), replacefile, IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                                dlcEditor.setReplaceFile(Program.filepath[Convert.ToString(i)].ToString().Replace('\\', '/') + replacefile, Path.GetDirectoryName(path) + "\\" + replacefile);
                            }
                            string oldSfar = dlcBase.fileName;
                            string newSfar = oldSfar + ".tmp";
                            FileInfo aa = new FileInfo(oldSfar);
                            if ((aa.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                File.SetAttributes(oldSfar, FileAttributes.Normal);
                            }
                            backgroundWorkerEditFile.RunWorkerAsync(new object[1] { newSfar });
                            while (backgroundWorkerEditFile.IsBusy)
                            {
                                // Keep UI messages moving, so the form remains 
                                // responsive during the asynchronous operation.
                                if (backgroundWorkerEditFile.CancellationPending)
                                    return;
                                else
                                    Application.DoEvents();
                            }
                            File.Delete(oldSfar);
                            foreach (string replacefile in arr1)
                            {
                                File.Delete(Path.GetDirectoryName(path)+"\\"+replacefile);
                            }
                            File.Move(newSfar, oldSfar);
                            updatecode = 2;
                        }
                        this.Invoke(new MethodInvoker(delegate() { listView1.Items[i + 1].SubItems[1].Text = fun.fileupdatcode(updatecode); }));
                        if (updatecode == 1 || updatecode == 2)
                        {
                            this.Invoke(new MethodInvoker(delegate() { listView1.Items[i + 1].SubItems[1].ForeColor = Color.Green; listView1.Items[i + 1].EnsureVisible(); }));
                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(delegate() { listView1.Items[i + 1].SubItems[1].ForeColor = Color.Red; listView1.Items[i + 1].EnsureVisible(); }));
                        }
                    }
                }

                fun.logger("处理完成，结果：" + fun.fileupdatcode(updatecode), false, System.Windows.Forms.Application.StartupPath);

                stdd = status;
                this.backgroundWorker2.ReportProgress(Convert.ToInt32(Math.Ceiling((((double)(i+1)/(double)Program.filestatus.Count))*100)));
            }
        }
        #endregion

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.progressBar2.Value != this.progressBar2.Maximum)
            {
                MessageBox.Show("汉化操作异常中止，请将位于" + System.Windows.Forms.Application.StartupPath + "\\zh-installlog.log 这一日志文件发送到clfqsa@gmail.com，以便确定问题所在。感谢支持！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                System.Environment.Exit(System.Environment.ExitCode);
            }
            string verion, reverion;
            if (stdd == 1)
            {
                verion = "玩家汉化版";
                reverion = "还原为英文版";
            }
            else
            {
                verion = "英文版";
                reverion = "再次汉化";
            }
            if (MessageBox.Show("操作成功,请问您是否需要创建桌面快捷方式？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "质量效应3玩家汉化版.lnk") || File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "质量效应3英文版.lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "质量效应3玩家汉化版.lnk");
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "质量效应3英文版.lnk");
                }
                fun.link(verion);
                COMRECT.SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
                MessageBox.Show("我是薛帕德指挥官，这是我在神堡最喜欢的汉化版本！如需要" + reverion + "，请重新执行本程序。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                File.Delete(System.IO.Path.GetTempPath() + "bgm1.wma");
                File.Delete(System.IO.Path.GetTempPath() + "bgm2.wma");
                fun.logger("所有操作全部成功完成，创建快捷方式，程序退出", false, System.Windows.Forms.Application.StartupPath);
                fun.dellog(System.Windows.Forms.Application.StartupPath + "\\zh-installlog.log", Program.switcher);
                System.Environment.Exit(System.Environment.ExitCode);
            }
            else
            {

                MessageBox.Show("我是薛帕德指挥官，这是我在神堡最喜欢的汉化版本！如需要" + reverion + "，请重新执行本程序。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                File.Delete(System.IO.Path.GetTempPath() + "bgm1.wma");
                File.Delete(System.IO.Path.GetTempPath() + "bgm2.wma");
                fun.logger("所有操作全部成功完成，未创建快捷方式，程序退出", false, System.Windows.Forms.Application.StartupPath);
                fun.dellog(System.Windows.Forms.Application.StartupPath + "\\zh-installlog.log", Program.switcher);
                System.Environment.Exit(System.Environment.ExitCode);
            }        
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                progressBar2.Value = e.ProgressPercentage;
                label4.Text = Convert.ToString(e.ProgressPercentage) + "%";
            }));
        }

    }
}
