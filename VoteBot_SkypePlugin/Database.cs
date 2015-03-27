using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace VoteBot_SkypePlugin
{
    class Database
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private frmMain mMainFrm;

        //Constructor
        public Database(frmMain aMainFrm, string aServer, string aDatabase, string aUID, string aPassword)
        {
            try
            {
                mMainFrm = aMainFrm;
                server = aServer;
                database = aDatabase;
                uid = aUID;
                password = aPassword;

                string connectionString;
                connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

                connection = new MySqlConnection(connectionString);
                //this.OpenConnection();
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
            }
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
                return false;
            }
        }

        //Insert statement
        public void Insert(string aTable, string aDate, string aName, string aPlace, string aTime)
        {
            try
            {
                string query = "INSERT INTO " + aTable + " (tag, voter, ort, zeit) VALUES ('" + aDate + "', '" + aName + "', '" + aPlace + "', '" + aTime + "')";

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
            }
        }

        //Update statement
        public void Update(string aTable, string aID, string aPlace, string aTime)
        {
            try
            {
                string query = "UPDATE " + aTable + " SET ort = '" + aPlace + "' WHERE id = " + aID;
                string query2 = "UPDATE " + aTable + " SET zeit = '" + aTime + "' WHERE id = " + aID;

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //create mysql command
                    MySqlCommand cmd = new MySqlCommand();
                    //Assign the query using CommandText
                    cmd.CommandText = query;

                    //Assign the connection using Connection
                    cmd.Connection = connection;

                    //Execute query
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = query2;

                    //Assign the connection using Connection

                    //Execute query
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
            }
        }

        //Delete statement
        public void Delete()
        {
            try
            {
                string query = "DELETE FROM datensaetze WHERE voter = 'test' and tag = '2015-03-12'";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
            }
        }

        //Select statement
        public List<string>[] Select(string aName, string aDay)
        {
            try
            {
                string query = "SELECT * FROM datensaetze WHERE voter = '" + aName + "' and tag = '" + aDay + "'";

                //Create a list to store the result
                List<string>[] list = new List<string>[5];
                list[0] = new List<string>();
                list[1] = new List<string>();
                list[2] = new List<string>();
                list[3] = new List<string>();
                list[4] = new List<string>();

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        list[0].Add(dataReader["id"] + "");
                        list[1].Add(dataReader["tag"] + "");
                        list[2].Add(dataReader["voter"] + "");
                        list[3].Add(dataReader["ort"] + "");
                        list[4].Add(dataReader["zeit"] + "");
                    }

                    //close Data Reader
                    dataReader.Close();

                    //close Connection
                    this.CloseConnection();

                    //return list to be displayed
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
                return null;
            }
        }

        //Select statement
        public List<string>[] Load(string aDay)
        {
            try
            {
                string query = "SELECT * FROM datensaetze WHERE tag = '" + aDay + "'";

                //Create a list to store the result
                List<string>[] list = new List<string>[0];

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    int counter = 0;
                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        Array.Resize(ref list, counter + 1);
                        list[counter] = new List<string>();
                        list[counter].Add(dataReader["id"] + "");
                        list[counter].Add(dataReader["tag"] + "");
                        list[counter].Add(dataReader["voter"] + "");
                        list[counter].Add(dataReader["ort"] + "");
                        list[counter].Add(dataReader["zeit"] + "");
                        counter++;
                    }

                    //close Data Reader
                    dataReader.Close();

                    //close Connection
                    this.CloseConnection();

                    //return list to be displayed
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
                return null;
            }
        }

        //Count statement
        public int Count()
        {
            try
            {
                string query = "SELECT Count(*) FROM `datensaetze`";
                int Count = -1;

                //Open Connection
                if (this.OpenConnection() == true)
                {
                    //Create Mysql Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //ExecuteScalar will return one value
                    Count = int.Parse(cmd.ExecuteScalar() + "");

                    //close Connection
                    this.CloseConnection();

                    return Count;
                }
                else
                {
                    return Count;
                }
            }
            catch (Exception ex)
            {
                mMainFrm.printMessage(ex.Message);
                return 0;
            }
        }
    }
}
