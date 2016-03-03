using System;
using System.Reflection;
using System.Windows.Forms;

namespace FootballSimulationApp
{
    internal partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();

            var attributes = new AssemblyAttributes(Assembly.GetExecutingAssembly());

            Text = "About " + attributes.Title;
            labelProductName.Text = attributes.Product;
            labelVersion.Text = "Version " + attributes.Version;
            labelCopyright.Text = attributes.Copyright;
            labelCompanyName.Text = attributes.Company;
            textBoxDescription.Text = attributes.Description;
        }

        private void okButton_Click(object sender, EventArgs e) => Close();
    }
}