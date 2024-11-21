using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace SocketStudy
{
    public partial class ServerUI : Form
    {
        #region Properties
        private ConcurrentDictionary<string,Socket> ConnectedSockets { get; }
        private bool IslisteningToClient { get; set; } = false;
        private bool IslisteningToInfo { get; set; } = false;
        private CancellationTokenSource TokenSource { get; }

        delegate void UpdateInfo(string info, bool remove = false);
        private UpdateInfo UpdateUserInfo { get; set; }
        private UpdateInfo UpdateChatWindowInfo { get; set; }
        private Socket ServerSocket { get; set; }

        private int MaxUserCount { get { return Convert.ToInt32(UpDown_MaxUserCount.Value); } }

        private IPAddress LocalIP { get { return IPAddress.Parse(TextBox_LocalIP.Text.Trim()); } }
        private int LocalPort { get { return Convert.ToInt32(TextBox_LocalPort.Text.Trim()); } }
        private IPEndPoint LocalEP { get { return new IPEndPoint(LocalIP, LocalPort); } }
        #endregion
        #region Methods
        public ServerUI()
        {
            InitializeComponent();
            ServerSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = 1024 * 1024 * 5
            };
            ConnectedSockets = new();
            TokenSource = new();
            ////Waiting to trigger
            UpdateUserInfo += ChangeUserInfo;
            UpdateChatWindowInfo += ChangeChatWindowInfo;

            TextBox_LocalIP.Text = "127.0.0.1";
            TextBox_LocalPort.Text = "123";
        }
        private async void OnButton_ServiceStartClicked(object sender, EventArgs e)
        {
            Button_ServiceStart.Enabled = false;
            UpDown_MaxUserCount.Enabled = false;
            var token = TokenSource.Token;
            await StartServiceAsync(token);
        }
        private async void OnButton_SendSelectedClicked(object sender, EventArgs e)
        {
            string messageToSend = TextBox_MessageToSend.Text;
            byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
            if (ListBox_UserInfo.SelectedItems.Count == 0)
            {
                MessageBox.Show("Detected invalid choice, please choose send target first.");
                return;
            }
            else
            {
                foreach (string item in ListBox_UserInfo.SelectedItems)
                {
                    await ConnectedSockets[item].SendAsync(bytesToSend);
                    UpdateChatWindowInfo?.Invoke($"[To] {item}: {messageToSend}");
                }
                TextBox_MessageToSend.Clear();
            }
        }
        private async void OnButton_SendAllClicked(object sender, EventArgs e)
        {
            string messageToSend = TextBox_MessageToSend.Text;
            byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
            foreach (string item in ListBox_UserInfo.Items)
            {
                await ConnectedSockets[item].SendAsync(bytesToSend);
                UpdateChatWindowInfo?.Invoke($"[To] {item}: {messageToSend}");
            }
            TextBox_MessageToSend.Clear();
        }
        private async Task StartServiceAsync(CancellationToken token)
        {
            try
            {
                ServerSocket.Bind(LocalEP);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start service: Encounter excption '{ex}'.");
            }
            finally
            {
                ServerSocket.Listen(MaxUserCount);
                MessageBox.Show("Service start success.");
                IslisteningToClient = true;
                await ListenToClientConnectionAsync(token);
                MessageBox.Show("Service ended.");
            }
        }
        private async Task ListenToClientConnectionAsync(CancellationToken token)
        {
            while (IslisteningToClient)
            {
                try
                {
                    Socket acceptSocket = await ServerSocket.AcceptAsync(token);
                    if (acceptSocket.RemoteEndPoint is IPEndPoint clientEndpoint)
                    {
                        ConnectedSockets.TryAdd(clientEndpoint.ToString(), acceptSocket);
                        UpdateUserInfo?.Invoke(clientEndpoint.ToString());
                        IslisteningToInfo = true;
                        _ = Task.Run(() => ListenToInfoReceiveAsync(acceptSocket,token), token);//Fire and forget
                    };
                }
                catch (Exception) { }
                
            }

        }
        private async Task ListenToInfoReceiveAsync(Socket socket,CancellationToken token)
        {
            if (socket.RemoteEndPoint is IPEndPoint clientEndPoint)
            {
                while (IslisteningToInfo)
                {
                    byte[] receivedBytes = new byte[ServerSocket.ReceiveBufferSize];
                    int count = 0;
                    try
                    {
                        count = await socket.ReceiveAsync(receivedBytes,token);
                    }
                    catch(TaskCanceledException)
                    {
                        UpdateChatWindowInfo?.Invoke($"{clientEndPoint} off line.");
                        UpdateUserInfo?.Invoke(clientEndPoint.ToString(), true);
                        var socketToRemove = ConnectedSockets.FirstOrDefault(s => s.Key == clientEndPoint.ToString());
                        ConnectedSockets.TryRemove(socketToRemove);
                        break;
                    }
                    if (count == 0)
                    {
                        UpdateChatWindowInfo?.Invoke($"{clientEndPoint} off line.");
                        UpdateUserInfo?.Invoke(clientEndPoint.ToString(), true);
                        var socketToRemove = ConnectedSockets.FirstOrDefault(s => s.Key == clientEndPoint.ToString());
                        ConnectedSockets.TryRemove(socketToRemove);
                        break;
                    }
                    else if (count > 0)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(receivedBytes, 0, count);
                        UpdateChatWindowInfo?.Invoke($"[From] {clientEndPoint} | {receivedMessage}");
                    }
                }
            }
        }
        
        private void ChangeUserInfo(string info, bool remove = false)
        {
            if (ListBox_UserInfo.InvokeRequired)
            {
                ListBox_UserInfo.Invoke(new Action<string, bool>(ChangeUserInfo), info, remove);
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
        private void ChangeChatWindowInfo(string info, bool remove = false)
        {
            if (TextBox_ChatWindow.InvokeRequired)
            {
                TextBox_ChatWindow.Invoke(new Action<string, bool>(ChangeChatWindowInfo), info, remove);
            }
            else
            {
                TextBox_ChatWindow.AppendText($"{info}{Environment.NewLine}");
            }
        }
        #endregion
    }
}
