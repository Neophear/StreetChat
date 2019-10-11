namespace StreetChat
{
    partial class StreetChat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreetChat));
            this.btnSendMsg = new System.Windows.Forms.Button();
            this.lstbxUsers = new System.Windows.Forms.ListBox();
            this.rtxtbxChat = new System.Windows.Forms.RichTextBox();
            this.cntxtMnuChat = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cntxtMnuItemDelAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtMnuItemCopyTxt = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtMnuItemCopyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.lblBottomText = new System.Windows.Forms.Label();
            this.chkbxNotifyNewMsg = new System.Windows.Forms.CheckBox();
            this.spltcntChat = new System.Windows.Forms.SplitContainer();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrSendEcho = new System.Windows.Forms.Timer(this.components);
            this.tmrCleanUpUsers = new System.Windows.Forms.Timer(this.components);
            this.tmrNewMsg = new System.Windows.Forms.Timer(this.components);
            this.cntxtMnuUserList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cntxtMnuItemSeeIP = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtMnuItemSeeVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtMnuItemWriteTo = new System.Windows.Forms.ToolStripMenuItem();
            this.cntxtMnuItemKick = new System.Windows.Forms.ToolStripMenuItem();
            this.rtxtbxMsg = new System.Windows.Forms.RichTextBox();
            this.cmbbxLanguage = new System.Windows.Forms.ComboBox();
            this.cntxtMnuChat.SuspendLayout();
            this.spltcntChat.Panel1.SuspendLayout();
            this.spltcntChat.Panel2.SuspendLayout();
            this.spltcntChat.SuspendLayout();
            this.cntxtMnuUserList.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSendMsg
            // 
            this.btnSendMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendMsg.Location = new System.Drawing.Point(397, 226);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new System.Drawing.Size(75, 23);
            this.btnSendMsg.TabIndex = 1;
            this.btnSendMsg.Text = "Send";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            this.btnSendMsg.Click += new System.EventHandler(this.btnSendMsg_Click);
            // 
            // lstbxUsers
            // 
            this.lstbxUsers.DisplayMember = "DisplayText";
            this.lstbxUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstbxUsers.HorizontalScrollbar = true;
            this.lstbxUsers.ItemHeight = 14;
            this.lstbxUsers.Location = new System.Drawing.Point(0, 0);
            this.lstbxUsers.Name = "lstbxUsers";
            this.lstbxUsers.Size = new System.Drawing.Size(126, 210);
            this.lstbxUsers.TabIndex = 5;
            this.lstbxUsers.ValueMember = "IPAddress";
            this.lstbxUsers.DoubleClick += new System.EventHandler(this.lstbxUsers_DoubleClick);
            this.lstbxUsers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstbxUsers_MouseUp);
            // 
            // rtxtbxChat
            // 
            this.rtxtbxChat.BackColor = System.Drawing.Color.White;
            this.rtxtbxChat.ContextMenuStrip = this.cntxtMnuChat;
            this.rtxtbxChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtbxChat.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rtxtbxChat.HideSelection = false;
            this.rtxtbxChat.Location = new System.Drawing.Point(0, 0);
            this.rtxtbxChat.Name = "rtxtbxChat";
            this.rtxtbxChat.ReadOnly = true;
            this.rtxtbxChat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtbxChat.Size = new System.Drawing.Size(330, 210);
            this.rtxtbxChat.TabIndex = 4;
            this.rtxtbxChat.Text = "";
            this.rtxtbxChat.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rtxtbxChat_MouseUp);
            // 
            // cntxtMnuChat
            // 
            this.cntxtMnuChat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cntxtMnuItemDelAll,
            this.cntxtMnuItemCopyTxt,
            this.cntxtMnuItemCopyAll});
            this.cntxtMnuChat.Name = "cntxtMnuChat";
            this.cntxtMnuChat.ShowImageMargin = false;
            this.cntxtMnuChat.Size = new System.Drawing.Size(112, 70);
            this.cntxtMnuChat.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cntxtMnuChat_ItemClicked);
            // 
            // cntxtMnuItemDelAll
            // 
            this.cntxtMnuItemDelAll.Name = "cntxtMnuItemDelAll";
            this.cntxtMnuItemDelAll.Size = new System.Drawing.Size(111, 22);
            this.cntxtMnuItemDelAll.Tag = "delall";
            this.cntxtMnuItemDelAll.Text = "Slet alt";
            this.cntxtMnuItemDelAll.ToolTipText = "Slet al tekst i chatten.";
            // 
            // cntxtMnuItemCopyTxt
            // 
            this.cntxtMnuItemCopyTxt.Name = "cntxtMnuItemCopyTxt";
            this.cntxtMnuItemCopyTxt.Size = new System.Drawing.Size(111, 22);
            this.cntxtMnuItemCopyTxt.Tag = "copytxt";
            this.cntxtMnuItemCopyTxt.Text = "Kopier tekst";
            this.cntxtMnuItemCopyTxt.ToolTipText = "Kopier markeret tekst.";
            // 
            // cntxtMnuItemCopyAll
            // 
            this.cntxtMnuItemCopyAll.Name = "cntxtMnuItemCopyAll";
            this.cntxtMnuItemCopyAll.Size = new System.Drawing.Size(111, 22);
            this.cntxtMnuItemCopyAll.Tag = "copyall";
            this.cntxtMnuItemCopyAll.Text = "Kopier alt";
            this.cntxtMnuItemCopyAll.ToolTipText = "Kopier al tekst i chatten.";
            // 
            // lblBottomText
            // 
            this.lblBottomText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBottomText.AutoSize = true;
            this.lblBottomText.Location = new System.Drawing.Point(12, 274);
            this.lblBottomText.Name = "lblBottomText";
            this.lblBottomText.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblBottomText.Size = new System.Drawing.Size(70, 14);
            this.lblBottomText.TabIndex = 6;
            this.lblBottomText.Text = "lblBottomText";
            this.lblBottomText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkbxNotifyNewMsg
            // 
            this.chkbxNotifyNewMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkbxNotifyNewMsg.AutoSize = true;
            this.chkbxNotifyNewMsg.Checked = true;
            this.chkbxNotifyNewMsg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbxNotifyNewMsg.Location = new System.Drawing.Point(12, 253);
            this.chkbxNotifyNewMsg.Name = "chkbxNotifyNewMsg";
            this.chkbxNotifyNewMsg.Size = new System.Drawing.Size(174, 18);
            this.chkbxNotifyNewMsg.TabIndex = 2;
            this.chkbxNotifyNewMsg.Text = "Lyd/statustekst ved ny besked";
            this.chkbxNotifyNewMsg.UseVisualStyleBackColor = true;
            // 
            // spltcntChat
            // 
            this.spltcntChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spltcntChat.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.spltcntChat.Location = new System.Drawing.Point(12, 12);
            this.spltcntChat.Name = "spltcntChat";
            // 
            // spltcntChat.Panel1
            // 
            this.spltcntChat.Panel1.Controls.Add(this.rtxtbxChat);
            // 
            // spltcntChat.Panel2
            // 
            this.spltcntChat.Panel2.Controls.Add(this.lstbxUsers);
            this.spltcntChat.Size = new System.Drawing.Size(460, 210);
            this.spltcntChat.SplitterDistance = 330;
            this.spltcntChat.TabIndex = 9;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // tmrSendEcho
            // 
            this.tmrSendEcho.Interval = 5000;
            this.tmrSendEcho.Tick += new System.EventHandler(this.tmrSendEcho_Tick);
            // 
            // tmrCleanUpUsers
            // 
            this.tmrCleanUpUsers.Enabled = true;
            this.tmrCleanUpUsers.Interval = 2500;
            this.tmrCleanUpUsers.Tick += new System.EventHandler(this.tmrCleanUpUsers_Tick);
            // 
            // tmrNewMsg
            // 
            this.tmrNewMsg.Enabled = true;
            this.tmrNewMsg.Interval = 1000;
            this.tmrNewMsg.Tick += new System.EventHandler(this.tmrNewMsg_Tick);
            // 
            // cntxtMnuUserList
            // 
            this.cntxtMnuUserList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cntxtMnuItemSeeIP,
            this.cntxtMnuItemSeeVersion,
            this.cntxtMnuItemWriteTo,
            this.cntxtMnuItemKick});
            this.cntxtMnuUserList.Name = "cntxtMnuUserList";
            this.cntxtMnuUserList.ShowImageMargin = false;
            this.cntxtMnuUserList.Size = new System.Drawing.Size(103, 92);
            this.cntxtMnuUserList.Opening += new System.ComponentModel.CancelEventHandler(this.cntxtMnuUserList_Opening);
            this.cntxtMnuUserList.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cntxtMnuUserList_ItemClicked);
            // 
            // cntxtMnuItemSeeIP
            // 
            this.cntxtMnuItemSeeIP.Name = "cntxtMnuItemSeeIP";
            this.cntxtMnuItemSeeIP.Size = new System.Drawing.Size(102, 22);
            this.cntxtMnuItemSeeIP.Tag = "seeip";
            this.cntxtMnuItemSeeIP.Text = "Se IP";
            // 
            // cntxtMnuItemSeeVersion
            // 
            this.cntxtMnuItemSeeVersion.Name = "cntxtMnuItemSeeVersion";
            this.cntxtMnuItemSeeVersion.Size = new System.Drawing.Size(102, 22);
            this.cntxtMnuItemSeeVersion.Tag = "seeversion";
            this.cntxtMnuItemSeeVersion.Text = "Se version";
            // 
            // cntxtMnuItemWriteTo
            // 
            this.cntxtMnuItemWriteTo.Name = "cntxtMnuItemWriteTo";
            this.cntxtMnuItemWriteTo.Size = new System.Drawing.Size(102, 22);
            this.cntxtMnuItemWriteTo.Tag = "writeto";
            this.cntxtMnuItemWriteTo.Text = "Skriv til";
            // 
            // cntxtMnuItemKick
            // 
            this.cntxtMnuItemKick.Name = "cntxtMnuItemKick";
            this.cntxtMnuItemKick.Size = new System.Drawing.Size(102, 22);
            this.cntxtMnuItemKick.Tag = "kick";
            this.cntxtMnuItemKick.Text = "Kick";
            // 
            // rtxtbxMsg
            // 
            this.rtxtbxMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtbxMsg.DetectUrls = false;
            this.rtxtbxMsg.Location = new System.Drawing.Point(12, 228);
            this.rtxtbxMsg.MaxLength = 999;
            this.rtxtbxMsg.Multiline = false;
            this.rtxtbxMsg.Name = "rtxtbxMsg";
            this.rtxtbxMsg.Size = new System.Drawing.Size(379, 21);
            this.rtxtbxMsg.TabIndex = 10;
            this.rtxtbxMsg.Text = "";
            this.rtxtbxMsg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtxtbxMsg_KeyDown);
            // 
            // cmbbxLanguage
            // 
            this.cmbbxLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbbxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbbxLanguage.FormattingEnabled = true;
            this.cmbbxLanguage.Items.AddRange(new object[] {
            "English",
            "Dansk"});
            this.cmbbxLanguage.Location = new System.Drawing.Point(351, 262);
            this.cmbbxLanguage.Name = "cmbbxLanguage";
            this.cmbbxLanguage.Size = new System.Drawing.Size(121, 22);
            this.cmbbxLanguage.TabIndex = 11;
            this.cmbbxLanguage.SelectedIndexChanged += new System.EventHandler(this.cmbbxLanguage_SelectedIndexChanged);
            // 
            // StreetChat
            // 
            this.AcceptButton = this.btnSendMsg;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(484, 296);
            this.Controls.Add(this.cmbbxLanguage);
            this.Controls.Add(this.rtxtbxMsg);
            this.Controls.Add(this.spltcntChat);
            this.Controls.Add(this.chkbxNotifyNewMsg);
            this.Controls.Add(this.lblBottomText);
            this.Controls.Add(this.btnSendMsg);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 334);
            this.Name = "StreetChat";
            this.Text = "StreetChat";
            this.Activated += new System.EventHandler(this.StreetChat_Activated);
            this.Deactivate += new System.EventHandler(this.StreetChat_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Chat_FormClosing);
            this.Shown += new System.EventHandler(this.StreetChat_Shown);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.StreetChat_HelpRequested);
            this.cntxtMnuChat.ResumeLayout(false);
            this.spltcntChat.Panel1.ResumeLayout(false);
            this.spltcntChat.Panel2.ResumeLayout(false);
            this.spltcntChat.ResumeLayout(false);
            this.cntxtMnuUserList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSendMsg;
        private System.Windows.Forms.ListBox lstbxUsers;
        private System.Windows.Forms.RichTextBox rtxtbxChat;
        private System.Windows.Forms.Label lblBottomText;
        private System.Windows.Forms.CheckBox chkbxNotifyNewMsg;
        private System.Windows.Forms.SplitContainer spltcntChat;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Timer tmrSendEcho;
        private System.Windows.Forms.Timer tmrCleanUpUsers;
        private System.Windows.Forms.Timer tmrNewMsg;
        private System.Windows.Forms.ContextMenuStrip cntxtMnuChat;
        private System.Windows.Forms.ToolStripMenuItem cntxtMnuItemDelAll;
        private System.Windows.Forms.ToolStripMenuItem cntxtMnuItemCopyTxt;
        private System.Windows.Forms.ToolStripMenuItem cntxtMnuItemCopyAll;
        private System.Windows.Forms.ContextMenuStrip cntxtMnuUserList;
        private System.Windows.Forms.ToolStripMenuItem cntxtMnuItemSeeIP;
        private System.Windows.Forms.ToolStripMenuItem cntxtMnuItemSeeVersion;
        private System.Windows.Forms.ToolStripMenuItem cntxtMnuItemWriteTo;
        private System.Windows.Forms.ToolStripMenuItem cntxtMnuItemKick;
        private System.Windows.Forms.RichTextBox rtxtbxMsg;
        private System.Windows.Forms.ComboBox cmbbxLanguage;

    }
}