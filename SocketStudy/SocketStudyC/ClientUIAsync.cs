using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace SocketStudyC
{
    public partial class ClientUI : Form
    {
        private CancellationTokenSource TokenSource { get; }
        private Socket ClientSocket { get; set; }
        private IPAddress ServerIP { get { return IPAddress.Parse(TextBox_ServerIP.Text.Trim()); } }
        private int ServerPort { get { return Convert.ToInt32(TextBox_ServerPort.Text.Trim()); } }
        private IPEndPoint ServerEP { get { return new IPEndPoint(ServerIP, ServerPort); } }
        private bool IslisteningToInfo { get; set; } = false;

        public ClientUI()
        {
            InitializeComponent();
            ClientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = 1024 * 1024 * 2
            };
            TokenSource = new();

            TextBox_ServerIP.Text = "127.0.0.1";
            TextBox_ServerPort.Text = "123";
        }
        private async void OnButton_ConnectionStartClicked(object sender, EventArgs e)
        {
            TextBox_ChatWindow.AppendText($"Connecting to server '{ServerIP}:{ServerPort}'{Environment.NewLine}");
            Button_ConnectionStart.Enabled = false;
            TextBox_LocalName.Enabled = false;
            var token = TokenSource.Token;
            await ConnectToServerAsync(token);
            
        }
        private async void OnButton_SendClicked(object sender, EventArgs e)
        {
            string messageToSend = TextBox_MessageToSend.Text;
            byte[] bytesToSend = Encoding.UTF8.GetBytes($"{TextBox_LocalName.Text}: {messageToSend}");
            var token = TokenSource.Token;
            await SendInfoAsync(bytesToSend, token);
            Invoke(new Action(() => TextBox_ChatWindow.AppendText($"[To] '{ServerIP}:{ServerPort}': {messageToSend}{Environment.NewLine}")));
            TextBox_MessageToSend.Clear();
        }
        private async Task SendInfoAsync(byte[] info,CancellationToken token)
        {
            try
            {
                await ClientSocket.SendAsync(info, token);
            }
            catch (TaskCanceledException) { }
        }
        private async Task ConnectToServerAsync(CancellationToken token)
        {
            try
            {
                await ClientSocket.ConnectAsync(ServerEP,token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect server: Encounter excption '{ex}'.");
            }
            finally
            {
                MessageBox.Show("Server connect success.");
                IslisteningToInfo = true;
                _ = Task.Run(() => ListenToInfoReceivedAsync(token), token);
            }
        }
        private async Task ListenToInfoReceivedAsync(CancellationToken token)
        {
            try
            {
                if (ClientSocket.RemoteEndPoint is IPEndPoint clientEndPoint)
                {
                    while (IslisteningToInfo)
                    {
                        byte[] receivedBytes = new byte[ClientSocket.ReceiveBufferSize];
                        int count = 0;
                        try
                        {
                            count = await ClientSocket.ReceiveAsync(receivedBytes, token);
                        }
                        catch(TaskCanceledException) { }
                        catch (SocketException se)
                        {
                            MessageBox.Show(se.Message);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Invoke(new Action(() => TextBox_ChatWindow.AppendText($"Connetion lost: {ex}{Environment.NewLine}")));
                            break;
                        }
                        if (count > 0)
                        {
                            string receivedMessage = Encoding.UTF8.GetString(receivedBytes, 0, count);
                            Invoke(new Action(() => TextBox_ChatWindow.AppendText($"[From] '{ServerIP}:{ServerPort}': {receivedMessage}{Environment.NewLine}")));
                        }
                    }
                }
            }
            catch (TaskCanceledException) { }
        }
    }
}