using System;
using System.Windows.Forms;
using System.Collections;

namespace ME3Patch
{
    static class Program
    {
        public static Hashtable orgfile = hashdata.oldFilehashlist();
        public static Hashtable chofile = hashdata.newFilehashlist();
        public static Hashtable chefile = hashdata.new2Filehashlist();
        public static Hashtable filepath = hashdata.filePathhashlist();
        //public static Hashtable filename = hashdata.fileNamehashlist();
        public static Hashtable filestatus = hashdata.filestatus();
        public static Hashtable badfile = new Hashtable();
        public static Hashtable choupdfile = hashdata.cupdFilehashlist();
        public static Hashtable cheupdfile = hashdata.cupd2Filehashlist();
        public static Hashtable filehash = new Hashtable();
        public static Hashtable repfilehash = hashdata.replaceFilehashlist();
        public static Hashtable verhash = hashdata.fileversion();
        public static bool switcher = true;
        public static Hashtable filecode = hashdata.filename();
        public static bool flag;
        public static bool updst=false;
        public static string IniPath;
        public static int ChoseVer;
        public static bool fristrun = false;
        public static bool AdvMOD = false;
        public static bool RusVer = false;
        public static bool DLCbak = false;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string temp = System.IO.Path.GetTempPath();
            fun.SaveRecordFile(temp, "bgm1", ".wma");
            fun.SaveRecordFile(temp, "config", ".ini");
            IniPath = temp + "config.ini";
            fun.logger("启动程序", true, System.Windows.Forms.Application.StartupPath);
            if (checkfiles.checkdir(1) == false)//检测是否放在安装目录下
            {
                MessageBox.Show("这不是《质量效应3》的安装目录吧？难道你在忽悠老夫？", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fun.logger("未检测到游戏本体，程序退出", false, System.Windows.Forms.Application.StartupPath);
                System.Environment.Exit(System.Environment.ExitCode);

            }
            
            AboutBox1 fm2 = new AboutBox1();
            
            fm2.ShowDialog();
            if(AdvMOD)
            {            
                Application.Run(new Adv());
            }
            else Application.Run(new Form1());
        }
    }
}
