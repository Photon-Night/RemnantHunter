using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GameServer
{
    class CfgSvc : Singleton<CfgSvc>
    {
        private Dictionary<int, GuideCfg> guideTaskDic = new Dictionary<int, GuideCfg>();
        private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
        private Dictionary<int, TaskCfg> taskDic = new Dictionary<int, TaskCfg>();
        public void Init()
        {
            PECommon.Log("CfgService Loading");
            PECommon.Log("//--------------------Cfg Loading--------------------//");
            InitGuideCfg();
            InitStrongCfg();
            InitTaskCfg();
            PECommon.Log("//--------------------Load Complete--------------------//");
        }
        private void InitGuideCfg()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(@"D:\UnityProject\Dark God\Dark-God\Client\Dark God\Assets\Resources\ResCfgs\guide.xml");
            if (doc == null)
            {
                PECommon.Log("Guide cfg is not exist");
                return;
            }
            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                GuideCfg gc = new GuideCfg()
                { ID = ID };
                foreach (XmlElement e in ele)
                {
                    switch (e.Name)
                    {
                        case "coin":
                            gc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            gc.exp = int.Parse(e.InnerText);
                            break;
                    }
                }
                guideTaskDic.Add(ID, gc);
            }

            PECommon.Log("Load Guide Data " + guideTaskDic.Count);
        }
        public GuideCfg GetGuideCfgData(int id)
        {
            GuideCfg data = null;
            if (guideTaskDic.TryGetValue(id, out data))
            {
                return data;
            }

            return null;

        }


        private void InitStrongCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"D:\UnityProject\Dark God\Dark-God\Client\Dark God\Assets\Resources\ResCfgs\strong.xml");
            if (doc == null)
            {
                PECommon.Log("strong cfg is not exist");
                return;
            }
            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                StrongCfg sc = new StrongCfg { ID = ID };
                foreach (XmlElement e in ele)
                {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name)
                    {
                        case "pos":
                            sc.pos = val;
                            break;
                        case "starlv":
                            sc.starLv = val;
                            break;
                        case "addhp":
                            sc.addHp = val;
                            break;
                        case "addhurt":
                            sc.addHurt = val;
                            break;
                        case "adddef":
                            sc.addDef = val;
                            break;
                        case "minlv":
                            sc.minLv = val;
                            break;
                        case "coin":
                            sc.coin = val;
                            break;
                        case "crystal":
                            sc.crystal = val;
                            break;
                    }
                }

                Dictionary<int, StrongCfg> dic;
                if (strongDic.TryGetValue(sc.pos, out dic))
                {
                    dic.Add(sc.starLv, sc);
                }
                else
                {
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(sc.starLv, sc);
                    strongDic.Add(sc.pos, dic);
                }

            }

            PECommon.Log("Load Strong Data " + strongDic.Count);
        }
        public StrongCfg GetStrongCfgData(int pos, int starLv)
        {
            Dictionary<int, StrongCfg> dic = null;
            if (strongDic.TryGetValue(pos, out dic))
            {
                StrongCfg sc = null;
                if (dic.TryGetValue(starLv, out sc))
                {
                    return sc;
                }
            }
            return null;
        }

        private void InitTaskCfg()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(@"D:\UnityProject\Dark God\Dark-God\Client\Dark God\Assets\Resources\ResCfgs\task.xml");
            if (doc == null)
            {
                PECommon.Log("Task cfg is not exist");
                return;
            }
            XmlNodeList nodeLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement ele = nodeLst[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int id = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TaskCfg task = new TaskCfg
                { ID = id };

                foreach (XmlElement e in ele)
                {
                    switch (e.Name)
                    {
                        case "taskName":
                            task.taskName = e.InnerText;
                            break;
                        case "count":
                            task.count = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            task.exp = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            task.coin = int.Parse(e.InnerText);
                            break;
                    }
                }

                taskDic.Add(id, task);
            }

            PECommon.Log("Load Task Data " + taskDic.Count);
        }
        public TaskCfg GetTaskCfgData(int id)
        {
            TaskCfg data = null;
            if (taskDic.TryGetValue(id, out data))
            {
                return data;
            }

            return null;
        }

        public int GetTaskConut()
        {
            return taskDic.Count;
        }
    }
}

    public class BaseData<T>
    {
        public int ID;
    }
    public class GuideCfg : BaseData<GuideCfg>
    {
        public int coin;
        public int exp;
    }
    public class StrongCfg : BaseData<StrongCfg>
    {
        public int pos;
        public int starLv;
        public int addHp;
        public int addHurt;
        public int addDef;
        public int minLv;
        public int coin;
        public int crystal;
    }
    public class TaskRewardData : BaseData<TaskRewardData>
    {
        public int prgs;
        public bool taked;
    }
    public class TaskCfg : BaseData<TaskCfg>
    {
        public string taskName;
        public int count;
        public int coin;
        public int exp;
    }





