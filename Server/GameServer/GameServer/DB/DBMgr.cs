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

        public bool QueryPlayerTaskData(ref PlayerData pd)
        {
            MySqlDataReader reader = null;
            try 
            {
                int cnt = 0;
                MySqlCommand cmd = new MySqlCommand("select * from task where playerId = @playerId", conn);
                cmd.Parameters.AddWithValue("playerId", pd.id);
                reader = cmd.ExecuteReader();
                List<NTaskInfo> infos = new List<NTaskInfo>();
                while(reader.Read())
                {
                    NTaskInfo info = new NTaskInfo
                    {
                        npcID = reader.GetInt32("npcId"),
                        taskID = reader.GetInt32("taskId"),
                        prg = reader.GetInt32("prg"),
                    };

                    int status = reader.GetInt32("status");
                    switch (status)
                    {
                        case 0:
                            info.taskState = PEProtocol.TaskStatus.InProgress;
                            break;
                        case 1:
                            info.taskState = PEProtocol.TaskStatus.Complated;
                            break;
                        case 2:
                            info.taskState = PEProtocol.TaskStatus.Finished;
                            break;
                        case 3:
                            info.taskState = PEProtocol.TaskStatus.Failed;
                            break;
                        default:
                            info.taskState = PEProtocol.TaskStatus.None;
                            break;
                    }

                    infos.Add(info);
                }
                pd.taskDatas = infos.ToArray();
                infos.Clear();
                reader.Close();
                return true;
            }
            catch
            {
                PECommon.Log("Get Player Task Data Error", LogType.Error);
                return false;
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
                            diamond = reader.GetInt32("diamond"),
                            hp = reader.GetInt32("hp"),
                            ad = reader.GetInt32("ad"),
                            ap = reader.GetInt32("ap"),
                            addef = reader.GetInt32("addef"),
                            apdef = reader.GetInt32("apdef"),
                            dodge = reader.GetInt32("dodge"),
                            pierce = reader.GetInt32("pierce"),
                            critical = reader.GetInt32("critical"),
                            guideid = reader.GetInt32("guideid"),
                            crystal = reader.GetInt32("crystal"),
                            time = reader.GetInt64("time"),
                            mission = reader.GetInt32("mission"),
                            modle = reader.GetString("modle"),
                            equipment = reader.GetString("equipment"),
                        };

                        string[] strong_strArr = reader.GetString("strong").Split('#');
                        int[] strongArr = new int[6];
                        for(int i = 0; i < strong_strArr.Length; i++)
                        {
                            if(strong_strArr[i] == "")
                            {
                                continue;
                            }
                            if(int.TryParse(strong_strArr[i], out int num))
                            {
                                strongArr[i] = num;
                            }
                            else
                            {
                                PECommon.Log("Parse Strong String Error", LogType.Error);
                            }
                        }

                        playerData.strong = strongArr;

                        string[] task_strArr = reader.GetString("task").Split('#');
                        playerData.task = new string[task_strArr.Length - 1];
                        for (int i = 0; i < playerData.task.Length; i++)
                        {
                            if (task_strArr[i] == "")
                            {
                                continue;
                            }
                            else if (task_strArr[i].Length >= 5)
                            {
                                playerData.task[i] = task_strArr[i];
                            }
                            else
                            {
                                PECommon.Log("Data Error",LogType.Error);
                            }
                        }

                        playerData.bag = reader.GetString("bag").Split('|');

                        reader.Close();
                        QueryPlayerTaskData(ref playerData);
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
                        diamond = 500,

                        hp = 2000,
                        ad = 275,
                        ap = 265,
                        addef = 67,
                        apdef = 43,
                        dodge = 7,
                        pierce = 5,
                        critical = 2,
                        guideid = 1001,

                        strong = new int[6],
                        crystal = 500,
                        time = TimerSvc.Instance.GetNowTime(),
                        task = new string[CfgSvc.Instance.GetTaskConut()],
                        mission = 10001,
                        taskDatas = { },
                        modle = "",
                        bag = new string[] { "10000001#5", "10000002#5", "10000016#1", "10000005#1" },
                        equipment = "10000016|10000005",
                    };

                    string[] _taskArr = playerData.task;
                    for (int i = 0; i < _taskArr.Length; i++)
                    {
                        _taskArr[i] = (i+1) + "|0|0";
                    }


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
                    "diamond=@diamond,hp=@hp,ad=@ad," +
                    "ap=@ap,addef=@addef,apdef=@apdef," +
                    "dodge=@dodge,pierce=@pierce," +
                    "critical=@critical, guideid=@guideid," +
                    "strong=@strong,crystal=@crystal,time=@time" +
                    ",task=@task,mission=@mission,modle=@modle,bag=@bag,equipment=@equipment", conn);
                cmd.Parameters.AddWithValue("acc", acc);
                cmd.Parameters.AddWithValue("pas", pas);
                cmd.Parameters.AddWithValue("name", pd.name);
                cmd.Parameters.AddWithValue("level", pd.lv);
                cmd.Parameters.AddWithValue("exp", pd.exp);
                cmd.Parameters.AddWithValue("power", pd.power);
                cmd.Parameters.AddWithValue("coin", pd.coin);
                cmd.Parameters.AddWithValue("diamond", pd.diamond);

                cmd.Parameters.AddWithValue("hp", pd.hp);
                cmd.Parameters.AddWithValue("ad", pd.ad);
                cmd.Parameters.AddWithValue("ap", pd.ap);
                cmd.Parameters.AddWithValue("addef", pd.addef);
                cmd.Parameters.AddWithValue("apdef", pd.apdef);
                cmd.Parameters.AddWithValue("dodge", pd.dodge);
                cmd.Parameters.AddWithValue("pierce", pd.pierce);
                cmd.Parameters.AddWithValue("critical", pd.critical);
                cmd.Parameters.AddWithValue("guideid", pd.guideid);
                cmd.Parameters.AddWithValue("crystal", pd.crystal);
                cmd.Parameters.AddWithValue("time", pd.time);
                cmd.Parameters.AddWithValue("mission", pd.mission);
                cmd.Parameters.AddWithValue("modle", pd.modle);
                cmd.Parameters.AddWithValue("equipment", pd.equipment);
                int[] _strongArr = pd.strong;
                string strongDBInfo = "";
                for (int i = 0; i < _strongArr.Length; i++)
                {
                    strongDBInfo += _strongArr[i];
                    strongDBInfo += "#";
                }

                cmd.Parameters.AddWithValue("strong", strongDBInfo);

                string[] _taskArr = pd.task;
                string taskDBInfo = "";
                for (int i = 0; i < _taskArr.Length; i++)
                {
                    taskDBInfo += _taskArr[i];
                    taskDBInfo += "#";
                }

                cmd.Parameters.AddWithValue("task", taskDBInfo);

                var bagStr = string.Join("|", pd.bag);
                cmd.Parameters.AddWithValue("bag", bagStr);

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

        public bool UpdatePlayerData(int id, PlayerData pd)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
               "update account set name=@name," +
               "level=@level,exp=@exp,power=@power," +
               "coin=@coin,diamond=@diamond,hp=@hp," +
               "ad=@ad,ap=@ap,addef=@addef,apdef=@apdef," +
               "dodge=@dodge,pierce=@pierce,critical=@critical," +
               "guideid=@guideid,strong=@strong,crystal=@crystal," +
               "time=@time,task=@task,mission=@mission,modle=@modle,bag=@bag,equipment=@equipment" + 
               " where id =@id", conn);

                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("name", pd.name);
                cmd.Parameters.AddWithValue("level", pd.lv);
                cmd.Parameters.AddWithValue("exp", pd.exp);
                cmd.Parameters.AddWithValue("power", pd.power);
                cmd.Parameters.AddWithValue("coin", pd.coin);
                cmd.Parameters.AddWithValue("diamond", pd.diamond);

                cmd.Parameters.AddWithValue("hp", pd.hp);
                cmd.Parameters.AddWithValue("ad", pd.ad);
                cmd.Parameters.AddWithValue("ap", pd.ap);
                cmd.Parameters.AddWithValue("addef", pd.addef);
                cmd.Parameters.AddWithValue("apdef", pd.apdef);
                cmd.Parameters.AddWithValue("dodge", pd.dodge);
                cmd.Parameters.AddWithValue("pierce", pd.pierce);
                cmd.Parameters.AddWithValue("critical", pd.critical);
                cmd.Parameters.AddWithValue("guideid", pd.guideid);
                cmd.Parameters.AddWithValue("crystal", pd.crystal);
                cmd.Parameters.AddWithValue("mission", pd.mission);
                cmd.Parameters.AddWithValue("modle", pd.modle);
                cmd.Parameters.AddWithValue("bag", string.Join("|", pd.bag));
                cmd.Parameters.AddWithValue("equipment", pd.equipment);
                int[] _strongArr = pd.strong;
                string strongDBInfo = "";
                for (int i = 0; i < _strongArr.Length; i++)
                {
                    strongDBInfo += _strongArr[i];
                    strongDBInfo += "#";
                }

                cmd.Parameters.AddWithValue("strong", strongDBInfo);

                cmd.Parameters.AddWithValue("time", pd.time);

                string[] _taskArr = pd.task;
                string taskDBInfo = "";
                
                for (int i = 0; i < _taskArr.Length; i++)
                {
                    taskDBInfo += _taskArr[i];
                    taskDBInfo += "#";
                }


                cmd.Parameters.AddWithValue("task", taskDBInfo);
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

        public bool InsertNewTaskInfo(int playerID, ref NTaskInfo info)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    "Insert into task set playerId=@playerId," +
                    "taskId=@taskId," +
                    "npcId=@npcId," +
                    "prg=@prg," +
                    "status=@status", conn);
                cmd.Parameters.AddWithValue("playerId", playerID);
                cmd.Parameters.AddWithValue("taskId", info.taskID);
                cmd.Parameters.AddWithValue("npcId", info.npcID);
                cmd.Parameters.AddWithValue("prg", info.prg);
                cmd.Parameters.AddWithValue("status", info.taskState);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                PECommon.Log("Insert Player Task Data Error", LogType.Error);
                return false;
            }         
        }

        public bool UpdateTaskInfo(int playerID, NTaskInfo info)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    "Update task set npcId=@npcId," +
                    "prg=@prg," +
                    "status=@status" +
                    " where playerId = @playerId and taskId = @taskId", conn);
                cmd.Parameters.AddWithValue("playerId", playerID);
                cmd.Parameters.AddWithValue("taskId", info.taskID);
                cmd.Parameters.AddWithValue("npcId", info.npcID);
                cmd.Parameters.AddWithValue("prg", info.prg);
                cmd.Parameters.AddWithValue("status", info.taskState);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                PECommon.Log("Update Player Task Info Error", LogType.Error);
                return false;
            }
        }

        public bool DeleteTaskInfo(int playerID, int taskID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    "Delete from task where playerId = @playerId and taskId = @taskId", conn);
                cmd.Parameters.AddWithValue("playerId", playerID);
                cmd.Parameters.AddWithValue("taskId", taskID);                        
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                PECommon.Log("Delete Player Task Info Error", LogType.Error);
                return false;
            }
        }

        public bool FindTaskInfo(int playerID, int taskID)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from task where playerId = @playerId and taskId = @taskId", conn);
                cmd.Parameters.AddWithValue("playerId", playerID);
                cmd.Parameters.AddWithValue("taskId", taskID);
                reader = cmd.ExecuteReader();
                
                if(reader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                
                PECommon.Log("Get Player Task Info Error", LogType.Error);
                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public NTaskInfo GetPlayerTaskInfo(int playerID, int taskID)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from task where playerId = @playerId and taskId = @taskId", conn);
                cmd.Parameters.AddWithValue("playerId", playerID);
                cmd.Parameters.AddWithValue("taskId", taskID);
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new NTaskInfo
                    {
                        taskID = reader.GetInt32("taskId"),
                        npcID = reader.GetInt32("npcId"),
                        prg = reader.GetInt32("prg"),
                        taskState = (PEProtocol.TaskStatus)reader.GetInt32("status"),
                    };
                }
                return null;
            }
            catch
            {
                PECommon.Log("Get Player Task Info Error", LogType.Error);
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public bool UpdatePlayerTaskPrg(int playerID, int taskID, int newPrg)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    "Update task set prg=@prg where playerId = @playerId and taskId = @taskId", conn);
                cmd.Parameters.AddWithValue("playerId", playerID);
                cmd.Parameters.AddWithValue("taskId", taskID);
                cmd.Parameters.AddWithValue("prg", newPrg);   

                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                PECommon.Log("Update Player Task Prg Error", LogType.Error);
                return false;
            }
        }
    }


}
