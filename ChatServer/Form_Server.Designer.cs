namespace ChatServer
{
    partial class Form_Server
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_RichTextBox = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel_RichTextBox
            // 
            this.panel_RichTextBox.Location = new System.Drawing.Point(12, 12);
            this.panel_RichTextBox.Name = "panel_RichTextBox";
            this.panel_RichTextBox.Size = new System.Drawing.Size(200, 226);
            this.panel_RichTextBox.TabIndex = 0;
            // 
            // Form_Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 346);
            this.Controls.Add(this.panel_RichTextBox);
            this.Name = "Form_Server";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_RichTextBox;

    }
}

