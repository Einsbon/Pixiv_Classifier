using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace pixivClassifier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string imgPath;
        string pastePath;
        int count = 0;
        bool working = false;

        private void btnClassify_Click(object sender, EventArgs e)
        {
            StringReader strReader = new StringReader(txtTag.Text);
            List<string> userTags = new List<string>();
            string line;
            while (true)
            {
                line = strReader.ReadLine();
                if(line != null)
                {
                    userTags.Add(line);
                }
                else
                {
                    break;
                }
            }
            strReader.Dispose();

            if (Directory.Exists(imgPath)& Directory.Exists(pastePath))
            {
                working = true;
                progressBar1.Enabled = true;
                DirectoryInfo di = new DirectoryInfo(imgPath);
                int fileCount = di.GetFiles().Length;
                progressBar1.Step = fileCount;
                int workCount = 0;
                foreach(var img in di.GetFiles())
                {
                    string imgNumStr = img.Name.Substring(0, img.Name.IndexOf("_"));
                    webBrowser1.Navigate("https://www.pixiv.net/member_illust.php?mode=medium&illust_id="+imgNumStr);
                    while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                    {
                        Application.DoEvents();
                    }
                    
                    HtmlDocument hd = webBrowser1.Document;
                    HtmlElementCollection hecTags = hd.GetElementsByTagName("li");
                    List<string> tagList = new List<string>();
                    foreach (HtmlElement he in hecTags)
                    {

                        if (he.GetAttribute("className").ToString() == "tag")
                        {
                            tagList.Add(he.Children[1].InnerHtml);
                        }
                    }

                    bool move = false;
                    if(radioButton3.Checked == true) //하나 이상을 포함
                    {
                        foreach(string item in userTags)
                        {
                            if (tagList.Contains(item))
                            {
                                move = true;
                            }
                        }
                    }
                    else //전체를 포함
                    {
                        move = true;
                        foreach(string item in userTags)
                        {
                            if (!tagList.Contains(item))
                            {
                                move = false;
                            }
                        }
                    }

                    if(move == true)
                    {
                        if (radioButton1.Checked == false)
                        {
                            img.MoveTo(pastePath + "\\" + Path.GetFileName(img.FullName));
                        }
                        else
                        {
                            img.CopyTo(pastePath + "\\" + Path.GetFileName(img.FullName), true);
                        }
                    }
                    tagList.Clear();
                    progressBar1.PerformStep();
                    workCount++;
                    lblProgress.Text = "진행 상황: " + workCount + "/" + fileCount;

                    if(working == false)
                    {
                        break;
                    }
                }

                if (working == true)
                {
                    webBrowser1.DocumentText = "";
                    // webBrowser1.Navigate("about:blank");
                    userTags.Clear();
                    lblProgress.Text = "진행 상황: 완료됨";
                    progressBar1.Enabled = false;
                    MessageBox.Show("분류 완료");
                }

                
            }
            else
            {
                MessageBox.Show("대상 폴더, 복사/이동할 폴더의 경로를 설정하시오!");
            }
            
        }

        private void btnClassifyStop_Click(object sender, EventArgs e)
        {
            working = false;
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            if(this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                imgPath = this.folderBrowserDialog1.SelectedPath;
                label1.Text = imgPath;
            }
        }

        private void btnFolderPaste_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                pastePath = this.folderBrowserDialog1.SelectedPath;
                label2.Text = pastePath;
            }
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            count++;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.pixiv.net/");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == false)
            {
                webBrowser1.Visible = false;
            }
            else
            {
                webBrowser1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("픽시브 자동 분류 프로그램 입니다. 태그 이름으로 분류가 가능해요. " +
                "프로그램의 사용 방법은 다음 동영상 주소에 있습니다.\r\n\r\n" +
                "https://youtu.be/1ihgWXzztQQ" +
                "\r\n\r\n개발자 블로그: http://blog.naver.com/einsbon" +
                "\r\n 오류 지적 환영.");
        }
    }
}
