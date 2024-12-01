namespace FS_Client
{
    partial class UI
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
            Button_SelectFile = new Button();
            Button_Send = new Button();
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
            SuspendLayout();
            // 
            // Button_SelectFile
            // 
            Button_SelectFile.Enabled = false;
            Button_SelectFile.Location = new Point(678, 374);
            Button_SelectFile.Name = "Button_SelectFile";
            Button_SelectFile.Size = new Size(94, 29);
            Button_SelectFile.TabIndex = 53;
            Button_SelectFile.Text = "File";
            Button_SelectFile.UseVisualStyleBackColor = true;
            Button_SelectFile.Click += OnButton_SelectFileClicked;
            // 
            // Button_Send
            // 
            Button_Send.Enabled = false;
            Button_Send.Location = new Point(678, 410);
            Button_Send.Name = "Button_Send";
            Button_Send.Size = new Size(94, 29);
            Button_Send.TabIndex = 50;
            Button_Send.Text = "Send";
            Button_Send.UseVisualStyleBackColor = true;
            Button_Send.Click += OnButton_SendInfoClicked;
            // 
            // Lable_ReceiveMessage
            // 
            Lable_ReceiveMessage.AutoSize = true;
            Lable_ReceiveMessage.Location = new Point(29, 387);
            Lable_ReceiveMessage.Name = "Lable_ReceiveMessage";
            Lable_ReceiveMessage.Size = new Size(133, 20);
            Lable_ReceiveMessage.TabIndex = 49;
            Lable_ReceiveMessage.Text = "Message to send";
            // 
            // TextBox_MessageToSend
            // 
            TextBox_MessageToSend.Location = new Point(29, 410);
            TextBox_MessageToSend.Multiline = true;
            TextBox_MessageToSend.Name = "TextBox_MessageToSend";
            TextBox_MessageToSend.ScrollBars = ScrollBars.Vertical;
            TextBox_MessageToSend.Size = new Size(632, 29);
            TextBox_MessageToSend.TabIndex = 48;
            // 
            // Lable_ChatWindow
            // 
            Lable_ChatWindow.AutoSize = true;
            Lable_ChatWindow.Location = new Point(29, 57);
            Lable_ChatWindow.Name = "Lable_ChatWindow";
            Lable_ChatWindow.Size = new Size(106, 20);
            Lable_ChatWindow.TabIndex = 47;
            Lable_ChatWindow.Text = "Chat Window";
            // 
            // Lable_ConnectionStart
            // 
            Lable_ConnectionStart.AutoSize = true;
            Lable_ConnectionStart.Location = new Point(552, 16);
            Lable_ConnectionStart.Name = "Lable_ConnectionStart";
            Lable_ConnectionStart.Size = new Size(93, 20);
            Lable_ConnectionStart.TabIndex = 46;
            Lable_ConnectionStart.Text = "Connection";
            // 
            // TextBox_ChatWindow
            // 
            TextBox_ChatWindow.Location = new Point(29, 80);
            TextBox_ChatWindow.Multiline = true;
            TextBox_ChatWindow.Name = "TextBox_ChatWindow";
            TextBox_ChatWindow.ScrollBars = ScrollBars.Vertical;
            TextBox_ChatWindow.Size = new Size(743, 288);
            TextBox_ChatWindow.TabIndex = 45;
            // 
            // Button_ConnectionStart
            // 
            Button_ConnectionStart.Location = new Point(651, 11);
            Button_ConnectionStart.Name = "Button_ConnectionStart";
            Button_ConnectionStart.Size = new Size(94, 29);
            Button_ConnectionStart.TabIndex = 44;
            Button_ConnectionStart.Text = "ON";
            Button_ConnectionStart.UseVisualStyleBackColor = true;
            Button_ConnectionStart.Click += OnButton_ConnectionStartClicked;
            // 
            // TextBox_ServerPort
            // 
            TextBox_ServerPort.Location = new Point(367, 13);
            TextBox_ServerPort.Name = "TextBox_ServerPort";
            TextBox_ServerPort.Size = new Size(121, 27);
            TextBox_ServerPort.TabIndex = 43;
            // 
            // Lable_LocalPort
            // 
            Lable_LocalPort.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lable_LocalPort.AutoSize = true;
            Lable_LocalPort.Location = new Point(317, 16);
            Lable_LocalPort.Name = "Lable_LocalPort";
            Lable_LocalPort.Size = new Size(44, 20);
            Lable_LocalPort.TabIndex = 42;
            Lable_LocalPort.Text = "Port:";
            // 
            // TextBox_ServerIP
            // 
            TextBox_ServerIP.Location = new Point(112, 13);
            TextBox_ServerIP.Name = "TextBox_ServerIP";
            TextBox_ServerIP.Size = new Size(121, 27);
            TextBox_ServerIP.TabIndex = 41;
            // 
            // Lable_LocalIP
            // 
            Lable_LocalIP.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lable_LocalIP.AutoSize = true;
            Lable_LocalIP.Location = new Point(29, 16);
            Lable_LocalIP.Name = "Lable_LocalIP";
            Lable_LocalIP.Size = new Size(77, 20);
            Lable_LocalIP.TabIndex = 40;
            Lable_LocalIP.Text = "Server IP:";
            // 
            // UI
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Button_SelectFile);
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "UI";
            Text = "FS_Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Button_SelectFile;
        private Button Button_Send;
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
    }
}
