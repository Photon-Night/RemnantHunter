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
        public void Init()
        {
            InitGuideCfg();
            InitStrongCfg();
            PECommon.Log("CfgService Loading");
        }
        private void InitGuideCfg()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(@"D:\UnityProject\Dark God\Dark-God\Client\Dark God\Assets\Resources\ResCfgs\guide.xml");
            if (doc == null)
            {
                PECommon.Log("Guide cfg is not exist");
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
        }
        public GuideCfg GetGuideCfgData(int id)
        {
            GuideCfg data = null;
            if (guideTaskDic.TryGetValue(id, out data))
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
        private void InitStrongCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"D:\UnityProject\Dark God\Dark-God\Client\Dark God\Assets\Resources\ResCfgs\strong.xml");
            if(doc == null)
            {
                PECommon.Log("strong cfg is not exist");
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





}
