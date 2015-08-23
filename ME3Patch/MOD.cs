using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ME3Patch
{
    public partial class MOD : Form
    {
        public MOD()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {             
                Program.AdvMOD = false;
                fun.logger("用户选择普通模式", false, System.Windows.Forms.Application.StartupPath);this.Close();
            }
            if (this.radioButton2.Checked)
            {
                Program.AdvMOD = true;
                fun.logger("用户选择高级模式", false, System.Windows.Forms.Application.StartupPath);
                if (MessageBox.Show("使用高级模式有一定的风险，继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    this.Close();
                }
                
            }
                
         }

        private void MOD_Load(object sender, EventArgs e)
        {
            if (checkfiles.checkverion(fun.md5_hash(System.Windows.Forms.Application.StartupPath + "\\BIOGame\\CookedPCConsole\\BIOGame_INT.tlk"), Program.verhash) == 1)
            {
                MessageBox.Show("检测到您已安装旧版玩家汉化，为了防止出错，建议使用相应的玩家汉化安装程序还原英文或是将游戏更新至最新版后再运行本程序。您也可以使用高级模式安装汉化，但会有一定的风险。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fun.logger("检测到版本冲突，普通模式关闭", false, System.Windows.Forms.Application.StartupPath);
                this.radioButton1.Checked = false;
                this.radioButton1.Enabled = false;
                this.radioButton2.Checked = true;
            }
            if (checkfiles.checkverion(fun.md5_hash(System.Windows.Forms.Application.StartupPath + "\\BIOGame\\DLC\\DLC_UPD_Patch01\\CookedPCConsole\\Default.sfar"), Program.verhash) == 1)
            {
                MessageBox.Show("检测到您未安装最新V1.05补丁，为了防止出错，建议使用相应的玩家汉化安装程序还原英文或是将游戏更新至最新版后再运行本程序。您也可以使用高级模式安装汉化，但会有一定的风险。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fun.logger("检测到版本冲突，普通模式关闭", false, System.Windows.Forms.Application.StartupPath);
                this.radioButton1.Checked = false;
                this.radioButton1.Enabled = false;
                this.radioButton2.Checked = true;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

     }
}

