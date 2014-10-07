using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mere;
using Mere.Attributes;
using Mere.Files.Attributes;


namespace Mere.Files
{
    public partial class MereFileUtils
    {
        public static ConcurrentDictionary<RuntimeTypeHandle, MereFile> MereFileCache = new ConcurrentDictionary<RuntimeTypeHandle, MereFile>();

        public static List<Type> WritableTypes = new List<Type>
        {
            typeof(DateTime),
            typeof(DateTime?),
            typeof(int),
            typeof(int?),
            typeof(decimal),
            typeof(decimal?),
            typeof(long),
            typeof(long?),
            typeof(double),
            typeof(double?),
            typeof(string)
        };

        public static bool IsWritable(Type type)
        {
            return WritableTypes.Contains(type);
        }

        public static void WriteLinesToFile(string filePath, IEnumerable<string> lines, bool append = false, string lineDelimiter = "\r\n")
        {
            using (var fs = new FileStream(filePath, (append ? FileMode.Append : FileMode.OpenOrCreate), FileAccess.Write))
            using (var swrt = new StreamWriter(fs))
            {
                if (fs.Length > 0)
                    swrt.Write(lineDelimiter);
                swrt.Write(string.Join(lineDelimiter, lines));
            }
        }

        public static MereFile<T> CacheCheckSubRec<T>(MereFileField fileFieldForSubRecord) where T : new()
        {
            var f = CacheCheckFile<T>();
            f.MereFileFieldForSubRecord = fileFieldForSubRecord;
            return f;
        }

        /// <summary>
        /// Check cache for MereFile object for T if one does not exist then creates, adds to cache, and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MereFile<T> CacheCheckFile<T>() where T : new()
        {
            var type = typeof(T);

            MereFile foundMereFile;
            if (MereFileCache.TryGetValue(type.TypeHandle, out foundMereFile))
                return (MereFile<T>)foundMereFile;

            var mereTable = MereUtils.CacheCheck<T>();

            var properties = TypeDescriptor.GetProperties(type).PropertyDescriptorCollectionToList();
            
            var fields = properties.Where(
                    w => (type.GetProperty(w.Name).GetCustomAttributes(typeof(MereFlatFileFieldAttribute), true).Any()
                            || type.GetProperty(w.Name).GetCustomAttributes(typeof(MereDelimitedFileFieldAttribute), true).Any()))
                          .Select(
                              s => new
                              {
                                  FlatFileFieldAttributes =
                              type.GetProperty(s.Name)
                                  .GetCustomAttributes(typeof(MereFlatFileFieldAttribute), true)
                                  .Select(x => (MereFlatFileFieldAttribute)x),
                                  DelimitedFieldAttributes =
                              type.GetProperty(s.Name)
                                  .GetCustomAttributes(typeof(MereDelimitedFileFieldAttribute), true)
                                  .Select(x => (MereDelimitedFileFieldAttribute)x),
                                  PropertyDescriptor = s
                              }).ToList();

            var mereFlatFileFields = new List<MereFileField>();
            var mereDelimitedFileFields = new List<MereFileField>();

            var mereFlatFileFieldRecords = new List<MereFile>();
            var mereDelimitedFileFieldRecords = new List<MereFile>();

            foreach (var field in fields)
            {
                var mereColumn = mereTable.AllMereColumns.GetMereColumnByPropertyName(field.PropertyDescriptor.Name);

                if (IsWritable(mereColumn.PropertyDescriptor.PropertyType))
                {
                    mereDelimitedFileFields.AddRange(
                        field.DelimitedFieldAttributes.Select(fieldAttr => GetFileField(mereColumn, fieldAttr)
                    ));

                    mereFlatFileFields.AddRange(
                        field.FlatFileFieldAttributes.Select(fieldAttr => GetFileField(mereColumn, fieldAttr)
                    ));
                }
                else
                {
                    //check if implements IEnumerable 
                    var isEnum = typeof(IEnumerable).IsAssignableFrom(field.PropertyDescriptor.PropertyType);
                    var found = field.DelimitedFieldAttributes.Select(fieldAttr =>
                    {
                        var enType = isEnum
                            ? field.PropertyDescriptor.PropertyType.GetGenericArguments()[0]
                            : field.PropertyDescriptor.PropertyType;

                        var meth = typeof (MereFileUtils).GetMethod("CacheCheckSubRec")
                            .MakeGenericMethod(new[] {enType});

                        var subMereFile = (MereFile) meth.Invoke(null, new[] {GetFileField(mereColumn, fieldAttr)});
                        return subMereFile;
                    }).ToList();

                    if (field.DelimitedFieldAttributes.Any())
                    {
                        mereDelimitedFileFieldRecords.AddRange(found);
                    }

                    if (field.FlatFileFieldAttributes.Any())
                    {
                        mereFlatFileFieldRecords.AddRange(found);
                    }
                }

                
            }

            var fileTableAttrs = new
                              {
                                  DelimitedFileTableTypeFormatAttrs =
                                      type.GetCustomAttributes(typeof(MereDelimitedFileTableTypeFormatAttribute), true)
                                          .Select(x => (MereFileTableTypeFormatAttribute)x),
                                  DelimitedFileTableAttr =
                                      (MereFileTableAttribute)type.GetCustomAttributes(typeof(MereDelimitedFileTableAttribute), true)
                                          .FirstOrDefault(),
                                  FlatFileTableAttr =
                                      (MereFileTableAttribute)type.GetCustomAttributes(typeof(MereFlatFileTableAttribute), true)
                                          .FirstOrDefault(),
                                  DelimitedFileTableTypeParsingAttrs =
                                      type.GetCustomAttributes(typeof(MereDelimitedFileTableTypeParsingAttribute), true)
                                          .Select(x => (MereFileTableTypeParsingAttribute)x),
                                  FlatFileTableTypeFormatAttrs =
                                      type.GetCustomAttributes(typeof(MereFlatFileTableTypeFormatAttribute), true)
                                          .Select(x => (MereFileTableTypeFormatAttribute)x),
                                  FlatFileTableTypeParsingAttrs =
                                      type.GetCustomAttributes(typeof(MereFlatFileTableTypeParsingAttribute), true)
                                          .Select(x => (MereFileTableTypeParsingAttribute)x),
                              };

            var mereFile = new MereFile<T>
            {
                FlatFileSubRecords = mereFlatFileFieldRecords,
                DelimitedSubRecords = mereDelimitedFileFieldRecords,
                DelimitedFileTableAttr = fileTableAttrs.DelimitedFileTableAttr,
                FlatFileTableAttr = fileTableAttrs.FlatFileTableAttr,
                DelimitedFields = mereDelimitedFileFields.ToList(),
                FlatFileFields = mereFlatFileFields.ToList(),
                DelimitedFileTableTypeFormats = fileTableAttrs.DelimitedFileTableTypeFormatAttrs.ToDictionary(x => x.TypeToFormat, x => x.ToStringFormat),
                FlatFileTableTypeFormats = fileTableAttrs.FlatFileTableTypeFormatAttrs.ToDictionary(x => x.TypeToFormat, x => x.ToStringFormat),
                DelimitedFileTableTypeParsingOptions = fileTableAttrs.DelimitedFileTableTypeParsingAttrs.Where(x => x.TypeToFormat != null).ToDictionary(x => x.TypeToFormat, x => x.FileFieldParsingOptions),
                FlatFileTableTypeParsingOptions = fileTableAttrs.FlatFileTableTypeParsingAttrs.Where(x => x.TypeToFormat != null).ToDictionary(x => x.TypeToFormat, x => x.FileFieldParsingOptions),
                DelimitedFileTableParsingOptions = fileTableAttrs.DelimitedFileTableTypeParsingAttrs.Where(x => x.TypeToFormat == null).SelectMany(x => x.FileFieldParsingOptions).ToList(),
                FlatFileTableParsingOptions = fileTableAttrs.FlatFileTableTypeParsingAttrs.Where(x => x.TypeToFormat == null).SelectMany(x => x.FileFieldParsingOptions).ToList()
            };

            MereFileCache.TryAdd(type.TypeHandle, mereFile);

            return mereFile;

        }

        public static MereFileField GetFileField(MereColumn mereColumn, IMereFileFieldAttribute fileFieldAttr)
        {
            return new MereFileField(mereColumn, fileFieldAttr.Delimiter, fileFieldAttr.Index, fileFieldAttr.RecordKey,
                fileFieldAttr.ToStringFormat, fileFieldAttr.PadChar, fileFieldAttr.FileFieldParsingOptions,
                fileFieldAttr.FieldLength);
        }
    }

}
