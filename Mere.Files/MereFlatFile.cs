//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Mere.Attributes;
//
//namespace Mere.Files
//{
//    public class MereFlatFile// : MereTableMin<T>
//    {
//
////        public virtual IEnumerable<MereColumn> DelimitedFields
////        {
////            get { return PropertyHelpersIndexed.Values.Where(x => x.DelimitedFieldAttr != null).OrderBy(x => x.DelimitedFieldAttr.Index); }
////        }
//
////        public virtual IEnumerable<MereColumn> DelimitedFields
////        {
////            get { return SelectMereColumns.Where(x => x.DelimitedFieldAttr != null).OrderBy(x => x.DelimitedFieldAttr.Index); }
////        }
////
////        public virtual IEnumerable<MereColumn> FlatFileFields
////        {
////            get { return SelectMereColumns.Where(x => x.FlatFileFieldAttr != null).OrderBy(x => x.FlatFileFieldAttr.Index); }
////        } 
//        #region FlatFileParsing
//
//        //#region MultipleFiles
//
//        //public static IList<T> ParseAndInsertMultipleFlatFiles<T>(params string[] filePaths)
//        //{
//        //    var toReturn = new List<T>();
//        //    Parallel.ForEach(filePaths,
//        //                     x =>
//        //                     {
//        //                         var found = ParseFlatFileAsync<T>(x).Result;
//        //                         if (found.Any())
//        //                             found.MereBulkInsert();
//
//        //                         toReturn.AddRange(found);
//        //                     });
//        //    return toReturn;
//        //}
//
//        //public static IList<T> ParseAndInsertMultipleFlatFiles<T>(params FileInfo[] files)
//        //{
//        //    var toReturn = new List<T>();
//        //    Parallel.ForEach(files,
//        //                     x =>
//        //                     {
//        //                         var found = ParseFlatFileAsync<T>(x).Result;
//        //                         if (found.Any())
//        //                             found.MereBulkInsert();
//
//        //                         toReturn.AddRange(found);
//        //                     });
//        //    return toReturn;
//        //}
//
//        //public static IList<T> ParseAndInsertMultipleFlatFiles<T>(int batchSize, params string[] filePaths)
//        //{
//        //    var toReturn = new List<T>();
//        //    Parallel.ForEach(filePaths,
//        //                     x =>
//        //                     {
//        //                         var found = ParseFlatFileAsync<T>(x).Result;
//        //                         if (found.Any())
//        //                             found.MereBulkInsert(batchSize);
//
//        //                         toReturn.AddRange(found);
//        //                     });
//        //    return toReturn;
//        //}
//
//        //public static IList<T> ParseAndInsertMultipleFlatFiles<T>(int batchSize, params FileInfo[] files)
//        //{
//        //    var toReturn = new List<T>();
//        //    Parallel.ForEach(files,
//        //                     x =>
//        //                     {
//        //                         var found = ParseFlatFileAsync<T>(x).Result;
//        //                         if (found.Any())
//        //                             found.MereBulkInsert(batchSize);
//
//        //                         toReturn.AddRange(found);
//        //                     });
//        //    return toReturn;
//        //}
//
//        //public static IList<T> ParseAndInsertNameFilterFlatFiles<T>(string dirPath, string containsFilter, int batchSize)
//        //{
//        //    var dir = new DirectoryInfo(dirPath);
//        //    if (!dir.Exists)
//        //    {
//        //        Debug.WriteLine("Could not find directory when trying to parse flat files");
//        //        return null;
//        //    }
//
//        //    var files = dir.GetFiles("*" + containsFilter + "*");
//
//        //    if (!files.Any())
//        //    {
//        //        Debug.WriteLine("Did not find any files");
//        //        return null;
//        //    }
//
//        //    return ParseAndInsertMultipleFlatFiles<T>(files);
//        //}
//
//        //public static IList<T> ParseAndInsertNameFilterFlatFiles<T>(string dirPath, string containsFilter)
//        //{
//        //    var dir = new DirectoryInfo(dirPath);
//        //    if (!dir.Exists)
//        //    {
//        //        Debug.WriteLine("Could not find directory when trying to parse flat files");
//        //        return null;
//        //    }
//
//        //    var files = dir.GetFiles("*" + containsFilter + "*");
//
//        //    if (!files.Any())
//        //    {
//        //        Debug.WriteLine("Did not find any files");
//        //        return null;
//        //    }
//
//        //    return ParseAndInsertMultipleFlatFiles<T>(files);
//        //}
//
//        //public static IList<T> ParseNameFilterFlatFiles<T>(string dirPath, string containsFilter)
//        //{
//        //    var dir = new DirectoryInfo(dirPath);
//        //    if (!dir.Exists)
//        //    {
//        //        Debug.WriteLine("Could not find directory when trying to parse flat files");
//        //        return null;
//        //    }
//
//        //    var files = dir.GetFiles("*" + containsFilter + "*");
//        //    var toReturn = new List<T>();
//        //    Parallel.ForEach(files,
//        //                     x => toReturn.AddRange(ParseFlatFileAsync<T>(x).Result));
//        //    return toReturn;
//        //}
//
//        //public static IList<T> ParseMultipleFlatFiles<T>(params string[] filePaths)
//        //{
//        //    var toReturn = new List<T>();
//        //    Parallel.ForEach(filePaths,
//        //                     x => toReturn.AddRange(ParseFlatFileAsync<T>(new FileInfo(x)).Result));
//        //    return toReturn;
//        //}
//
//        //public static IList<T> ParseMultipleFlatFiles<T>(params FileInfo[] files)
//        //{
//        //    var toReturn = new List<T>();
//        //    Parallel.ForEach(files,
//        //                     x => toReturn.AddRange(ParseFlatFileAsync<T>(x).Result));
//        //    return toReturn;
//        //}
//        //#endregion
//
//        public static IList<T> ParseFlatFile<T>(string filePath) where T : new()
//        {
//            return ParseFlatFile<T>(new FileInfo(filePath));
//        }
//
//        public static IList<T> ParseFlatFile<T>(FileInfo file) where T : new()
//        {
//            var mereTable = MereUtils.CacheCheckFile<T>();
//            var flatFileFields = mereTable.FlatFileFields.ToList();
//            var toReturn = new List<T>();
//            string fileContent;
//            var isIndexed = mereTable.FlatFileTableAttr.IsIndexed;
//            var headers = flatFileFields
//                .Where(x => !string.IsNullOrEmpty(x.FlatFileFieldAttr.RecordKey))
//                .GroupBy(x => x.FlatFileFieldAttr.RecordKey, (key, group) => new { key, group })
//                .ToDictionary(x => x.key, x => x.group.OrderBy(o => o.FlatFileFieldAttr.Index).ToList());
//
//            var hasHeaders = headers.Any();
//            var headerLength = mereTable.FlatFileTableAttr.RecordKeyLength;
//            var headerRecordKey = mereTable.FlatFileTableAttr.HeaderRecordKey;
//            var headersToIgnore =
//                headers.Where(x => x.Value.Any(a => a.FlatFileFieldAttr.RecordMereTable != null)).Select(x => x.Key).ToList();
//
//            var headerRecordFields = headerRecordKey != null
//                ? mereTable.FlatFileFields.Where(x => x.FlatFileFieldAttr.RecordKey == headerRecordKey).ToList() : null;
//
//
//            var n = Activator.CreateInstance<T>();
//            var processedHeaders = new List<string>();
//            var pastHeader = false;
//
//            using (var sr = new StreamReader(file.FullName))
//            {
//                fileContent = sr.ReadToEnd();
//            }
//
//            var lines = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
//            var curPos = headerLength;
//            if (hasHeaders)
//            {
//                foreach (var line in lines)
//                {
//                    var header = string.Join("", line.Take(headerLength)).Trim();
//
//                    if (headers.ContainsKey(header))
//                    {
//                        if (headerRecordKey != null && pastHeader && header == headerRecordKey)
//                        {
//                            toReturn.Add(n);
//                            n = Activator.CreateInstance<T>();
//                            processedHeaders.Clear();
//                        }
//                        else
//                            pastHeader = true;
//
//                        if (!headersToIgnore.Contains(header) && headerRecordFields != null && processedHeaders.Contains(header))
//                        {
//                            processedHeaders.Clear();
//
//                            var nn = Activator.CreateInstance<T>();
//                            foreach (var field in headerRecordFields)
//                            {
//                                var val = field.Get(n);
//                                field.Set(nn, val);
//                            }
//                            toReturn.Add(n);
//                            n = nn;
//                        }
//
//                        var inHeader = headers[header];
//                        ParseFlatFileLine(ref n, line, inHeader, mereTable, curPos, isIndexed);
//
//                        processedHeaders.Add(header);
//                    }
//                }
//                if (n != null)
//                    toReturn.Add(n);
//            }
//            else
//            {
//                foreach (var line in lines)
//                {
//                    curPos = 0;
//                    n = Activator.CreateInstance<T>();
//                    ParseFlatFileLine(ref n, line, flatFileFields, mereTable, curPos, isIndexed);
//
//                    toReturn.Add(n);
//                    n = default(T);
//                }
//            }
//
//            return toReturn;
//        }
//
//        public static Task<IList<T>> ParseFlatFileAsync<T>(string filePath) where T : new()
//        {
//            return ParseFlatFileAsync<T>(new FileInfo(filePath));
//        }
//
//        public async static Task<IList<T>> ParseFlatFileAsync<T>(FileInfo file) where T : new()
//        {
//            var mereTable = MereUtils.CacheCheckFile<T>();
//            var flatFileFields = mereTable.FlatFileFields.ToList();
//            var toReturn = new List<T>();
//            string fileContent;
//            var isIndexed = mereTable.FlatFileTableAttr.IsIndexed;
//            var headers = flatFileFields
//                .Where(x => !string.IsNullOrEmpty(x.FlatFileFieldAttr.RecordKey))
//                .GroupBy(x => x.FlatFileFieldAttr.RecordKey, (key, group) => new { key, group })
//                .ToDictionary(x => x.key, x => x.group.OrderBy(o => o.FlatFileFieldAttr.Index).ToList());
//
//            var hasHeaders = headers.Any();
//            var headerLength = mereTable.FlatFileTableAttr.RecordKeyLength;
//            var headerRecordKey = mereTable.FlatFileTableAttr.HeaderRecordKey;
//            var headersToIgnore =
//                headers.Where(x => x.Value.Any(a => a.FlatFileFieldAttr.RecordMereTable != null)).Select(x => x.Key).ToList();
//
//            var headerRecordFields = headerRecordKey != null
//                ? mereTable.FlatFileFields.Where(x => x.FlatFileFieldAttr.RecordKey == headerRecordKey).ToList() : null;
//
//
//            var n = Activator.CreateInstance<T>();
//            var processedHeaders = new List<string>();
//            var pastHeader = false;
//
//            using (var sr = new StreamReader(file.FullName))
//            {
//                fileContent = await sr.ReadToEndAsync();
//            }
//
//            var lines = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
//            var curPos = headerLength;
//            if (hasHeaders)
//            {
//                foreach (var line in lines)
//                {
//                    var header = string.Join("", line.Take(headerLength)).Trim();
//
//                    if (headers.ContainsKey(header))
//                    {
//                        if (headerRecordKey != null && pastHeader && header == headerRecordKey)
//                        {
//                            toReturn.Add(n);
//                            n = Activator.CreateInstance<T>();
//                            processedHeaders.Clear();
//                        }
//                        else
//                            pastHeader = true;
//
//                        if (!headersToIgnore.Contains(header) && headerRecordFields != null && processedHeaders.Contains(header))
//                        {
//                            processedHeaders.Clear();
//
//                            var nn = Activator.CreateInstance<T>();
//                            foreach (var field in headerRecordFields)
//                            {
//                                var val = field.Get(n);
//                                field.Set(nn, val);
//                            }
//                            toReturn.Add(n);
//                            n = nn;
//                        }
//
//                        var inHeader = headers[header];
//                        ParseFlatFileLine(ref n, line, inHeader, mereTable, curPos, isIndexed);
//
//                        processedHeaders.Add(header);
//                    }
//                }
//                if (n != null)
//                    toReturn.Add(n);
//            }
//            else
//            {
//                Parallel.ForEach(lines,
//                                 (line, i) =>
//                                 {
//                                     curPos = 0;
//                                     n = Activator.CreateInstance<T>();
//                                     ParseFlatFileLine(ref n, line, flatFileFields, mereTable, curPos, isIndexed);
//
//                                     toReturn.Add(n);
//                                     n = default(T);
//                                 });
//            }
//
//
//            return toReturn;
//        }
//
//        public static void ParseFlatFileLine<T>(ref T record, string line, IEnumerable<MereColumn> mereColumns, MereTable mereTable, int curPos = 0, bool isIndexed = false)
//        {
//            foreach (var mereColumn in mereColumns)
//            {
//                if (mereColumn.FlatFileFieldAttr.RecordMereTable != null)
//                {
//                    var mereSubTable = mereColumn.FlatFileFieldAttr.RecordMereTable;
//                    var n = Activator.CreateInstance(mereColumn.FlatFileFieldAttr.RecordMereTable.TableClassType);
//                    foreach (var flatFileField in mereSubTable.FlatFileFields)
//                    {
//                        curPos = mereSubTable.FlatFileTableAttr.IsIndexed
//                                     ? curPos
//                                     : flatFileField.FlatFileFieldAttr.Index;
//                        var subVal =
//                            string.Join("", line.Skip(curPos).Take(flatFileField.FlatFileFieldAttr.Length)).Trim();
//                        subVal = MereFileFieldParser.Parse(flatFileField, subVal, false);
//                        curPos += flatFileField.FlatFileFieldAttr.Length;
//                        flatFileField.Set(n, subVal);
//                    }
//                    if (mereColumn.FlatFileFieldAttr.RecordKey == mereTable.FlatFileTableAttr.HeaderRecordKey)
//                        mereColumn.Set(record, n);
//                    else
//                    {
//                        var fieldVal = (IList)mereColumn.Get(record);
//                        if (fieldVal == null)
//                        {
//                            var listType = typeof(List<>);
//                            var constructedListType = listType.MakeGenericType(mereColumn.FlatFileFieldAttr.RecordMereTable.TableClassType);
//
//                            var instance = (IList)Activator.CreateInstance(constructedListType);
//                            instance.Add(n);
//                            mereColumn.Set(record, instance);
//
//                        }
//                        else
//                        {
//                            fieldVal.Add(n);
//                            mereColumn.Set(record, fieldVal);
//                        }
//                    }
//                }
//                else
//                {
//                    curPos = isIndexed ? curPos : mereColumn.FlatFileFieldAttr.Index;
//                    var val = string.Join("", line.Skip(curPos).Take(mereColumn.FlatFileFieldAttr.Length)).Trim();
//                    val = MereFileFieldParser.Parse(mereColumn, val, false);
//                    curPos += mereColumn.FlatFileFieldAttr.Length;
//                    mereColumn.Set(record, val);
//                }
//            }
//        }
//
//        #region OldForSubRecordsAboveShouldHandleNow
//        //public static IList<T> ParseFlatFileWithSubRecords<T>(string filePath) 
//        //{
//        //    return ParseFlatFileWithSubRecordsAsync<T>(new FileInfo(filePath)).Result;
//        //}
//
//        //public static IList<T> ParseFlatFileWithSubRecords<T>(FileInfo file) 
//        //{
//        //    return ParseFlatFileWithSubRecordsAsync<T>(file).Result;
//        //}
//
//        //public static Task<IList<T>> ParseFlatFileWithSubRecordsAsync<T>(string filePath) 
//        //{
//        //    return ParseFlatFileWithSubRecordsAsync<T>(new FileInfo(filePath));
//        //}
//
//        //public static async Task<IList<T>> ParseFlatFileWithSubRecordsAsync<T>(FileInfo file) 
//        //{
//        //    var mereTable = CacheCheck<T>();
//        //    string fileContent;
//        //    var toReturn = new List<T>();
//
//        //    using (var sr = new StreamReader(file.FullName))
//        //    {
//        //        fileContent = await sr.ReadToEndAsync();
//        //    }
//
//        //    var lines = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
//
//        //    var pastfirst = false;
//        //    var mainN = Activator.CreateInstance<T>();
//        //    var dic = new Dictionary<string, IList>();
//
//        //    foreach (var line in lines)
//        //    {
//        //        var key = string.Join("", line.Take(mereTable.FlatFileTableAttr.RecordKeyLength));
//        //        var curRec = mereTable.PropertyHelpersIndexed.Values.FirstOrDefault(x => x.FlatFileFieldAttr != null && x.FlatFileFieldAttr.RecordKey == key);
//
//
//        //        if (curRec == null)
//        //            continue;
//
//        //        if (curRec.FlatFileFieldAttr.RecordKey == mereTable.FlatFileTableAttr.HeaderRecordKey)
//        //        {
//        //            if (pastfirst)
//        //            {
//        //                foreach (var d in dic)
//        //                {
//        //                    var curRecDic = mereTable.PropertyHelpersIndexed.Values.FirstOrDefault(x => x.FlatFileFieldAttr != null && x.FlatFileFieldAttr.RecordKey == d.Key);
//
//        //                    if (curRecDic == null)
//        //                        continue;
//
//
//        //                    curRecDic.Set(mainN, d.Value);
//
//        //                }
//        //                toReturn.Add(mainN);
//        //                mainN = Activator.CreateInstance<T>();
//        //                dic = new Dictionary<string, IList>();
//        //            }
//
//        //            pastfirst = true;
//        //        }
//
//        //        var mereSubTable = CacheCheck(curRec.FlatFileFieldAttr.RecordMereTable);
//        //        var subFields = mereSubTable.FlatFileFields.ToList();
//        //        var n = Activator.CreateInstance(curRec.FlatFileFieldAttr.RecordMereTable);
//        //        var curPos = mereTable.FlatFileTableAttr.RecordKeyLength;
//        //        foreach (var mereColumn in subFields)
//        //        {
//        //            var val = string.Join("", line.Skip(curPos).Take(mereColumn.FlatFileFieldAttr.Length)).Trim();
//        //            val = MereFileFieldParser.Parse(mereColumn, val, false);
//        //            curPos += mereColumn.FlatFileFieldAttr.Length;
//        //            mereColumn.Set(n, val);
//        //        }
//        //        if (curRec.FlatFileFieldAttr.RecordKey == mereTable.FlatFileTableAttr.HeaderRecordKey)
//        //        {
//        //            curRec.Set(mainN, n);
//        //        }
//        //        else
//        //        {
//        //            if (!dic.ContainsKey(key))
//        //            {
//        //                var listType = typeof(List<>);
//        //                var constructedListType = listType.MakeGenericType(curRec.FlatFileFieldAttr.RecordMereTable);
//
//        //                var instance = Activator.CreateInstance(constructedListType);
//        //                dic.Add(key, (IList)instance);
//        //            }
//
//        //            dic[key].Add(n);
//        //        }
//
//        //    }
//
//
//        //    foreach (var d in dic)
//        //    {
//        //        var curRecDic = mereTable.PropertyHelpersIndexed.Values.FirstOrDefault(x => x.FlatFileFieldAttr != null && x.FlatFileFieldAttr.RecordKey == d.Key);
//
//        //        if (curRecDic == null)
//        //            continue;
//
//
//        //        curRecDic.Set(mainN, d.Value);
//
//        //    }
//        //    toReturn.Add(mainN);
//
//
//
//        //    return toReturn;
//        //}
//        #endregion
//
//        #endregion
//
//        #region WriteToFlatFile
//        #region Single
//        public static void WriteSingleToFlatFile<T>(string filePath, T toWrite) where T : new()
//        {
//            WriteToFlatFile(new FileInfo(filePath), new[] { toWrite }, false, false);
//        }
//
//        public static void WriteSingleToFlatFile<T>(FileInfo file, T toWrite) where T : new()
//        {
//            if (toWrite is IList)
//                WriteToFlatFile(file, (IEnumerable<object>)toWrite, false, false);
//            else
//                WriteToFlatFile(file, new[] { toWrite }, false, false);
//
//        }
//
//        public static Task WriteSingleToFlatFileAsync<T>(string filePath, T toWrite) where T : new()
//        {
//            if (toWrite is IList || toWrite is IEnumerable)
//                return WriteToFlatFileAsync(new FileInfo(filePath), (IEnumerable<object>)toWrite, false, false);
//
//            return WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, false, false);
//        }
//
//        public static Task WriteSingleToFlatFileAsync<T>(FileInfo file, T toWrite) where T : new()
//        {
//            if (toWrite is IList)
//                return WriteToFlatFileAsync(file, (IEnumerable<object>)toWrite, false, false);
//
//            return WriteToFlatFileAsync(file, new[] { toWrite }, false, false);
//        }
//
//        public static void WriteSingleToFlatFile<T>(string filePath, T toWrite, bool append) where T : new()
//        {
//            if (toWrite is IList)
//                WriteToFlatFile(new FileInfo(filePath), (IEnumerable<object>)toWrite, append, false);
//            else
//                WriteToFlatFile(new FileInfo(filePath), new[] { toWrite }, append, false);
//        }
//
//        public static void WriteSingleToFlatFile<T>(FileInfo file, T toWrite, bool append) where T : new()
//        {
//            if (toWrite is IList)
//                WriteToFlatFile(file, (IEnumerable<object>)toWrite, append, false);
//            else
//                WriteToFlatFile(file, new[] { toWrite }, append, false);
//
//        }
//
//        public static Task WriteSingleToFlatFileAsync<T>(string filePath, T toWrite, bool append) where T : new()
//        {
//            if (toWrite is IList)
//                return WriteToFlatFileAsync(new FileInfo(filePath), (IEnumerable<object>)toWrite, append, false);
//
//            return WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, append, false);
//        }
//
//        public static Task WriteSingleToFlatFileAsync<T>(FileInfo file, T toWrite, bool append) where T : new()
//        {
//            return WriteToFlatFileAsync(file, new[] { toWrite }, append, false);
//        }
//
//        public static void WriteSingleToFlatFile<T>(string filePath, T toWrite, bool append, bool lineFeed) where T : new()
//        {
//            WriteToFlatFile(new FileInfo(filePath), new[] { toWrite }, append, lineFeed);
//        }
//
//        public static void WriteSingleToFlatFile<T>(FileInfo file, T toWrite, bool append, bool lineFeed) where T : new()
//        {
//            WriteToFlatFile(file, new[] { toWrite }, append, lineFeed);
//        }
//
//        public static Task WriteSingleToFlatFileAsync<T>(string filePath, T toWrite, bool append, bool lineFeed) where T : new()
//        {
//            return WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, append, lineFeed);
//        }
//
//        public static Task WriteSingleToFlatFileAsync<T>(FileInfo file, T toWrite, bool append,
//                                                         bool lineFeed) where T : new()
//        {
//            return WriteToFlatFileAsync(file, new[] { toWrite }, append, lineFeed);
//        }
//        #endregion
//
//        public static void WriteToFlatFile<T>(string filePath, IEnumerable<T> toWrite) where T : new()
//        {
//            WriteToFlatFile(new FileInfo(filePath), toWrite, false, false);
//        }
//
//        public static void WriteToFlatFile<T>(FileInfo file, IEnumerable<T> toWrite) where T : new()
//        {
//            WriteToFlatFile(file, toWrite, false, false);
//        }
//
//        public static Task WriteToFlatFileAsync<T>(string filePath, IEnumerable<T> toWrite) where T : new()
//        {
//            return WriteToFlatFileAsync(new FileInfo(filePath), toWrite, false, false);
//        }
//
//        public static Task WriteToFlatFileAsync<T>(FileInfo file, IEnumerable<T> toWrite) where T : new()
//        {
//            return WriteToFlatFileAsync(file, toWrite, false, false);
//        }
//
//        public static void WriteToFlatFile<T>(string filePath, IEnumerable<T> toWrite, bool append) where T : new()
//        {
//            WriteToFlatFile(new FileInfo(filePath), toWrite, append, false);
//        }
//
//        public static void WriteToFlatFile<T>(FileInfo file, IEnumerable<T> toWrite, bool append) where T : new()
//        {
//            WriteToFlatFile(file, toWrite, append, false);
//        }
//
//        public static Task WriteToFlatFileAsync<T>(string filePath, IEnumerable<T> toWrite, bool append) where T : new()
//        {
//            return WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, false);
//        }
//
//        public static Task WriteToFlatFileAsync<T>(FileInfo file, IEnumerable<T> toWrite, bool append) where T : new()
//        {
//            return WriteToFlatFileAsync(file, toWrite, append, false);
//        }
//
//        public static void WriteToFlatFile<T>(string filePath, IEnumerable<T> toWrite, bool append, bool lineFeed) where T : new()
//        {
//            WriteToFlatFile(new FileInfo(filePath), toWrite, append, lineFeed);
//        }
//
//        public static void WriteToFlatFile<T>(FileInfo file, IEnumerable<T> toWrite, bool append, bool lineFeed) where T : new()
//        {
//            //WriteToFlatFileAsync(file, toWrite, append, lineFeed).Wait();
//            var type = typeof(T);
//            var newLineType = !lineFeed ? "\r\n" : "\n";
//            var mereTable = MereUtils.CacheCheckFile<T>();
//            var flatFileFields = mereTable.FlatFileFields.ToList();
//
//            var linesToWrite = new List<string>();
//            foreach (var val in toWrite)
//            {
//                var curLine = new StringBuilder();
//                foreach (var field in flatFileFields)
//                {
//
//                    var objVal = field.Get(val);
//                    var v = MereFileFieldParser.ParseFieldForWrite(field, objVal, false);
//                    curLine.Append(v);
//                }
//                linesToWrite.Add(curLine.ToString());
//            }
//
//            var fileContent = append ? newLineType : "";
//            fileContent += string.Join(newLineType, linesToWrite);
//            if (!file.Exists)
//                file.Create().Close();
//
//            using (var sw = new StreamWriter(file.FullName, append))
//            {
//                sw.Write(fileContent);
//            }
//        }
//
//        public static Task WriteToFlatFileAsync<T>(string filePath, IEnumerable<T> toWrite, bool append, bool lineFeed) where T : new()
//        {
//            return WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, lineFeed);
//        }
//
//        public async static Task WriteToFlatFileAsync<T>(FileInfo file, IEnumerable<T> toWrite, bool append, bool lineFeed) where T : new()
//        {
//            var type = typeof (T);
//            var newLineType = !lineFeed ? "\r\n" : "\n";
//            var mereTable = MereUtils.CacheCheckFile<T>();
//            var flatFileFields = mereTable.FlatFileFields.ToList();
//
//            var linesToWrite = new List<string>();
//            foreach (var val in toWrite)
//            {
//                var curLine = new StringBuilder();
//                foreach (var field in flatFileFields)
//                {
//
//                    var objVal = field.Get(val);
//                    var v = MereFileFieldParser.ParseFieldForWrite(field, objVal, false);
//                    curLine.Append(v);
//                }
//                linesToWrite.Add(curLine.ToString());
//            }
//
//            var fileContent = append ? newLineType : "";
//            fileContent += string.Join(newLineType, linesToWrite);
//            if (!file.Exists)
//                file.Create().Close();
//
//            using (var sw = new StreamWriter(file.FullName, append))
//            {
//                await sw.WriteAsync(fileContent);
//            }
//
//        }
//
//
//        #region WithSubRecords
//        #region Single
//        public static void WriteSingleToFlatFileWithSubRecords<T>(string filePath, T toWrite) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), new[] { toWrite }, false, false, false).Wait();
//        }
//
//        public static void WriteSingleToFlatFileWithSubRecords<T>(FileInfo file, T toWrite) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(file, new[] { toWrite }, false, false, false).Wait();
//        }
//
//        public static Task WriteSingleToFlatFileWithSubRecordsAsync<T>(string filePath, T toWrite) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), new[] { toWrite }, false, false, false);
//        }
//
//        public static Task WriteSingleToFlatFileWithSubRecordsAsync<T>(FileInfo file, T toWrite) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(file, new[] { toWrite }, false, false, false);
//        }
//
//        public static void WriteSingleToFlatFileWithSubRecords<T>(string filePath, T toWrite, bool includeHeaderKey, bool append) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), new[] { toWrite }, includeHeaderKey, append, false).Wait();
//        }
//
//        public static void WriteSingleToFlatFileWithSubRecords<T>(FileInfo file, T toWrite, bool includeHeaderKey, bool append) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(file, new[] { toWrite }, includeHeaderKey, append, false).Wait();
//        }
//
//        public static Task WriteSingleToFlatFileWithSubRecordsAsync<T>(string filePath, T toWrite, bool includeHeaderKey, bool append) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), new[] { toWrite }, includeHeaderKey, append, false);
//        }
//
//        public static Task WriteSingleToFlatFileWithSubRecordsAsync<T>(FileInfo file, T toWrite, bool includeHeaderKey, bool append) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(file, new[] { toWrite }, includeHeaderKey, append, false);
//        }
//
//        public static void WriteSingleToFlatFileWithSubRecords<T>(string filePath, T toWrite, bool includeHeaderKey, bool append, bool lineFeed) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), new[] { toWrite }, includeHeaderKey, append, lineFeed).Wait();
//        }
//
//        public static void WriteSingleToFlatFileWithSubRecords<T>(FileInfo file, T toWrite, bool includeHeaderKey, bool append, bool lineFeed) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(file, new[] { toWrite }, includeHeaderKey, append, lineFeed).Wait();
//        }
//
//        public static Task WriteSingleToFlatFileWithSubRecordsAsync<T>(string filePath, T toWrite, bool includeHeaderKey, bool append, bool lineFeed) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), new[] { toWrite }, includeHeaderKey, append, lineFeed);
//        }
//
//        public static Task WriteSingleToFlatFileWithSubRecordsAsync<T>(FileInfo file, T toWrite, bool includeHeaderKey, bool append,
//                                                         bool lineFeed) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(file, new[] { toWrite }, includeHeaderKey, append, lineFeed);
//        }
//        #endregion
//
//        public static void WriteToFlatFileWithSubRecords<T>(string filePath, IEnumerable<T> toWrite) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), toWrite, false, false, false).Wait();
//        }
//
//        public static void WriteToFlatFileWithSubRecords<T>(FileInfo file, IEnumerable<T> toWrite) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(file, toWrite, false, false, false).Wait();
//        }
//
//        public static Task WriteToFlatFileWithSubRecordsAsync<T>(string filePath, IEnumerable<T> toWrite) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), toWrite, false, false, false);
//        }
//
//        public static Task WriteToFlatFileWithSubRecordsAsync<T>(FileInfo file, IEnumerable<T> toWrite) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(file, toWrite, false, false, false);
//        }
//
//        public static void WriteToFlatFileWithSubRecords<T>(string filePath, IEnumerable<T> toWrite, bool includeHeaderKey, bool append) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), toWrite, includeHeaderKey, append, false).Wait();
//        }
//
//        public static void WriteToFlatFileWithSubRecords<T>(FileInfo file, IEnumerable<T> toWrite, bool includeHeaderKey, bool append) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(file, toWrite, includeHeaderKey, append, false).Wait();
//        }
//
//        public static Task WriteToFlatFileWithSubRecordsAsync<T>(string filePath, IEnumerable<T> toWrite, bool includeHeaderKey, bool append) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), toWrite, includeHeaderKey, append, false);
//        }
//
//        public static Task WriteToFlatFileWithSubRecordsAsync<T>(FileInfo file, IEnumerable<T> toWrite, bool includeHeaderKey,
//                                                         bool append) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(file, toWrite, includeHeaderKey, append, false);
//        }
//
//        public static void WriteToFlatFileWithSubRecords<T>(string filePath, IEnumerable<T> toWrite, bool includeHeaderKey, bool append, bool lineFeed) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), toWrite, includeHeaderKey, append, lineFeed).Wait();
//        }
//
//        public static void WriteToFlatFileWithSubRecords<T>(FileInfo file, IEnumerable<T> toWrite, bool includeHeaderKey, bool append, bool lineFeed) where T : new()
//        {
//            WriteToFlatFileWithSubRecordsAsyncInternal(file, toWrite, includeHeaderKey, append, lineFeed).Wait();
//        }
//
//        public static Task WriteToFlatFileWithSubRecordsAsync<T>(string filePath, IEnumerable<T> toWrite, bool includeHeaderKey, bool append, bool lineFeed) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(new FileInfo(filePath), toWrite, includeHeaderKey, append, lineFeed);
//        }
//
//        protected static Task WriteToFlatFileWithSubRecordsAsync<T>(FileInfo file, IEnumerable<T> toWrite, bool includeHeaderKey, bool append,
//                                                                  bool lineFeed) where T : new()
//        {
//            return WriteToFlatFileWithSubRecordsAsyncInternal(file, toWrite, includeHeaderKey, append, lineFeed);
//        }
//
//
//        protected static async Task WriteToFlatFileWithSubRecordsAsyncInternal<T>(FileInfo file, IEnumerable<T> toWrite, bool includeHeaderKey, bool append, bool lineFeed) where T : new()
//        {
//            var mereTable = MereUtils.CacheCheckFile<T>();
//            var flatFileHeaderRecord = mereTable.FlatFileFields.FirstOrDefault(x => x.FlatFileFieldAttr.RecordKey == mereTable.FlatFileTableAttr.HeaderRecordKey);
//            var flatFileSubRecords = mereTable.FlatFileFields.Where(x => x.FlatFileFieldAttr.RecordKey != null && x.FlatFileFieldAttr.RecordKey != flatFileHeaderRecord.FlatFileFieldAttr.RecordKey).OrderBy(x => x.FlatFileFieldAttr.Index).ToList();
//
//            if (flatFileHeaderRecord == null)
//                throw new Exception("Unable to find flat file header record");
//
//            var mereTableSubRecord = flatFileHeaderRecord.FlatFileFieldAttr.RecordMereTable;
//            var newLineType = !lineFeed ? "\r\n" : "\n";
//            var flatFileFields = mereTableSubRecord.FlatFileFields.ToList();
//
//            var linesToWrite = new List<string>();
//            foreach (var main in toWrite)
//            {
//                var headerVal = flatFileHeaderRecord.Get(main);
//                var curLine = new StringBuilder();
//
//                if (includeHeaderKey && flatFileHeaderRecord.FlatFileFieldAttr.RecordKey != null)
//                    curLine.Append(flatFileHeaderRecord.FlatFileFieldAttr.RecordKey);
//
//                foreach (var field in flatFileFields)
//                {
//                    var objVal = field.Get(headerVal);
//                    var v = MereFileFieldParser.ParseFieldForWrite(field, objVal, false);
//                    curLine.Append(v);
//                }
//                linesToWrite.Add(curLine.ToString());
//
//                foreach (var sub in flatFileSubRecords)
//                {
//                    mereTableSubRecord = sub.FlatFileFieldAttr.RecordMereTable;
//                    flatFileFields = mereTableSubRecord.FlatFileFields.ToList();
//                    var val = sub.Get(main);
//                    if (val == null)
//                        continue;
//                    foreach (var o in (IEnumerable)val)
//                    {
//                        curLine.Clear();
//                        if (includeHeaderKey && sub.FlatFileFieldAttr.RecordKey != null)
//                            curLine.Append(sub.FlatFileFieldAttr.RecordKey);
//                        foreach (var field in flatFileFields)
//                        {
//                            var objVal = field.Get(o);
//                            var v = MereFileFieldParser.ParseFieldForWrite(field, objVal, false);
//                            curLine.Append(v);
//                        }
//                        linesToWrite.Add(curLine.ToString());
//                    }
//                }
//            }
//
//
//            var fileContent = append ? newLineType : "";
//            fileContent += string.Join(newLineType, linesToWrite);
//            if (!file.Exists)
//                file.Create().Close();
//
//            using (var sw = new StreamWriter(file.FullName, append))
//            {
//                await sw.WriteAsync(fileContent);
//            }
//
//        }
//
//        #endregion
//        #endregion
//    }
//}
