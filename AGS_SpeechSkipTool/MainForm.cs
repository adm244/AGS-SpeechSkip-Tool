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
