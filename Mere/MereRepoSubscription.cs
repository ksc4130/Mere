using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public class MereRepoSubscription<T>
    {
        public MereRepoSubscription(string id, Action offCallback)
        {
            Id = id;
            Off = offCallback;
        }

        public string Id { get; set; }
        public Action Off { get; private set; } 
        public Action<T> UpdateCallback { get; private set; } 
        public Action<T> CreateCallback { get; private set; } 
        public Action<T> DeleteCallback { get; private set; } 

        public MereRepoSubscription<T> OnUpdate(Action<T> callback)
        {
            UpdateCallback = callback;
            return this;
        }
        public MereRepoSubscription<T> OnCreate(Action<T> callback)
        {
            CreateCallback = callback;
            return this;
        }
        public MereRepoSubscription<T> OnDelete(Action<T> callback)
        {
            DeleteCallback = callback;
            return this;
        }
    }
}
