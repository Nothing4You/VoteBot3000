#!/usr/bin/env python3
from skpy import Skype, SkypeEventLoop, SkypeAuthException, SkypeUtils, SkypeNewMessageEvent

import pymysql
import os
import sys
import traceback
import logging
import datetime
import google
import random
import time

from config import *

class MySkype(SkypeEventLoop):
  locs = {
    1: ["1", "losteria", "l'osteria", "luli", "lulli", "pizza", "(pizza)"],
    2: ["2", "kantine", "bistro", "dahlienfeld", "bielefeld", "rolf", "burger"],
    3: ["3", "rohmühle", "rohmuehle"],
    5: ["5", "alm", "rheinalm"],
    6: ["6", "kameha", "next-level", "brasserie", "next", "nlr"]
  }

  locSuffixes = {
    1: ["eria", "zza"],
    2: ["ine", "feld", "rolf"],
    5: ["alm"],
    6: ["eha"]
  }

  locNames = {
    1: "L'Osteria",
    2: "Bistro Dahlienfeld",
    3: "Rohmühle",
    5: "Rheinalm",
    6: "Kameha Brasserie Next Level"
  }

  def eightball(self):
    pos = [
      "Es ist sicher",
      "Es ist so entschieden",
      "Ohne jeden Zweifel",
      "Definitiv ja",
      "Darauf kannst du dich verlassen",
      "Soweit ich es verstehe, ja",
      "Höchstwahrscheinlich",
      "Gute Aussichten",
      "Ja",
      "Zeichen deuten auf ja",
      "JA!"
    ]
    neut = [
      "Antwort unklar, versuchs noch mal",
      "Frag später noch mal",
      "Das sag ich dir jetzt besser nicht",
      "Kann ich jetzt nicht vorhersagen",
      "Konzentriere dich und frag noch mal",
      "KEINE AHNUNG!"
    ]
    neg = [
      "Zähl nicht drauf",
      "Meine Antwort ist nein",
      "Meine Quellen sagen nein",
      "Nicht so gute Aussichten",
      "Sehr zweifelhaft",
      "NEIN!"
    ]

    return random.choice(random.choice([pos,neg,neut]))

  def getLoc(self, loc):
    loc = loc.lower()
    ret = next((k for k, v in self.locs.items() if loc in v), None)

    if ret == None:
      ret = next((k for k, v in self.locSuffixes.items() if any(s for s in v if loc.endswith(s))), None)

    return ret


  def onEvent(self, event):
    try:
      if isinstance(event, SkypeNewMessageEvent) and not event.msg.userId == self.userId:
        text = event.msg.plain.strip()

        if (text.startswith("!") or text.startswith(".")) and len(text) > 1: # Command
          text = text[1:]
          parts = text.split(" ")
          command = parts[0].lower()

          if command == "vote" or command == "v":
            if len(parts) >= 2:
              if len(parts) < 3:
                parts.append("1200")
              self.castVote(event.msg.chat, event.msg.userId, parts[1], parts[2])
              if event.msg.userId == "matthias.neid" and datetime.datetime.today().date() == datetime.datetime.strptime("2017-01-16", "%Y-%m-%d").date():
                self.castVote(event.msg.chat, "maiiko94", parts[1], parts[2])
            else:
              self.showVoteHelp(event.msg.userId)

          elif command == "setvote":
            if len(parts) >= 4 and event.msg.userId == "live:mail_8717":
              self.castVote(event.msg.chat, parts[1], parts[2], parts[3])

          elif command == "g":
            if len(parts) >= 2:
              event.msg.chat.setTyping(True)

              search_string = " ".join(parts[1:])

              try:
                response = google.lucky(search_string, tld="de", only_standard=True)
              except Exception:
                logging.exception("Exception")
                traceback.print_exc()
                response = "NEIN!"

              event.msg.chat.sendMsg(response)
              event.msg.chat.setTyping(False)
            else:
              self.getPrivateChat(event.msg.userId).sendMsg("Syntax: !g <query>")

          elif command == "ask" or command == "a":
            event.msg.chat.setTyping(True)
#            if random.choice([True, False]):
#              response = "JA!"
#            else:
#              response = "NEIN!"

            response = self.eightball()

            time.sleep(random.random()*2)

            event.msg.chat.sendMsg(response)
            event.msg.chat.setTyping(False)

          elif command == "karte" or command == "k":
            event.msg.chat.setTyping(True)

            response = None

            if len(parts) >= 2:
              loc = self.getLoc(parts[1])

              if loc == 1:
                response = "http://losteria.de/menu/woche/"

              if loc == 2:
                response = "http://bistro-dahlienfeld.de/Portals/0/pdf/SER/aktuelle%20Speisekarte.pdf"

              if loc in [3]:
                response = "Unbekannt"

              if loc == 5:
                response = "https://www.kamehabonn.de/de/bayrischer-mittags-schmankerl.html"

              if loc == 6:
                isocal = datetime.datetime.utcnow().isocalendar()
                response = "https://www.kamehabonn.de/fileadmin/kameha/Ressources/Speisekarten/Brasserie/Plat_du_jour/Brasserie_Aufsteller_{0}_KW{1}.pdf".format(str(isocal[0]), str(isocal[1]))

            if response == None:
              response = "Syntax: !karte <ort>"

            event.msg.chat.sendMsg(response)
            event.msg.chat.setTyping(False)

          elif command == "quit" or command == "q":
            if event.msg.userId == "live:mail_8717":
              self.run = False

    except Exception:
      logging.exception("Exception")
      traceback.print_exc()

  def showVoteHelp(self, recipient):
    self.getPrivateChat(recipient).sendMsg("Tippe !vote 'Ort' 'Zeit'" + "\r\n" + "\r\n" + "Orte:" + "\r\n" + "1 = L'Osteria" + "\r\n" + "2 = Kantine" + "\r\n" + "3 = Rohmühle" + "\r\n" + "5 = Rheinalm" + "\r\n" + "6 = Kameha Next Level Restaurant" + "\r\n" + "\r\n" + "Zeiten:" + "\r\n" + "1150 = 11:50 Uhr" + "\r\n" + "1200 = 12:00 Uhr" + "\r\n" + "1215 = 12:15 Uhr" + "\r\n" + "1230 = 12:30 Uhr")

  def castVote(self, chat, user, location, time):
    logging.info("%s votes for %s at %s" % (user, location, time))

    chat.setTyping(True)

    loc = self.getLoc(location)

    if loc == None:
      chat.sendMsg(str(location) + " ist kein gültiger Ort!")
      chat.setTyping(False)
      return

    timeErr = False
    try:
      time = int(time.replace(":", ""))
    except Exception:
      timeErr = True
    if timeErr or time not in [1150, 1200, 1215, 1230]:
      chat.sendMsg(str(time) + " ist keine gültige Zeit!")
      chat.setTyping(False)
      return

    if loc == 5 and datetime.datetime.today().weekday() == 0:
      chat.sendMsg("Montags geschlossen!")
      chat.setTyping(False)
      return

    self.dbConnect()
    cursor = self.db.cursor()

    sql = "REPLACE INTO votes (date, user, location, time) VALUES (CURRENT_DATE(), '%s', '%d', '%d')" % (user, loc, time)

    try:
      cursor.execute(sql)
      self.db.commit()
      chat.setTyping(False)
      chat.sendMsg("Erfolgreich für %s um %d abgestimmt" % (self.locNames[loc], time))
    except Exception:
      logging.exception("Exception")
      #traceback.print_exc()
      chat.sendMsg("Es ist ein Fehler beim Speichervorgang aufgetreten!")
      chat.setTyping(False)
      self.db.rollback()

    self.dbDisconnect()


  def respond(self, orig_message, message):
    try:
      orig_message.chat.sendMsg(message)
    except:
      self.getPrivateChat(orig_message.userId).sendMsg(message)

  def getPrivateChat(self, recipient):
    return self.chats['8:' + recipient]

  def setUpMysql(self, hostname='localhost', username='root', password='', database='mysql', port=3306):
    self.db_hostname = hostname
    self.db_port     = port
    self.db_username = username
    self.db_password = password
    self.db_database = database

  def dbConnect(self):
    self.db = pymysql.connect(self.db_hostname, self.db_username, self.db_password, self.db_database, self.db_port)

  def dbDisconnect(self):
    self.db.close()

  def loop(self):
    self.run = True

    while self.run:
      try:
        self.cycle()
      except KeyboardInterrupt:
        logging.info("Gracefully stopping...")
        self.run = False

def main():
  #logging.basicConfig(level=logging.DEBUG, format='%(asctime)s - %(levelname)s - %(message)s', handlers=[logging.FileHandler("skpy.log"), logging.StreamHandler()])
  logging.basicConfig(level=logging.DEBUG, format='%(asctime)s - %(levelname)s - %(message)s', handlers=[logging.FileHandler("skpy.log")])

  sk = MySkype(SKYPE_USER, SKYPE_PASS, tokenFile=".tokens")

  richard_id = '8:live:mail_8717'
  sk.chats[richard_id].sendMsg("Service available!")

  sk.setUpMysql(SQL_HOST, SQL_USER, SQL_PASS, SQL_DB, SQL_PORT)

  sk.loop()

if __name__ == "__main__":
  pidfile = "skpy.pid"
  pid = str(os.getpid())
  open(pidfile, "w").write(pid)

  try:
    main()
  finally:
    os.unlink(pidfile)
