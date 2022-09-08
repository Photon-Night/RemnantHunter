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
            PECommon.Log("CfgService Loading");
        }
        private void InitGuideCfg()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(@"D:\UnityProject\Dark God\Dark-God\Client\Dark God\Assets\Resources\ResCfgs\guide.xml");
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

}
