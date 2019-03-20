/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

using AGS_SpeechSkipTool.Properties;

namespace AGS_SpeechSkipTool
{
  partial class MainForm
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
      this.Icon = Resources.cup_icon;

      this.btnSelectGameFolder = new System.Windows.Forms.Button();
      this.cbSpeechSkipType = new System.Windows.Forms.ComboBox();
      this.lblGameFolder = new System.Windows.Forms.Label();
      this.lblSpeechSkipType = new System.Windows.Forms.Label();
      this.tbGameFolder = new System.Windows.Forms.TextBox();
      this.btnPatch = new System.Windows.Forms.Button();
      this.cbMakeBackup = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // btnSelectGameFolder
      // 
      this.btnSelectGameFolder.Location = new System.Drawing.Point(228, 22);
      this.btnSelectGameFolder.Name = "btnSelectGameFolder";
      this.btnSelectGameFolder.Size = new System.Drawing.Size(26, 23);
      this.btnSelectGameFolder.TabIndex = 0;
      this.btnSelectGameFolder.Text = "...";
      this.btnSelectGameFolder.UseVisualStyleBackColor = true;
      this.btnSelectGameFolder.Click += new System.EventHandler(this.btnSelectGameFolder_Click);
      // 
      // cbSpeechSkipType
      // 
      this.cbSpeechSkipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbSpeechSkipType.FormattingEnabled = true;
      this.cbSpeechSkipType.Location = new System.Drawing.Point(15, 72);
      this.cbSpeechSkipType.Name = "cbSpeechSkipType";
      this.cbSpeechSkipType.Size = new System.Drawing.Size(239, 21);
      this.cbSpeechSkipType.TabIndex = 1;
      // 
      // lblGameFolder
      // 
      this.lblGameFolder.AutoSize = true;
      this.lblGameFolder.Location = new System.Drawing.Point(12, 9);
      this.lblGameFolder.Name = "lblGameFolder";
      this.lblGameFolder.Size = new System.Drawing.Size(62, 13);
      this.lblGameFolder.TabIndex = 2;
      this.lblGameFolder.Text = "Game path:";
      this.lblGameFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lblSpeechSkipType
      // 
      this.lblSpeechSkipType.AutoSize = true;
      this.lblSpeechSkipType.Location = new System.Drawing.Point(12, 56);
      this.lblSpeechSkipType.Name = "lblSpeechSkipType";
      this.lblSpeechSkipType.Size = new System.Drawing.Size(92, 13);
      this.lblSpeechSkipType.TabIndex = 3;
      this.lblSpeechSkipType.Text = "Speech skip type:";
      // 
      // tbGameFolder
      // 
      this.tbGameFolder.Location = new System.Drawing.Point(15, 25);
      this.tbGameFolder.Name = "tbGameFolder";
      this.tbGameFolder.ReadOnly = true;
      this.tbGameFolder.Size = new System.Drawing.Size(207, 20);
      this.tbGameFolder.TabIndex = 4;
      // 
      // btnPatch
      // 
      this.btnPatch.Location = new System.Drawing.Point(179, 116);
      this.btnPatch.Name = "btnPatch";
      this.btnPatch.Size = new System.Drawing.Size(75, 23);
      this.btnPatch.TabIndex = 5;
      this.btnPatch.Text = "Patch";
      this.btnPatch.UseVisualStyleBackColor = true;
      this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
      // 
      // cbMakeBackup
      // 
      this.cbMakeBackup.AutoSize = true;
      this.cbMakeBackup.Checked = true;
      this.cbMakeBackup.CheckState = System.Windows.Forms.CheckState.Checked;
      this.cbMakeBackup.Location = new System.Drawing.Point(15, 120);
      this.cbMakeBackup.Name = "cbMakeBackup";
      this.cbMakeBackup.Size = new System.Drawing.Size(84, 17);
      this.cbMakeBackup.TabIndex = 7;
      this.cbMakeBackup.Text = "Backup files";
      this.cbMakeBackup.UseVisualStyleBackColor = true;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(266, 151);
      this.Controls.Add(this.cbMakeBackup);
      this.Controls.Add(this.btnPatch);
      this.Controls.Add(this.tbGameFolder);
      this.Controls.Add(this.lblSpeechSkipType);
      this.Controls.Add(this.lblGameFolder);
      this.Controls.Add(this.cbSpeechSkipType);
      this.Controls.Add(this.btnSelectGameFolder);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.Text = "AGS SpeechSkipTool";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnSelectGameFolder;
    private System.Windows.Forms.ComboBox cbSpeechSkipType;
    private System.Windows.Forms.Label lblGameFolder;
    private System.Windows.Forms.Label lblSpeechSkipType;
    private System.Windows.Forms.TextBox tbGameFolder;
    private System.Windows.Forms.Button btnPatch;
    private System.Windows.Forms.CheckBox cbMakeBackup;
  }
}

