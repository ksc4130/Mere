using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public class MereRepo<T> where T : new()
    {
        private MereTableMin<T> _mereTable;
        private MereColumn _keyColumn;
        private bool _updateWriting;
        private List<Task> _taskQue = new List<Task>();
        private Dictionary<string, MereRepoSubscription<T>> _subscriptions = new Dictionary<string, MereRepoSubscription<T>>();

        public MereRepo()
        {
            _mereTable = MereUtils.CacheCheck<T>();
            if (string.IsNullOrEmpty(_mereTable.KeyColumnName))
            {
                throw new Exception("Must have a key column to use mere repo for right now");
            }
            _keyColumn = _mereTable.GetMereColumn(_mereTable.KeyColumnName);
        }

        private void Run(Action cb)
        {
            var toRun = new Task(
                () =>
                    {
                        cb();
                        var toRun2 = _taskQue.LastOrDefault();
                        if (toRun2 != null)
                        {
                            toRun2.Start();
                            _taskQue.Remove(toRun2);
                        }
                    });
            _taskQue.Add(toRun);
            if (_taskQue.Count <= 1)
            {
                var r = _taskQue.LastOrDefault();
                if (r != null)
                {
                    r.Start();
                    _taskQue.Remove(r);
                }
            }
        }

        public MereRepoSubscription<T> Subscribe()
        {
            var id = Guid.NewGuid().ToString();
            var sub = new MereRepoSubscription<T>(id,
                                                  () => Run(
                                                      () => _subscriptions.Remove(id)));
            Run(() => _subscriptions.Add(id, sub));
            return sub;
        }

//        /// <summary>
//        /// subscribe to updates 
//        /// </summary>
//        /// <param name="cb">callback for update</param>
//        /// <returns>id for subscription</returns>
//        public MereRepoSubscription<T> OnUpdate(Action<T> cb)
//        {
//            var id = Guid.NewGuid().ToString();
//            var sub = new MereRepoSubscription<T>(id,
//                                                  () => Run(
//                                                      () => _subscriptions.Remove(id)));
//            sub.OnUpdate(cb);
//            Run(() => _subscriptions.Add(id, sub));
//            return sub;
//        }

        public void Off(string subscriptionId)
        {
            while (_updateWriting)
            {
                Debug.WriteLine("off waiting on _writing in data repo for " + _mereTable.TableName);
            }
            _updateWriting = true;

        }

        public void PubUpdate(T updateObj)
        {
            foreach (var sub in _subscriptions.Values.Where(x => x.UpdateCallback != null))
            {
                sub.UpdateCallback(updateObj);
            }
        }

        public void PubCreate(T updateObj)
        {
            foreach (var sub in _subscriptions.Values.Where(x => x.CreateCallback != null))
            {
                sub.CreateCallback(updateObj);
            }
        }

        public void PubDelete(T updateObj)
        {
            foreach (var sub in _subscriptions.Values.Where(x => x.DeleteCallback != null))
            {
                sub.UpdateCallback(updateObj);
            }
        }

        //TODO: enable partial update call backs with expando objects maybe in MereRepo instead of MereRepo<T>
//        public void PubUpdate(ExpandoObject updateObj)
//        {
//            foreach (var sub in _subscriptions.Values)
//            {
//                if (sub.UpdateCallback != null)
//                    sub.UpdateCallback(updateObj);
//            }
//        }

//        public void Update(ExpandoObject updateObj)
//        {
//            var d = (IDictionary<string, object>)updateObj;
//            if (d.ContainsKey(_keyColumn.ColumnName))
//            {
//                var val = d[_keyColumn.ColumnName];
//                var str = val != null ? val.ToString() : "0";
//                if (_keyColumn.IsIdentity)
//                {
//                    var key = int.Parse(str);
//                    if (key > 0)
//                    {
//                        updateObj.MereUpdateSync<T>();
//                    }
//                }
//            }
//        }

        public void Delete(T toDelete)
        {
            var deleted = MereDelete.Create<T>()
                               .AddFilter(_keyColumn.ColumnName, SqlOperator.EqualTo, _keyColumn.GetBase(toDelete))
                               .Execute();
            
            if(deleted > 0)
                PubDelete(toDelete);
        }

        public void Save(ExpandoObject updateObj)
        {
            T found;
            var isCreate = false;
            var d = (IDictionary<string, object>)updateObj;
            if (d.ContainsKey(_keyColumn.ColumnName))
            {
                found = MereQuery.Create<T>()
                                   .AddFilter(_keyColumn.ColumnName, SqlOperator.EqualTo, d[_keyColumn.ColumnName])
                                   .ExecuteFirstOrDefault();
            }
            else
            {
                //create 
                isCreate = true;
                found = new T();
            }

            if (found != null)
            {
                foreach (var k in d.Keys)
                {
                    var col = _mereTable.GetMereColumn(k);
                    if (col != null)
                    {
                        col.SetBase(found, d[k]);
                    }
                }
                found.MereSave();
                if(isCreate)
                    PubCreate(found);
                else
                    PubUpdate(found);  
            }
        }

        public void Save(T updateObj)
        {
            var isCreate = false;
            var found = MereQuery.Create<T>()
                                .AddFilter(_keyColumn.ColumnName, SqlOperator.EqualTo, _keyColumn.GetBase(updateObj))
                                .ExecuteFirstOrDefault();
//
//
//            if (found == null)
//            {
//                found = updateObj;
//            }
//
//            foreach (var k in d.Keys)
//                {
//                    var col = _mereTable.GetMereColumn(k);
//                    if (col != null)
//                    {
//                        col.Set(found, d[k]);
//                    }
//                }
            isCreate = found != null;
            if (isCreate)
                PubCreate(found);
            else
                PubUpdate(found); 
        }

        //        /// <summary>
        //        /// subscribe to changes 
        //        /// </summary>
        //        /// <param name="cb">callback for change</param>
        //        /// <returns>id for subscription</returns>
        //        public string Subscribe(Action<T> cb)
        //        {
        //            var id = Guid.NewGuid().ToString();
        //            while (_writing)
        //            {
        //                Debug.WriteLine("waiting on _writing in data repo for " + _mereTable.TableName);
        //            }
        //            _subscriptions.Add(id, cb);
        //            return id;
        //        }

        //        public bool Update(ExpandoObject updateObj)
        //        {
        //            return Update(updateObj, null);
        //        }
        //
        //        public bool Update(ExpandoObject updateObj, int? modifiedBy)
        //        {
        //            var mereTable = MereUtils.CacheCheckMin<T>();
        //            var modifiedOn =
        //                mereTable.SelectMereColumns.FirstOrDefault(
        //                    x => string.Compare(x.ColumnName, "modifiedon", StringComparison.CurrentCultureIgnoreCase) == 0);
        //
        //            var modifiedByCol =
        //                mereTable.SelectMereColumns.FirstOrDefault(
        //                    x => string.Compare(x.ColumnName, "modifiedby", StringComparison.CurrentCultureIgnoreCase) == 0);
        //
        //            var d = (IDictionary<string, object>)updateObj;
        //
        //            if (modifiedBy != null && modifiedByCol != null)
        //                d[modifiedByCol.PropertyName] = (int)modifiedBy;
        //
        //            if (modifiedOn != null)
        //                d[modifiedOn.PropertyName] = DateTime.Now;
        //
        //
        //
        //            return (updateObj.MereUpdateAsync<T>(true)) > 0;
        //        }
    }
}
