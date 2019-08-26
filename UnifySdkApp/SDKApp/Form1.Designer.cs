namespace SDKApp
{
    partial class MainPage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.buttonLoginAgent = new System.Windows.Forms.Button();
            this.buttonLogOut = new System.Windows.Forms.Button();
            this.buttonAvailable = new System.Windows.Forms.Button();
            this.buttonUnAvailable = new System.Windows.Forms.Button();
            this.buttonWork = new System.Windows.Forms.Button();
            this.listViewAgentsList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dataGridCallDetail = new System.Windows.Forms.DataGridView();
            this.CallID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Agent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WaitTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.loadBox = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCallDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonLoginAgent
            // 
            this.buttonLoginAgent.BackColor = System.Drawing.Color.Transparent;
            this.buttonLoginAgent.ForeColor = System.Drawing.Color.Black;
            this.buttonLoginAgent.Location = new System.Drawing.Point(102, 379);
            this.buttonLoginAgent.Name = "buttonLoginAgent";
            this.buttonLoginAgent.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonLoginAgent.Size = new System.Drawing.Size(79, 23);
            this.buttonLoginAgent.TabIndex = 8;
            this.buttonLoginAgent.Text = "Login";
            this.buttonLoginAgent.UseVisualStyleBackColor = false;
            this.buttonLoginAgent.Click += new System.EventHandler(this.buttonLoginAgent_Click);
            // 
            // buttonLogOut
            // 
            this.buttonLogOut.BackColor = System.Drawing.Color.Transparent;
            this.buttonLogOut.ForeColor = System.Drawing.Color.Black;
            this.buttonLogOut.Location = new System.Drawing.Point(187, 379);
            this.buttonLogOut.Name = "buttonLogOut";
            this.buttonLogOut.Size = new System.Drawing.Size(59, 23);
            this.buttonLogOut.TabIndex = 16;
            this.buttonLogOut.Text = "Logout";
            this.buttonLogOut.UseVisualStyleBackColor = false;
            this.buttonLogOut.Click += new System.EventHandler(this.buttonLogOut_Click);
            // 
            // buttonAvailable
            // 
            this.buttonAvailable.BackColor = System.Drawing.Color.Transparent;
            this.buttonAvailable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonAvailable.Location = new System.Drawing.Point(257, 379);
            this.buttonAvailable.Name = "buttonAvailable";
            this.buttonAvailable.Size = new System.Drawing.Size(59, 23);
            this.buttonAvailable.TabIndex = 20;
            this.buttonAvailable.Text = "Available";
            this.buttonAvailable.UseVisualStyleBackColor = false;
            this.buttonAvailable.Click += new System.EventHandler(this.buttonAvailable_Click);
            // 
            // buttonUnAvailable
            // 
            this.buttonUnAvailable.BackColor = System.Drawing.Color.Transparent;
            this.buttonUnAvailable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonUnAvailable.Location = new System.Drawing.Point(322, 379);
            this.buttonUnAvailable.Name = "buttonUnAvailable";
            this.buttonUnAvailable.Size = new System.Drawing.Size(76, 23);
            this.buttonUnAvailable.TabIndex = 21;
            this.buttonUnAvailable.Text = "UnAvailable";
            this.buttonUnAvailable.UseVisualStyleBackColor = false;
            this.buttonUnAvailable.Click += new System.EventHandler(this.buttonUnAvailable_Click);
            // 
            // buttonWork
            // 
            this.buttonWork.BackColor = System.Drawing.Color.Transparent;
            this.buttonWork.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonWork.Location = new System.Drawing.Point(404, 379);
            this.buttonWork.Name = "buttonWork";
            this.buttonWork.Size = new System.Drawing.Size(59, 23);
            this.buttonWork.TabIndex = 22;
            this.buttonWork.Text = "work";
            this.buttonWork.UseVisualStyleBackColor = false;
            this.buttonWork.Click += new System.EventHandler(this.buttonWork_Click);
            // 
            // listViewAgentsList
            // 
            this.listViewAgentsList.BackColor = System.Drawing.Color.AliceBlue;
            this.listViewAgentsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewAgentsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listViewAgentsList.Dock = System.Windows.Forms.DockStyle.Top;
            this.listViewAgentsList.FullRowSelect = true;
            this.listViewAgentsList.GridLines = true;
            this.listViewAgentsList.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.listViewAgentsList.Location = new System.Drawing.Point(0, 0);
            this.listViewAgentsList.Name = "listViewAgentsList";
            this.listViewAgentsList.Size = new System.Drawing.Size(725, 364);
            this.listViewAgentsList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewAgentsList.TabIndex = 24;
            this.listViewAgentsList.UseCompatibleStateImageBehavior = false;
            this.listViewAgentsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Extenssion";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            this.columnHeader3.Width = 122;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Ring Time";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Call Time";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "CallID";
            this.columnHeader6.Width = 160;
            // 
            // dataGridCallDetail
            // 
            this.dataGridCallDetail.AllowUserToAddRows = false;
            this.dataGridCallDetail.AllowUserToDeleteRows = false;
            this.dataGridCallDetail.AllowUserToOrderColumns = true;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dataGridCallDetail.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridCallDetail.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridCallDetail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridCallDetail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridCallDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridCallDetail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CallID,
            this.Agent,
            this.WaitTime});
            this.dataGridCallDetail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dataGridCallDetail.GridColor = System.Drawing.SystemColors.Desktop;
            this.dataGridCallDetail.Location = new System.Drawing.Point(0, 415);
            this.dataGridCallDetail.Margin = new System.Windows.Forms.Padding(10, 10, 3, 3);
            this.dataGridCallDetail.MultiSelect = false;
            this.dataGridCallDetail.Name = "dataGridCallDetail";
            this.dataGridCallDetail.ReadOnly = true;
            this.dataGridCallDetail.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridCallDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridCallDetail.Size = new System.Drawing.Size(725, 233);
            this.dataGridCallDetail.TabIndex = 26;
            // 
            // CallID
            // 
            this.CallID.HeaderText = "CallID";
            this.CallID.Name = "CallID";
            this.CallID.ReadOnly = true;
            this.CallID.Width = 300;
            // 
            // Agent
            // 
            this.Agent.HeaderText = "Agent";
            this.Agent.Name = "Agent";
            this.Agent.ReadOnly = true;
            this.Agent.Width = 200;
            // 
            // WaitTime
            // 
            this.WaitTime.HeaderText = "WaitTime";
            this.WaitTime.Name = "WaitTime";
            this.WaitTime.ReadOnly = true;
            // 
            // loadBox
            // 
            this.loadBox.AutoSize = true;
            this.loadBox.Font = new System.Drawing.Font("Arial Rounded MT Bold", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadBox.ForeColor = System.Drawing.SystemColors.Highlight;
            this.loadBox.Location = new System.Drawing.Point(268, 26);
            this.loadBox.Name = "loadBox";
            this.loadBox.Size = new System.Drawing.Size(142, 37);
            this.loadBox.TabIndex = 27;
            this.loadBox.Text = "Loading";
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 677);
            this.Controls.Add(this.loadBox);
            this.Controls.Add(this.listViewAgentsList);
            this.Controls.Add(this.dataGridCallDetail);
            this.Controls.Add(this.buttonWork);
            this.Controls.Add(this.buttonUnAvailable);
            this.Controls.Add(this.buttonAvailable);
            this.Controls.Add(this.buttonLogOut);
            this.Controls.Add(this.buttonLoginAgent);
            this.Name = "MainPage";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCallDetail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonLoginAgent;
        private System.Windows.Forms.Button buttonLogOut;
        private System.Windows.Forms.Button buttonAvailable;
        private System.Windows.Forms.Button buttonUnAvailable;
        private System.Windows.Forms.Button buttonWork;
        private System.Windows.Forms.ListView listViewAgentsList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.DataGridView dataGridCallDetail;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.DataGridViewTextBoxColumn CallID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Agent;
        private System.Windows.Forms.DataGridViewTextBoxColumn WaitTime;
        private System.Windows.Forms.Label loadBox;
    }
}

