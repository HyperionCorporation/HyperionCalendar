namespace Calendar
{
    partial class EventModifier
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
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblTimeBegin = new System.Windows.Forms.Label();
            this.lblTimeEnd = new System.Windows.Forms.Label();
            this.txtBoxLocation = new System.Windows.Forms.TextBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.txtBoxDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.dtpBeginDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate.Value = this.dtpBeginDate.Value; //Make them equal
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.dtpBeginTime = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // txtBoxName
            // 
            this.txtBoxName.Location = new System.Drawing.Point(96, 22);
            this.txtBoxName.Name = "txtBoxName";
            this.txtBoxName.Size = new System.Drawing.Size(170, 20);
            this.txtBoxName.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(13, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Name";
            // 
            // lblTimeBegin
            // 
            this.lblTimeBegin.AutoSize = true;
            this.lblTimeBegin.Location = new System.Drawing.Point(12, 47);
            this.lblTimeBegin.Name = "lblTimeBegin";
            this.lblTimeBegin.Size = new System.Drawing.Size(60, 13);
            this.lblTimeBegin.TabIndex = 4;
            this.lblTimeBegin.Text = "Time Begin";
            // 
            // lblTimeEnd
            // 
            this.lblTimeEnd.AutoSize = true;
            this.lblTimeEnd.Location = new System.Drawing.Point(12, 74);
            this.lblTimeEnd.Name = "lblTimeEnd";
            this.lblTimeEnd.Size = new System.Drawing.Size(52, 13);
            this.lblTimeEnd.TabIndex = 5;
            this.lblTimeEnd.Text = "Time End";
            // 
            // txtBoxLocation
            // 
            this.txtBoxLocation.Location = new System.Drawing.Point(96, 101);
            this.txtBoxLocation.Name = "txtBoxLocation";
            this.txtBoxLocation.Size = new System.Drawing.Size(121, 20);
            this.txtBoxLocation.TabIndex = 6;
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(12, 101);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(48, 13);
            this.lblLocation.TabIndex = 7;
            this.lblLocation.Text = "Location";
            // 
            // txtBoxDescription
            // 
            this.txtBoxDescription.Location = new System.Drawing.Point(96, 128);
            this.txtBoxDescription.Multiline = true;
            this.txtBoxDescription.Name = "txtBoxDescription";
            this.txtBoxDescription.Size = new System.Drawing.Size(240, 135);
            this.txtBoxDescription.TabIndex = 8;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(12, 128);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 9;
            this.lblDescription.Text = "Description";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(66, 289);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(228, 289);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 11;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(147, 289);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 14;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dtpBeginDate
            // 
            this.dtpBeginDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBeginDate.Location = new System.Drawing.Point(96, 47);
            this.dtpBeginDate.Name = "dtpBeginDate";
            this.dtpBeginDate.Size = new System.Drawing.Size(81, 20);
            this.dtpBeginDate.TabIndex = 15;
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(96, 74);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(81, 20);
            this.dtpEndDate.TabIndex = 16;
            // 
            // dtpBeginTime
            // 
            this.dtpBeginTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpBeginTime.Location = new System.Drawing.Point(184, 46);
            this.dtpBeginTime.Name = "dtpBeginTime";
            this.dtpBeginTime.ShowUpDown = true;
            this.dtpBeginTime.Size = new System.Drawing.Size(97, 20);
            this.dtpBeginTime.TabIndex = 17;
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpEndTime.Location = new System.Drawing.Point(184, 75);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.ShowUpDown = true;
            this.dtpEndTime.Size = new System.Drawing.Size(97, 20);
            this.dtpEndTime.TabIndex = 18;
            // 
            // EventModifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 324);
            this.Controls.Add(this.dtpEndTime);
            this.Controls.Add(this.dtpBeginTime);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.dtpBeginDate);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtBoxDescription);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.txtBoxLocation);
            this.Controls.Add(this.lblTimeEnd);
            this.Controls.Add(this.lblTimeBegin);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtBoxName);
            this.Name = "EventModifier";
            this.Text = "AddEvent";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBoxName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblTimeBegin;
        private System.Windows.Forms.Label lblTimeEnd;
        private System.Windows.Forms.TextBox txtBoxLocation;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.TextBox txtBoxDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DateTimePicker dtpBeginDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DateTimePicker dtpBeginTime;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
    }
}