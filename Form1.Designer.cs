namespace AOTW_P2PChat
{
    partial class Form1
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
            SendMessage = new Button();
            MessageBox = new TextBox();
            IPBox = new TextBox();
            PortBox = new TextBox();
            ReadBox = new RichTextBox();
            SuspendLayout();
            // 
            // SendMessage
            // 
            SendMessage.Location = new Point(713, 415);
            SendMessage.Name = "SendMessage";
            SendMessage.Size = new Size(75, 23);
            SendMessage.TabIndex = 0;
            SendMessage.Text = "Send";
            SendMessage.UseVisualStyleBackColor = true;
            SendMessage.Click += button1_Click;
            // 
            // MessageBox
            // 
            MessageBox.Location = new Point(12, 416);
            MessageBox.Name = "MessageBox";
            MessageBox.Size = new Size(680, 23);
            MessageBox.TabIndex = 1;
            // 
            // IPBox
            // 
            IPBox.Location = new Point(12, 12);
            IPBox.Name = "IPBox";
            IPBox.Size = new Size(145, 23);
            IPBox.TabIndex = 2;
            // 
            // PortBox
            // 
            PortBox.Location = new Point(163, 12);
            PortBox.Name = "PortBox";
            PortBox.Size = new Size(62, 23);
            PortBox.TabIndex = 3;
            // 
            // ReadBox
            // 
            ReadBox.Location = new Point(12, 62);
            ReadBox.Name = "ReadBox";
            ReadBox.Size = new Size(776, 304);
            ReadBox.TabIndex = 4;
            ReadBox.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ReadBox);
            Controls.Add(PortBox);
            Controls.Add(IPBox);
            Controls.Add(MessageBox);
            Controls.Add(SendMessage);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button SendMessage;
        private TextBox MessageBox;
        private TextBox IPBox;
        private TextBox PortBox;
        private RichTextBox ReadBox;
    }
}
