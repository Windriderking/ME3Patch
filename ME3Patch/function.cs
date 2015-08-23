using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace ME3Patch
{
    public class fun
    {
        public delegate void SetValue(ProgressBar C, int Value);
        public delegate void SetText(Label C, string Value);
        public static void DoSetValue(ProgressBar C, int Value)
        {
            C.Value = Value;
        }
        public static void DoSetMax(ProgressBar C, int Value)
        {
            C.Maximum = Value;
            C.Minimum = 0;
        }
        public static void DoSetText(Label C, string Value)
        {
            C.Text = Value;
        }
        /// <summary> 
        /// Converts file and directory paths to their respective 
        /// long and short name versions. 
        /// 文件或目录的长文件名与短文件名互转换类 
        /// </summary> 
        /// <remarks>This class uses InteropServices to call GetLongPathName and GetShortPathName</remarks> 
        public class ShellPathNameConvert
        {
            [DllImport("kernel32.dll")]
            static extern uint GetLongPathName(string shortname, StringBuilder
            longnamebuff, uint buffersize);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)]
   string path,
            [MarshalAs(UnmanagedType.LPTStr)]
   StringBuilder shortPath,
            int shortPathLength);

            /// <summary> 
            /// The ToShortPathNameToLongPathName function retrieves the long path form of a specified short input path 
            /// </summary> 
            /// <param name="shortName">The short name path</param> 
            /// <returns>A long name path string</returns> 
            public static string ToLongPathName(string shortName)
            {
                StringBuilder longNameBuffer = new StringBuilder(256);
                uint bufferSize = (uint)longNameBuffer.Capacity;

                GetLongPathName(shortName, longNameBuffer, bufferSize);

                return longNameBuffer.ToString();
            }

            /// <summary> 
            /// The ToLongPathNameToShortPathName function retrieves the short path form of a specified long input path 
            /// </summary> 
            /// <param name="longName">The long name path</param> 
            /// <returns>A short name path string</returns> 
            public static string ToShortPathName(string longName)
            {
                StringBuilder shortNameBuffer = new StringBuilder(256);
                int bufferSize = shortNameBuffer.Capacity;

                int result = GetShortPathName(longName, shortNameBuffer, bufferSize);

                return shortNameBuffer.ToString();
            }
        }
        public static void logger(string str, bool first, string path)
        {
            StreamWriter sw;
            if (!File.Exists(path + "\\zh-installlog.log"))
            {
                //不存在就新建一个文本文件,并写入一些内容
                sw = File.CreateText(path + "\\zh-installlog.log");
            }
            else
            {
                sw = File.AppendText(path + "\\zh-installlog.log");
            }
            if (first)
            {
                sw.WriteLine("-----------------------------------------------------");
                sw.WriteLine("质量效应3玩家汉化" + " V" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                sw.WriteLine();
                sw.WriteLine(DateTime.Now + " " + str + "。");
            }
            else
            {
                sw.WriteLine(DateTime.Now + " " + str + "。");

            }
            sw.Close();

        }
        public static void dellog(string path, bool switcher)
        {
            if (switcher)
            {
                File.Delete(path);
            }
        }
        public static int SaveRecordFile(string filepath, string filename, string fileExtension)//输出文件
        {
            try
            {
                string outputFileName = Path.GetFileNameWithoutExtension(filepath + filename);
                byte[] output = (byte[])ME3Patch.Properties.Resources.ResourceManager.GetObject(outputFileName);
                output = SevenZip.Compression.LZMA.SevenZipHelper.Decompress2(output);
                File.WriteAllBytes(filepath + outputFileName + fileExtension, output);
                return 0;
            }
            catch
            {
                return 1;
            }
        }
        public static void link(string verion)
        {
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "质量效应3" + verion + ".lnk");
            shortcut.TargetPath = System.Windows.Forms.Application.StartupPath + "\\Binaries\\Win32\\masseffect3.exe";
            shortcut.WorkingDirectory = System.Environment.CurrentDirectory;
            shortcut.WindowStyle = 1;
            shortcut.Description = "质量效应3" + verion;
            shortcut.IconLocation = System.Windows.Forms.Application.StartupPath + "\\Binaries\\Win32\\masseffect3.exe";
            shortcut.Save();
        }//创建快捷方式
        public static string codetrans(int code)//转换组件代码
        {
            return Convert.ToString(Program.filecode[Convert.ToString(code)]);
        }
        public static string codestatustrans(int code)//转换状态代码
        {
            if (code == 1)
                return "英文原版,可汉化";
            else if (code == 2)
                return "汉化单语，可还原";
            else if (code == 3)
                return "汉化单语，可升级";
            else if (code == 4)
                return "汉化双语，可还原";
            else if (code == 5)
                return "汉化双语，可升级";
            else if (code == 6)
                return "英文,可汉化";
            else if (code == 7)
                return "未安装";
            else return "无法识别";
        }
        public static uint SND_ASYNC = 0x0001;
        public static uint SND_FILENAME = 0x00020000;
        [DllImport("winmm.dll")]
        public static extern long mciSendString(string lpstrCommand, string lpstrReturnString, int length, int hwndcallback);
        public static void Play(int i)
        {
            if (i == 1)
            {
                string path;
                fun.logger("播放音乐", false, System.Windows.Forms.Application.StartupPath);
                path = fun.ShellPathNameConvert.ToShortPathName(System.IO.Path.GetTempPath() + "bgm1.wma");
                mciSendString("close temp_music", null, 0, 0);
                mciSendString("open " + path + " alias temp_music", null, 0, 0);
                mciSendString("play temp_music repeat", null, 0, 0);
                Form1.bgm = true;
            }
            else
            {
                string path;
                fun.logger("播放音乐", false, System.Windows.Forms.Application.StartupPath);
                path = fun.ShellPathNameConvert.ToShortPathName(System.IO.Path.GetTempPath() + "bgm2.wma");
                mciSendString("close temp_music", null, 0, 0);
                mciSendString("open " + path + " alias temp_music", null, 0, 0);
                mciSendString("play temp_music repeat", null, 0, 0);
                Form1.bgm = true;
            }

        }
        public static void Pause(int i)
        {
            if (i == 1)
            {
                string path;
                fun.logger("用户停止音乐", false, System.Windows.Forms.Application.StartupPath);
                path = fun.ShellPathNameConvert.ToShortPathName(System.IO.Path.GetTempPath() + "bgm1.wma");
                mciSendString("close temp_music", null, 0, 0);
                mciSendString("open " + path + " alias temp_music", null, 0, 0);
                mciSendString("Pause temp_music repeat", null, 0, 0);
                Form1.bgm = false;
            }
            else
            {
                string path;
                fun.logger("用户停止音乐", false, System.Windows.Forms.Application.StartupPath);
                path = fun.ShellPathNameConvert.ToShortPathName(System.IO.Path.GetTempPath() + "bgm2.wma");
                mciSendString("close temp_music", null, 0, 0);
                mciSendString("open " + path + " alias temp_music", null, 0, 0);
                mciSendString("Pause temp_music repeat", null, 0, 0);
                Form1.bgm = false;
            }
        }
        public static int fileupdate(string filepath, string filename, string type, int flag, int FileID)//输出文件
        {
            try
            {


                string typer;
                string[] arr1 = filename.Split('_');
                foreach (string typex in arr1)
                {
                    if (typex == "Fonts")
                    {
                        type = "COM";
                        break;
                    }
                }
                if (type == "COM")
                {
                    typer = "_COM"; if (flag == -1) typer = "_ENG";
                }
                else
                {
                    switch (flag)
                    {
                        case 1: typer = "_ENG"; break;
                        case 2: typer = "_CHN"; break;
                        case 3: typer = "_CHN"; break;
                        case 4: typer = "_CHE"; break;
                        case 5: typer = "_CHE"; break;
                        case 6: typer = "_ENG"; break;
                        case 7: typer = null; break;
                        default: typer = "_ENG"; break;
                    }

                }
                if (filename == "Data.pat")
                    typer = "_COM";
                if (typer == null)
                    throw new ArgumentException("Invalid Type!");
                string outputFileName = Path.GetFileNameWithoutExtension(filename);
                string fileExtension = Path.GetExtension(filename);
                if (outputFileName == "Data" || outputFileName == "DLC_Shared_INT")
                    typer += Convert.ToString(FileID);
                if (FileID == 3 && typer == "_ENG")
                    typer += "1";
                outputFileName += typer;
                SaveRecordFile(filepath + "\\", outputFileName, fileExtension);
                if (File.Exists(filepath + "\\" + filename))
                {
                    FileInfo fi = new FileInfo(filepath + "\\" + filename);
                    if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(filepath + "\\" + filename, FileAttributes.Normal);
                    }
                    File.Delete(filepath + "\\" + filename);
                }
                File.Move(filepath + "\\" + outputFileName + fileExtension, filepath + "\\" + filename);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public static string AnyscMD5(string filepath, ProgressBar pBar, Label lab)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            int bufferSize = 1048576; // 缓冲区大小，1MB 
            byte[] buff = new byte[bufferSize];
            double blockcount = Math.Ceiling(fs.Length / Convert.ToDouble(bufferSize));
            if (pBar.InvokeRequired == true)
            {
                SetText LSetText = new SetText(DoSetText);
                SetValue PSetValue = new SetValue(DoSetMax);
                pBar.Invoke(PSetValue, new Object[] { pBar, Convert.ToInt32(blockcount) });
                lab.Invoke(LSetText, new Object[] { lab, Convert.ToString(0) + "%" });
            }
            int i = 1;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            md5.Initialize();
            long offset = 0;
            while (offset < fs.Length)
            {
                long readSize = bufferSize;
                if (offset + readSize > fs.Length)
                {
                    readSize = fs.Length - offset;
                }

                fs.Read(buff, 0, Convert.ToInt32(readSize)); // 读取一段数据到缓冲区 

                if (offset + readSize < fs.Length) // 不是最后一块 
                {
                    md5.TransformBlock(buff, 0, Convert.ToInt32(readSize), buff, 0);
                }
                else // 最后一块 
                {
                    md5.TransformFinalBlock(buff, 0, Convert.ToInt32(readSize));
                }
                offset += bufferSize;
                if (pBar.InvokeRequired == true)
                {
                    SetValue PSetValue = new SetValue(DoSetValue);
                    SetText LSetText = new SetText(DoSetText);
                    pBar.Invoke(PSetValue, new Object[] { pBar, Convert.ToInt32(i) });
                    lab.Invoke(LSetText, new Object[] { lab, Convert.ToString(Math.Ceiling((double)(i / blockcount) * 100)) + "%" });
                    i++;
                    Application.DoEvents();
                }
            }
            fs.Close();
            byte[] result = md5.Hash;
            md5.Clear();
            StringBuilder sb = new StringBuilder(32);
            for (int j = 0; j < result.Length; j++)
            {
                sb.Append(result[j].ToString("x2"));
            }
            return sb.ToString();
        }
        public static string fileupdatcode(int code)//转换文件状态代码
        {
            if (code == 1 || code == -1)
                return "还原成功！";
            else if (code == 2 || code == 4)
                return "汉化成功！";
            else if (code == 7)
                return "未操作";
            else return "错误，代码" + Convert.ToString(code);
        }
        public static string md5_hash(string path)
        {
            try
            {
                FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(get_file);
                string resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "");
                get_file.Close();
                return resule.ToLower();
            }
            catch
            {

                return "";

            }
        }
    }
    public class checkfiles
    {
        public static int checkverion(string filehash, Hashtable verHash)//校验文件Md5,并确定是否为其他汉化文件版本
        {
            int i;
            if (filehash == "")
                return 3;
            for (i = 0; i < verHash.Count; i++)
            {
                if (filehash == (string)verHash[Convert.ToString(i)].ToString())
                    return 1;
                else continue;
            }
            return 4;
        }

        public static bool checkdir(int flag)
        {
            if (flag == 1)
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Binaries\\Win32\\masseffect3.exe") & Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\BIOGame"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return true;
        }//检查是否在根目录
        public static int checkfile(string filehash, Hashtable oldHash, Hashtable replaceHash, Hashtable choHash, Hashtable cheHash, Hashtable choupdHash, Hashtable cheupdHash)//校验文件Md5,并确定文件版本
        {
            int i;
            if (filehash == "")
                return 7;
            for (i = 0; i < choupdHash.Count; i++)
            {
                if (filehash == (string)choupdHash[Convert.ToString(i)].ToString())
                    return 3;
                else continue;
            }
            for (i = 0; i < cheupdHash.Count; i++)
            {
                if (filehash == (string)cheupdHash[Convert.ToString(i)].ToString())
                    return 5;
                else continue;
            }
            for (i = 0; i < oldHash.Count; i++)
            {
                if (filehash == (string)oldHash[Convert.ToString(i)].ToString())
                    return 1;
                else continue;
            }
            for (i = 0; i < replaceHash.Count; i++)
            {
                if (filehash == (string)replaceHash[Convert.ToString(i)].ToString())
                    return 6;
                else continue;
            }
            for (i = 0; i < choHash.Count; i++)
            {
                if (filehash == (string)choHash[Convert.ToString(i)].ToString())
                    return 2;
                else continue;
            }
            for (i = 0; i < cheHash.Count; i++)
            {
                if (filehash == (string)cheHash[Convert.ToString(i)].ToString())
                    return 4;
                else continue;
            }
            return 8;
        }
    }
    public class COMRECT
    {
        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

    }

}