using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using PEProtocol;
using Game.Common;
using UnityEngine.Timeline;

public class ResService : MonoSingleton<ResService>, IService
{
    public void ServiceInit()
    {
        PECommon.Log("ResService Loading");
        
        InitRDNameCfg(PathDefine.RDNameCfg);
        InitMonsterCfg(PathDefine.MonsterCfg);
        InitGroupCfg(PathDefine.GroupCfg);
        InitMapCfg(PathDefine.MapCfg);
        InitStrongCfg(PathDefine.StrongCfg);
        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
        InitSkillActionData(PathDefine.SkillActionData);
        InitNPCCfg(PathDefine.NPCCfg);
        InitTaskCfg(PathDefine.TaskCfg);
        InitTalkCfg(PathDefine.TalkCfg);
        InitItemCfg(PathDefine.ItemCfg);
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
                if (loaded != null)
                {
                    loaded();
                }

                GameRoot.Instance.loadingWin.SetWinState(false);
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

    #region Timeline
    Dictionary<string, TimelineAsset> timelineDic = new Dictionary<string, TimelineAsset>();
    public TimelineAsset GetTimelineAsset(string path, bool cache = true)
    {
        TimelineAsset tla = null;
        if(!timelineDic.TryGetValue(name, out tla))
        {
            tla = Resources.Load<TimelineAsset>("ResTimeline/" + path);
            if (tla != null && cache)
            {
                timelineDic.Add(path, tla);
            }
        }
        return tla;
        
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
                    monsterLst = new List<MonsterData>(),
                    monsterGroups = new List<GroupData>(),
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
                                string[] groupArr = e.InnerText.Split('|');
                                for (int j = 0; j < groupArr.Length; j++)
                                {
                                    GroupData gd = GetGroupData(int.Parse(groupArr[j]));
                                    mc.monsterGroups.Add(gd);
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
                            data.mType = MonsterType.Normal;
                        else if (e.InnerText == "2")
                            data.mType = MonsterType.Boss;
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

    #region Group
    private Dictionary<int, GroupData> groupDic = new Dictionary<int, GroupData>();
    private void InitGroupCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == null)
        {
            PECommon.Log($"xml file {path} is not existed", PEProtocol.LogType.Error);
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
                GroupData gd = new GroupData()
                {
                    ID = ID,
                    monsters = new List<MonsterData>(),
                };
                foreach (XmlElement e in ele)
                {
                    switch (e.Name)
                    {
                        case "pos":
                            string[] posArr = e.InnerText.Split(',');
                            gd.pos = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
                            break;
                        case "lv":
                            gd.lv = int.Parse(e.InnerText);
                            break;
                        case "normalRange":
                            gd.normalRange = float.Parse(e.InnerText);
                            break;
                        case "battleRange":
                            gd.battleRange = float.Parse(e.InnerText);
                            break;
                        case "monsters":
                            {
                                string[] monstersDataArr = e.InnerText.Split('|');

                                for(int j = 0; j < monstersDataArr.Length; j++)
                                {
                                    string[] monsterArr = monstersDataArr[j].Split(',');
                                    MonsterCfg mc = GetMonsterCfg(int.Parse(monsterArr[0]));
                                    Vector3 pos = new Vector3(float.Parse(monsterArr[1]),
                                                              float.Parse(monsterArr[2]),
                                                              float.Parse(monsterArr[3]));
                                    float rotate = float.Parse(monsterArr[4]);
                                    MonsterData md = new MonsterData()
                                    {
                                        mCfg = mc,
                                        mBornPos = pos,
                                        mBornRote = new Vector3(0, rotate, 0),
                                    };
                                    gd.monsters.Add(md);
                                }
                                
                                break;
                            }
                        
                            
                    }
                }

                groupDic.Add(ID, gd);
            }
        }
    }
    public GroupData GetGroupData(int id)
    {
        GroupData data;
        if(groupDic.TryGetValue(id, out data))
        {
            return data;
        }

        return null;

    }
    #endregion

    #region Talk

    private Dictionary<int, List<TalkCfg>> talkDic = new Dictionary<int, List<TalkCfg>>();
    private Dictionary<int, TalkCfg> npcTalkRootDic = new Dictionary<int, TalkCfg>();
    private void InitTalkCfg(string path)
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

            for(int i = 0; i < nodeLst.Count; i++)
            {
                XmlElement ele = nodeLst[i] as XmlElement;
                if(ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int id = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TalkCfg tc = new TalkCfg
                {
                    ID = id,
                };

                foreach (XmlElement e in ele)
                {
                    switch (e.Name)
                    {
                        case "index":
                            tc.index = int.Parse(e.InnerText);
                            break;

                        case "entityID":
                            tc.entityID = int.Parse(e.InnerText);
                            break;

                        case "dialogArr":
                            tc.dialogArr = e.InnerText.Split('#');
                            break;

                        case "type":
                            tc.type = (TalkType)int.Parse(e.InnerText);
                            break;

                        case "selectLst":
                            string[] selectArr = e.InnerText.Split('|');

                            if (selectArr.Length != 0)
                                tc.selectLst = new int[selectArr.Length];

                            for (int j = 0; j < selectArr.Length; j++)
                            {
                                tc.selectLst[j] = int.Parse(selectArr[j]);
                            }
                            break;

                        case "nextTalkID":
                            tc.nextTalkID = int.Parse(e.InnerText);
                            break;

                        case "actID":
                            tc.actID = (NPCFunction)int.Parse(e.InnerText);
                            break;

                        case "nextIndex":
                            tc.nextIndex = int.Parse(e.InnerText);
                            break;

                        case "isRoot":
                            int res = int.Parse(e.InnerText);
                            tc.isRoot = res == 1 ? true : false;
                            break;

                    }

                }

                if(!talkDic.ContainsKey(tc.ID))
                {
                    talkDic[tc.ID] = new List<TalkCfg>();
                }

                talkDic[tc.ID].Add(tc);

                if(tc.isRoot)
                {
                    npcTalkRootDic[tc.entityID] = tc;
                }
            }
        }
    }

    public TalkCfg GetNpcTalkRootData(int npcID)
    {
        TalkCfg data = null;
        if(npcTalkRootDic.TryGetValue(npcID, out data))
        {
            return data;
        }

        return null;
    }

    public TalkCfg GetTalkData(int talkID, int index)
    {
        if(talkDic.ContainsKey(talkID))
        {
            if(talkDic[talkID].Count > index)
            {
                return talkDic[talkID][index];
            }
        }

        return null;
    }



    #endregion

    #region Strong
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
    private Dictionary<int, SkillData> skillDic = new Dictionary<int, SkillData>();
    private Dictionary<int, Dictionary<SkillType, List<SkillData>>> entitySkillDic = new Dictionary<int, Dictionary<SkillType, List<SkillData>>>();
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

        for (int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            SkillData skill = new SkillData
            {
                ID = id,
                skillActionLst = new List<int>(),
                skillDamageLst = new List<int>(),
            };
            foreach (XmlElement e in ele)
            {
                switch (e.Name)
                {
                    case "entityID":
                        skill.entityID = int.Parse(e.InnerText);
                        break;

                    case "skillName":
                        skill.skillName = e.InnerText;
                        break;

                    case "animName":
                        skill.animName = e.InnerText;
                        break;

                    case "fxName":
                        skill.animName = e.InnerText;
                        break;

                    case "skillType":
                        skill.skillType = (SkillType)int.Parse(e.InnerText);
                        break;

                    case "cost":
                        skill.powerCost = float.Parse(e.InnerText);
                        break;

                    case "nextComboID":
                        skill.nextComboID = int.Parse(e.InnerText);
                        break;

                    case "comboCheckTime":
                        skill.comboCheckTime = int.Parse(e.InnerText);
                        break;

                    case "isCollider":
                        skill.isCollide = int.Parse(e.InnerText) == 0 ? false : true;
                        break;

                    case "isBreak":
                        skill.isBreak = int.Parse(e.InnerText) == 0 ? false : true;
                        break;

                    case "dmgType":
                        skill.dmgType = (DmgType)int.Parse(e.InnerText);
                        break;

                    case "skillActionLst":
                        string[] actionArr = e.InnerText.Split('|');
                        for (int j = 0; j < actionArr.Length; j++)
                        {
                            skill.skillActionLst.Add(int.Parse(actionArr[j]));
                        }
                        break;

                    case "skillDamageLst":
                        string[] dmgArr = e.InnerText.Split('|');
                        for (int j = 0; j < dmgArr.Length; j++)
                        {
                            skill.skillDamageLst.Add(int.Parse(dmgArr[j]));
                        }
                        break;
                }
            }

            skillDic.Add(id, skill);

            if (!entitySkillDic.ContainsKey(skill.entityID))
            {
                entitySkillDic[skill.entityID] = new Dictionary<SkillType, List<SkillData>>();
            }
            if (!entitySkillDic[skill.entityID].ContainsKey(skill.skillType))
            {
                entitySkillDic[skill.entityID][skill.skillType] = new List<SkillData>();
            }

            entitySkillDic[skill.entityID][skill.skillType].Add(skill);
        }
    }

    public SkillData GetSkillData(int id)
    {
        SkillData data;
        if(skillDic.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }

    public List<SkillData> GetEntitySkillLst(int entityID, SkillType type)
    {
        if(entitySkillDic.ContainsKey(entityID))
        {
            if (entitySkillDic[entityID].ContainsKey(type))
            {
                return entitySkillDic[entityID][type];
            }
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
                        nc.type = (NPCType)int.Parse(e.InnerText);
                        break;
                    case "function":
                        nc.func = (NPCFunction)int.Parse(e.InnerText);
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
    private void InitTaskCfg(string path)
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
                        case "acceptNpcID":
                            tf.acceptNpcID = int.Parse(e.InnerText);
                            break;
                        case "submitNpcID":
                            tf.submitNpcID = int.Parse(e.InnerText);
                            break;
                        case "accTalkID":
                            tf.accTalkID = int.Parse(e.InnerText);
                            break;
                        case "subTalkID":
                            tf.subTalkID = int.Parse(e.InnerText);
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
                            if(posArr.Length > 2)
                            {
                                tf.targetPos = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
                            }
                            break;
                        case "description":
                            tf.description = e.InnerText;
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
                        case "item":
                            tf.item = e.InnerText.Split('|');
                            break;
                    }
                }

                taskcfgDic.Add(tf.ID, tf);
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
    private Dictionary<int, SkillActionData> skillActionDic = new Dictionary<int, SkillActionData>();
    private void InitSkillActionData(string path)
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
            SkillActionData data = new SkillActionData
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


    public SkillActionData GetSkillActionData(int id)
    {
        SkillActionData data = null;
        if(skillActionDic.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }
    #endregion

    #region Item
    private Dictionary<int, GameItemCfg> itemDic = new Dictionary<int, GameItemCfg>();
    private void InitItemCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == null)
        {
            PECommon.Log("xml file:" + path + "is not existed", PEProtocol.LogType.Error);
            return;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml.text);

        XmlNodeList nodeLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodeLst.Count; i++)
        {
            XmlElement ele = nodeLst[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int id = System.Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            GameItemCfg item = new GameItemCfg
            {
                ID = id            
            };
            foreach (XmlElement e in ele)
            {
                switch (e.Name)
                {
                    case "name":
                        item.name = e.InnerText;
                        break;    
                    case "ItemType":
                        item.ItemType = (BagItemType)int.Parse(e.InnerText);
                        break;
                    case "equipmentType":
                        item.equipmentType = (EquipmentType)int.Parse(e.InnerText);
                        break;
                    case "funcType":
                        item.funcType = (ItemFunction)int.Parse(e.InnerText);
                        break;
                    case "funcNum":
                        item.funcNum = float.Parse(e.InnerText);
                        break;
                    case "duration":
                        item.duration = float.Parse(e.InnerText);
                        break;
                    case "objPath":
                        item.objPath = e.InnerText;
                        break;
                    case "iconPath":
                        item.iconPath = e.InnerText;
                        break;
                    case "useWithoutBattle":
                        item.useWithoutBattle = int.Parse(e.InnerText) == 1 ? true : false;
                        break;
                    case "des":
                        item.des = e.InnerText;
                        break;
                }
            }

            itemDic.Add(id, item);

            
        }
    }

    public GameItemCfg GetGameItemCfg(int ID)
    {
        if(itemDic.TryGetValue(ID, out GameItemCfg data))
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
