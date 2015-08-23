using System;
using System.Reflection;
using System.Windows.Forms;
//using check;

namespace ME3Patch
{

    partial class AboutBox1 : Form
    {
        protected override void WndProc(ref   Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                //   User   clicked   close   button   
                return;
            }
            base.WndProc(ref   m);
        }        
        public AboutBox1()
        {
            InitializeComponent();
            this.Text = String.Format("关于 {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("版本 {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.richTextBox1.Text = AssemblyDescription;
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        #region 程序集特性访问器

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (Program.fristrun != true)
            {
                if (!this.checkBox1.Checked)
                {
                    if (MessageBox.Show("即将开始采用普通模式安装，单击“确定”继续，单击“取消”退出程序。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    {
                        System.Environment.Exit(0);
                    }
                    Program.AdvMOD = false;
                    fun.logger("用户选择普通模式", false, System.Windows.Forms.Application.StartupPath); this.Close();
                }
                else
                {
                    if (MessageBox.Show("即将开始采用高级模式安装，存在一定的风险！单击“确定”继续，单击“取消”退出程序。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    {
                        System.Environment.Exit(0);
                    }
                    Program.AdvMOD = true;
                    fun.logger("用户选择高级模式", false, System.Windows.Forms.Application.StartupPath);
                }        
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText); 
        }

        private void Aboutbox_Load(object sender, EventArgs e)
        {
            if (Program.fristrun != true)
            {
                this.checkBox1.Visible = true;
                if (checkfiles.checkverion(fun.md5_hash(System.Windows.Forms.Application.StartupPath + "\\BIOGame\\CookedPCConsole\\BIOGame_INT.tlk"), Program.verhash) == 1)
                {
                    MessageBox.Show("检测到您已安装旧版玩家汉化，为了防止出错，建议使用相应的玩家汉化安装程序还原英文或是将游戏更新至最新版后再运行本程序。您也可以使用高级模式安装汉化，但会有一定的风险。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fun.logger("检测到版本冲突，普通模式关闭", false, System.Windows.Forms.Application.StartupPath);
                    Program.AdvMOD = true;
                    this.checkBox1.Checked = true;
                    this.checkBox1.Enabled = false;
                }
                if (checkfiles.checkverion(fun.md5_hash(System.Windows.Forms.Application.StartupPath + "\\BIOGame\\DLC\\DLC_UPD_Patch01\\CookedPCConsole\\Default.sfar"), Program.verhash) == 1)
                {
                    MessageBox.Show("检测到您未安装最新V1.05补丁，为了防止出错，建议使用相应的玩家汉化安装程序还原英文或是将游戏更新至最新版后再运行本程序。您也可以使用高级模式安装汉化，但会有一定的风险。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fun.logger("检测到版本冲突，普通模式关闭", false, System.Windows.Forms.Application.StartupPath);
                    Program.AdvMOD = true;
                    //this.checkBox1.Visible = true;
                    this.checkBox1.Checked = true;
                    this.checkBox1.Enabled = false;
                }
            }
            else
            {
                this.checkBox1.Visible = false;
                this.checkBox1.Enabled = false;
            }
            fun.logger("显示版权信息", false, System.Windows.Forms.Application.StartupPath);
        }

        private void checkBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
