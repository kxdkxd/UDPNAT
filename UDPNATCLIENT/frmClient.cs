using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using UDPCOMMON;


namespace UDPNATCLIENT
{
    public partial class frmClient : Form
    {
        private Client _client;

        public frmClient()
        {
            InitializeComponent();
        }

        private void frmClient_Load(object sender, EventArgs e)
        {
            _client = new Client { OnWriteMessage = WriteLog, OnUserChanged = OnUserChanged };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _client.Login(textBox2.Text, "");
            _client.Start();
            C2S_ReadytoSendFileMessage msgReadyToSend = new C2S_ReadytoSendFileMessage(textBox2.Text);
            _client.SendMessageToHost(msgReadyToSend);
        }

        private void WriteLog(string msg)
        {
            listBox2.Items.Add(msg);
            listBox2.SelectedIndex = listBox2.Items.Count - 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_client != null)
            {
                User user = listBox1.SelectedItem as User;
                _client.HolePunching(user);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_client != null) _client.DownloadUserList();
        }

        private void frmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_client != null) _client.Logout();
        }

        private void OnUserChanged(UserCollection users)
        {
            listBox1.DisplayMember = "FullName";
            listBox1.DataSource = null;
            listBox1.DataSource = users;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            P2P_TalkMessage msg = new P2P_TalkMessage(textBox1.Text);
            User user = listBox1.SelectedItem as User;
            _client.SendMessage(msg, user);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }
            

        private void btn_sendfile_Click(object sender, EventArgs e)
        {
            const int FILE_BLOCK_SIZE = 1000; // 1kB per Block

            string filePath = tb_filepath.Text;
            int blockCount = (int)Math.Ceiling((double)(FileUtils.GetFileSize(filePath) / FILE_BLOCK_SIZE));
            User user = listBox1.SelectedItem as User;
            _client.DoWriteLog("FileSize: " + FileUtils.GetFileSize(filePath));
            _client.DoWriteLog("Total block to be transferred: " + blockCount.ToString());
            for(int i=0; i < blockCount; i++)
            {
                byte[] fileContent = FileUtils.GetFileData(filePath, i * FILE_BLOCK_SIZE, FILE_BLOCK_SIZE);
                _client.DoWriteLog("Transferring block " + i.ToString());
                _client.FileSending(fileContent, user, i);
            }
        }
    }
}
