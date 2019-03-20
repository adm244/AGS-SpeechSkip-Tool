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

using System;
using System.IO;
using System.Windows.Forms;
using AGS_SpeechSkipTool.Patcher;

namespace AGS_SpeechSkipTool
{
  public partial class MainForm : Form
  {
    private OpenFileDialog ofd = new OpenFileDialog();
    private SpeechSkipPatcher patcher = new SpeechSkipPatcher();

    public MainForm()
    {
      InitializeComponent();
      InitializeSpeechTypeSelector();
    }

    private void InitializeSpeechTypeSelector()
    {
      foreach (SpeechSkipType type in Enum.GetValues(typeof(SpeechSkipType)))
      {
        cbSpeechSkipType.Items.Add(type);
      }
      cbSpeechSkipType.SelectedIndex = 0;
    }

    private void btnPatch_Click(object sender, EventArgs e)
    {
      if (File.Exists(tbGameFolder.Text))
      {
        bool result = patcher.Patch(tbGameFolder.Text, (SpeechSkipType)cbSpeechSkipType.SelectedItem, cbMakeBackup.Checked);
        if (result)
        {
          if (cbMakeBackup.Checked)
            cbMakeBackup.Checked = false;

          MessageBox.Show(this, "Game was successfully patched!",
            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          MessageBox.Show(this, "Could not patch game!",
            "Failed to patch game", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
      else
      {
        MessageBox.Show(this, "Selected path does not exist!",
          "File does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnSelectGameFolder_Click(object sender, EventArgs e)
    {
      ofd.CheckPathExists = true;
      ofd.CheckFileExists = true;
      ofd.Filter = "AGS game executable|*.exe";

      if (ofd.ShowDialog(this) == DialogResult.OK)
      {
        if (File.Exists(ofd.FileName))
        {
          tbGameFolder.Text = ofd.FileName;
        }
        else
        {
          MessageBox.Show(this, "Selected path does not exist!", "File does not exist.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }
  }
}
