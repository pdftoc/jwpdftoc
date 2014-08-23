using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.Net;

using Ionic.Zip;
using Ionic.Zlib;

namespace jwpubtoc
{
    public partial class MainForm : Form
    {
        private static string app_path;
        private static string src_path;
        private static string dest_path;
        private static string toc_path;
        private static string tmp_path;
        private List<Dictionary<string, string>> languageList;
        private List<List<Dictionary<string, string>>> languageBookList;

        //private static MemoryStream requestData;
        //private static byte[] bufferData;
        private static string download_filename;
        private static int download_filetype; // 1.jpdfbookmarks 2.PDF
        private static long download_length;
        private static DateTime download_lastmodified;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            app_path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            src_path = app_path + "\\jw_org";
            dest_path = app_path + "\\output";
            toc_path = app_path + "\\toc";
            tmp_path = app_path + "\\tmp";
            languageList = new List<Dictionary<string, string>>();
            languageBookList = new List<List<Dictionary<string, string>>>();

            init_dir();
            init_publication();

            if (read_xml() != 0)
            {
                return;
            }
            load_language();
            load_publication(false);
        }

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_publication(false);
        }

        private void PublicationListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            int langage_index = LanguageComboBox.SelectedIndex;
            int book_index = -1;
            if (langage_index >= 0)
            {
                if (PublicationListView.SelectedItems.Count > 0)
                {
                    book_index = PublicationListView.SelectedItems[0].Index;
                }
            }
            if (book_index == -1)
            {
                DisableTocButton();
            }
            else
            {
                if (!File.Exists(toc_path + "\\" + languageBookList[langage_index][book_index]["txt"]))
                {
                    DisableTocButton();
                }
                else if (!File.Exists(src_path + "\\" + languageBookList[langage_index][book_index]["pdf"]))
                {
                    DisableTocButton();
                }
                else
                {
                    EnableTocButton();
                }
            }
        }

        private void PublicationListView_DoubleClick(object sender, EventArgs e)
        {
            int langage_index = LanguageComboBox.SelectedIndex;
            int book_index = -1;
            if (langage_index >= 0)
            {
                if (PublicationListView.SelectedItems.Count > 0)
                {
                    book_index = PublicationListView.SelectedItems[0].Index;
                }
            }
            open_output_dir(languageBookList[langage_index][book_index]["pdf"]);
        }

        private void open_output_dir(string filename)
        {
            string arg = "";
            string pdf = dest_path + "\\" + filename;
            if (File.Exists(pdf))
            {
                arg = "/select,\"" + pdf + "\"";
            }
            else
            {
                arg = "\"" + dest_path + "\"";
            }
            Process.Start("explorer.exe", arg);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            load_publication(true);
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            int langage_index = LanguageComboBox.SelectedIndex;
            if (langage_index < 0)
            {
                MessageBox.Show("Language is not selected.");
                return;
            }

            if (PublicationListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Publication is not selected.");
                return;
            }
            int book_index = PublicationListView.SelectedItems[0].Index;

            if (DownloadBackgroundWorker.IsBusy)
            {
                return;
            }

            // download
            string url = languageBookList[langage_index][book_index]["url"];
            download_filename = languageBookList[langage_index][book_index]["pdf"];
            if (head_url(url) != 0)
            {
                MessageBox.Show("Download " + download_filename + " Failed.");
                toolStripStatusLabel1.Text = "Download " + download_filename + " Failed.";
                return;
            }

            try
            {
                if (File.Exists(src_path + "\\" + download_filename))
                {
                    DateTime cur = File.GetLastWriteTime(src_path + "\\" + download_filename);
                    int a = cur.CompareTo(download_lastmodified);
                    if (cur.CompareTo(download_lastmodified) >= 0)
                    {
                        MessageBox.Show("Exist file is up to date.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("read_xml Exception raised!");
                Debug.WriteLine("Source:" + ex.Source);
                Debug.WriteLine("Message:" + ex.Message);
            }

            DisableDownloadButton();
            DisableTocButton();

            download_filetype = 2;
            DownloadBackgroundWorker.WorkerReportsProgress = true;
            DownloadBackgroundWorker.WorkerSupportsCancellation = true;
            DownloadBackgroundWorker.RunWorkerAsync(url);
        }

        private void TocButton_Click(object sender, EventArgs e)
        {
            if (init_jpdfbookmarks() == 0)
            {
                return;
            }

            bool use_name = true;
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                use_name = false;
            }

            int langage_index = LanguageComboBox.SelectedIndex;
            if (langage_index < 0)
            {
                MessageBox.Show("Language is not selected.");
                return;
            }
            if (PublicationListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Publication is not selected.");
                return;
            }

            int book_index = PublicationListView.SelectedItems[0].Index;

            string src = languageBookList[langage_index][book_index]["pdf"];
            string toc = languageBookList[langage_index][book_index]["txt"];
            string dest = languageBookList[langage_index][book_index]["pdf"];
            if (use_name)
            {
                dest = languageBookList[langage_index][book_index]["name"] + ".pdf";
            }

            try
            {
                if (!File.Exists(src_path + "\\" + src))
                {
                    MessageBox.Show("Source file not found.");
                    return;
                }
                if (!File.Exists(toc_path + "\\" + toc))
                {
                    MessageBox.Show("TOC file not found.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("read_xml Exception raised!");
                Debug.WriteLine("Source:" + ex.Source);
                Debug.WriteLine("Message:" + ex.Message);
            }

            if (TocBackgroundWorker.IsBusy)
            {
                return;
            }

            DisableDownloadButton();
            DisableTocButton();

            List<string> param = new List<string>();
            param.Add(app_path);
            param.Add(src);
            param.Add(toc);
            param.Add(dest);

            TocBackgroundWorker.WorkerReportsProgress = true;
            TocBackgroundWorker.WorkerSupportsCancellation = true;
            TocBackgroundWorker.RunWorkerAsync(param);
        }

        private void init_dir()
        {
            if (!Directory.Exists(src_path))
            {
                Directory.CreateDirectory(src_path);
            }
            if (!Directory.Exists(tmp_path))
            {
                Directory.CreateDirectory(tmp_path);
            }
            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }
        }

        /* download and extract jpdfbookmarks */
        private int init_jpdfbookmarks()
        {
            if (File.Exists(app_path + "\\jpdfbookmarks\\jpdfbookmarks_cli.exe"))
            {
                return 1;
            }

            if (MessageBox.Show("jpdfbookmarks not found. Download?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return 0;
            }

            DisableDownloadButton();
            DisableTocButton();

            // download
            string url = "http://sourceforge.net/projects/jpdfbookmarks/files/JPdfBookmarks-2.5.2/jpdfbookmarks-2.5.2.zip/download";
            download_filename = "jpdfbookmarks-2.5.2.zip";
            if (head_url(url) != 0)
            {
                MessageBox.Show("Download " + download_filename + " Failed.");
                toolStripStatusLabel1.Text = "Download " + download_filename + " Failed.";
            }
            download_filetype = 1;
            DownloadBackgroundWorker.WorkerReportsProgress = true;
            DownloadBackgroundWorker.WorkerSupportsCancellation = true;
            DownloadBackgroundWorker.RunWorkerAsync(url);
#if false
            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(url);
            IAsyncResult r = (IAsyncResult)webreq.BeginGetResponse(new AsyncCallback(HTTPResponseCallback), webreq);
#endif
#if false
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                RequestState myRequestState = new RequestState();
                myRequestState.request = myHttpWebRequest;
                IAsyncResult result = (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(HttpRespCallback), myRequestState);
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), myHttpWebRequest, DefaultTimeout, true);
                allDone.WaitOne();
                myRequestState.response.Close();
            }
            catch (WebException e)
            {
                Debug.WriteLine("init_jpdfbookmarks Exception raised!");
                Debug.WriteLine("Message:" + e.Message);
                Debug.WriteLine("nStatus:" + e.Status.ToString());
                MessageBox.Show("Download failed.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("init_jpdfbookmarks Exception raised!");
                Debug.WriteLine("Source:" + e.Source);
                Debug.WriteLine("Message:" + e.Message);
                MessageBox.Show("Download failed.");
            }
#endif
            return 0;
        }

        private void post_jpdfbookmarks_download()
        {
            // extract
            ZipFile zip = new ZipFile(tmp_path + "\\jpdfbookmarks-2.5.2.zip");
            zip.ExtractAll(tmp_path, ExtractExistingFileAction.OverwriteSilently);
            zip.Dispose();
            Directory.Move(tmp_path + "\\jpdfbookmarks-2.5.2", app_path + "\\jpdfbookmarks");
        }

        private void post_pdf_download()
        {
            if (File.Exists(src_path + "\\" + download_filename))
            {
                DateTime dt = File.GetLastWriteTime(src_path + "\\" + download_filename);
                string dest_filename = Path.GetFileNameWithoutExtension(download_filename) + "_" + dt.ToString("yyyyMMdd_HHmmss") + Path.GetExtension(download_filename);
                File.Move(src_path + "\\" + download_filename, src_path + "\\" + dest_filename);
            }
            File.Move(tmp_path + "\\" + download_filename, src_path + "\\" + download_filename);
        }

        private int read_xml()
        {
            string xml_file = app_path + "\\" + "publication.xml";
            XmlDocument xml = new XmlDocument();
            XmlElement root;

            try
            {
                xml.Load(xml_file);
            }
            catch (Exception e)
            {
                Debug.WriteLine("read_xml Exception raised!");
                Debug.WriteLine("Source:" + e.Source);
                Debug.WriteLine("Message:" + e.Message);
                MessageBox.Show("Cannot read XML file.");
                return -1;
            }

            try
            {
                root = xml.DocumentElement;
                foreach (XmlElement languageNode in root.ChildNodes)
                {
                    Dictionary<string, string> lang = new Dictionary<string, string>();
                    lang.Add("name", languageNode.GetAttribute("name"));
                    lang.Add("lang", languageNode.GetAttribute("lang"));
                    languageList.Add(lang);

                    List<Dictionary<string, string>> bookList = new List<Dictionary<string, string>>();

                    foreach (XmlElement bookNode in languageNode.ChildNodes)
                    {
                        Dictionary<string, string> book = new Dictionary<string, string>();
                        XmlNodeList nodelist;
                        nodelist = bookNode.GetElementsByTagName("name");
                        if (nodelist.Count == 0)
                        {
                            throw new Exception();
                        }
                        book.Add("name", nodelist.Item(0).InnerText);
                        nodelist = bookNode.GetElementsByTagName("url");
                        if (nodelist.Count == 0)
                        {
                            throw new Exception();
                        }
                        book.Add("url", nodelist.Item(0).InnerText);
                        string file = Path.GetFileNameWithoutExtension(book["url"]);
                        book.Add("sign", file);
                        book.Add("pdf", file + ".pdf");
                        book.Add("txt", file + ".txt");

                        bookList.Add(book);
                    }

                    languageBookList.Add(bookList);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("read_xml Exception raised!");
                Debug.WriteLine("Source:" + e.Source);
                Debug.WriteLine("Message:" + e.Message);
                MessageBox.Show("Cannot parse XML file.");
                return -1;
            }

            return 0;
        }

        /* set languages to ComboBox */
        private void load_language()
        {
            foreach (Dictionary<string, string> language in languageList)
            {
                LanguageComboBox.Items.Add(language["name"]);
            }
            if (languageList.Count > 0)
            {
                LanguageComboBox.SelectedIndex = 0;
            }
        }

        /* initialize ListView */
        private void init_publication()
        {
            ColumnHeader column_book = new ColumnHeader();
            ColumnHeader column_sign = new ColumnHeader();
            ColumnHeader column_src = new ColumnHeader();
            ColumnHeader column_toc = new ColumnHeader();
            ColumnHeader column_dest = new ColumnHeader();

            column_book.Text = "Publication";
            column_sign.Text = "Sign";
            column_src.Text = "Orig";
            column_toc.Text = "TOC text";
            column_dest.Text = "Output";
#if true
            column_book.Width = 330;
            column_sign.Width = 60;
            column_src.Width = 40;
            column_toc.Width = 60;
            column_dest.Width = 50;
#else
            column_book.Width = -2;
            column_src.Width = -2;
            column_toc.Width = -2;
            column_dest.Width = -2;
#endif
            column_src.TextAlign = HorizontalAlignment.Center;
            column_toc.TextAlign = HorizontalAlignment.Center;
            column_dest.TextAlign = HorizontalAlignment.Center;
            ColumnHeader[] colHeaderRegValue = { column_book, column_sign, column_src, column_toc, column_dest };
            PublicationListView.Columns.AddRange(colHeaderRegValue);
        }

        /* set publications to ListView */
        private void load_publication(bool select)
        {
            int langage_index = LanguageComboBox.SelectedIndex;
            if (langage_index < 0)
            {
                MessageBox.Show("Language is not selected.");
                return;
            }

            int index = -1;
            if (PublicationListView.SelectedItems.Count > 0)
            {
                index = PublicationListView.SelectedItems[0].Index;
            }

            PublicationListView.Items.Clear();
            foreach (Dictionary<string, string> book in languageBookList[langage_index])
            {
                string toc = "o";
                string src = "o";
                string dest = "o";

                // check toc file
                if (!File.Exists(toc_path + "\\" + book["txt"]))
                {
                    toc = "x";
                }

                // check source file
                if (!File.Exists(src_path + "\\" + book["pdf"]))
                {
                    src = "x";
                }

                // check destination file
                if (!File.Exists(dest_path + "\\" + book["pdf"]) &&
                    !File.Exists(dest_path + "\\" + book["name"] + ".pdf"))
                {
                    dest = "x";
                }

                ListViewItem item = new ListViewItem(new string[] { book["name"], book["sign"], src, toc, dest });
                PublicationListView.Items.Add(item);
            }

            if (select && index > 0 && PublicationListView.Items.Count >= index)
            {
                PublicationListView.Items[index].Selected = true;
            }

            // PublicationListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private int head_url(string url)
        {
            HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(url);
            webreq.Method = "HEAD";
            HttpWebResponse webres;
            try
            {
                webres = (HttpWebResponse)webreq.GetResponse();
            }
            catch (WebException e)
            {
                Debug.WriteLine("head_url Exception raised!");
                Debug.WriteLine("Message:" + e.Message);
                Debug.WriteLine("nStatus:" + e.Status.ToString());
                return 1;
            }
            download_lastmodified = webres.LastModified;
            download_length = webres.ContentLength;
            webres.Close();
            return 0;
        }

        private void TocBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            List<string> param = (List<string>)e.Argument;
            string app_path = param[0];
            string src = param[1];
            string toc = param[2];
            string dest = param[3];

            bgWorker.ReportProgress(0); // "Converting..."

            string jpdfboomarks = app_path + "\\jpdfbookmarks\\jpdfbookmarks_cli.exe";
            string src_file = src_path + "\\" + src;
            string dest_file = dest_path + "\\" + dest;
            string toc_file = toc_path + "\\" + toc;
            string tmp_file = tmp_path + "\\" + src;

            if (!File.Exists(jpdfboomarks))
            {
                MessageBox.Show("jpdfbookmarks_cli.exe not found.");
                e.Result = 2;
                return;
            }

            if (!File.Exists(toc_file))
            {
                MessageBox.Show("TOC file not found.");
                e.Result = 2;
                return;
            }

            if (!File.Exists(src_file))
            {
                MessageBox.Show("Source file not found.");
                e.Result = 2;
                return;
            }

            if (File.Exists(dest_file))
            {
                if (MessageBox.Show(dest + " is exist. Overwrite exist file?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Result = 2;
                    return;
                }
            }

            if (File.Exists(tmp_file))
            {
                File.Delete(tmp_file);
            }

            ProcessStartInfo psInfo = new ProcessStartInfo();
            psInfo.FileName = jpdfboomarks;
            psInfo.CreateNoWindow = true;
            psInfo.UseShellExecute = false;
            psInfo.Arguments = "\"" + src_file + "\" -e UTF-8 -a \"" + toc_file + "\" -o \"" + tmp_file + "\"";

            Process myProcess = Process.Start(psInfo);
            myProcess.WaitForExit();
            if (myProcess.ExitCode != 0)
            {
                if (File.Exists(tmp_file))
                {
                    File.Delete(tmp_file);
                }
                e.Result = 1;
                return;
            }

            if (File.Exists(dest_file))
            {
                File.Delete(dest_file);
            }
            File.Move(tmp_file, dest_file);
            File.SetLastWriteTime(dest_file, File.GetLastWriteTime(src_file));

            e.Result = 0;
        }

        private void TocBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                toolStripStatusLabel1.Text = "Converting...";
            }
        }

        private void TocBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                toolStripStatusLabel1.Text = "Convert failed.";
            }
            else
            {
                int result = (int)e.Result;

                if (result == 0)
                {
                    toolStripStatusLabel1.Text = "Convert complete.";
                    open_output_dir("");
                }
                else if (result == 1)
                {
                    toolStripStatusLabel1.Text = "Convert failed.";
                }
                else
                {
                    toolStripStatusLabel1.Text = "";
                }
            }

            load_publication(true);

            EnableDownloadButton();
            EnableTocButton();
        }

        private void DownloadBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            string url = (string)e.Argument;

            try
            {
                HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();
                Stream st = webres.GetResponseStream();
                FileStream fs = new FileStream(tmp_path + "\\" + download_filename, FileMode.Create);
                //requestData = new System.IO.MemoryStream();
                byte[] bufferData = new byte[1024];
                int readSize;

                long percent = -1;
                long total_length = 0;
                long tmp_length = 0;
                string status;
                while ((readSize = st.Read(bufferData, 0, bufferData.Length)) > 0)
                {
                    //requestData.Write(bufferData, 0, readSize);
                    fs.Write(bufferData, 0, readSize);
                    total_length += readSize;
                    //if (requestData.Length < tmp_length + 25600) // 25KB
                    if (total_length < tmp_length + 25600) // 25KB
                    {
                        continue;
                    }
                    //tmp_length = requestData.Length;
                    tmp_length = total_length;

                    if (download_length != -1)
                    {
                        //percent = (requestData.Length * 100) / download_length;
                        percent = (total_length * 100) / download_length;
                        //status = "Downloading " + download_filename + " " + String.Format("{0:#,0}", requestData.Length) + "bytes. " +
                        status = "Downloading " + download_filename + " " + String.Format("{0:#,0}", total_length) + "bytes. " +
                                 "(" + percent.ToString() + "%)";
                    }
                    else
                    {
                        //status = "Downloading " + download_filename + " " + String.Format("{0:#,0}", requestData.Length) + "bytes.";
                        status = "Downloading " + download_filename + " " + String.Format("{0:#,0}", total_length) + "bytes.";
                    }
                    bgWorker.ReportProgress((int)percent, status);
                }

                st.Close();

                //FileStream fs = new FileStream(tmp_path + "\\" + download_filename, FileMode.Create);
                //byte[] bytes = new byte[requestData.Length];
                //requestData.Seek(0, SeekOrigin.Begin);
                //requestData.Read(bytes, 0, bytes.Length);
                //fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                File.SetLastWriteTime(tmp_path + "\\" + download_filename, download_lastmodified);

                //requestData.Close();
                webres.Close();

                e.Result = 0;
            }
            catch (WebException ex)
            {
                Debug.WriteLine("DownloadBackgroundWorker_DoWork Exception raised!");
                Debug.WriteLine("Message:" + ex.Message);
                Debug.WriteLine("nStatus:" + ex.Status.ToString());
                MessageBox.Show("Download failed.");
                e.Result = -1;
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadBackgroundWorker_DoWork Exception raised!");
                Debug.WriteLine("Source:" + ex.Source);
                Debug.WriteLine("Message:" + ex.Message);
                e.Result = -1;
                return;
            }
        }

        private void DownloadBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string status = (string)e.UserState;
            toolStripStatusLabel1.Text = status;
        }

        private void DownloadBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                toolStripStatusLabel1.Text = "Download " + download_filename + " failed.";
                MessageBox.Show("Download " + download_filename + " failed.");
            }
            else
            {
                int result = (int)e.Result;
                if (result == 0)
                {
                    toolStripStatusLabel1.Text = "Download " + download_filename + " complete.";

                    if (download_filetype == 1)
                    {
                        post_jpdfbookmarks_download();
                    }
                    else if (download_filetype == 2)
                    {
                        post_pdf_download();
                        load_publication(true);
                    }
                }
                else
                {
                    toolStripStatusLabel1.Text = "Download " + download_filename + " failed.";
                    MessageBox.Show("Download " + download_filename + " failed.");
                }
            }
            EnableDownloadButton();
            EnableTocButton();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                load_publication(true);
            }
        }

        private void EnableDownloadButton()
        {
            DownloadButton.Enabled = true;
        }

        private void DisableDownloadButton()
        {
            DownloadButton.Enabled = false;
        }

        private void EnableTocButton()
        {
            TocButton.Enabled = true;
        }

        private void DisableTocButton()
        {
            TocButton.Enabled = false;
        }

    }
}