using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using System.Xml;
using MetroFramework.Controls;
using System.IO;
 
namespace CSharp_Training_ProblemSetter
{
    public partial class ProblemSetter : MetroForm
    {
        string workFileName;
        bool unsaved, flagDelete, flagShow1, flagShow2, flagShow3;

        public void InitializeDataGrid()
        {
            // I tried to use DataTable until I found GridView is working well as DataTable.
            /*
            DataTable _table = new DataTable();
            _table.ReadXml(Application.StartupPath + @"\default.xml");
            metroGrid1.DataSource = _table;
            */

            // 设置GridView的项目值
            metroGrid1.Columns.Add("input", "input");
            metroGrid1.Columns.Add("output", "output");
            metroGrid1.Columns.Add("note", "note");
            metroGrid1.Columns[2].Width = 150;

            // 设置GridView的属性
            metroGrid1.Font = new Font("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Pixel);
            metroGrid1.AllowUserToAddRows = false;

            // 新建并初始化用于储存测试数据的testData.xml
            XmlDocument testData = new XmlDocument();
            testData.LoadXml("<?xml version='1.0' encoding='UTF-8'?>" +
                             "<fps version='1.2'>" +　
                             "</fps>");
            testData.Save(Application.StartupPath + @"\testData.xml");

            // 初始化flag
            unsaved = flagDelete = false;
            flagShow1 = flagShow2 = flagShow3 = true;
        }

        public ProblemSetter()
        {
            InitializeComponent();
            InitializeDataGrid();
        }

        // 调试用函数，以messagebox的形式显示传入的string的内容
        public void debugShowString(string s)
        {
            MetroMessageBox.Show(this, s, "String Content");
        }

        // 导入problem，更改Edit problem选项卡中相应项内容
        public void problem_Import()
        {
            // 打开准备导入的.xml文件
            XmlDocument doc = new XmlDocument();
            doc.Load(workFileName);

            // 声明用于读取问题描述的节点
            XmlNode readNode = doc.DocumentElement["item"];

            // 导入问题描述到TextBox
            if (null != readNode["title"])
                metroTextBox1.Text = readNode["title"].InnerText;

            if (null != readNode["time_limit unit='s'"])
                metroTextBox3.Text = readNode["time_limit unit='s'"].InnerText;

            if (null != readNode["memory_limit unit='mb'"])
                metroTextBox4.Text = readNode["memory_limit unit='mb'"].InnerText;

            if (null != readNode["description"])
                metroTextBox2.Text = readNode["description"].InnerText;

            if (null != readNode["input"])
                metroTextBox12.Text = readNode["input"].InnerText;

            if (null != readNode["output"])
                metroTextBox11.Text = readNode["output"].InnerText;

            if (null != readNode["sample_input"])
                metroTextBox5.Text = readNode["sample_input"].InnerText;

            if (null != readNode["sample_output"])
                metroTextBox6.Text = readNode["sample_output"].InnerText;

            if (null != readNode["hint"])
                metroTextBox8.Text = readNode["hint"].InnerText;

            if (null != readNode["source"])
                metroTextBox7.Text = readNode["source"].InnerText;
        }

        // 导入data，更改Add data选项卡中相应项内容
        public void data_Import()
        {
            // 打开准备导入的.xml文件
            XmlDocument testData = new XmlDocument();
            testData.Load(workFileName);
            
            // 新增GridView的项目并导入所有test data的input到GridView
            XmlNodeList dataList = testData.DocumentElement.SelectNodes("descendant::test_input");
            foreach (XmlNode dataNode in dataList)
            {
                metroGrid1.Rows.Add(dataNode.InnerText);
            }

            // 导入所有test data的output到GridView
            int index = 0;
            dataList = testData.DocumentElement.SelectNodes("descendant::test_output");
            foreach (XmlNode dataNode in dataList)
            {
                metroGrid1.Rows[index++].Cells[1].Value = dataNode.InnerText;
            }

            // 导入所有test data的note到GridView
            index = 0;
            dataList = testData.DocumentElement.SelectNodes("descendant::note");
            foreach (XmlNode dataNode in dataList)
            {
                metroGrid1.Rows[index++].Cells[2].Value = dataNode.InnerText;
            }
        }

        // 导出data为.xml文件
        public void data_Export()
        {
            // 打开储存测试数据的testData.xml
            XmlDocument testData = new XmlDocument();
            testData.Load(Application.StartupPath + @"\testData.xml");

            // 将测试数据写入到testData.xml
            XmlNode addPoint = testData.DocumentElement;
            foreach (DataGridViewRow item in this.metroGrid1.Rows)
            {
                using (XmlWriter dataWriter = addPoint.CreateNavigator().AppendChild())
                {
                    dataWriter.WriteStartElement("data");

                    dataWriter.WriteStartElement("test_input");
                    dataWriter.WriteCData((string)item.Cells[0].Value);
                    dataWriter.WriteEndElement();

                    dataWriter.WriteStartElement("test_output");
                    dataWriter.WriteCData((string)item.Cells[1].Value);
                    dataWriter.WriteEndElement();

                    dataWriter.WriteStartElement("note");
                    dataWriter.WriteCData((string)item.Cells[2].Value);
                    dataWriter.WriteEndElement();
                }
            }

            // 导出保存.xml
            testData.Save(Application.StartupPath + @"\testData.xml");
        }

        // 导出problem为.xml文件
        public void problem_Export()
        {
            // 导出data为.xml文件
            data_Export();

            // 新建XmlDocument
            XmlDocument doc = new XmlDocument();
            // 写入problem description
            doc.LoadXml("<?xml version='1.0' encoding='UTF-8'?>" +
                        "<fps version='1.2'>" +
                        "<item>" +
                        "<title><![CDATA[" + metroTextBox1.Text + "]]></title>" +
                        "<time_limit unit='s'><![CDATA[" + metroTextBox3.Text + "]]></time_limit>" +
                        "<memory_limit unit='mb'><![CDATA[" + metroTextBox4.Text + "]]></memory_limit>" +
                        "<description><![CDATA[" + metroTextBox2.Text + "]]></description>" +
                        "<input><![CDATA[" + metroTextBox12.Text + "]]></input>" +
                        "<output><![CDATA[" + metroTextBox11.Text + "]]></output>" +
                        "<sample_input><![CDATA[" + metroTextBox5.Text + "]]></sample_input>" +
                        "<sample_output><![CDATA[" + metroTextBox6.Text + "]]></sample_output>" +
                        "<hint><![CDATA[" + metroTextBox8.Text + "]]></hint>" +
                        "<source><![CDATA[" + metroTextBox7.Text + "]]></source>" +
                        "</item>" +
                        "</fps>");

            // 打开储存测试数据的testData.xml
            XmlDocument testData = new XmlDocument();
            testData.Load(Application.StartupPath + @"\testData.xml");

            // 根据表达式提取出所需节点生成NodeList
            XmlNodeList dataList;
            dataList = testData.DocumentElement.SelectNodes("descendant::data");

            // 将测试数据写入到问题文档
            XmlNode addPoint = doc.DocumentElement["item"];
            foreach (XmlNode dataNode in dataList)
            {
                // 若节点来自不同的文档，需要通过ImportNode将节点导入当前文档才能进行AppendChild操作
                addPoint.AppendChild(doc.ImportNode(dataNode["test_input"], true));
                addPoint.AppendChild(doc.ImportNode(dataNode["test_output"], true));
                addPoint.AppendChild(doc.ImportNode(dataNode["note"], true));
            }

            // 导出保存.xml
            doc.Save(workFileName);

            // 若flagDelete为true则删除程序生成的临时文件
            if (flagDelete == true)
            {
                File.Delete(Application.StartupPath + @"\testData.xml");
            }
        }

        // 点击new按钮时，创建一个新的.xml文件
        private void metroTile1_Click(object sender, EventArgs e)
        {
            // 若flagShow3设置为true且有未保存内容则显示提示
            if (flagShow3 == true && unsaved == true)
            {
                if (DialogResult.Yes
                    != MetroMessageBox.Show(this, "Are you sure to create a new project with not saving the unsave content?", "Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return;
                }
            }

            // 读取用户选择的文件路径
            saveFileDialog1.Filter = "xml files (*.xml)|*.xml";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && saveFileDialog1.FileName.Length > 0)
            {
                workFileName = saveFileDialog1.FileName;
                // debugShowString(workFileName);
            }
        }

        // 点击open按钮时，打开一个已有的.xml文件
        private void metroTile2_Click(object sender, EventArgs e)
        {
            // 若flagShow3设置为true且有未保存内容则显示提示
            if (flagShow3 == true && unsaved == true)
            {
                if (DialogResult.Yes
                    != MetroMessageBox.Show(this, "Are you sure to create a new project with not saving the unsave content?", "Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return;
                }
            }

            // 读取用户选择的文件路径
            openFileDialog1.Filter = "xml files (*.xml)|*.xml";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && openFileDialog1.FileName.Length > 0)
            {
                workFileName = openFileDialog1.FileName;
                // debugShowString(workFileName);
            }

            // 导入问题描述和测试数据
            problem_Import();
            data_Import();
        }

        // 点击save as按钮时，另存为一个新的.xml文件
        private void metroTile3_Click(object sender, EventArgs e)
        {
            // 读取用户选择的文件路径
            saveFileDialog2.Filter = "xml files (*.xml)|*.xml";
            if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && saveFileDialog2.FileName.Length > 0)
            {
                workFileName = saveFileDialog2.FileName;
                // debugShowString(workFileName);
            }

            // 导出为整个问题文件
            problem_Export();

            // 重置未保存状态
            unsaved = false;
        }

        // 点击Delete按钮时，将选中的test data项从GridView中删除
        private void metroTile5_Click(object sender, EventArgs e)
        {
            // 设置未保存状态
            unsaved = true;

            // 删除选中的数据项
            // 若flagShow1为true则显示提示
            if (flagShow1 == true)
            {
                if (DialogResult.Yes
                    == MetroMessageBox.Show(this, "Are you sure to delete all the test data that you selected?", "Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    foreach (DataGridViewRow item in this.metroGrid1.SelectedRows)
                    {
                        metroGrid1.Rows.RemoveAt(item.Index);
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow item in this.metroGrid1.SelectedRows)
                {
                    metroGrid1.Rows.RemoveAt(item.Index);
                }
            }
            // debugShowString(metroGrid1.RowCount.ToString());
        }

        // 点击Add按钮时，将正在编辑的test data增加到GridView中
        private void metroTile4_Click(object sender, EventArgs e)
        {
            // 设置未保存状态
            unsaved = true;

            // 往GridView增加TextBox的内容
            metroGrid1.Rows.Add(metroTextBox10.Text, metroTextBox9.Text);
            // debugShowString(metroTextBox10.Text + metroTextBox9.Text);
        }

        // 更改颜色主题
        private void metroTile6_Click(object sender, EventArgs e)
        {
            // 获取新颜色主题的值
            var m = new Random();
            int next = m.Next(0, 13);
            // 若为白色则再次获取
            while (next == 2)
                next = m.Next(0, 13);

            // 设置所有在TabPage下的子控件的Style属性为新主题的值
            foreach (Control tc in metroTabPage.Controls)
            {
                foreach (Control c in ((MetroTabPage)tc).Controls)
                {
                    if (c is MetroGrid)
                    {
                        MetroGrid t = c as MetroGrid;
                        t.Style = (MetroColorStyle)next;
                        continue;
                    }
                    if (c is MetroTabControl)
                    {
                        MetroTabControl t = c as MetroTabControl;
                        t.Style = (MetroColorStyle)next;
                        continue;
                    }
                    if (c is MetroTabPage)
                    {
                        MetroTabPage t = c as MetroTabPage;
                        t.Style = (MetroColorStyle)next;
                        continue;
                    }
                    if (c is MetroLabel)
                    {
                        MetroLabel t = c as MetroLabel;
                        t.Style = (MetroColorStyle)next;
                        continue;
                    }
                    if (c is MetroTile)
                    {
                        MetroTile t = c as MetroTile;
                        t.Style = (MetroColorStyle)next;
                        continue;
                    }
                    if (c is MetroToggle)
                    {
                        MetroToggle t = c as MetroToggle;
                        t.Style = (MetroColorStyle)next;
                        continue;
                    }
                }
            }
            // 设置TabPage和主窗口的Style属性为新主题的值
            this.Style = metroTabPage.Style = (MetroColorStyle)next;

            // 刷新
            this.Refresh();
        }

        // 改变Time Limit TextBox的值时
        private void metroTextBox3_TextChanged(object sender, EventArgs e)
        {
            // 修改未保存状态
            unsaved = true;

            // 检查输入值是否非数字
            int ok = 1, len = metroTextBox3.Text.Length, sum = 0;
            for (int i = 0; i < len; ++i)
            {
                if (!Char.IsDigit(metroTextBox3.Text[i]))
                {
                    ok = 0;
                    break;
                }
                sum *= 10;
                sum += metroTextBox3.Text[i] - '0';
            }
            if (0 == ok || 0 == sum && 0 != len)
            {
                metroTextBox3.Text = metroTextBox3.Text.Remove(len - 1);
                MetroMessageBox.Show(this, "The value of Time Limit should be positive integer.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            // here is a bug occur when the textbox has no content.
        }

        // 改变Memory Limit TextBox的值时
        private void metroTextBox4_TextChanged(object sender, EventArgs e)
        {
            // 修改未保存状态
            unsaved = true;

            // 检查输入值是否非数字
            int ok = 1, len = metroTextBox4.Text.Length, sum = 0;
            for (int i = 0; i < len; ++i)
            {
                if (!Char.IsDigit(metroTextBox4.Text[i]))
                {
                    ok = 0;
                    break;
                }
                sum *= 10;
                sum += metroTextBox4.Text[i] - '0';
            }
            if (0 == ok || 0 == sum && 0 != len)
            {
                metroTextBox4.Text = metroTextBox4.Text.Remove(len - 1);
                MetroMessageBox.Show(this, "The value of Memory Limit should be positive integer.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            // here is a bug occur when the textbox has no content.
        }

        // 当TextBox的TextChanged事件被触发时
        private void TextChanged(object sender, EventArgs e)
        {
            // 修改未保存状态
            unsaved = true;
        }

        // 当metroTrackBar1的ValueChanged事件被触发时
        private void metroTrackBar1_ValueChanged(object sender, EventArgs e)
        {
            // 修改窗体透明度
            this.Opacity = metroTrackBar1.Value * 1.0 / 100;
        }

        // 当开关metroToggle1时
        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            // 修改flagDelete
            flagDelete = flagDelete == true ? false : true;
        }

        // 当开关metroToggle2时
        private void metroToggle2_CheckedChanged(object sender, EventArgs e)
        {
            // 修改flagShow1
            flagShow1 = flagShow1 == true ? false : true;
        }

        // 当开关metroToggle3时
        private void metroToggle3_CheckedChanged(object sender, EventArgs e)
        {
            // 修改flagShow2
            flagShow2 = flagShow2 == true ? false : true;
        }

        // 当开关metroToggle4时
        private void metroToggle4_CheckedChanged(object sender, EventArgs e)
        {
            // 修改flagShow3
            flagShow3 = flagShow3 == true ? false : true;
        }

        // 当尝试关闭程序时
        private void ProblemSetter_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 若flagShow2为true并且有未保存内容则显示提示
            if (flagShow2 == true && unsaved == true)
            {
                if (DialogResult.Yes
                    != MetroMessageBox.Show(this, "Are you sure to close the software with not saving the unsave content?", "Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
