
namespace FS_Client
{
    public partial class UI : Form
    {
        #region 属性/Property
        private CancellationTokenSource CTS { get; }

        public delegate void UpdateInfo(string info, bool remove = false);
        public UpdateInfo AppendChatWindowInfo { get; private set; }
        public UpdateInfo AppendMessageToSendInfo { get; private set; }
        #endregion
        #region 方法/Method
        public UI()
        {
            InitializeComponent();
            CTS = new CancellationTokenSource();
            AppendChatWindowInfo += AppendingChatWindowInfo;
            AppendMessageToSendInfo += AppendingMessageToSendInfo;
            //默认为本机
            TextBox_ServerIP.Text = "127.0.0.1";
            TextBox_ServerPort.Text = "123";
        }
        private async void OnButton_SelectFileClicked(object sender, EventArgs e)
        {
            await SocketManager.Instance.PrepareFileAsync(CTS.Token);
        }
        private async void OnButton_ConnectionStartClicked(object sender, EventArgs e)
        {
            //初始化并链接
            SocketManager.Instance.Initialize(this, TextBox_ServerIP.Text, TextBox_ServerPort.Text);
            try
            {
                Button_ConnectionStart.Enabled = false;
                Button_Send.Enabled = true;
                Button_SelectFile.Enabled = true;
                await SocketManager.Instance.StartConnectAsync(CTS.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start service: '{ex}'");
            }
        }
        private async void OnButton_SendInfoClicked(object sender, EventArgs e)
        {
            await SocketManager.Instance.SendInfoAsync(TextBox_MessageToSend.Text, CTS.Token);
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
