﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public partial class WelcomeForm : Form
    {
        private Manifest man;

        public WelcomeForm()
        {
            InitializeComponent();
            man = Manifest.GetManifest();
        }

        private void btnJustStart_Click(object sender, EventArgs e)
        {
            // Mark as not first run anymore
            man.FirstRun = false;
            man.Save();

            showMainForm();
        }

        private void btnImportConfig_Click(object sender, EventArgs e)
        {
            // Let the user select the config dir
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select the folder of your old Steam Desktop Authenticator install";
            DialogResult userClickedOK = folderBrowser.ShowDialog();

            if (userClickedOK == DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;
                string pathToCopy = null;

                if (Directory.Exists(path + "/maFiles"))
                {
                    // User selected the root install dir
                    pathToCopy = path + "/maFiles";
                }
                else if (File.Exists(path + "/manifest.json"))
                {
                    // User selected the maFiles dir
                    pathToCopy = path;
                }
                else
                {
                    // Could not find either.
                    MessageBox.Show("This folder does not contain either a manifest.json or an maFiles folder.\nPlease select the location where you had Steam Desktop Authenticator installed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                    return;
                }

                // Copy the contents of the config dir to the new config dir
                string currentPath = Manifest.GetExecutableDir();

                // Create config dir if we don't have it
                if (!Directory.Exists(currentPath + "/maFiles"))
                {
                    Directory.CreateDirectory(currentPath + "/maFiles");
                }

                // Copy all files from the old dir to the new one
                foreach (string newPath in Directory.GetFiles(pathToCopy, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(pathToCopy, currentPath + "/maFiles"), true);
                }

                // Set first run in manifest
                man = Manifest.GetManifest(true);
                man.FirstRun = false;
                man.Save();

                // All done!
                MessageBox.Show("All accounts and settings have been imported! Click OK to continue.", "Import accounts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                showMainForm();
            }

        }

        private void showMainForm()
        {
            this.Hide();
            new MainForm().Show();
        }

        private void WelcomeForm_Load(object sender, EventArgs e)
        {

        }
    }
}
