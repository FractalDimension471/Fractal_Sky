using System.Net.Sockets;
using System.Text;

namespace FS_Server
{
    public partial class UI : Form
    {
        #region 属性/Property
        private CancellationTokenSource CTS { get; }
        public delegate void UpdateInfo(string info, bool remove = false);
        public UpdateInfo UpdateUserInfo { get; private set; }
        public UpdateInfo AppendChatWindowInfo { get; private set; }
        public UpdateInfo AppendMessageToSendInfo { get; private set; }
        #endregion
        #region 方法/Method
        public UI()
        {
            InitializeComponent();
            CTS = new CancellationTokenSource();
            UpdateUserInfo += UpdatingUserInfo;
            AppendChatWindowInfo += AppendingChatWindowInfo;
            AppendMessageToSendInfo += AppendingMessageToSendInfo;
            //默认为本机
            TextBox_LocalIP.Text = "127.0.0.1";
            TextBox_LocalPort.Text = "123";
        }

        private async void OnButton_SelectFileClicked(object sender, EventArgs e)
        {
            await SocketManager.Instance.PrepareFileAsync(CTS.Token);
        }
        private async void OnButton_ServiceStartClicked(object sender, EventArgs e)
        {
            //初始化并链接
            SocketManager.Instance.Initialize(this, TextBox_LocalIP.Text, TextBox_LocalPort.Text, UpDown_MaxUserCount.Value);
            try
            {
                Button_ServiceStart.Enabled = false;
                Button_SelectFile.Enabled = true;
                Button_SendSelected.Enabled = true;
                Button_SendAll.Enabled = true;
                await SocketManager.Instance.StartServiceAsync(CTS.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start service: '{ex}'");
            }
        }
        private async void OnButton_SendSelectedClicked(object sender, EventArgs e)
        {
            await SocketManager.Instance.SendInfoAsync(TextBox_MessageToSend.Text, ListBox_UserInfo, false, CTS.Token);
        }
        private async void OnButton_SendAllClicked(object sender, EventArgs e)
        {
            await SocketManager.Instance.SendInfoAsync(TextBox_MessageToSend.Text, ListBox_UserInfo, true, CTS.Token);
        }

        private void UpdatingUserInfo(string info, bool remove = false)
        {
            if (ListBox_UserInfo.InvokeRequired)
            {
                ListBox_UserInfo.Invoke(new Action<string, bool>(UpdatingUserInfo), info, remove);
            }
            else if (remove)
            {
                ListBox_UserInfo.Items.Remove(info);
            }
            else
            {
                ListBox_UserInfo.Items.Add(info);
            }
        }
        private void AppendingChatWindowInfo(string info, bool remove = false)
        {
            if (TextBox_ChatWindow.InvokeRequired)
            {
                TextBox_ChatWindow.Invoke(new Action<string, bool>(AppendingChatWindowInfo), info, remove);
            }
            else
            {
                TextBox_ChatWindow.AppendText($"{info}{Environment.NewLine}");
            }
        }
        private void AppendingMessageToSendInfo(string info, bool remove = false)
        {
            if (TextBox_MessageToSend.InvokeRequired)
            {
                TextBox_MessageToSend.Invoke(new Action<string, bool>(AppendingMessageToSendInfo), info, remove);
            }
            else
            {
                if (remove)
                {
                    TextBox_MessageToSend.Clear();
                }
                else
                {
                    TextBox_MessageToSend.AppendText($"{info}");
                }
            }
        }
        #endregion
    }
}
