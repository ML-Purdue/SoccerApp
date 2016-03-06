using System;
using System.Reflection;
using System.Windows.Forms;
using FootballSimulationApp.Properties;

namespace FootballSimulationApp
{
    internal sealed partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();

            var attributes = new AssemblyAttributes(Assembly.GetExecutingAssembly());

            Text = Resources.AboutBox_About + attributes.Title;
            labelProductName.Text = attributes.Product;
            labelVersion.Text = Resources.AboutBox_Version + attributes.Version;
            labelCopyright.Text = attributes.Copyright;
            labelCompanyName.Text = attributes.Company;
            textBoxDescription.Text = attributes.Description;
        }

        private void okButton_Click(object sender, EventArgs e) => Close();
    }
}