using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files
{
    public class MereDelimitedFile<T> : MereFile<T> where T :new()
    {
       
    }
}







//old
//namespace Mere.Files
//{
//    public class MereDelimitedFile
//    {
//        private MereDelimitedFile()
//        {
//
//        }
//        #region DelimitedFileParsing
//        #region MultipleFiles
//
//        public static IList<T> ParseAndInsertMultipleDelimitedFiles<T>(params string[] filePaths) where T : new()
//        {
//            var toReturn = new List<T>();
//            Parallel.ForEach(filePaths,
//                             x =>
//                             {
//                                 var found = ParseDelimitedFileAsync<T>(x).Result;
//                                 if (found.Any())
//                                     found.MereBulkInsert();
//
//                                 toReturn.AddRange(found);
//                             });
//            return toReturn;
//        }
//
//        public static IList<T> ParseAndInsertMultipleDelimitedFiles<T>(params FileInfo[] files) where T : new()
//        {
//            var toReturn = new List<T>();
//            Parallel.ForEach(files,
//                             x =>
//                             {
//                                 var found = ParseDelimitedFileAsync<T>(x).Result;
//                                 if (found.Any())
//                                     found.MereBulkInsert();
//
//                                 toReturn.AddRange(found);
//                             });
//            return toReturn;
//        }
//
//        public static IList<T> ParseAndInsertMultipleDelimitedFiles<T>(int batchSize, params string[] filePaths) where T : new()
//        {
//            var toReturn = new List<T>();
//            Parallel.ForEach(filePaths,
//                             x =>
//                             {
//                                 var found = ParseDelimitedFileAsync<T>(x).Result;
//                                 if (found.Any())
//                                     found.MereBulkInsert(batchSize);
//
//                                 toReturn.AddRange(found);
//                             });
//            return toReturn;
//        }
//
//        public static IList<T> ParseAndInsertMultipleDelimitedFiles<T>(int batchSize, params FileInfo[] files) where T : new()
//        {
//            var toReturn = new List<T>();
//            Parallel.ForEach(files,
//                             x =>
//                             {
//                                 var found = ParseDelimitedFileAsync<T>(x).Result;
//                                 if (found.Any())
//                                     found.MereBulkInsert(batchSize);
//
//                                 toReturn.AddRange(found);
//                             });
//            return toReturn;
//        }
//
//        public static IList<T> ParseAndInsertNameFilterDelimitedFiles<T>(string dirPath, string containsFilter, int batchSize) where T : new()
//        {
//            var dir = new DirectoryInfo(dirPath);
//            if (!dir.Exists)
//            {
//                Debug.WriteLine("Could not find directory when trying to parse Delimited files");
//                return null;
//            }
//
//            var files = dir.GetFiles("*" + containsFilter + "*");
//
//            if (!files.Any())
//            {
//                Debug.WriteLine("Did not find any files");
//                return null;
//            }
//
//            return ParseAndInsertMultipleDelimitedFiles<T>(files);
//        }
//
//        public static IList<T> ParseAndInsertNameFilterDelimitedFiles<T>(string dirPath, string containsFilter) where T : new()
//        {
//            var dir = new DirectoryInfo(dirPath);
//            if (!dir.Exists)
//            {
//                Debug.WriteLine("Could not find directory when trying to parse Delimited files");
//                return null;
//            }
//
//            var files = dir.GetFiles("*" + containsFilter + "*");
//
//            if (!files.Any())
//            {
//                Debug.WriteLine("Did not find any files");
//                return null;
//            }
//
//            return ParseAndInsertMultipleDelimitedFiles<T>(files);
//        }
//
//        public static IList<T> ParseNameFilterDelimitedFiles<T>(string dirPath, string containsFilter) where T : new()
//        {
//            var dir = new DirectoryInfo(dirPath);
//            if (!dir.Exists)
//            {
//                Debug.WriteLine("Could not find directory when trying to parse Delimited files");
//                return null;
//            }
//
//            var files = dir.GetFiles("*" + containsFilter + "*");
//            var toReturn = new List<T>();
//            Parallel.ForEach(files,
//                             x => toReturn.AddRange(ParseDelimitedFileAsync<T>(x).Result));
//            return toReturn;
//        }
//
//        public static IList<T> ParseMultipleDelimitedFiles<T>(params string[] filePaths) where T : new()
//        {
//            var toReturn = new List<T>();
//            Parallel.ForEach(filePaths,
//                             x => toReturn.AddRange(ParseDelimitedFileAsync<T>(new FileInfo(x)).Result));
//            return toReturn;
//        }
//
//        public static IList<T> ParseMultipleDelimitedFiles<T>(params FileInfo[] files) where T : new()
//        {
//            var toReturn = new List<T>();
//            Parallel.ForEach(files,
//                             x => toReturn.AddRange(ParseDelimitedFileAsync<T>(x).Result));
//            return toReturn;
//        }
//        #endregion
//
//        public static IList<T> ParseDelimitedFile<T>(string filePath) where T : new()
//        {
//            return ParseDelimitedFileAsync<T>(new FileInfo(filePath), null, null).Result;
//        }
//
//        public static IList<T> ParseDelimitedFile<T>(FileInfo file) where T : new()
//        {
//            return ParseDelimitedFileAsync<T>(file, null, null).Result;
//        }
//
//        public static Task<IList<T>> ParseDelimitedFileAsync<T>(string filePath) where T : new()
//        {
//            return ParseDelimitedFileAsync<T>(new FileInfo(filePath), null, null);
//        }
//
//        public static Task<IList<T>> ParseDelimitedFileAsync<T>(FileInfo file) where T : new()
//        {
//            return ParseDelimitedFileAsync<T>(file, null, null);
//        }
//
//        public static IList<T> ParseDelimitedFile<T>(string filePath, string trimStart, string trimEnd) where T : new()
//        {
//            return ParseDelimitedFileAsync<T>(new FileInfo(filePath), trimStart, trimEnd).Result;
//        }
//
//        public static IList<T> ParseDelimitedFile<T>(FileInfo file, string trimStart, string trimEnd) where T : new()
//        {
//            return ParseDelimitedFileAsync<T>(file, trimStart, trimEnd).Result;
//        }
//
//        public static Task<IList<T>> ParseDelimitedFileAsync<T>(string filePath, string trimStart, string trimEnd) where T : new()
//        {
//            return ParseDelimitedFileAsync<T>(new FileInfo(filePath), trimStart, trimEnd);
//        }
//
//        public async static Task<IList<T>> ParseDelimitedFileAsync<T>(FileInfo file, string trimStart, string trimEnd) where T : new()
//        {
//            return await Task.Run(
//                () =>
//                {
//                    var mereTable = MereUtils.CacheCheckFile<T>();
//                    var delimitedFields = mereTable.DelimitedFields.ToList();
//                    string fileContent;
//                    var toReturn = new List<T>();
//                    List<string> header = null;
//
//
//                    var lines = File.ReadLines(file.FullName).Select(s => s.Split(new[] { mereTable.Delimiter }, StringSplitOptions.None).ToList()).ToList();
//
//                    if (lines.Count < 1 || (mereTable.DelimitedHasHeader && lines.Count < 2))
//                        return null;
//
//                    if (mereTable.DelimitedHasHeader)
//                    {
//                        header = lines.FirstOrDefault();
//                        lines.RemoveAt(0);
//                    }
//
//                    //foreach (var line in lines)
//                    Parallel.ForEach(lines, line =>
//                    {
//
//                        var n = Activator.CreateInstance<T>();
//                        foreach (var mereColumn in delimitedFields)
//                        {
//
//                            if (mereColumn.DelimitedFieldAttr.Index >= line.Count)
//                                continue;
//
//                            var val = line[mereColumn.DelimitedFieldAttr.Index];
//
//                            if (trimStart != null) val = val.TrimStart(trimStart.ToCharArray());
//                            if (trimEnd != null) val = val.TrimEnd(trimEnd.ToCharArray());
//
//                            if (mereColumn.DelimitedFieldAttr.TrimStart != null)
//                                val = val.TrimStart(mereColumn.DelimitedFieldAttr.TrimStart.ToCharArray());
//                            if (mereColumn.DelimitedFieldAttr.TrimEnd != null)
//                                val = val.TrimEnd(mereColumn.DelimitedFieldAttr.TrimEnd.ToCharArray());
//
//                            if (mereTable.DelimitedTableAttr != null && mereTable.DelimitedTableAttr.TrimStart != null)
//                                val = val.TrimStart(mereTable.DelimitedTableAttr.TrimStart.ToCharArray());
//                            if (mereTable.DelimitedTableAttr != null && mereTable.DelimitedTableAttr.TrimEnd != null)
//                                val = val.TrimEnd(mereTable.DelimitedTableAttr.TrimEnd.ToCharArray());
//
//                            //if (mereColumn.DelimitedFieldAttr.ParsingOptions != null && mereColumn.DelimitedFieldAttr.ParsingOptions.Any())
//                            val = MereFileFieldParser.Parse(mereColumn, val, true);
//
//                            mereColumn.Set(n, val);
//                        }
//
//                        toReturn.Add(n);
//                    });
//
//                    return toReturn;
//                });
//        }
//
//        //public async static Task<IList<T>> ParseDelimitedFileAsync<T>(FileInfo file, string trimStart, string trimEnd) 
//        //{
//        //    var mereTable = CacheCheck<T>();
//        //    var delimitedFields = mereTable.DelimitedFields.ToList();
//        //    string fileContent;
//        //    var toReturn = new List<T>();
//        //    List<string> header = null;
//
//        //    using (var sr = new StreamReader(file.FullName))
//        //    {
//        //        fileContent = await sr.ReadToEndAsync();
//        //    }
//
//        //    var lines = fileContent.SplitOnNewLine(true)
//        //        .Select(s => s.Split(new[] { mereTable.Delimiter }, StringSplitOptions.None).ToList()).ToList();
//
//        //    if (lines.Count < 1 || (mereTable.DelimitedHasHeader && lines.Count < 2))
//        //        return null;
//
//        //    if (mereTable.DelimitedHasHeader)
//        //    {
//        //        header = lines.FirstOrDefault();
//        //        lines.RemoveAt(0);
//        //    }
//
//        //    foreach (var line in lines)
//        //    {
//
//        //        var n = Activator.CreateInstance<T>();
//        //        foreach (var mereColumn in delimitedFields)
//        //        {
//
//        //            if(mereColumn.DelimitedFieldAttr.Index >= line.Count)
//        //                continue;
//
//        //            var val = line[mereColumn.DelimitedFieldAttr.Index];
//
//        //            if (trimStart != null) val = val.TrimStart(trimStart.ToCharArray());
//        //            if (trimEnd != null) val = val.TrimEnd(trimEnd.ToCharArray());
//
//        //            if (mereColumn.DelimitedFieldAttr.TrimStart != null) val = val.TrimStart(mereColumn.DelimitedFieldAttr.TrimStart.ToCharArray());
//        //            if (mereColumn.DelimitedFieldAttr.TrimEnd != null) val = val.TrimEnd(mereColumn.DelimitedFieldAttr.TrimEnd.ToCharArray());
//
//        //            if (mereTable.DelimitedTableAttr != null && mereTable.DelimitedTableAttr.TrimStart != null) val = val.TrimStart(mereTable.DelimitedTableAttr.TrimStart.ToCharArray());
//        //            if (mereTable.DelimitedTableAttr != null && mereTable.DelimitedTableAttr.TrimEnd != null) val = val.TrimEnd(mereTable.DelimitedTableAttr.TrimEnd.ToCharArray());
//
//        //            //if (mereColumn.DelimitedFieldAttr.ParsingOptions != null && mereColumn.DelimitedFieldAttr.ParsingOptions.Any())
//        //                val = MereFileFieldParser.Parse(mereColumn, val, true);
//
//        //            mereColumn.Set(n, val);
//        //        }
//
//        //        toReturn.Add(n);
//        //    }
//
//        //    return toReturn;
//        //}
//        #endregion
//
//        #region WriteToDelimitedFile
//        #region Single
//        public static void WriteSingleToDelimitedFile<T>(string filePath, T toWrite) where T : new()
//        {
//            WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, false, false).Wait();
//        }
//
//        public static void WriteSingleToDelimitedFile<T>(FileInfo file, T toWrite) where T : new()
//        {
//            WriteToDelimitedFileAsync(file, new[] { toWrite }, false, false).Wait();
//        }
//
//        public static Task WriteSingleToDelimitedFileAsync<T>(string filePath, T toWrite) where T : new()
//        {
//            return WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, false, false);
//        }
//
//        public static Task WriteSingleToDelimitedFileAsync<T>(FileInfo file, T toWrite) where T : new()
//        {
//            return WriteToDelimitedFileAsync(file, new[] { toWrite }, false, false);
//        }
//
//        public static void WriteSingleToDelimitedFile<T>(string filePath, T toWrite, bool append) where T : new()
//        {
//            WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, append, false).Wait();
//        }
//
//        public static void WriteSingleToDelimitedFile<T>(FileInfo file, T toWrite, bool append) where T : new()
//        {
//            WriteToDelimitedFileAsync(file, new[] { toWrite }, append, false).Wait();
//        }
//
//        public static Task WriteSingleToDelimitedFileAsync<T>(string filePath, T toWrite, bool append) where T : new()
//        {
//            return WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, append, false);
//        }
//
//        public static Task WriteSingleToDelimitedFileAsync<T>(FileInfo file, T toWrite, bool append) where T : new()
//        {
//            return WriteToDelimitedFileAsync(file, new[] { toWrite }, append, false);
//        }
//
//        public static void WriteSingleToDelimitedFile<T>(string filePath, T toWrite, bool append, bool lineFeed) where T : new()
//        {
//            WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, append, lineFeed).Wait();
//        }
//
//        public static void WriteSingleToDelimitedFile<T>(FileInfo file, T toWrite, bool append, bool lineFeed) where T : new()
//        {
//            WriteToDelimitedFileAsync(file, new[] { toWrite }, append, lineFeed).Wait();
//        }
//
//        public static Task WriteSingleToDelimitedFileAsync<T>(string filePath, T toWrite, bool append, bool lineFeed) where T : new()
//        {
//            return WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, append, lineFeed);
//        }
//
//        public static Task WriteSingleToDelimitedFileAsync<T>(FileInfo file, T toWrite, bool append,
//                                                         bool lineFeed) where T : new()
//        {
//            return WriteToDelimitedFileAsync(file, new[] { toWrite }, append, lineFeed);
//        }
//        #endregion
//
//        #region Multiple
//        public static void WriteToDelimitedFile<T>(string filePath, IEnumerable<T> toWrite) where T : new()
//        {
//            WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, false, false).Wait();
//        }
//
//        public static void WriteToDelimitedFile<T>(FileInfo file, IEnumerable<T> toWrite) where T : new()
//        {
//            WriteToDelimitedFileAsync(file, toWrite, false, false).Wait();
//        }
//
//        public static Task WriteToDelimitedFileAsync<T>(string filePath, IEnumerable<T> toWrite) where T : new()
//        {
//            return WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, false, false);
//        }
//
//        public static Task WriteToDelimitedFileAsync<T>(FileInfo file, IEnumerable<T> toWrite) where T : new()
//        {
//            return WriteToDelimitedFileAsync(file, toWrite, false, false);
//        }
//
//        public static void WriteToDelimitedFile<T>(string filePath, IEnumerable<T> toWrite, bool append) where T : new()
//        {
//            WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, false).Wait();
//        }
//
//        public static void WriteToDelimitedFile<T>(FileInfo file, IEnumerable<T> toWrite, bool append) where T : new()
//        {
//            WriteToDelimitedFileAsync(file, toWrite, append, false).Wait();
//        }
//
//        public static Task WriteToDelimitedFileAsync<T>(string filePath, IEnumerable<T> toWrite, bool append) where T : new()
//        {
//            return WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, false);
//        }
//
//        public static Task WriteToDelimitedFileAsync<T>(FileInfo file, IEnumerable<T> toWrite,
//                                                         bool append) where T : new()
//        {
//            return WriteToDelimitedFileAsync(file, toWrite, append, false);
//        }
//
//        public static void WriteToDelimitedFile<T>(string filePath, IEnumerable<T> toWrite, bool append, bool lineFeed) where T : new()
//        {
//            WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, lineFeed).Wait();
//        }
//
//        public static void WriteToDelimitedFile<T>(FileInfo file, IEnumerable<T> toWrite, bool append, bool lineFeed) where T : new()
//        {
//            WriteToDelimitedFileAsync(file, toWrite, append, lineFeed).Wait();
//        }
//
//        public static Task WriteToDelimitedFileAsync<T>(string filePath, IEnumerable<T> toWrite, bool append, bool lineFeed) where T : new()
//        {
//            return WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, lineFeed);
//        }
//
//        #endregion
//        public static async Task WriteToDelimitedFileAsync<T>(FileInfo file, IEnumerable<T> toWrite, bool append, bool lineFeed) where T : new()
//        {
//            var newLineType = !lineFeed ? "\r\n" : "\n";
//            var mereTable = MereUtils.CacheCheckFile<T>();
//            var delimiter = mereTable.Delimiter ?? ",";
//            var delimitedFields = mereTable.DelimitedFields.ToList();
//
//            if (delimitedFields.Count <= 0)
//                delimitedFields = mereTable.PropertyHelpersIndexed.Values.ToList();
//
//            var linesToWrite = new List<string>();
//            linesToWrite.Add(string.Join(delimiter, delimitedFields.Select(x => x.ColumnName)));
//            foreach (var val in toWrite)
//            {
//                var curLine = new List<string>();
//                foreach (var field in delimitedFields)
//                {
//
//                    var objVal = field.Get(val);
//                    var v = MereFileFieldParser.ParseFieldForWrite(field, objVal, true);
//                    curLine.Add(v);
//                }
//                linesToWrite.Add(string.Join(delimiter, curLine));
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
//        }
//        #endregion
//    }
//
//    public class MereDelimitedFile<T>
//    {
//        private MereDelimitedFile()
//        {
//
//        }
//    }
//}
