﻿using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FS_Server
{
    internal class SocketManager
    {
        #region 属性/Property
        //单例模式
        private static readonly object _lock = new();
        public static SocketManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new();
                    }
                }
                return _instance;
            }
        }
        private static SocketManager? _instance = null;

        private Socket? ActiveSocket { get; set; }
        private IPEndPoint? ActiveEndPoint { get; set; }
        private UI? ActiveUI { get; set; }
        private int MaxListenCount {  get; set; }
        private byte[]? FileData { get; set; }

        private Dictionary<string, Socket> ConnectedSockets { get; }
        #endregion
        #region 方法/Method
        private SocketManager()
        {
            ConnectedSockets = [];
        }
        public void Initialize(UI ui, string address, string port, decimal listenCount)
        {
            ActiveUI = ui;

            if (!IPAddress.TryParse(address.Trim(), out IPAddress? ipAddress))
            {
                throw new Exception($"Failed to parse address '{address}'.");
            }

            IPEndPoint endPoint = new(ipAddress, Convert.ToInt32(port.Trim()));
            ActiveEndPoint = endPoint;

            MaxListenCount = Convert.ToInt32(listenCount);
            ActiveSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public async Task SendInfoAsync(string info,ListBox list,bool sendAll, CancellationToken token)
        {
            byte[] bytesToSend = Encoding.UTF8.GetBytes(info);
            if (FileData == null)
            {
                if(sendAll)
                {
                    foreach(string item in list.Items)
                    {
                        await ConnectedSockets[item].SendAsync(bytesToSend,token);
                        ActiveUI?.AppendChatWindowInfo?.Invoke($"[To] {item}: {info}");
                        ActiveUI?.AppendMessageToSendInfo?.Invoke("", true);
                    }
                }
                else
                {
                    foreach(string item in list.SelectedItems)
                    {
                        await ConnectedSockets[item].SendAsync(bytesToSend,token);
                        ActiveUI?.AppendChatWindowInfo?.Invoke($"[To] {item}: {info}");
                        ActiveUI?.AppendMessageToSendInfo?.Invoke("", true);
                    }
                }
            }
            else
            {
                if(sendAll)
                {
                    foreach(string item in list.Items)
                    {
                        await ConnectedSockets[item].SendAsync(FileData, token);
                        await ConnectedSockets[item].SendAsync(bytesToSend, token);
                        ActiveUI?.AppendChatWindowInfo?.Invoke($"[To] {item}: [File]{info}");
                        ActiveUI?.AppendMessageToSendInfo?.Invoke("", true);
                    }
                }
                else
                {
                    foreach(string item in list.SelectedItems)
                    {
                        await ConnectedSockets[item].SendAsync(FileData, token);
                        await ConnectedSockets[item].SendAsync(bytesToSend,token);
                        ActiveUI?.AppendChatWindowInfo?.Invoke($"[To] {item}: [File]{info}");
                        ActiveUI?.AppendMessageToSendInfo?.Invoke("", true);
                    }
                }
            }
        }
        public async Task PrepareFileAsync(CancellationToken token)
        {
            string path = "";
            OpenFileDialog openFileDialog = new()
            {
                Filter = "压缩文件|*.zip"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
            }
            if (File.Exists(path)) 
            {
                byte[] fileBytes = await File.ReadAllBytesAsync(path, token);
                byte[] fileData = new byte[fileBytes.Length + 1];

                fileData[0] = 0x02;
                Array.Copy(fileBytes, 0, fileData, 1, fileBytes.Length);
                FileData = fileData;
                string fileName = Path.GetFileName(path);
                ActiveUI?.AppendMessageToSendInfo?.Invoke("", true);
                ActiveUI?.AppendMessageToSendInfo?.Invoke($"{fileName}");
            }
        }
        public async Task StartServiceAsync(CancellationToken token)
        {
            if (ActiveEndPoint != null)
            {
                try
                {
                    ActiveSocket?.Bind(ActiveEndPoint);
                }
                catch(Exception ex)
                {
                    throw new Exception($"Bind error: '{ex}'");
                }
                try
                {
                    ActiveSocket?.Listen(MaxListenCount);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Listen error: '{ex}'");
                }
                finally
                {
                    MessageBox.Show("Service start success.");
                    await ListenToClientAsync(token);
                }
            }
        }
        public async Task ListenToClientAsync(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                if (ActiveSocket != null)
                {
                    Socket clientSocket = await ActiveSocket.AcceptAsync(token);
                    if (clientSocket.RemoteEndPoint is IPEndPoint clientEndpoint)
                    {
                        ConnectedSockets.TryAdd(clientEndpoint.ToString(), clientSocket);
                        ActiveUI?.UpdateUserInfo?.Invoke(clientEndpoint.ToString());
                    };
                    var receiveData = Task.Factory.StartNew(() => ListenToReceiveDataAsync(clientSocket,token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }
        }
        public async Task ListenToReceiveDataAsync(Socket socket,CancellationToken token)
        {
            if (socket.RemoteEndPoint is IPEndPoint clientEndPoint && ActiveSocket != null && ActiveUI != null)
            {
                bool fileReceived = false;
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    byte[] buffer = new byte[1024 * 1024 * 10];
                    int count;
                    try
                    {
                        count = await socket.ReceiveAsync(buffer, token);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Encounter exception when listening to info, ex: {ex}");
                        ActiveUI.AppendChatWindowInfo?.Invoke($"{clientEndPoint} off line.");
                        ActiveUI.UpdateUserInfo?.Invoke(clientEndPoint.ToString(), true);
                        ConnectedSockets.Remove(clientEndPoint.ToString());
                        break;
                    }
                    if (count == 0)
                    {
                        ActiveUI.AppendChatWindowInfo?.Invoke($"{clientEndPoint} off line.");
                        ActiveUI.UpdateUserInfo?.Invoke(clientEndPoint.ToString(), true);
                        ConnectedSockets.Remove(clientEndPoint.ToString());
                        break;
                    }
                    else if (count > 0)
                    {
                        if (buffer[0] != 0x02)
                        {
                            if (!fileReceived)
                            {
                                string message = Encoding.UTF8.GetString(buffer, 0, count);
                                ActiveUI.AppendChatWindowInfo?.Invoke($"[From] {clientEndPoint} | {message}");
                            }
                            else
                            {
                                try
                                {
                                    var origin = Path.Combine(Environment.CurrentDirectory, "Files", "tmp.zip");
                                    var renamed = Path.Combine(Environment.CurrentDirectory, "Files", Encoding.UTF8.GetString(buffer, 0, count));
                                    if (File.Exists(origin))
                                    {
                                        File.Move(origin, renamed);

                                        ActiveUI.AppendChatWindowInfo?.Invoke($"[From] {clientEndPoint} | Receive file: {renamed}");
                                    }
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show($"Failed to rename file, ex:'{ex}'");
                                }
                                finally
                                {
                                    fileReceived = false;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                var savePath = Path.Combine(Environment.CurrentDirectory, "Files", "tmp.zip");
                                FileStream fileStream = new(savePath, FileMode.Create, FileAccess.ReadWrite);
                                //await fileStream.WriteAsync(buffer.AsMemory(1, count - 1), token);
                                fileStream.Write(buffer, 1, count - 1);
                                fileStream.Close();
                                fileStream.Dispose();
                                fileReceived = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to receive file, ex:'{ex}'");
                            }
                        }
                    }
                }
            }
        }
        #endregion

    }
}