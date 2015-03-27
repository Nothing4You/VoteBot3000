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
        private Database database ;
        private SkypeHandler skypeHandler;
        public frmMain()
        {
            InitializeComponent();
            skypeHandler = new SkypeHandler(this);
            database = new Database(this, "localhost", "skypebot", "root", "");
        }

        public void printMessage(string message)
        {
            listBox.Items.Add(message);
        }

        public void setPassword(string sender, string password)
        {
            List<string>[] mList = new List<string>[3];
            mList = database.SelectPassword(sender);

            password = GetMD5Hash(password);

            if (mList[0].Count == 0)
            {
                database.InsertPassword("user", sender, password);
            }
            else
            {
                database.UpdatePassword("user", sender, password);
            }
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

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
        }

        private void frmMain_ResizeBegin(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
        }

        private string GetMD5Hash(string value)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                // ANSI (varchar)
                var valueBytes = Encoding.Default.GetBytes(value);
                var md5HashBytes = md5.ComputeHash(valueBytes);
                var builder = new StringBuilder(md5HashBytes.Length * 2);
                foreach (var md5Byte in md5HashBytes)
                    builder.Append(md5Byte.ToString("X2"));
                return builder.ToString();
            }
        }
    }
}
