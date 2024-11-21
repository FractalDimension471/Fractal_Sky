//using System.Text;
//using System.Net;
//using System.Net.Sockets;

//#nullable enable
//namespace SocketStudy
//{
//    public partial class ServerUI : Form
//    {
//        #region Properties
//        private bool IsServiceOn = false;
//        private bool IsStartingService = false;
//        private bool IsEndingService = false;
//        private Dictionary<string, Socket> Sockets { get; set; }
        

//        private Thread DetectClientConnetion { get; set; }
//        #endregion
//        #region Methods

//        private void CheakServiceStatus()
//        {
//            while (true)
//            {
//                if (IsServiceOn)
//                {
//                    if (IsStartingService)
//                    {
//                        try
//                        {
//                            ServerSocket.Bind(LocalEP);
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show($"Failed to start service: Encounter excption '{ex}'.");
//                        }
//                        finally
//                        {
//                            ServerSocket.Listen(MaxUserCount);
//                            DetectClientConnetion.Start();
//                            IsStartingService = false;
//                            MessageBox.Show("Service start success.");
//                        }
//                    }

//                }
//                else if (IsEndingService)
//                {
//                    try
//                    {
//                        DetectClientConnetion.Interrupt();
//                        DetectClientConnetion.Join();
//                        ServerSocket.Close();
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"Failed to end service: Encounter excption '{ex}'.");
//                    }
//                }
//            }
//        }
//        private void InitializeValue()
//        {
//            TextBox_LocalIP.Text = "127.0.0.1";
//            TextBox_LocalPort.Text = "123";
//        }
//        private void ListenToClientConnection()
//        {
//            try
//            {
//                while (true)
//                {
//                    Socket currentClientSocket = ServerSocket.Accept();

//                    if (currentClientSocket.RemoteEndPoint is IPEndPoint clientEndpoint)
//                    {
//                        Sockets.Add(clientEndpoint.ToString(), currentClientSocket);
//                        UpdateUserInfo?.Invoke(clientEndpoint.ToString());
//                    };
//                    Thread detectInfoReceived = new(ListenToInfoReceived)
//                    {
//                        IsBackground = true
//                    };//BG thread
//                    detectInfoReceived.Start(currentClientSocket);
//                    Thread.Sleep(50);
//                }
//            }
//            catch (ThreadInterruptedException) { }
//            finally
//            {
//                MessageBox.Show("Service ended.");
//                IsEndingService = false;
//            }
//        }
//        private void ListenToInfoReceived(object? sock)
//        {
//            try
//            {
//                if (sock is not Socket currentClientSocket)
//                {
//                    return;
//                }
//                if (currentClientSocket.RemoteEndPoint is IPEndPoint clientEndPoint)
//                {
//                    while (true)
//                    {
//                        byte[] receivedBytes = new byte[ServerSocket.ReceiveBufferSize];
//                        int count = 0;
//                        try
//                        {
//                            count = currentClientSocket.Receive(receivedBytes);
//                        }
//                        catch
//                        {
//                            UpdateChatWindowInfo?.Invoke($"{clientEndPoint.ToString()} off line.");
//                            UpdateUserInfo?.Invoke(clientEndPoint.ToString(), true);
//                            Sockets.Remove(clientEndPoint.ToString());
//                            break;
//                        }
//                        if (count == 0)
//                        {
//                            UpdateChatWindowInfo?.Invoke($"{clientEndPoint.ToString()} off line.");
//                            UpdateUserInfo?.Invoke(clientEndPoint.ToString(), true);
//                            Sockets.Remove(clientEndPoint.ToString());
//                            break;
//                        }
//                        else if (count > 0)
//                        {
//                            string receivedMessage = Encoding.UTF8.GetString(receivedBytes, 0, count);
//                            UpdateChatWindowInfo?.Invoke($"[From] {clientEndPoint} | {receivedMessage}");
//                        }
//                    }
//                }
//            }
//            catch (ThreadInterruptedException) { }
//        }
        
//        private void Button_ServiceStart_Click(object sender, EventArgs e)
//        {
//            IsServiceOn = true;
//            IsStartingService = true;
//            Button_ServiceStart.Enabled = false;
//            UpDown_MaxUserCount.Enabled = false;
//        }


//        private void Button_SendSelected_Click(object sender, EventArgs e)
//        {
//            string messageToSend = TextBox_MessageToSend.Text;
//            byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
//            if (ListBox_UserInfo.SelectedItems.Count == 0)
//            {
//                MessageBox.Show("Detected invalid choice, please choose send target first.");
//                return;
//            }
//            else
//            {
//                foreach (string item in ListBox_UserInfo.SelectedItems)
//                {
//                    Sockets[item].Send(bytesToSend);
//                    UpdateChatWindowInfo?.Invoke($"[To] {item}: {messageToSend}");
//                }
//                TextBox_MessageToSend.Clear();
//            }
//        }

//        private void Button_SendAll_Click(object sender, EventArgs e)
//        {
//            string messageToSend = TextBox_MessageToSend.Text;
//            byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
//            foreach (string item in ListBox_UserInfo.Items)
//            {
//                Sockets[item].Send(bytesToSend);
//                UpdateChatWindowInfo?.Invoke($"[To] {item}: {messageToSend}");
//            }
//            TextBox_MessageToSend.Clear();
//        }

//        #endregion
//    }
//}
