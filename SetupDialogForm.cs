using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ASCOM.Utilities;
using ASCOM.VantagePro;

namespace ASCOM.VantagePro
{
    [ComVisible(false)]					// Form not registered for COM!

    public partial class SetupDialogForm : Form
    {
        private VantagePro vantagePro = VantagePro.Instance;

        public SetupDialogForm()
        {
            vantagePro.ReadProfile();

            InitializeComponent();
            // Initialise current values of user settings from the ASCOM Profile
            InitUI();
        }

        private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            bool ok = true;

            if (radioButtonSerialPort.Checked)
            {
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();
                List<string> portsList = new List<string>(ports);

                if (portsList.Contains((string)comboBoxComPort.SelectedItem))
                {
                    vantagePro.PortName = (string)comboBoxComPort.SelectedItem;
                    vantagePro.OperationalMode = VantagePro.OpMode.Serial;
                }
                else
                {
                    labelStatus.Text = "Invalid serial port name";
                    labelStatus.ForeColor = Color.Red;
                    ok = false;
                }
            }
            else if (radioButtonDataFile.Checked)
            {
                if (System.IO.File.Exists(textBoxReportFile.Text))
                {
                    vantagePro.DataFile = textBoxReportFile.Text;
                    vantagePro.OperationalMode = VantagePro.OpMode.File;
                }
                else
                {
                    labelStatus.Text = "Invalid report file name";
                    labelStatus.ForeColor = Color.Red;
                    ok = false;
                }
            }
            else
            {
                labelStatus.Text = "One of the radio buttons must be checked";
                labelStatus.ForeColor = Color.Red;
                ok = false;
            }
            ObservingConditions.tl.Enabled = chkTrace.Checked;

            if (ok)
                vantagePro.WriteProfile();
        }

        private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void InitUI()
        {
            chkTrace.Checked = ObservingConditions.tl.Enabled;

            comboBoxComPort.Items.Clear();
            comboBoxComPort.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());      // use System.IO because it's static

            if (comboBoxComPort.Items.Contains(vantagePro.PortName))
            {
                comboBoxComPort.SelectedItem = vantagePro.PortName;
            }

            VantagePro.OpMode opMode = vantagePro.OperationalMode;

            radioButtonDataFile.Checked = opMode == VantagePro.OpMode.File;
            radioButtonSerialPort.Checked = opMode == VantagePro.OpMode.Serial;

            textBoxReportFile.Text = vantagePro.DataFile;
        }
    }
}