//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//namespace SocketStudyC
//{
//    public partial class ClientUI : Form
//    {
//        #region Properties
//        private Thread DetectInfoReceived { get; set; }
//        #endregion
//        #region Methods

//        private void ListenToInfoReceived()
//        {
//            try
//            {
//                var clientEndPoint = ClientSocket.RemoteEndPoint as IPEndPoint;
//                if (clientEndPoint != null)
//                {
//                    while (true)
//                    {
//                        byte[] receivedBytes = new byte[ClientSocket.ReceiveBufferSize];
//                        int count = 0;
//                        try
//                        {
//                            count = ClientSocket.Receive(receivedBytes);
//                        }
//                        catch(SocketException se)
//                        {
//                            MessageBox.Show(se.Message);
//                            break;
//                        }
//                        catch(Exception ex)
//                        {
//                            Invoke(new Action(() => TextBox_ChatWindow.AppendText($"Connetion lost: {ex}{Environment.NewLine}")));
//                            break;
//                        }
//                        if (count > 0)
//                        {
//                            string receivedMessage = Encoding.UTF8.GetString(receivedBytes, 0, count);
//                            Invoke(new Action(()=>TextBox_ChatWindow.AppendText($"[From] '{ServerIP}:{ServerPort}': {receivedMessage}{Environment.NewLine}")));
//                        }
//                    }
//                }
//            }
//            catch (ThreadInterruptedException) { }
//        }
//        private void Button_ConnectionStart_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                TextBox_ChatWindow.AppendText($"Connecting to server '{ServerIP}:{ServerPort}'{Environment.NewLine}");
//                ClientSocket.Connect(ServerEP);
//                MessageBox.Show("Server connect success.");
//                Button_ConnectionStart.Enabled = false;
//                TextBox_LocalName.Enabled = false;
//                DetectInfoReceived.Start();
//            }
//            catch(Exception ex)
//            {
//                MessageBox.Show($"Failed to connect server: Encounter excption '{ex}'.");
//            }
//        }

//        private void Button_Send_Click(object sender, EventArgs e)
//        {
//            string messageToSend = TextBox_MessageToSend.Text;
//            byte[] bytesToSend = Encoding.UTF8.GetBytes($"{TextBox_LocalName.Text}: {messageToSend}");
//            ClientSocket.Send(bytesToSend);
//            Invoke(new Action(() => TextBox_ChatWindow.AppendText($"[To] '{ServerIP}:{ServerPort}': {messageToSend}{Environment.NewLine}")));
//            TextBox_MessageToSend.Clear();
//        }
//        #endregion
//    }
//}
