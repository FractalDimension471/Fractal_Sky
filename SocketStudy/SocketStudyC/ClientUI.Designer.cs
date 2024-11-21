
namespace SocketStudyC
{
    partial class ClientUI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Lable_ReceiveMessage = new Label();
            TextBox_MessageToSend = new TextBox();
            Lable_ChatWindow = new Label();
            Lable_ConnectionStart = new Label();
            TextBox_ChatWindow = new TextBox();
            Button_ConnectionStart = new Button();
            TextBox_ServerPort = new TextBox();
            Lable_LocalPort = new Label();
            TextBox_ServerIP = new TextBox();
            Lable_LocalIP = new Label();
            Button_Send = new Button();
            TextBox_LocalName = new TextBox();
            Label_LocalName = new Label();
            SuspendLayout();
            // 
            // Lable_ReceiveMessage
            // 
            Lable_ReceiveMessage.AutoSize = true;
            Lable_ReceiveMessage.Location = new Point(34, 387);
            Lable_ReceiveMessage.Name = "Lable_ReceiveMessage";
            Lable_ReceiveMessage.Size = new Size(133, 20);
            Lable_ReceiveMessage.TabIndex = 33;
            Lable_ReceiveMessage.Text = "Message to send";
            // 
            // TextBox_MessageToSend
            // 
            TextBox_MessageToSend.Location = new Point(34, 410);
            TextBox_MessageToSend.Multiline = true;
            TextBox_MessageToSend.Name = "TextBox_MessageToSend";
            TextBox_MessageToSend.ScrollBars = ScrollBars.Vertical;
            TextBox_MessageToSend.Size = new Size(632, 29);
            TextBox_MessageToSend.TabIndex = 32;
            // 
            // Lable_ChatWindow
            // 
            Lable_ChatWindow.AutoSize = true;
            Lable_ChatWindow.Location = new Point(34, 62);
            Lable_ChatWindow.Name = "Lable_ChatWindow";
            Lable_ChatWindow.Size = new Size(106, 20);
            Lable_ChatWindow.TabIndex = 31;
            Lable_ChatWindow.Text = "Chat Window";
            // 
            // Lable_ConnectionStart
            // 
            Lable_ConnectionStart.AutoSize = true;
            Lable_ConnectionStart.Location = new Point(557, 16);
            Lable_ConnectionStart.Name = "Lable_ConnectionStart";
            Lable_ConnectionStart.Size = new Size(93, 20);
            Lable_ConnectionStart.TabIndex = 29;
            Lable_ConnectionStart.Text = "Connection";
            // 
            // TextBox_ChatWindow
            // 
            TextBox_ChatWindow.Location = new Point(34, 85);
            TextBox_ChatWindow.Multiline = true;
            TextBox_ChatWindow.Name = "TextBox_ChatWindow";
            TextBox_ChatWindow.ScrollBars = ScrollBars.Vertical;
            TextBox_ChatWindow.Size = new Size(743, 283);
            TextBox_ChatWindow.TabIndex = 28;
            // 
            // Button_ConnectionStart
            // 
            Button_ConnectionStart.Location = new Point(656, 11);
            Button_ConnectionStart.Name = "Button_ConnectionStart";
            Button_ConnectionStart.Size = new Size(94, 29);
            Button_ConnectionStart.TabIndex = 26;
            Button_ConnectionStart.Text = "ON";
            Button_ConnectionStart.UseVisualStyleBackColor = true;
            Button_ConnectionStart.Click += this.OnButton_ConnectionStartClicked;
            // 
            // TextBox_ServerPort
            // 
            TextBox_ServerPort.Location = new Point(372, 13);
            TextBox_ServerPort.Name = "TextBox_ServerPort";
            TextBox_ServerPort.Size = new Size(121, 27);
            TextBox_ServerPort.TabIndex = 25;
            // 
            // Lable_LocalPort
            // 
            Lable_LocalPort.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lable_LocalPort.AutoSize = true;
            Lable_LocalPort.Location = new Point(322, 16);
            Lable_LocalPort.Name = "Lable_LocalPort";
            Lable_LocalPort.Size = new Size(44, 20);
            Lable_LocalPort.TabIndex = 24;
            Lable_LocalPort.Text = "Port:";
            // 
            // TextBox_ServerIP
            // 
            TextBox_ServerIP.Location = new Point(117, 13);
            TextBox_ServerIP.Name = "TextBox_ServerIP";
            TextBox_ServerIP.Size = new Size(121, 27);
            TextBox_ServerIP.TabIndex = 23;
            // 
            // Lable_LocalIP
            // 
            Lable_LocalIP.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lable_LocalIP.AutoSize = true;
            Lable_LocalIP.Location = new Point(34, 16);
            Lable_LocalIP.Name = "Lable_LocalIP";
            Lable_LocalIP.Size = new Size(77, 20);
            Lable_LocalIP.TabIndex = 22;
            Lable_LocalIP.Text = "Server IP:";
            // 
            // Button_Send
            // 
            Button_Send.Location = new Point(683, 410);
            Button_Send.Name = "Button_Send";
            Button_Send.Size = new Size(94, 29);
            Button_Send.TabIndex = 36;
            Button_Send.Text = "Send";
            Button_Send.UseVisualStyleBackColor = true;
            Button_Send.Click += this.OnButton_SendClicked;
            // 
            // TextBox_LocalName
            // 
            TextBox_LocalName.Location = new Point(656, 47);
            TextBox_LocalName.Name = "TextBox_LocalName";
            TextBox_LocalName.Size = new Size(121, 27);
            TextBox_LocalName.TabIndex = 38;
            // 
            // Label_LocalName
            // 
            Label_LocalName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label_LocalName.AutoSize = true;
            Label_LocalName.Location = new Point(552, 50);
            Label_LocalName.Name = "Label_LocalName";
            Label_LocalName.Size = new Size(98, 20);
            Label_LocalName.TabIndex = 37;
            Label_LocalName.Text = "Local Name:";
            // 
            // ClientUI
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(TextBox_LocalName);
            Controls.Add(Label_LocalName);
            Controls.Add(Button_Send);
            Controls.Add(Lable_ReceiveMessage);
            Controls.Add(TextBox_MessageToSend);
            Controls.Add(Lable_ChatWindow);
            Controls.Add(Lable_ConnectionStart);
            Controls.Add(TextBox_ChatWindow);
            Controls.Add(Button_ConnectionStart);
            Controls.Add(TextBox_ServerPort);
            Controls.Add(Lable_LocalPort);
            Controls.Add(TextBox_ServerIP);
            Controls.Add(Lable_LocalIP);
            Name = "ClientUI";
            Text = "Socket_Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label Lable_ReceiveMessage;
        private TextBox TextBox_MessageToSend;
        private Label Lable_ChatWindow;
        private Label Lable_ConnectionStart;
        private TextBox TextBox_ChatWindow;
        private Button Button_ConnectionStart;
        private TextBox TextBox_ServerPort;
        private Label Lable_LocalPort;
        private TextBox TextBox_ServerIP;
        private Label Lable_LocalIP;
        private Button Button_Send;
        private TextBox TextBox_LocalName;
        private Label Label_LocalName;
    }
}
