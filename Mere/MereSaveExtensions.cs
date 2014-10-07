using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public static class MereSaveExtensions
    {
        #region sync
        public static bool MereSave<T>(this T src) where T : new()
        {
            return Mere.MereSave.Execute(src);
        }
        public static bool MereSave<T>(this T src, MereDataSource mereDataSource) where T : new()
        {
            return Mere.MereSave.Execute(src, mereDataSource);
        }
        #endregion
        public static async Task<bool> MereSaveAsync<T>(this T src) where T : new()
        {
            return await Mere.MereSave.ExecuteAsync(src);
        }
        public static async Task<bool> MereSaveAsync<T>(this T src, MereDataSource mereDataSource) where T : new()
        {
            return await Mere.MereSave.ExecuteAsync(src, mereDataSource);
        }

        public async static Task MereSaveAllAsync<T>(this T toSaveEn) where T : IEnumerable<object>
        {
            var saveCnt = 0;

            var saving = new List<Task>();

            foreach (var toSave in toSaveEn)
            {
                var s = toSave;
                saving.Add(s.MereSaveAsync());
            }

            await Task.WhenAll(saving);
        }

        public static bool MereSaveAllUpdateKey<T>(this T toSaveEn) where T : IEnumerable<object>
        {
            var saveCnt = 0;

            Parallel.ForEach(toSaveEn,
                             x =>
                             {
                                 if (x.MereSave())
                                 {
                                     saveCnt++;
                                 }
                             });
            return saveCnt == toSaveEn.Count();
        }
    }
}
