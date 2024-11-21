namespace SocketStudy
{
    partial class ServerUI
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
            Lable_LocalIP = new Label();
            TextBox_LocalIP = new TextBox();
            TextBox_LocalPort = new TextBox();
            Lable_LocalPort = new Label();
            Button_ServiceStart = new Button();
            ListBox_UserInfo = new ListBox();
            TextBox_ChatWindow = new TextBox();
            Lable_ServiceStart = new Label();
            Lable_UserInfo = new Label();
            Lable_ChatWindow = new Label();
            Lable_ReceiveMessage = new Label();
            TextBox_MessageToSend = new TextBox();
            Lable_MaxUserCount = new Label();
            UpDown_MaxUserCount = new NumericUpDown();
            Button_SendSelected = new Button();
            Button_SendAll = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)UpDown_MaxUserCount).BeginInit();
            SuspendLayout();
            // 
            // Lable_LocalIP
            // 
            Lable_LocalIP.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lable_LocalIP.AutoSize = true;
            Lable_LocalIP.Location = new Point(32, 15);
            Lable_LocalIP.Name = "Lable_LocalIP";
            Lable_LocalIP.Size = new Size(68, 20);
            Lable_LocalIP.TabIndex = 0;
            Lable_LocalIP.Text = "Local IP:";
            // 
            // TextBox_LocalIP
            // 
            TextBox_LocalIP.Location = new Point(115, 12);
            TextBox_LocalIP.Name = "TextBox_LocalIP";
            TextBox_LocalIP.Size = new Size(121, 27);
            TextBox_LocalIP.TabIndex = 2;
            // 
            // TextBox_LocalPort
            // 
            TextBox_LocalPort.Location = new Point(370, 12);
            TextBox_LocalPort.Name = "TextBox_LocalPort";
            TextBox_LocalPort.Size = new Size(121, 27);
            TextBox_LocalPort.TabIndex = 4;
            // 
            // Lable_LocalPort
            // 
            Lable_LocalPort.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lable_LocalPort.AutoSize = true;
            Lable_LocalPort.Location = new Point(320, 15);
            Lable_LocalPort.Name = "Lable_LocalPort";
            Lable_LocalPort.Size = new Size(44, 20);
            Lable_LocalPort.TabIndex = 3;
            Lable_LocalPort.Text = "Port:";
            // 
            // Button_ServiceStart
            // 
            Button_ServiceStart.Location = new Point(670, 15);
            Button_ServiceStart.Name = "Button_ServiceStart";
            Button_ServiceStart.Size = new Size(94, 29);
            Button_ServiceStart.TabIndex = 5;
            Button_ServiceStart.Text = "ON";
            Button_ServiceStart.UseVisualStyleBackColor = true;
            Button_ServiceStart.Click += OnButton_ServiceStartClicked;
            // 
            // ListBox_UserInfo
            // 
            ListBox_UserInfo.FormattingEnabled = true;
            ListBox_UserInfo.Location = new Point(542, 163);
            ListBox_UserInfo.Name = "ListBox_UserInfo";
            ListBox_UserInfo.Size = new Size(222, 204);
            ListBox_UserInfo.TabIndex = 6;
            // 
            // TextBox_ChatWindow
            // 
            TextBox_ChatWindow.Location = new Point(32, 84);
            TextBox_ChatWindow.Multiline = true;
            TextBox_ChatWindow.Name = "TextBox_ChatWindow";
            TextBox_ChatWindow.ScrollBars = ScrollBars.Vertical;
            TextBox_ChatWindow.Size = new Size(459, 283);
            TextBox_ChatWindow.TabIndex = 7;
            // 
            // Lable_ServiceStart
            // 
            Lable_ServiceStart.AutoSize = true;
            Lable_ServiceStart.Location = new Point(542, 15);
            Lable_ServiceStart.Name = "Lable_ServiceStart";
            Lable_ServiceStart.Size = new Size(116, 20);
            Lable_ServiceStart.TabIndex = 10;
            Lable_ServiceStart.Text = "Socket Service";
            // 
            // Lable_UserInfo
            // 
            Lable_UserInfo.AutoSize = true;
            Lable_UserInfo.Location = new Point(542, 140);
            Lable_UserInfo.Name = "Lable_UserInfo";
            Lable_UserInfo.Size = new Size(131, 20);
            Lable_UserInfo.TabIndex = 11;
            Lable_UserInfo.Text = "User Information";
            // 
            // Lable_ChatWindow
            // 
            Lable_ChatWindow.AutoSize = true;
            Lable_ChatWindow.Location = new Point(32, 61);
            Lable_ChatWindow.Name = "Lable_ChatWindow";
            Lable_ChatWindow.Size = new Size(106, 20);
            Lable_ChatWindow.TabIndex = 13;
            Lable_ChatWindow.Text = "Chat Window";
            // 
            // Lable_ReceiveMessage
            // 
            Lable_ReceiveMessage.AutoSize = true;
            Lable_ReceiveMessage.Location = new Point(32, 386);
            Lable_ReceiveMessage.Name = "Lable_ReceiveMessage";
            Lable_ReceiveMessage.Size = new Size(133, 20);
            Lable_ReceiveMessage.TabIndex = 15;
            Lable_ReceiveMessage.Text = "Message to send";
            // 
            // TextBox_MessageToSend
            // 
            TextBox_MessageToSend.Location = new Point(32, 409);
            TextBox_MessageToSend.Multiline = true;
            TextBox_MessageToSend.Name = "TextBox_MessageToSend";
            TextBox_MessageToSend.ScrollBars = ScrollBars.Vertical;
            TextBox_MessageToSend.Size = new Size(459, 29);
            TextBox_MessageToSend.TabIndex = 14;
            // 
            // Lable_MaxUserCount
            // 
            Lable_MaxUserCount.AutoSize = true;
            Lable_MaxUserCount.Location = new Point(542, 61);
            Lable_MaxUserCount.Name = "Lable_MaxUserCount";
            Lable_MaxUserCount.Size = new Size(125, 20);
            Lable_MaxUserCount.TabIndex = 17;
            Lable_MaxUserCount.Text = "Max User Count";
            // 
            // UpDown_MaxUserCount
            // 
            UpDown_MaxUserCount.Location = new Point(542, 84);
            UpDown_MaxUserCount.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            UpDown_MaxUserCount.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            UpDown_MaxUserCount.Name = "UpDown_MaxUserCount";
            UpDown_MaxUserCount.Size = new Size(150, 27);
            UpDown_MaxUserCount.TabIndex = 18;
            UpDown_MaxUserCount.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // Button_SendSelected
            // 
            Button_SendSelected.Location = new Point(542, 409);
            Button_SendSelected.Name = "Button_SendSelected";
            Button_SendSelected.Size = new Size(94, 29);
            Button_SendSelected.TabIndex = 19;
            Button_SendSelected.Text = "Selected";
            Button_SendSelected.UseVisualStyleBackColor = true;
            Button_SendSelected.Click += OnButton_SendSelectedClicked;
            // 
            // Button_SendAll
            // 
            Button_SendAll.Location = new Point(670, 409);
            Button_SendAll.Name = "Button_SendAll";
            Button_SendAll.Size = new Size(94, 29);
            Button_SendAll.TabIndex = 20;
            Button_SendAll.Text = "All";
            Button_SendAll.UseVisualStyleBackColor = true;
            Button_SendAll.Click += OnButton_SendAllClicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(542, 386);
            label1.Name = "label1";
            label1.Size = new Size(105, 20);
            label1.TabIndex = 21;
            label1.Text = "SendMethod";
            // 
            // ServerUI
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(Button_SendAll);
            Controls.Add(Button_SendSelected);
            Controls.Add(UpDown_MaxUserCount);
            Controls.Add(Lable_MaxUserCount);
            Controls.Add(Lable_ReceiveMessage);
            Controls.Add(TextBox_MessageToSend);
            Controls.Add(Lable_ChatWindow);
            Controls.Add(Lable_UserInfo);
            Controls.Add(Lable_ServiceStart);
            Controls.Add(TextBox_ChatWindow);
            Controls.Add(ListBox_UserInfo);
            Controls.Add(Button_ServiceStart);
            Controls.Add(TextBox_LocalPort);
            Controls.Add(Lable_LocalPort);
            Controls.Add(TextBox_LocalIP);
            Controls.Add(Lable_LocalIP);
            Name = "ServerUI";
            Text = "Socket_Server";
            ((System.ComponentModel.ISupportInitialize)UpDown_MaxUserCount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Lable_LocalIP;
        private TextBox TextBox_LocalIP;
        private TextBox TextBox_LocalPort;
        private Label Lable_LocalPort;
        private Button Button_ServiceStart;
        private ListBox ListBox_UserInfo;
        private TextBox TextBox_ChatWindow;
        private Label Lable_ServiceStart;
        private Label Lable_UserInfo;
        private Label Lable_ChatWindow;
        private Label Lable_ReceiveMessage;
        private TextBox TextBox_MessageToSend;
        private Label Lable_MaxUserCount;
        private NumericUpDown UpDown_MaxUserCount;
        private Button Button_SendSelected;
        private Button Button_SendAll;
        private Label label1;
    }
}
