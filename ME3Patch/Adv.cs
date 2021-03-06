﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using AmaroK86.MassEffect3;

namespace ME3Patch
{
    public partial class Adv : Form
    {
        DLCBase dlcBase = null;
        DLCEditor dlcEditor = null;
        int stdd;
        #region a
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
        #endregion
        public Adv()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                for (int j = 0; j < checkedListBox1.Items.Count; j++)
                    checkedListBox1.SetItemChecked(j, true);
            }
            else
            {
                for (int j = 0; j < checkedListBox1.Items.Count; j++)
                    checkedListBox1.SetItemChecked(j, false);
            }
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void Adv_Load(object sender, EventArgs e)
        {
            string path;
            for (int i = 0; i < Program.filecode.Count; i++)
            {
                this.checkedListBox1.Items.Add(fun.codetrans(i));
                checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                if (i >= 4)
                {
                    path = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(i)] + "Default.sfar";
                    if (!File.Exists(path))
                    {
                        checkedListBox1.SetItemCheckState(i, CheckState.Indeterminate);
                    }
                }
                

                    
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Program.fristrun = true;
            AboutBox1 fm2 = new AboutBox1();
            fm2.ShowDialog();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == CheckState.Indeterminate)
            {
                e.NewValue = CheckState.Indeterminate;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            fun.logger("辅助线程启动", false, System.Windows.Forms.Application.StartupPath);
            int i, status, updatecode=0, recode;
            for (i = 0; i < Program.filestatus.Count; i++)
            {
                this.Invoke(new MethodInvoker(delegate() { this.label6.Text=fun.codetrans(i);}));
                status = Convert.ToInt32(Program.filestatus[Convert.ToString(i)].ToString());
                fun.logger("开始处理文件#" + Convert.ToString(i), false, System.Windows.Forms.Application.StartupPath);
                string path = "", bakpath = "", pathRus="";
                if (i < 4)
                {
                    path = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(i)] + IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Filename", null);
                    pathRus = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(i)] + IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Filename2", null);
                }
                else
                {
                    path = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(i)] + "Default.sfar";
                    bakpath = System.Windows.Forms.Application.StartupPath + Program.filepath[Convert.ToString(i)] + "Default.sfar.mebak";
                }
                if (i < 4)
                {
                    
                        
                        this.Invoke(new MethodInvoker(delegate() { this.progressBar2.Maximum = 100; progressBar1.Maximum = 100; }));
                    
                    if (i > 1)
                    {
                        if (status == 1 && Program.ChoseVer != -1)
                        {
                            fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), null, status, i);
                            //fun.fileupdate(Path.GetDirectoryName(path), "Data.pat", "COM", status, i);
                            fun.fileupdate(Path.GetDirectoryName(path), "Data.pat", "COM", status, i);
                            recode = VPatch4cs.DoPatch.Patch(Path.GetDirectoryName(path) + "\\Data.pat", path, Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".tmp", progressBar1, label3, i);
                            if (recode == 0)
                            {
                                updatecode = Program.ChoseVer;
                            }
                            else updatecode = recode;
                        }
                        else
                        {                           
                            updatecode = fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                        }
                    }
                    else
                    {
                        if (Program.RusVer == true && (IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null)!="COM"))
                        {
                            File.Delete(pathRus);
                            File.Move(path, path+".bak");
                            recode = fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                            File.Move(path, pathRus);
                            File.Move(path + ".bak",path );
                            if (recode == 1)
                            {
                                updatecode = Program.ChoseVer;
                            }
                        }
                        else
                        {
                            recode = fun.fileupdate(Path.GetDirectoryName(path), Path.GetFileName(path), IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                            if (recode == 1)
                            {
                                updatecode = Program.ChoseVer;
                            }
                            else updatecode = recode;
                        }
                    }                   
                }
                else
                {
                    
                    if (status >= 7)
                    {
                        this.backgroundWorker1.ReportProgress(Convert.ToInt32(Math.Ceiling((((double)(i + 1) / (double)Program.filestatus.Count)) * 100)));
                        fun.logger("处理完成，结果：" + fun.fileupdatcode(status), false, System.Windows.Forms.Application.StartupPath);
                        continue;
                    }
                    else
                    {
                        if (Program.ChoseVer == -1)
                        {
                            dlcBase = new DLCBase(path);
                            dlcEditor = new DLCEditor(dlcBase);
                            string file = IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "FileName", null);
                            string[] arr1 = file.Split(';');
                            foreach (string replacefile in arr1)
                            {
                                fun.fileupdate(Path.GetDirectoryName(path), replacefile, null, -1, i);
                                dlcEditor.setReplaceFile(Program.filepath[Convert.ToString(i)].ToString().Replace('\\', '/') + replacefile, Path.GetDirectoryName(path) + "\\" + replacefile);
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
                            //Program.filehash[Convert.ToString(i)] = fun.AnyscMD5(path, progressBar1, label3);
                           // Program.filestatus[Convert.ToString(i - 1)] = Convert.ToString(checkfiles.checkfile((string)Program.filehash[Convert.ToString(i)].ToString(), Program.orgfile, Program.repfilehash, Program.chofile, Program.chefile, Program.choupdfile, Program.cheupdfile));
                            //status = Convert.ToInt32(Program.filestatus[Convert.ToString(i - 1)].ToString());
                            //fun.fileupdate(Path.GetDirectoryName(path), "Data.pat", IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                            //recode = VPatch4cs.DoPatch.Patch(Path.GetDirectoryName(path) + "\\Data.pat", path, Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".tmp", progressBar1, label3, i + 1);
                            //if (recode == 0)
                                updatecode = 1;
                            //else updatecode = recode;
                        }
                        else
                        {
                            if (Program.DLCbak == true&&(!File.Exists(bakpath)))
                            {
                                File.Copy(path,bakpath);
                            }
                            this.Invoke(new MethodInvoker(delegate() { this.progressBar1.Value = 0; this.label3.Text = "0%"; }));
                            dlcBase = new DLCBase(path);
                            dlcEditor = new DLCEditor(dlcBase);
                            string file = IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "FileName", null);
                            string file2 = IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "FileName2", null);
                            string[] arr1 = file.Split(';');
                            string[] arr2 = file.Split(';');
                            int j=0;
                            foreach (string replacefile in arr1)
                            {
                                fun.fileupdate(Path.GetDirectoryName(path), replacefile, IniFiles.ini.INIGetStringValue(Program.IniPath, Convert.ToString(i), "Type", null), Program.ChoseVer, i);
                                if (Program.RusVer == true)
                                { 
                                    if(replacefile!="SFXGUI_Fonts_DLC.pcc")
                                    {
                                        if (File.Exists(Path.GetDirectoryName(path) + "\\" + arr2[j].ToString()))
                                            File.Delete(Path.GetDirectoryName(path) + "\\" + arr2[j].ToString());
                                        File.Move(Path.GetDirectoryName(path) + "\\" + replacefile, Path.GetDirectoryName(path) + "\\" + arr2[j].ToString());
                                    }

                                    dlcEditor.setReplaceFile(Program.filepath[Convert.ToString(i)].ToString().Replace('\\', '/') + arr2[j].ToString(), Path.GetDirectoryName(path) + "\\" + arr2[j].ToString()); j++;
                                }
                                else
                                {
                                    dlcEditor.setReplaceFile(Program.filepath[Convert.ToString(i)].ToString().Replace('\\', '/') + replacefile, Path.GetDirectoryName(path) + "\\" + replacefile);
                                }
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
                            if (Program.RusVer == true)
                            {
                                foreach (string replacefile in arr2)
                                {
                                    File.Delete(Path.GetDirectoryName(path) + "\\" + replacefile);
                                }
                            }
                            else
                            {
                                foreach (string replacefile in arr1)
                                {
                                    File.Delete(Path.GetDirectoryName(path) + "\\" + replacefile);
                                }
                            }
                            File.Move(newSfar, oldSfar);
                            updatecode = 2;
                        }                      
                    }
                }

                fun.logger("处理完成，结果：" + fun.fileupdatcode(updatecode), false, System.Windows.Forms.Application.StartupPath);

                stdd = status;
                this.backgroundWorker1.ReportProgress(Convert.ToInt32(Math.Ceiling((((double)(i + 1) / (double)Program.filestatus.Count)) * 100)));
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                progressBar2.Value = e.ProgressPercentage;
                label4.Text = Convert.ToString(e.ProgressPercentage) + "%";
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string verion = "", opt = "";
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
            if (this.radioButton3.Checked)
            {
                verion = this.radioButton3.Text;
                opt = "安装";
                Program.ChoseVer = -1;
            }
            if (this.checkBox3.Checked==true)
            {
                Program.DLCbak = true;
            }
            if (this.checkBox2.Checked == true)
            {
                Program.RusVer = true;
            }  
            
            if (MessageBox.Show("您选择" + opt + "玩家汉化" + verion + ",确认请单击“确定”，如想更换或退出，请单击“取消”。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
               
                this.groupBox3.Enabled = false;
                this.progressBar2.Maximum = Program.filestatus.Count;
                this.progressBar2.Value = 0;
                this.progressBar1.Value = 0;
                this.label3.Text = "";
                this.label4.Text = "";
                this.button1.Enabled = false;
                this.button2.Enabled = false;
                this.button3.Enabled = false;
                this.checkBox1.Enabled = false;
                this.checkBox2.Enabled = false;
                this.checkBox3.Enabled = false;
                this.groupBox3.Enabled = false;
                this.groupBox2.Visible = true;
                this.checkedListBox1.Enabled = false;
                for (int i = 0; i < Program.filecode.Count; i++)
                {
                    if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                        Program.filestatus.Add(Convert.ToString(i), Convert.ToString(1));
                    else Program.filestatus.Add(Convert.ToString(i), Convert.ToString(7));
                }
                this.backgroundWorker1.RunWorkerAsync();
            }
        }

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

        

        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
            {
                this.checkBox3.Enabled= true;
            }
            else
            {
                this.checkBox3.Checked = true;
                this.checkBox3.Enabled = false;
                MessageBox.Show("警告！该功能使用尚存在风险！请谨慎选择！如果您已经使用过了俄版转英补丁，切勿勾选此项！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
