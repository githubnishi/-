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
using System.Reflection;

namespace Program_Translation_Tool
{
    public partial class Form1 : Form
    {
        Databace.Databace SQLite = new Databace.Databace();
        DataTable dt = new DataTable();
        int read_line, work_line;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SQLite.ConnectionOpen("./Database/Translate.db");
            toolStripStatusLabel1.Text = "ファイルをドラックしても読み込めます";
            this.MinimumSize = new System.Drawing.Size(300, 300);
            if (Properties.Settings.Default.timer_flg == true)
            {
                オンToolStripMenuItem.Checked = true;
                オフToolStripMenuItem.Checked = false;
                label4.Text = "自動更新の設定：" + "オン";
            }
            else
            {
                オンToolStripMenuItem.Checked = false;
                オフToolStripMenuItem.Checked = true;
                label4.Text = "自動更新の設定：" + "オフ";
            }
            trackBar1.Value = Properties.Settings.Default.slider_size;
            richTextBox1.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            richTextBox2.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            richTextBox1.Font = new Font("MS UI Gothic", trackBar1.Value);
            richTextBox2.Font = new Font("MS UI Gothic", trackBar1.Value);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.slider_size = trackBar1.Value;
            Properties.Settings.Default.Save();
            SQLite.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.timer_flg == true)
                proces();
            else
                timer1.Enabled = false;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                richTextBox1.Text = "";

                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string d in drags)
                {
                    if (!System.IO.File.Exists(d))
                    {
                        continue;
                    }
                    richTextBox1.Text += d;
                }
                e.Effect = DragDropEffects.Copy;
            }

            file_read(richTextBox1.Text);

            proces();
        }

        /// <summary>
        /// ファイルのドラッグを許可
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void ファイルから読み込むToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file_read(openFileDialog1.FileName);
            }
            proces();
        }

        private void 結果を出力するToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "新しいファイル.txt";
            saveFileDialog1.Filter = "TXTファイル(*.txt)|*.txt|cppファイル(*.cpp)|*.cpp|すべてのファイル(*.*)|*.*";
            saveFileDialog1.FilterIndex = 3;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.Stream stream;
                stream = saveFileDialog1.OpenFile();
                if (stream != null)
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);
                    foreach(string s in richTextBox2.Lines)
                    {
                        sw.WriteLine(s);
                    }
                    
                    sw.Close();
                    stream.Close();
                }
            }
        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "ここにソースコードを入力")
            {
                richTextBox1.Text = "";
                timer1.Enabled = false;
            }
        }
        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "")
            {
                richTextBox1.Text = "ここにソースコードを入力";
                toolStripStatusLabel1.Text = "ファイルをドラックしても読み込めます";
                timer1.Enabled = false;
            }
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Enabled = true;

            if(richTextBox1.Text.Length != 0)
                toolStripStatusLabel1.Text = "行数：" + richTextBox1.Lines.Length + " \t" + "総文字数：" + richTextBox1.Text.Length;
            else
                toolStripStatusLabel1.Text = "ファイルをドラックしても読み込めます";
        }

        private void richTextBox2_Enter(object sender, EventArgs e)
        {
            if (richTextBox2.Text == "ここに翻訳文が表示されます")
            {
                richTextBox2.Text = "";
            }
        }
        private void richTextBox2_Leave(object sender, EventArgs e)
        {
            if (richTextBox2.Text == "")
            {
                richTextBox2.Text = "ここに翻訳文が表示されます";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            proces();
        }

        /// <summary>
        /// パスからtextboxに入力
        /// </summary>
        /// <param name="path">ファイルパス</param>
        private void file_read(string path)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(
                                                    path,
                                                    System.Text.Encoding.GetEncoding("shift_jis"));
            richTextBox1.Text = stream.ReadToEnd();
            stream.Close();

            timer1.Enabled = false;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            richTextBox1.Font = new Font("MS UI Gothic", trackBar1.Value);
            richTextBox2.Font = new Font("MS UI Gothic", trackBar1.Value);
            timer1.Enabled = false;
        }

        private void オフToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.timer_flg = false;
            label4.Text = "自動更新の設定：" + "オフ";

            Chack_change(sender);
        }

        private void オンToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.timer_flg = true;
            label4.Text = "自動更新の設定：" + "オン";

            Chack_change(sender);
        }

        /// チェックの切り替え
        private void Chack_change(object sender)
        {
            foreach (ToolStripMenuItem item in 自動翻訳ToolStripMenuItem.DropDownItems)
            {
                if (item.Equals(sender))
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
            }
        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// メイン処理
        /// </summary>
        private void proces()
        {
            read_line = 0;
            work_line = 0;
            richTextBox2.Text = "";
            richTextBox2.SelectionLength = 0;
            while (richTextBox1.Lines.Length > read_line)
            {
                work_line = read_line;
                if (richTextBox1.Lines[read_line].IndexOf("//") >= 0)
                    richTextBox2.SelectedText = richTextBox1.Lines[read_line].Remove(richTextBox1.Lines[read_line].IndexOf("//"));
                else
                    richTextBox2.SelectedText = richTextBox1.Lines[read_line];
                richTextBox2.SelectionColor = Color.Green;
                if(Analyse_C.Analyse.hantei(richTextBox1.Lines[read_line]) != "")
                    richTextBox2.SelectedText = "       //" + Analyse_C.Analyse.hantei(richTextBox1.Lines[read_line]).Replace("\t", "");
                richTextBox2.SelectedText = "\r\n";
                read_line++;

            }

            timer1.Enabled = false;
        }
    }
}
