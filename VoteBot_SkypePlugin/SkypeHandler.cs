using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYPE4COMLib;

namespace VoteBot_SkypePlugin
{
    class SkypeHandler
    {
        private Skype skype;
        private Form1 mMainFrm;

        public SkypeHandler(Form1 aMainFrm)
        {
            try
            {
                mMainFrm = aMainFrm;

                skype = new Skype();

                skype.Attach(7, false);

                skype.MessageStatus += new _ISkypeEvents_MessageStatusEventHandler(skype_MessageStatus);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void skype_MessageStatus(ChatMessage msg, TChatMessageStatus status)
        {
            if (status == TChatMessageStatus.cmsReceived)
            {
                var test = msg.Chat;
                string message = msg.Body;

                if (!string.IsNullOrEmpty(message))
                {
                    if (msg.Body.IndexOf("!") == 0)
                    {
                        string command = msg.Body.Remove(0, 1).ToLower();

                        if (command == "vote")
                        {
                            cmdvote(msg.Sender.Handle);
                        }
                        else if (command.StartsWith("vote"))
                        {
                            string[] votesplit = command.Split(' ');
                            cmdvote(msg.Sender.Handle, Convert.ToInt32(votesplit[1]), votesplit[2]);
                        }

                        if (command.StartsWith("setvote"))
                        {
                            string[] commandsplit = command.Split(' ');
                            cmdsetvote(msg.Sender.Handle, commandsplit[1], Convert.ToInt32(commandsplit[2]), commandsplit[3]);
                        }

                        if (command == "status")
                        {
                            cmdstatus();
                        }

                    }

                }

            }
        }

        private void cmdvote(string sender)
        {
            skype.SendMessage(sender, "Tippe !vote 'Ort' 'Zeit'" + "\r\n" + "\r\n" + "Orte:" + "\r\n" + "1 = L'Osteria" + "\r\n" + "2 = Kantine" + "\r\n" + "3 = Rohmühle" + "\r\n" + "4 = Der Pate" + "\r\n" + "\r\n" + "Zeiten:" + "\r\n" + "1200 = 12:00 Uhr" + "\r\n" + "1215 = 12:15 Uhr" + "\r\n" + "1230 = 12:30 Uhr");
        }

        private void cmdvote(string sender, int place, string time)
        {
            if (place < 5 && place > 0)
            {
                if (time == "1200" || time == "1215" || time == "1230")
                {
                    mMainFrm.update(sender, Convert.ToString(place), time);
                    skype.SendMessage(sender, "Erfolgreich für " + Convert.ToString(place) + " um " + time + " abgestimmt");
                }
            }
        }

        private void cmdsetvote(string sender, string name, int place, string time)
        {
            if (sender == "niklas-bjonik")
            {
                if (place < 5 && place > 0)
                {
                    if (time == "1200" || time == "1215" || time == "1230")
                    {
                        mMainFrm.update(name, Convert.ToString(place), time);
                        skype.SendMessage(sender, "Set user " + name + " = " + Convert.ToString(place) + " " + time);
                    }
                }
            }
            else
            {
                skype.SendMessage(sender, "Du hast keine Berechtigung um diesen Befehl zu nutzen");
            }
        }

        private void cmdstatus()
        {

        }

    }
}
