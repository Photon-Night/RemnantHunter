using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerRoot.Instance.Init();
            while(true)
            {
                ServerRoot.Instance.Update();
                Thread.Sleep(20);
            }
        }
    }
}
