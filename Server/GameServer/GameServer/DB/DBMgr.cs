using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PENet;
using PEProtocol;


namespace GameServer
{
    public class DBMgr : Singleton<DBMgr>
    {

        private MySqlConnection conn = null;

        public void Init()
        {
            PECommon.Log("DBMgr Loading");
            try
            {
                conn = new MySqlConnection("server=localhost; " +
                    "port=3306;" +
                    "user = root;" +
                    "password = gcb05191811;" +
                    "database=darkgod;"
                    );
                conn.Open();
            }
            catch(Exception e)
            {
                PECommon.Log(e.Message);
            }
        }

        public PlayerData QueryPlayerData(string acc, string pas)
        {
            PlayerData playerData = null;
            MySqlDataReader reader = null;
            bool isNew = true;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from account where acc = @acc", conn);
                cmd.Parameters.AddWithValue("acc", acc);
                reader = cmd.ExecuteReader();

                if(reader.Read())
                {
                    string _pas = reader.GetString("pas");
                    isNew = false;
                    if (_pas.Equals(pas))
                    {
                        playerData = new PlayerData
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            lv = reader.GetInt32("level"),
                            exp = reader.GetInt32("exp"),
                            power = reader.GetInt32("power"),
                            coin = reader.GetInt32("coin"),
                            diamond = reader.GetInt32("diamond")
                        };

                    }
                }
            }
            catch (Exception e)
            {
                PECommon.Log("Query PlayerData By Acc&Pas Error:" + e, LogType.Error);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if(isNew)
                {
                    playerData = new PlayerData
                    {
                        id = -1,
                        name = "",
                        lv = 1,
                        exp = 0,
                        power = 150,
                        coin = 5000,
                        diamond = 500
                    };

                    playerData.id = InsertNewAccData(acc, pas, playerData);
                }
            }

            
            return playerData;
        }

        private int InsertNewAccData(string acc, string pas, PlayerData pd)
        {
            int _id = -1;
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    "insert into account set acc= @acc," +
                    "pas =@pas,name=@name,level=@level," +
                    "exp=@exp,power=@power,coin=@coin," +
                    "diamond=@diamond", conn);
                cmd.Parameters.AddWithValue("acc", acc);
                cmd.Parameters.AddWithValue("pas", pas);
                cmd.Parameters.AddWithValue("name", pd.name);
                cmd.Parameters.AddWithValue("level", pd.lv);
                cmd.Parameters.AddWithValue("exp", pd.exp);
                cmd.Parameters.AddWithValue("power", pd.power);
                cmd.Parameters.AddWithValue("coin", pd.coin);
                cmd.Parameters.AddWithValue("diamond", pd.diamond);

                cmd.ExecuteNonQuery();
                _id = (int)cmd.LastInsertedId;
            }
            catch (Exception e)
            {
                PECommon.Log("Query PlayerData By Acc&Pas Error:" + e, LogType.Error);
            }

            return _id;
        }

        public bool QueryNameData(string name)
        {
            bool exist = false;
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from account where name= @name", conn);
                cmd.Parameters.AddWithValue("name", name);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    exist = true;
                }
            }
            catch (Exception e)
            {
                PECommon.Log("Query Name By State Error:" + e, LogType.Error);
            }

            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return exist;
        }

        public bool UpdatePlayerData(int id, PlayerData playerData)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
               "update account set name=@name," +
               "level=@level,exp=@exp,power=@power," +
               "coin=@coin,diamond=@diamond where id =@id", conn);

                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("name", playerData.name);
                cmd.Parameters.AddWithValue("level", playerData.lv);
                cmd.Parameters.AddWithValue("exp", playerData.exp);
                cmd.Parameters.AddWithValue("power", playerData.power);
                cmd.Parameters.AddWithValue("coin", playerData.coin);
                cmd.Parameters.AddWithValue("diamond", playerData.diamond);

                //TOADD Others
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                PECommon.Log("Update PlayerData Error:" + e, LogType.Error);
                return false;
            }

            return true;
        }
    }


}
