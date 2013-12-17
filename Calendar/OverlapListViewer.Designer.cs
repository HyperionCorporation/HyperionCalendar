namespace Calendar
{
    partial class OverlapListViewer
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLocation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnStart = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnEnd = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnLocation,
            this.columnStart,
            this.columnEnd});
            this.listView1.Location = new System.Drawing.Point(5, 5);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(496, 166);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // columnName
            // 
            this.columnName.Text = "Event Name";
            // 
            // columnLocation
            // 
            this.columnLocation.Text = "Event Location";
            // 
            // columnStart
            // 
            this.columnStart.Text = "Event Start Time";
            // 
            // columnEnd
            // 
            this.columnEnd.Text = "Event End Time";
            // 
            // OverlapListViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 262);
            this.Controls.Add(this.listView1);
            this.Name = "OverlapListViewer";
            this.Text = "OverlapListViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnLocation;
        private System.Windows.Forms.ColumnHeader columnStart;
        private System.Windows.Forms.ColumnHeader columnEnd;
    }
}