using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VoteBot_SkypePlugin
{
    public partial class frmMain : Form
    {
        private Database database = new Database("localhost", "skypebot", "root", "");
        private SkypeHandler skypeHandler;
        public frmMain()
        {
            InitializeComponent();
            skypeHandler = new SkypeHandler(this);
        }

        public void update(string sender, string place, string time)
        {
            DateTime date = DateTime.Now;
            string sDate = date.ToString("yyyy-MM-dd");

            List<string>[] mList = new List<string>[5];
            mList = database.Select(sender, sDate);

            if (mList[0].Count == 0)
            {
                database.Insert("datensaetze", sDate, sender, place, time);
            }
            else
            {
                database.Update("datensaetze", mList[0][0], place, time);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }
    }
}
