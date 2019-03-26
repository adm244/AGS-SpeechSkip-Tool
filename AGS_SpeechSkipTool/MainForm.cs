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
    private SpeechSkipPatcher patcher;

    public MainForm()
    {
      InitializeComponent();
      InitializeSpeechTypeSelector();
      InitializePatcher();
    }

    private void InitializePatcher()
    {
      patcher = new SpeechSkipPatcher();
      patcher.OnPatcherEvent += OnPatcherEvent;
    }

    private void InitializeSpeechTypeSelector()
    {
      foreach (SpeechSkipType type in Enum.GetValues(typeof(SpeechSkipType)))
      {
        cbSpeechSkipType.Items.Add(type);
      }
      cbSpeechSkipType.SelectedIndex = 0;
    }

    private DialogResult ShowError(string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
      return MessageBox.Show(this, message, "Error", buttons, MessageBoxIcon.Error);
    }

    private DialogResult ShowWarning(string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
      return MessageBox.Show(this, message, "Warning", buttons, MessageBoxIcon.Warning);
    }

    private DialogResult ShowMessage(string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
      return MessageBox.Show(this, message, "Information", buttons, MessageBoxIcon.Information);
    }

    private bool OnPatcherEvent(PatcherEventType eventType, PatcherEventData data)
    {
      switch (eventType)
      {
        case PatcherEventType.UnsupportedDTA:
          {
            string message = "Unsupported DTA file version detected: " + data.DTAVersion + Environment.NewLine
              + Environment.NewLine
              + "It is STRONGLY recommended to make backup files before proceeding." + Environment.NewLine
              + Environment.NewLine
              + "Continue patching?";
            return ShowWarning(message, MessageBoxButtons.YesNo) == DialogResult.Yes;
          }

        default:
          return true;
      }
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

          ShowMessage("Game was successfully patched!");
        }
        else
        {
          ShowError("Could not patch game!");
        }
      }
      else
      {
        ShowError("Selected path does not exist!");
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
          ShowError("Selected path does not exist!");
        }
      }
    }
  }
}
