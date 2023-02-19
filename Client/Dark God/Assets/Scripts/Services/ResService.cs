using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using PEProtocol;

public class ResService : MonoSingleton<ResService>
{
    public void ServiceInit()
    {
        PECommon.Log("ResService Loading");
        
        InitRDNameCfg(PathDefine.RDNameCfg);
        InitMonsterCfg(PathDefine.MonsterCfg);
        InitMapCfg(PathDefine.MapCfg);
        InitGuideCfg(PathDefine.GuideCfg);
        InitStrongCfg(PathDefine.StrongCfg);
        InitTaskCfg(PathDefine.TaskCfg);
        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
        InitSkillActionCfg(PathDefine.SkillActionCfg);
        InitNPCCfg(PathDefine.NPCCfg);
    }

    public void ReSetSkillCfgData()
    {
        skillDic.Clear();
        skillmoveDis.Clear();

        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);

        PECommon.Log("Skill Data Reset");
    }

    #region SceneLoad
    private System.Action PrgCB = null;
    public void LoadSceneAsync(string sceneName, System.Action loaded = null)
    {
        GameRoot.Instance.loadingWin.SetWinState();
        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
        PrgCB = () =>
        {
            float val = sceneAsync.progress;
            GameRoot.Instance.loadingWin.SetProgress(val);
            if (val == 1)
            {
                PrgCB = null;
                sceneAsync = null;
                GameRoot.Instance.loadingWin.SetWinState(false);
                if (loaded != null)
                {
                    loaded();
                }
            }
        };
    }

    private void Update()
    {
        if (PrgCB != null)
            PrgCB();
    }
    #endregion

    #region Audio
    private Dictionary<string, AudioClip> adDic = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache = false)
    {
        AudioClip au = null;
        if (!adDic.TryGetValue(path, out au))
        {
            au = Resources.Load<AudioClip>(path);
            if (cache)
            {
                adDic.Add(path, au);
            }
        }
        return au;
    }
    #endregion

    #region Name
    private List<string> surnameList = new List<string>();
    private List<string> manList = new List<string>();
    private List<string> womanList = new List<string>();
    private void InitRDNameCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml)
        {
            PECommon.Log("xml file: " + path + "is not exist", PEProtocol.LogType.Error);

        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList nodList = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodList.Count; i++)
            {
                XmlElement ele = nodList[i] as XmlElement;
                if(ele.GetAttributeNode("ID") == null)
                {
                    continue; 
                }

                int ID = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                foreach (XmlElement e in nodList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "surname":
                            surnameList.Add(e.InnerText);
                            break;
                        case "man":
                            manList.Add(e.InnerText);
                            break;
                        case "woman":
                            womanList.Add(e.InnerText);
                            break;
                    }
                }

            }
        }
    }

    public string GetRDNameData(bool man = true)
    {
        System.Random rd = new System.Random();
        string rdName = surnameList[PETools.RdInt(0, surnameList.Count - 1, rd)];
        if(man)
        {
            rdName += manList[PETools.RdInt(0, manList.Count - 1, rd)];
        }
        else
        {
            rdName += womanList[PETools.RdInt(0, womanList.Count, rd)];
        }
        PECommon.Log(rdName);
        return rdName;
    }
    #endregion

    #region MapData
    private Dictionary<int, MapCfg> mapCfgDataDic = new Dictionary<int, MapCfg>();
    private void InitMapCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file: " + path + "is not exist", PEProtocol.LogType.Error);

        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList nodList = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodList.Count; i++)
            {
                XmlElement ele = nodList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MapCfg mc = new MapCfg()
                {
                    ID = ID,
                    npcs = new List<int>(),
                    monsterLst = new List<MonsterData>()
                };

                foreach (XmlElement e in nodList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mapName":
                            mc.mapName = e.InnerText;
                            break;
                        case "sceneName":
                            mc.sceneName = e.InnerText;
                            break;
                        case "power":
                            mc.power = int.Parse(e.InnerText);
                            break;
                        case "mainCamPos":
                            {
                                string[] valArr = e.InnerText.Split(',');
                                mc.mainCamPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                                break;
                        case "mainCamRote":
                            {
                                string[] valArr = e.InnerText.Split(',');
                                mc.mainCamRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "playerBornPos":
                            {
                                string[] valArr = e.InnerText.Split(',');
                                mc.playerBornPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "playerBornRote":
                            {
                                string[] valArr = e.InnerText.Split(',');
                                mc.playerBornRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "monsterLst":
                            {
                                string[] valArr = e.InnerText.Split('#');
                                for(int waveIndex = 1; waveIndex < valArr.Length; waveIndex++)
                                {
                                    string[] tempArr = valArr[waveIndex].Split('|');
                                    for(int j = 1; j < tempArr.Length; j++)
                                    {
                                        string[] arr = tempArr[j].Split(',');
                                        MonsterData mData = new MonsterData
                                        {
                                            ID = int.Parse(arr[0]),
                                            mWave = waveIndex,
                                            mIndex = j,
                                            mCfg = GetMonsterCfg(int.Parse(arr[0])),
                                            mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                            mBornRote = new Vector3(0, float.Parse(arr[4]), 0),
                                            lv = int.Parse(arr[5])
                                        };
                                        mc.monsterLst.Add(mData);                       
                                    }
                                }
                            }
                            break;
                        case "npcLst":
                            {
                                string[] npcArr = e.InnerText.Split('#');
                                for(int j = 0; j < npcArr.Length; j++)
                                {
                                    mc.npcs.Add(int.Parse(npcArr[j]));
                                }
                            }
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "crystal":
                            mc.crystal = int.Parse(e.InnerText);
                            break;
                    }
                }
                mapCfgDataDic.Add(ID, mc);
            }
        }
    }
    
    public MapCfg GetMapCfgData(int id)
    {
        MapCfg data = null;
        if(mapCfgDataDic.TryGetValue(id, out data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Monster
    private Dictionary<int, MonsterCfg> monsterDic = new Dictionary<int, MonsterCfg>();
    private void InitMonsterCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(xml == null)
        {
            PECommon.Log("xml file: " + path + "is not exist", PEProtocol.LogType.Error);
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml.text);

        XmlNodeList nodeLst = doc.SelectSingleNode("root").ChildNodes;
        for(int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if(ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int ID = int.Parse(ele.GetAttributeNode("ID").InnerText);

            MonsterCfg data = new MonsterCfg
            {
                ID = ID,
                bps = new BattleProps()
            };

            foreach (XmlElement e in ele)
            {
                switch (e.Name)
                {
                    case "mName":
                        data.mName = e.InnerText;
                        break;

                    case "mType":
                        if (e.InnerText == "1")
                            data.mType = Message.MonsterType.Normal;
                        else if (e.InnerText == "2")
                            data.mType = Message.MonsterType.Boss;
                        break;

                    case "isStop":
                        if (e.InnerText == "0")
                            data.isStop = false;
                        else if (e.InnerText == "1")
                            data.isStop = true;
                        break;

                    case "resPath":
                        data.resPath = e.InnerText;
                        break;

                    case "skillID":
                        data.skillID = int.Parse(e.InnerText);
                        break;

                    case "atkDis":
                        data.bps.atkDis = float.Parse(e.InnerText);
                        Debug.Log(data.bps.atkDis);
                        break;

                    case "hp":
                        data.bps.hp = int.Parse(e.InnerText);
                        break;

                    case "ad":
                        data.bps.ad = int.Parse(e.InnerText);
                        break;

                    case "ap":
                        data.bps.ap = int.Parse(e.InnerText);
                        break;

                    case "addef":
                        data.bps.addef = int.Parse(e.InnerText);
                        break;

                    case "apdef":
                        data.bps.apdef = int.Parse(e.InnerText);
                        break;

                    case "dodge":
                        data.bps.dodge = int.Parse(e.InnerText);
                        break;

                    case "pierce":
                        data.bps.pierce = int.Parse(e.InnerText);
                        break;

                    case "critical":
                        data.bps.critical = int.Parse(e.InnerText);
                        break;
                }
            }

            monsterDic.Add(ID, data);
        }
    }

    public MonsterCfg GetMonsterCfg(int id)
    {
        MonsterCfg data = null;
        if(monsterDic.TryGetValue(id, out data))
        {
            return data;
        }
        return null;
    }
    #endregion

    #region GuideData
    private Dictionary<int, GuideCfg> guideTaskDic = new Dictionary<int, GuideCfg>();
    private void InitGuideCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == null)
        {
            PECommon.Log("xml file:" + path + "is not existed", PEProtocol.LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;
            for(int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;
                if(ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                GuideCfg gc = new GuideCfg()
                { ID = ID };
                foreach (XmlElement e  in ele)
                {
                    switch (e.Name)
                    {
                        case "npcID":
                            gc.npcID = int.Parse(e.InnerText);
                            break;
                        case "dilogArr":
                            gc.dilogArr = e.InnerText;
                            break;
                        case "actID":
                            gc.actID = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            gc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            gc.exp = int.Parse(e.InnerText);
                            break;
                    }
                }
                gc.ID = gc.npcID;
                guideTaskDic.Add(gc.ID, gc);
            }
        }
    }

    public GuideCfg GetGuideCfgData(int id)
    {

        GuideCfg data = null;
        if(guideTaskDic.TryGetValue(id, out data))
        {
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region TaskData
    private Dictionary<int, TaskCfg> taskDic = new Dictionary<int, TaskCfg>();
    private void InitTaskCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(xml == null)
        {
            PECommon.Log("xml file:" + path + "is not existed", PEProtocol.LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList nodeLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement ele = nodeLst[i] as XmlElement;
                if(ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int id = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TaskCfg task = new TaskCfg
                { ID = id};

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

                taskDic.Add(task.ID, task);
            }
        }
    }

    public TaskCfg GetTaskCfgData(int id)
    {
        TaskCfg data = null;
        if(taskDic.TryGetValue(id, out data))
        {
            return data;
        }
        else
        return null;
    }
    #endregion

    #region StrongData
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(xml == null)
        {
            PECommon.Log("xml file:" + path + "is not existed", PEProtocol.LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                StrongCfg sc = new StrongCfg{ ID = ID };
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

                Dictionary<int, StrongCfg> dic = null;
                if(strongDic.TryGetValue(sc.pos, out dic))
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
        }
    }
    public StrongCfg GetStrongCfgData(int pos, int starLv)
    {
        Dictionary<int, StrongCfg> dic = null;
        if(strongDic.TryGetValue(pos, out dic))
        {
            StrongCfg sc = null;
            if(dic.TryGetValue(starLv, out sc))
            {
                return sc;
            }
        }
        return null;
    }
    public int GetPropAddValPreLv(int pos, int starLv, int type)
    {
        int val = 0;
        Dictionary<int, StrongCfg> posDic = null;
        if(strongDic.TryGetValue(pos, out posDic))
        {
            for (int i = 1; i < starLv + 1; i ++)
            {
                StrongCfg sd;
                if(posDic.TryGetValue(i, out sd))
                {
                    switch (type)
                    {
                        case 1:
                            val += sd.addHp;
                            break;
                        case 2:
                            val += sd.addHurt;
                            break;
                        case 3:
                            val += sd.addDef;
                            break;

                    }
                }
            }
        }
        return val;
    }
    #endregion

    #region Skill
    private Dictionary<int, SkillCfg> skillDic = new Dictionary<int, SkillCfg>();
    private void InitSkillCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(xml == null)
        {
            PECommon.Log("xml file:" + path + "is not existed", PEProtocol.LogType.Error);
            return;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml.text);

        XmlNodeList nodeLst = doc.SelectSingleNode("root").ChildNodes;

        for(int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if(ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            SkillCfg skill = new SkillCfg
            {
                ID = id,
                skillMoveLst = new List<int>(),
                skillActionLst = new List<int>(),
                skillDamageLst = new List<int>(),
            };
            foreach (XmlElement e in ele)
            {
                switch(e.Name)
                {
                    case "skillName":
                        skill.skillName = e.InnerText;
                        break;

                    case "cdTime":
                        skill.cdTime = int.Parse(e.InnerText);
                        break;

                    case "skillTime":
                        skill.skillTime = int.Parse(e.InnerText);
                        break;

                    case "aniAction":
                        skill.aniAction = int.Parse(e.InnerText);
                        break;

                    case "fx":
                        skill.fx = e.InnerText;
                        break;

                    case "isCombo":
                        if (e.InnerText == "0")
                            skill.isCombo = false;
                        else if (e.InnerText == "1")
                            skill.isCombo = true;
                        break;

                    case "isCollide":
                        if (e.InnerText == "0")
                            skill.isCollide = true;
                        else if (e.InnerText == "1")
                            skill.isCollide = false;
                        break;

                    case "isBreak":
                        if (e.InnerText == "0")
                            skill.isBreak = true;
                        else if (e.InnerText == "1")
                            skill.isBreak = false;
                        break;

                    case "dmgType":
                        if (e.InnerText.Equals("1"))
                        {
                            skill.dmgType = Message.DmgType.AD;
                        }
                        else if(e.InnerText.Equals("2"))
                        {
                            skill.dmgType = Message.DmgType.AP;
                        }
                        break;

                    case "skillMoveLst":
                        string[] _skillMoveStr = e.InnerText.Split('|');
                        for(int j = 0; j < _skillMoveStr.Length; j++)
                        {
                            if(_skillMoveStr[j] != "")
                            {
                                skill.skillMoveLst.Add(int.Parse(_skillMoveStr[j]));
                            }
                        }
                        break;

                    case "skillActionLst":
                        string[] _skillActionStr = e.InnerText.Split('|');
                        for(int j = 0; j < _skillActionStr.Length; j++)
                        {
                            if(_skillActionStr[j] != "")
                            {
                                skill.skillActionLst.Add(int.Parse(_skillActionStr[j]));
                            }
                        }
                        break;

                    case "skillDamageLst":
                        string[] _skillDamageStr = e.InnerText.Split('|');
                        for(int j = 0; j < _skillDamageStr.Length; j++)
                        {
                            if(_skillDamageStr[j] != "")
                            skill.skillDamageLst.Add(int.Parse(_skillDamageStr[j]));
                        }
                        break;
                }
            }

            skillDic.Add(id, skill);
        }
    }

    public SkillCfg GetSkillData(int id)
    {
        SkillCfg data;
        if(skillDic.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }
    #endregion

    #region NPC
    private Dictionary<int, NPCCfg> npcDic = new Dictionary<int, NPCCfg>();

    private void InitNPCCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == null)
        {
            PECommon.Log("xml file:" + path + "is not existed", PEProtocol.LogType.Error);
            return;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml.text);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;


        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int ID = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            NPCCfg nc = new NPCCfg()
            {
                ID = ID,
            };

            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        nc.name = e.InnerText;
                        break;
                    case "resPath":
                        nc.resPath = e.InnerText;
                        break;
                    case "pos":
                        string[] posArr = e.InnerText.Split('|');
                        nc.pos = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
                        break;
                    case "type":
                        nc.type = (Message.NPCType)int.Parse(e.InnerText);
                        break;
                    case "function":
                        nc.func = (Message.NPCFunction)int.Parse(e.InnerText);
                        break;


                }
            }
            npcDic.Add(ID, nc);
        }

    }

    public NPCCfg GetNPCData(int id)
    {
        NPCCfg data = null;
        if(npcDic.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }
    #endregion

    #region Task
    private Dictionary<int, TaskDefine> taskcfgDic = new Dictionary<int, TaskDefine>();
    private void InitTaskData(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PECommon.Log("xml file: " + path + "is not exist", PEProtocol.LogType.Error);

        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList nodList = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodList.Count; i++)
            {
                XmlElement ele = nodList[i] as XmlElement;
                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TaskDefine tf = new TaskDefine
                {
                    ID = ID,
                };

                foreach (XmlElement e in nodList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "taskName":
                            tf.taskName = e.InnerText;
                            break;
                        case "taskType":
                            tf.taskType = (TaskType)int.Parse(e.InnerText);
                            break;
                        case "preTaskID":
                            tf.preTaskID = int.Parse(e.InnerText);
                            break;
                        case "isAutoGetNextTask":
                            tf.isAutoGetNextTask = int.Parse(e.InnerText) == 0 ? false : true;
                            break;
                        case "limitLevel":
                            tf.limitLevel = int.Parse(e.InnerText);
                            break;
                        case "targetID":
                            tf.targetID = int.Parse(e.InnerText);
                            break;
                        case "targetCount":
                            tf.targetCount = int.Parse(e.InnerText);
                            break;
                        case "targetPos":
                            string[] posArr = e.InnerText.Split(',');
                            if(posArr.Length > 0)
                            {
                                tf.targetPos = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
                            }
                            break;
                        case "exp":
                            tf.exp = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            tf.coin = int.Parse(e.InnerText);
                            break;
                        case "diomand":
                            tf.diomand = int.Parse(e.InnerText);
                            break;
                    }
                }
            }
        }
    }

    public TaskDefine GetTaskData(int id)
    {
        TaskDefine data = null;
        if(taskcfgDic.TryGetValue(id, out data))
        {
            return data;
        }
        return null;
    }

    public Dictionary<int, TaskDefine> GetTaskDic()
    {
        return taskcfgDic;
    }
    #endregion

    #region SkillMove

    private Dictionary<int, SkillMoveCfg> skillmoveDis = new Dictionary<int, SkillMoveCfg>();

    private void InitSkillMoveCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(xml == null)
        {
            PECommon.Log("xml file:" + path + "is not existed", PEProtocol.LogType.Error);
            return;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml.text);

        XmlNodeList nodeLst = doc.SelectSingleNode("root").ChildNodes;

        for(int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            SkillMoveCfg skillMove = new SkillMoveCfg
            {
                ID = id
            };

            foreach (XmlElement e in ele)
            {
                switch (e.Name)
                {
                    case "delayTime":
                        skillMove.delayTime = float.Parse(e.InnerText);
                        break;
                    case "moveTime":
                        skillMove.moveTime = int.Parse(e.InnerText);
                        break;
                    case "moveDis":
                        skillMove.moveDis = float.Parse(e.InnerText);
                        break;
                }

            }

            skillmoveDis.Add(id, skillMove);
        }

    }

    public SkillMoveCfg GetSkillMoveCfg(int id)
    {
        SkillMoveCfg data = null;
        if(skillmoveDis.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }

    #endregion

    #region SKillAction
    private Dictionary<int, SkillActionCfg> skillActionDic = new Dictionary<int, SkillActionCfg>();
    private void InitSkillActionCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml)
        {
            PECommon.Log("xml file:" + path + "is not existed", PEProtocol.LogType.Error);
            return;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml.text);

        XmlNodeList nodeLst = doc.SelectSingleNode("root").ChildNodes;
        for(int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if(ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            SkillActionCfg data = new SkillActionCfg
            {
                ID = id
            };

            foreach (XmlElement e in ele)
            {
                switch (e.Name)
                {
                    case "delayTime":
                        data.delayTime = float.Parse(e.InnerText);
                        break;
                    case "radius":
                        data.radius = float.Parse(e.InnerText);
                        break;
                    case "angle":
                        data.angle = float.Parse(e.InnerText);
                        break;
                }
            }

            skillActionDic.Add(id, data);
        }
    }


    public SkillActionCfg GetSkillActionCfg(int id)
    {
        SkillActionCfg data = null;
        if(skillActionDic.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }
    #endregion

    #region Prefab
    private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    public GameObject LoadPrefab(string path, bool isCache = false)
    {
        GameObject prefab = null;
        if(!goDic.TryGetValue(path, out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if(isCache)
            {
                goDic.Add(path, prefab);
            }
        }
        GameObject go = null;
        if(prefab != null)
        {
            go = Instantiate(prefab);
        }
        return go;
    }
    #endregion 

    #region Sprite
    private Dictionary<string, Sprite> sprDic = new Dictionary<string, Sprite>(); 
    public Sprite LoadSprite(string path, bool isCache = false)
    {
        Sprite sp = null;
        if(!sprDic.TryGetValue(path, out sp))
        {
            sp = Resources.Load<Sprite>(path);
            if(sp == null)
            {
                PECommon.Log("Sprite:" + path + "is not exited", PEProtocol.LogType.Error);
                return null;
            }
            if(isCache)
            {
                sprDic.Add(path, sp);
            }
        }
        return sp;

        
    }
    #endregion

}
