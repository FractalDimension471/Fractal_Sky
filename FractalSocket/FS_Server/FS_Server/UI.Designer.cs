namespace FS_Server
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
            label1 = new Label();
            Button_SendAll = new Button();
            Button_SendSelected = new Button();
            UpDown_MaxUserCount = new NumericUpDown();
            Lable_MaxUserCount = new Label();
            Lable_ReceiveMessage = new Label();
            TextBox_MessageToSend = new TextBox();
            Lable_ChatWindow = new Label();
            Lable_UserInfo = new Label();
            Lable_ServiceStart = new Label();
            TextBox_ChatWindow = new TextBox();
            ListBox_UserInfo = new ListBox();
            Button_ServiceStart = new Button();
            TextBox_LocalPort = new TextBox();
            Lable_LocalPort = new Label();
            TextBox_LocalIP = new TextBox();
            Lable_LocalIP = new Label();
            ((System.ComponentModel.ISupportInitialize)UpDown_MaxUserCount).BeginInit();
            SuspendLayout();
            // 
            // Button_SelectFile
            // 
            Button_SelectFile.Enabled = false;
            Button_SelectFile.Location = new Point(399, 409);
            Button_SelectFile.Name = "Button_SelectFile";
            Button_SelectFile.Size = new Size(94, 29);
            Button_SelectFile.TabIndex = 40;
            Button_SelectFile.Text = "SelectFile";
            Button_SelectFile.UseVisualStyleBackColor = true;
            Button_SelectFile.Click += OnButton_SelectFileClicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(544, 387);
            label1.Name = "label1";
            label1.Size = new Size(105, 20);
            label1.TabIndex = 39;
            label1.Text = "SendMethod";
            // 
            // Button_SendAll
            // 
            Button_SendAll.Enabled = false;
            Button_SendAll.Location = new Point(672, 410);
            Button_SendAll.Name = "Button_SendAll";
            Button_SendAll.Size = new Size(94, 29);
            Button_SendAll.TabIndex = 38;
            Button_SendAll.Text = "All";
            Button_SendAll.UseVisualStyleBackColor = true;
            Button_SendAll.Click += OnButton_SendAllClicked;
            // 
            // Button_SendSelected
            // 
            Button_SendSelected.Enabled = false;
            Button_SendSelected.Location = new Point(544, 410);
            Button_SendSelected.Name = "Button_SendSelected";
            Button_SendSelected.Size = new Size(94, 29);
            Button_SendSelected.TabIndex = 37;
            Button_SendSelected.Text = "Selected";
            Button_SendSelected.UseVisualStyleBackColor = true;
            Button_SendSelected.Click += OnButton_SendSelectedClicked;
            // 
            // UpDown_MaxUserCount
            // 
            UpDown_MaxUserCount.Location = new Point(544, 85);
            UpDown_MaxUserCount.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            UpDown_MaxUserCount.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            UpDown_MaxUserCount.Name = "UpDown_MaxUserCount";
            UpDown_MaxUserCount.Size = new Size(150, 27);
            UpDown_MaxUserCount.TabIndex = 36;
            UpDown_MaxUserCount.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // Lable_MaxUserCount
            // 
            Lable_MaxUserCount.AutoSize = true;
            Lable_MaxUserCount.Location = new Point(544, 62);
            Lable_MaxUserCount.Name = "Lable_MaxUserCount";
            Lable_MaxUserCount.Size = new Size(125, 20);
            Lable_MaxUserCount.TabIndex = 35;
            Lable_MaxUserCount.Text = "Max User Count";
            // 
            // Lable_ReceiveMessage
            // 
            Lable_ReceiveMessage.AutoSize = true;
            Lable_ReceiveMessage.Location = new Point(34, 387);
            Lable_ReceiveMessage.Name = "Lable_ReceiveMessage";
            Lable_ReceiveMessage.Size = new Size(133, 20);
            Lable_ReceiveMessage.TabIndex = 34;
            Lable_ReceiveMessage.Text = "Message to send";
            // 
            // TextBox_MessageToSend
            // 
            TextBox_MessageToSend.Location = new Point(34, 410);
            TextBox_MessageToSend.Multiline = true;
            TextBox_MessageToSend.Name = "TextBox_MessageToSend";
            TextBox_MessageToSend.ScrollBars = ScrollBars.Vertical;
            TextBox_MessageToSend.Size = new Size(332, 29);
            TextBox_MessageToSend.TabIndex = 33;
            // 
            // Lable_ChatWindow
            // 
            Lable_ChatWindow.AutoSize = true;
            Lable_ChatWindow.Location = new Point(34, 62);
            Lable_ChatWindow.Name = "Lable_ChatWindow";
            Lable_ChatWindow.Size = new Size(106, 20);
            Lable_ChatWindow.TabIndex = 32;
            Lable_ChatWindow.Text = "Chat Window";
            // 
            // Lable_UserInfo
            // 
            Lable_UserInfo.AutoSize = true;
            Lable_UserInfo.Location = new Point(544, 141);
            Lable_UserInfo.Name = "Lable_UserInfo";
            Lable_UserInfo.Size = new Size(131, 20);
            Lable_UserInfo.TabIndex = 31;
            Lable_UserInfo.Text = "User Information";
            // 
            // Lable_ServiceStart
            // 
            Lable_ServiceStart.AutoSize = true;
            Lable_ServiceStart.Location = new Point(544, 16);
            Lable_ServiceStart.Name = "Lable_ServiceStart";
            Lable_ServiceStart.Size = new Size(116, 20);
            Lable_ServiceStart.TabIndex = 30;
            Lable_ServiceStart.Text = "Socket Service";
            // 
            // TextBox_ChatWindow
            // 
            TextBox_ChatWindow.Location = new Point(34, 85);
            TextBox_ChatWindow.Multiline = true;
            TextBox_ChatWindow.Name = "TextBox_ChatWindow";
            TextBox_ChatWindow.ScrollBars = ScrollBars.Vertical;
            TextBox_ChatWindow.Size = new Size(459, 283);
            TextBox_ChatWindow.TabIndex = 29;
            // 
            // ListBox_UserInfo
            // 
            ListBox_UserInfo.FormattingEnabled = true;
            ListBox_UserInfo.Location = new Point(544, 164);
            ListBox_UserInfo.Name = "ListBox_UserInfo";
            ListBox_UserInfo.Size = new Size(222, 204);
            ListBox_UserInfo.TabIndex = 28;
            // 
            // Button_ServiceStart
            // 
            Button_ServiceStart.Location = new Point(672, 11);
            Button_ServiceStart.Name = "Button_ServiceStart";
            Button_ServiceStart.Size = new Size(94, 29);
            Button_ServiceStart.TabIndex = 27;
            Button_ServiceStart.Text = "ON";
            Button_ServiceStart.UseVisualStyleBackColor = true;
            Button_ServiceStart.Click += OnButton_ServiceStartClicked;
            // 
            // TextBox_LocalPort
            // 
            TextBox_LocalPort.Location = new Point(372, 13);
            TextBox_LocalPort.Name = "TextBox_LocalPort";
            TextBox_LocalPort.Size = new Size(121, 27);
            TextBox_LocalPort.TabIndex = 26;
            // 
            // Lable_LocalPort
            // 
            Lable_LocalPort.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lable_LocalPort.AutoSize = true;
            Lable_LocalPort.Location = new Point(322, 16);
            Lable_LocalPort.Name = "Lable_LocalPort";
            Lable_LocalPort.Size = new Size(44, 20);
            Lable_LocalPort.TabIndex = 25;
            Lable_LocalPort.Text = "Port:";
            // 
            // TextBox_LocalIP
            // 
            TextBox_LocalIP.Location = new Point(117, 13);
            TextBox_LocalIP.Name = "TextBox_LocalIP";
            TextBox_LocalIP.Size = new Size(121, 27);
            TextBox_LocalIP.TabIndex = 24;
            // 
            // Lable_LocalIP
            // 
            Lable_LocalIP.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lable_LocalIP.AutoSize = true;
            Lable_LocalIP.Location = new Point(34, 16);
            Lable_LocalIP.Name = "Lable_LocalIP";
            Lable_LocalIP.Size = new Size(68, 20);
            Lable_LocalIP.TabIndex = 23;
            Lable_LocalIP.Text = "Local IP:";
            // 
            // UI
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Button_SelectFile);
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "UI";
            Text = "FS_Server";
            ((System.ComponentModel.ISupportInitialize)UpDown_MaxUserCount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Button_SelectFile;
        private Label label1;
        private Button Button_SendAll;
        private Button Button_SendSelected;
        private NumericUpDown UpDown_MaxUserCount;
        private Label Lable_MaxUserCount;
        private Label Lable_ReceiveMessage;
        private TextBox TextBox_MessageToSend;
        private Label Lable_ChatWindow;
        private Label Lable_UserInfo;
        private Label Lable_ServiceStart;
        private TextBox TextBox_ChatWindow;
        private ListBox ListBox_UserInfo;
        private Button Button_ServiceStart;
        private TextBox TextBox_LocalPort;
        private Label Lable_LocalPort;
        private TextBox TextBox_LocalIP;
        private Label Lable_LocalIP;
    }
}
