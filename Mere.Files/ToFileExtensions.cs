using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files
{
    public static class MereToFileExtensions
    {
        public static void WriteLinesToFile(this IEnumerable<string> lines, string filePath, bool append = false, string lineDelimiter = "\r\n")
        {
            MereFileUtils.WriteLinesToFile(filePath, lines, append, lineDelimiter);
        }

        public static void WriteToDelimitedFile<T>(this IEnumerable<T> toWrite, string filePath, bool append) where T : new()
        {
            MereFile.WriteToDelimitedFile(toWrite, filePath, append);
        }

        public static void WriteToFlatFile<T>(this IEnumerable<T> toWrite, string filePath) where T : new()
        {
            MereFile.WriteToFlatFile(toWrite, filePath);
        }
        public static void WriteToFlatFile<T>(this IEnumerable<T> toWrite, string filePath, bool append) where T : new()
        {
            MereFile.WriteToFlatFile(toWrite, filePath, append);
        }
    }
}

//        #region ToDelimitedFile
//        #region Single
//        //public static void ToDelimitedFile<T>(this T toWrite, string filePath)
//        //{
//        //    MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, false, false).Wait();
//        //}
//
//        //public static void ToDelimitedFile<T>(this T toWrite, FileInfo file)
//        //{
//        //    MereDelimitedFile.WriteToDelimitedFileAsync(file, new[] { toWrite }, false, false).Wait();
//        //}
//
//        //public static Task ToDelimitedFileAsync<T>(this T toWrite, string filePath)
//        //{
//        //    return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, false, false);
//        //}
//
//        //public static Task ToDelimitedFileAsync<T>(this T toWrite, FileInfo file)
//        //{
//        //    return MereDelimitedFile.WriteToDelimitedFileAsync(file, new[] { toWrite }, false, false);
//        //}
//
//        //public static void ToDelimitedFile<T>(this T toWrite, string filePath, bool append)
//        //{
//        //    MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, append, false).Wait();
//        //}
//
//        //public static void ToDelimitedFile<T>(this T toWrite, FileInfo file, bool append)
//        //{
//        //    MereDelimitedFile.WriteToDelimitedFileAsync(file, new[] { toWrite }, append, false).Wait();
//        //}
//
//        //public static Task ToDelimitedFileAsync<T>(this T toWrite, string filePath, bool append)
//        //{
//        //    return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, append, false);
//        //}
//
//        //public static Task ToDelimitedFileAsync<T>(this T toWrite, FileInfo file, bool append)
//        //{
//        //    return MereDelimitedFile.WriteToDelimitedFileAsync(file, new[] { toWrite }, append, false);
//        //}
//
//        //public static void ToDelimitedFile<T>(this T toWrite, string filePath, bool append, bool lineFeed)
//        //{
//        //    MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, append, lineFeed).Wait();
//        //}
//
//        //public static void ToDelimitedFile<T>(this T toWrite, FileInfo file, bool append, bool lineFeed)
//        //{
//        //    MereDelimitedFile.WriteToDelimitedFileAsync(file, new[] { toWrite }, append, lineFeed).Wait();
//        //}
//
//        //public static Task ToDelimitedFileAsync<T>(this T toWrite, string filePath, bool append, bool lineFeed)
//        //{
//        //    return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), new[] { toWrite }, append, lineFeed);
//        //}
//
//        //public static Task ToDelimitedFileAsync<T>(this T toWrite, FileInfo file, bool append,
//        //                                                 bool lineFeed)
//        //{
//        //    return MereDelimitedFile.WriteToDelimitedFileAsync(file, new[] { toWrite }, append, lineFeed);
//        //}
//        #endregion
//
//        public static void ToDelimitedFile<T, TClass>(this T toWrite, string filePath)
//            where T : IEnumerable<TClass>, new()
//            where TClass : class, new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, false, false).Wait();
//        }
//
//        public static void ToDelimitedFile<T>(this IEnumerable<T> toWrite, FileInfo file) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, false, false).Wait();
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IEnumerable<T> toWrite, string filePath) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, false, false);
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IEnumerable<T> toWrite, FileInfo file) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, false, false);
//        }
//
//        public static void ToDelimitedFile<T>(this IEnumerable<T> toWrite, string filePath, bool append) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, false).Wait();
//        }
//
//        public static void ToDelimitedFile<T>(this IEnumerable<T> toWrite, FileInfo file, bool append) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, append, false).Wait();
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IEnumerable<T> toWrite, string filePath, bool append) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, false);
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IEnumerable<T> toWrite, FileInfo file, bool append) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, append, false);
//        }
//
//        public static void ToDelimitedFile<T>(this IEnumerable<T> toWrite, string filePath, bool append, bool lineFeed) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, lineFeed).Wait();
//        }
//
//        public static void ToDelimitedFile<T>(this IEnumerable<T> toWrite, FileInfo file, bool append, bool lineFeed) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, append, lineFeed).Wait();
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IEnumerable<T> toWrite, string filePath, bool append, bool lineFeed) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, lineFeed);
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IEnumerable<T> toWrite, FileInfo file, bool append,
//                                                                  bool lineFeed) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, append, lineFeed);
//        }
//
//        #region IList
//        public static void ToDelimitedFile<T>(this IList<T> toWrite, string filePath) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, false, false).Wait();
//        }
//
//        public static void ToDelimitedFile<T>(this IList<T> toWrite, FileInfo file) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, false, false).Wait();
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IList<T> toWrite, string filePath) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, false, false);
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IList<T> toWrite, FileInfo file) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, false, false);
//        }
//
//        public static void ToDelimitedFile<T>(this IList<T> toWrite, string filePath, bool append) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, false).Wait();
//        }
//
//        public static void ToDelimitedFile<T>(this IList<T> toWrite, FileInfo file, bool append) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, append, false).Wait();
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IList<T> toWrite, string filePath, bool append) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, false);
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IList<T> toWrite, FileInfo file, bool append) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, append, false);
//        }
//
//        public static void ToDelimitedFile<T>(this IList<T> toWrite, string filePath, bool append, bool lineFeed) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, lineFeed).Wait();
//        }
//
//        public static void ToDelimitedFile<T>(this IList<T> toWrite, FileInfo file, bool append, bool lineFeed) where T : new()
//        {
//            MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, append, lineFeed).Wait();
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IList<T> toWrite, string filePath, bool append, bool lineFeed) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(new FileInfo(filePath), toWrite, append, lineFeed);
//        }
//
//        public static Task ToDelimitedFileAsync<T>(this IList<T> toWrite, FileInfo file, bool append,
//                                                                  bool lineFeed) where T : new()
//        {
//            return MereDelimitedFile.WriteToDelimitedFileAsync(file, toWrite, append, lineFeed);
//        }
//        #endregion
//        #endregion
//        #region ToFlatFile
//        #region Single
//        //public static void ToFlatFile<T>(this T toWrite, string filePath)
//        //{
//        //    MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, false, false).Wait();
//        //}
//
//        //public static void ToFlatFile<T>(this T toWrite, FileInfo file)
//        //{
//        //    MereFlatFile.WriteToFlatFileAsync(file, new[] { toWrite }, false, false).Wait();
//        //}
//
//        //public static Task ToFlatFileAsync<T>(this T toWrite, string filePath)
//        //{
//        //    return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, false, false);
//        //}
//
//        //public static Task ToFlatFileAsync<T>(this T toWrite, FileInfo file)
//        //{
//        //    return MereFlatFile.WriteToFlatFileAsync(file, new[] { toWrite }, false, false);
//        //}
//
//        //public static void ToFlatFile<T>(this T toWrite, string filePath, bool append)
//        //{
//        //    MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, append, false).Wait();
//        //}
//
//        //public static void ToFlatFile<T>(this T toWrite, FileInfo file, bool append)
//        //{
//        //    MereFlatFile.WriteToFlatFileAsync(file, new[] { toWrite }, append, false).Wait();
//        //}
//
//        //public static Task ToFlatFileAsync<T>(this T toWrite, string filePath, bool append)
//        //{
//        //    return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, append, false);
//        //}
//
//        //public static Task ToFlatFileAsync<T>(this T toWrite, FileInfo file, bool append)
//        //{
//        //    return MereFlatFile.WriteToFlatFileAsync(file, new[] { toWrite }, append, false);
//        //}
//
//        //public static void ToFlatFile<T>(this T toWrite, string filePath, bool append, bool lineFeed)
//        //{
//        //    MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, append, lineFeed).Wait();
//        //}
//
//        //public static void ToFlatFile<T>(this T toWrite, FileInfo file, bool append, bool lineFeed)
//        //{
//        //    MereFlatFile.WriteToFlatFileAsync(file, new[] { toWrite }, append, lineFeed).Wait();
//        //}
//
//        //public static Task ToFlatFileAsync<T>(this T toWrite, string filePath, bool append, bool lineFeed)
//        //{
//        //    return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), new[] { toWrite }, append, lineFeed);
//        //}
//
//        //public static Task ToFlatFileAsync<T>(this T toWrite, FileInfo file, bool append,
//        //                                                 bool lineFeed)
//        //{
//        //    return MereFlatFile.WriteToFlatFileAsync(file, new[] { toWrite }, append, lineFeed);
//        //}
//        #endregion
//
//        public static void ToFlatFile<T>(this IEnumerable<T> toWrite, string filePath) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, false, false).Wait();
//        }
//
//        public static void ToFlatFile<T>(this IEnumerable<T> toWrite, FileInfo file) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(file, toWrite, false, false).Wait();
//        }
//
//        public static Task ToFlatFileAsync<T>(this IEnumerable<T> toWrite, string filePath) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, false, false);
//        }
//
//        public static Task ToFlatFileAsync<T>(this IEnumerable<T> toWrite, FileInfo file) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(file, toWrite, false, false);
//        }
//
//        public static void ToFlatFile<T>(this IEnumerable<T> toWrite, string filePath, bool append) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, false).Wait();
//        }
//
//        public static void ToFlatFile<T>(this IEnumerable<T> toWrite, FileInfo file, bool append) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(file, toWrite, append, false).Wait();
//        }
//
//        public static Task ToFlatFileAsync<T>(this IEnumerable<T> toWrite, string filePath, bool append) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, false);
//        }
//
//        public static Task ToFlatFileAsync<T>(this IEnumerable<T> toWrite, FileInfo file, bool append) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(file, toWrite, append, false);
//        }
//
//        public static void ToFlatFile<T>(this IEnumerable<T> toWrite, string filePath, bool append, bool lineFeed) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, lineFeed).Wait();
//        }
//
//        public static void ToFlatFile<T>(this IEnumerable<T> toWrite, FileInfo file, bool append, bool lineFeed) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(file, toWrite, append, lineFeed).Wait();
//        }
//
//        public static Task ToFlatFileAsync<T>(this IEnumerable<T> toWrite, string filePath, bool append, bool lineFeed) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, lineFeed);
//        }
//
//        public static Task ToFlatFileAsync<T>(this IEnumerable<T> toWrite, FileInfo file, bool append,
//                                                                  bool lineFeed) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(file, toWrite, append, lineFeed);
//        }
//
//        #region IList
//        public static void ToFlatFile<T>(this IList<T> toWrite, string filePath) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, false, false).Wait();
//        }
//
//        public static void ToFlatFile<T>(this IList<T> toWrite, FileInfo file) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(file, toWrite, false, false).Wait();
//        }
//
//        public static Task ToFlatFileAsync<T>(this IList<T> toWrite, string filePath) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, false, false);
//        }
//
//        public static Task ToFlatFileAsync<T>(this IList<T> toWrite, FileInfo file) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(file, toWrite, false, false);
//        }
//
//        public static void ToFlatFile<T>(this IList<T> toWrite, string filePath, bool append) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, false).Wait();
//        }
//
//        public static void ToFlatFile<T>(this IList<T> toWrite, FileInfo file, bool append) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(file, toWrite, append, false).Wait();
//        }
//
//        public static Task ToFlatFileAsync<T>(this IList<T> toWrite, string filePath, bool append) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, false);
//        }
//
//        public static Task ToFlatFileAsync<T>(this IList<T> toWrite, FileInfo file, bool append) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(file, toWrite, append, false);
//        }
//
//        public static void ToFlatFile<T>(this IList<T> toWrite, string filePath, bool append, bool lineFeed) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, lineFeed).Wait();
//        }
//
//        public static void ToFlatFile<T>(this IList<T> toWrite, FileInfo file, bool append, bool lineFeed) where T : new()
//        {
//            MereFlatFile.WriteToFlatFileAsync(file, toWrite, append, lineFeed).Wait();
//        }
//
//        public static Task ToFlatFileAsync<T>(this IList<T> toWrite, string filePath, bool append, bool lineFeed) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(new FileInfo(filePath), toWrite, append, lineFeed);
//        }
//
//        public static Task ToFlatFileAsync<T>(this IList<T> toWrite, FileInfo file, bool append,
//                                                                  bool lineFeed) where T : new()
//        {
//            return MereFlatFile.WriteToFlatFileAsync(file, toWrite, append, lineFeed);
//        }
//        #endregion
//        #endregion
//    }
//}
