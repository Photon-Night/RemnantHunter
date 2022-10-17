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
        InitMapCfg(PathDefine.MapCfg);
        InitGuideCfg(PathDefine.GuideCfg);
        InitStrongCfg(PathDefine.StrongCfg);
        InitTaskCfg(PathDefine.TaskCfg);
        InitSkillCfg(PathDefine.SkillCfg);
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
                { ID = ID};

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

                guideTaskDic.Add(ID, gc);
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
        doc.Load(xml.text);

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
                ID = id
            };
            foreach (XmlElement e in ele)
            {
                switch(e.Name)
                {
                    case "skillName":
                        skill.skillName = e.InnerText;
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
                }
            }

            skillDic.Add(id, skill);
        }
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
