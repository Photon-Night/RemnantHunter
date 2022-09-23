using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class TimerSvc : Singleton<TimerSvc>
    {
        PETimer pt = null;
        private Queue<TaskPack> tpQue = new Queue<TaskPack>();
        private static readonly string tpQueLock = "tpQueLock";
        public void Init()
        {
            PECommon.Log("TimerService Loading");

            pt = new PETimer(100);

            pt.SetLog((string info) =>
            {
                PECommon.Log(info);
            });

            pt.SetHandle((Action<int> cb, int tid) =>
            {
                if(cb != null)
                {
                    lock(tpQueLock)
                    {
                        tpQue.Enqueue(new TaskPack(tid, cb));
                    }
                }
            });
        }

        public void Update()
        {
            if(tpQue.Count > 0)
            {
                TaskPack tp = null;
                lock(tpQueLock)
                {
                    tp = tpQue.Dequeue();
                }

                if(tp != null)
                {
                    tp.cb(tp.tid); 
                }
            }
        }

        public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1)
        {
            return pt.AddTimeTask(callback, delay, timeUnit, count);
        }
    }

    public class TaskPack
    {
        public int tid;
        public Action<int> cb;
        public TaskPack(int tid, Action<int> cb)
        {
            this.tid = tid;
            this.cb = cb;
        }
    }
}
