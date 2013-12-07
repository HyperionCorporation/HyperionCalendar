namespace Calendar
{
    partial class AddUser
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.lblNameWarn = new System.Windows.Forms.Label();
            this.lblEmailWarn = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPasswordVerify = new System.Windows.Forms.TextBox();
            this.lblPasswdMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(71, 30);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(121, 20);
            this.txtName.TabIndex = 0;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(71, 56);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(121, 20);
            this.txtEmail.TabIndex = 1;
            this.txtEmail.Leave += new System.EventHandler(this.txtEmail_Leave);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(71, 82);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(121, 20);
            this.txtPassword.TabIndex = 2;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 30);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Name";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(3, 56);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(32, 13);
            this.lblEmail.TabIndex = 4;
            this.lblEmail.Text = "Email";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(3, 82);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(37, 134);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(118, 134);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // lblNameWarn
            // 
            this.lblNameWarn.AutoSize = true;
            this.lblNameWarn.Location = new System.Drawing.Point(206, 30);
            this.lblNameWarn.Name = "lblNameWarn";
            this.lblNameWarn.Size = new System.Drawing.Size(81, 13);
            this.lblNameWarn.TabIndex = 8;
            this.lblNameWarn.Text = "Name Message";
            this.lblNameWarn.Visible = false;
            // 
            // lblEmailWarn
            // 
            this.lblEmailWarn.AutoSize = true;
            this.lblEmailWarn.Location = new System.Drawing.Point(206, 56);
            this.lblEmailWarn.Name = "lblEmailWarn";
            this.lblEmailWarn.Size = new System.Drawing.Size(78, 13);
            this.lblEmailWarn.TabIndex = 9;
            this.lblEmailWarn.Text = "Email Message";
            this.lblEmailWarn.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Confirm Pass";
            // 
            // txtPasswordVerify
            // 
            this.txtPasswordVerify.Location = new System.Drawing.Point(72, 108);
            this.txtPasswordVerify.Name = "txtPasswordVerify";
            this.txtPasswordVerify.PasswordChar = '*';
            this.txtPasswordVerify.Size = new System.Drawing.Size(121, 20);
            this.txtPasswordVerify.TabIndex = 3;
            this.txtPasswordVerify.TextChanged += new System.EventHandler(this.txtPasswordVerify_TextChanged);
            // 
            // lblPasswdMessage
            // 
            this.lblPasswdMessage.AutoSize = true;
            this.lblPasswdMessage.Location = new System.Drawing.Point(206, 108);
            this.lblPasswdMessage.Name = "lblPasswdMessage";
            this.lblPasswdMessage.Size = new System.Drawing.Size(116, 13);
            this.lblPasswdMessage.TabIndex = 12;
            this.lblPasswdMessage.Text = "Passwords don\'t match";
            this.lblPasswdMessage.Visible = false;
            // 
            // AddUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 161);
            this.Controls.Add(this.lblPasswdMessage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPasswordVerify);
            this.Controls.Add(this.lblEmailWarn);
            this.Controls.Add(this.lblNameWarn);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtName);
            this.Name = "AddUser";
            this.Text = "Add User";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lblNameWarn;
        private System.Windows.Forms.Label lblEmailWarn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPasswordVerify;
        private System.Windows.Forms.Label lblPasswdMessage;
    }
}