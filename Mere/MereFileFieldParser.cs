//using System;
//using System.Globalization;
//using System.Linq;
//using System.Text.RegularExpressions;
//
//namespace Mere
//{
//
//    public enum MereFileFieldOption
//    {
//        TrailingNegative,
//        Ebcidic,
//        AlignLeft,
//        RemoveAllSpaces,
//        TrimStartSpaces,
//        TrimEndSpaces,
//        Trim,
//        RemoveDoubleSpaces,
//        RemovedDecimal1,
//        RemovedDecimal2,
//        RemovedDecimal3,
//        RemovedDecimal4,
//        RemovedDecimal5,
//        RemovedDecimal6,
//        DecimalPrecision1,
//        DecimalPrecision2,
//        DecimalPrecision3,
//        DecimalPrecision4,
//        DecimalPrecision5,
//        DecimalPrecision6
//    }
//
//    public static class MereFileFieldParser
//    {
//        public static string Parse(MereColumn field, string value, bool delimited)
//        {
//            var toReturn = value.Trim();
//
//            var options = delimited ? field.DelimitedFieldAttr.ParsingOptions : field.FlatFileFieldAttr.FieldOptions;
//
//            if (options != null)
//            {
//                var l = string.IsNullOrEmpty(toReturn) ? ' ' : toReturn.Last();
//
//                if (options.Contains(MereFileFieldOption.Ebcidic))
//                {
//                    var pos = new[] { '{', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', ' ' };
//                    var neg = new[] { '}', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', ' ' };
//                    if (neg.Contains(l) || pos.Contains(l))
//                    {
//                        var isNeg = neg.Contains(l);
//                        var i = isNeg ? Array.IndexOf(neg, l) : Array.IndexOf(pos, l);
//
//                        if (isNeg)
//                            toReturn = "-" + toReturn;
//
//                        toReturn = string.Join("", toReturn.Take(toReturn.Length - 1)) + i;
//                    }
//                }
//
//                foreach (var option in options)
//                {
//                    int precision;
//                    switch (option)
//                    {
//                        case MereFileFieldOption.TrailingNegative:
//
//                            if (l == '-')
//                            {
//                                return "-" + toReturn.TrimEnd('-');
//                            }
//                            break;
//                        //case MereFileFieldOption.RemoveAllSpaces:
//                            //var regex = new Regex("\s/g");
//                        //case MereFileFieldOption.TrimStartSpaces:
//
//                        //case MereFileFieldOption.TrimEndSpaces:
//
//                        //case MereFileFieldOption.Trim:
//
//                        //case MereFileFieldOption.RemoveDoubleSpaces:
//
//                        case MereFileFieldOption.RemovedDecimal1:
//                            precision = 1;
//                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
//                            break;
//                        case MereFileFieldOption.RemovedDecimal2:
//                            precision = 2;
//                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
//                            break;
//                        case MereFileFieldOption.RemovedDecimal3:
//                            precision = 3;
//                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
//                            break;
//                        case MereFileFieldOption.RemovedDecimal4:
//                            precision = 4;
//                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
//                            break;
//                        case MereFileFieldOption.RemovedDecimal5:
//                            precision = 5;
//                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
//                            break;
//                        case MereFileFieldOption.RemovedDecimal6:
//                            precision = 6;
//                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
//                            break;
//                        //don't that these are needed here just for writing maybe round?
//                        //case MereFileFieldOption.DecimalPrecision1:
//                        //    precision = 1;
//                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
//                        //    break;
//                        //case MereFileFieldOption.DecimalPrecision2:
//                        //    precision = 2;
//                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
//                        //    break;
//                        //case MereFileFieldOption.DecimalPrecision3:
//                        //    precision = 3;
//                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
//                        //    break;
//                        //case MereFileFieldOption.DecimalPrecision4:
//                        //    precision = 4;
//                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
//                        //    break;
//                        //case MereFileFieldOption.DecimalPrecision5:
//                        //    precision = 5;
//                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
//                        //    break;
//                        //case MereFileFieldOption.DecimalPrecision6:
//                        //    precision = 6;
//                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
//                        //    break;
//                        //end don't that these are needed here just for writing maybe round?
//                    }
//                }//end foreach option
//            }
//
//
//
//            var toStringFormat = delimited ? field.DelimitedFieldAttr.ToStringFormat : field.FlatFileFieldAttr.ToStringFormat;
//            if (!string.IsNullOrEmpty(toReturn) && toStringFormat != null)
//            {
//
//                if (field.PropertyDescriptor.PropertyType == typeof(DateTime) || field.PropertyDescriptor.PropertyType == typeof(DateTime?))
//                {
//                    DateTime d;
//                    if (DateTime.TryParseExact(toReturn, toStringFormat, new CultureInfo("en-US"), DateTimeStyles.None, out d)) // field.Get(value);
//                        toReturn = d.ToString();
//                }
//            }
//
//            return toReturn;
//        }
//
//        public static string ParseFieldForWrite(MereColumn field, object value, bool delimited)
//        {
//            string v;
//            var toStringFormat = delimited ? field.DelimitedFieldAttr != null ? field.DelimitedFieldAttr.ToStringFormat : null : field.FlatFileFieldAttr != null ? field.FlatFileFieldAttr.ToStringFormat : null;
//            if (toStringFormat != null)
//            {
//                if (field.PropertyDescriptor.PropertyType == typeof(DateTime))
//                {
//                    var d = (DateTime)value; // field.Get(value);
//                    v = d.ToString(toStringFormat);
//                }
//                else if (field.PropertyDescriptor.PropertyType == typeof(DateTime?))
//                {
//                    var o = value; // field.Get(value);
//                    if (o != null)
//                    {
//                        var d = (DateTime)o;
//                        v = d.ToString(toStringFormat);
//                    }
//                    else
//                    {
//                        v = "";
//                    }
//
//                }
//                else if (field.PropertyDescriptor.PropertyType == typeof(int))
//                {
//                    var d = (int)field.Get(value);
//                    v = d.ToString(toStringFormat);
//                }
//                else if (field.PropertyDescriptor.PropertyType == typeof(int?))
//                {
//                    var o = value; // field.Get(value);
//                    if (o != null)
//                    {
//                        var d = (int)o;
//                        v = d.ToString(toStringFormat);
//                    }
//                    else
//                    {
//                        v = "";
//                    }
//
//                }
//                else if (field.PropertyDescriptor.PropertyType == typeof(decimal))
//                {
//                    var d = (decimal)value;
//                    v = d.ToString(toStringFormat);
//                }
//                else if (field.PropertyDescriptor.PropertyType == typeof(decimal?))
//                {
//                    var o = value; //field.Get(value);
//                    if (o != null)
//                    {
//                        var d = (decimal)o;
//                        v = d.ToString(toStringFormat);
//                    }
//                    else
//                    {
//                        v = "";
//                    }
//
//                }
//                else
//                {
//                    //var o = field.Get(value);
//                    v = value == null ? "" : value.ToString();
//                }
//            }
//            else
//            {
//                v = value == null ? "" : value.ToString();
//            }
//
//            var options = delimited ? field.DelimitedFieldAttr == null ? null : field.DelimitedFieldAttr.ParsingOptions : field.FlatFileFieldAttr == null ? null : field.FlatFileFieldAttr.FieldOptions;
//
//            if (options != null && options.Any())
//            {
//                //decimal precision
//                if (options.Contains(MereFileFieldOption.DecimalPrecision1))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".0");
//                if (options.Contains(MereFileFieldOption.DecimalPrecision2))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".00");
//                if (options.Contains(MereFileFieldOption.DecimalPrecision3))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".000");
//                if (options.Contains(MereFileFieldOption.DecimalPrecision4))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".0000");
//                if (options.Contains(MereFileFieldOption.DecimalPrecision5))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".00000");
//                if (options.Contains(MereFileFieldOption.DecimalPrecision6))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".000000");
//                //end decimal precision
//
//                //removed decimal
//                if (options.Contains(MereFileFieldOption.RemovedDecimal1))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".0").Replace(".", "");
//                if (options.Contains(MereFileFieldOption.RemovedDecimal2))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".00").Replace(".", "");
//                if (options.Contains(MereFileFieldOption.RemovedDecimal3))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".000").Replace(".", "");
//                if (options.Contains(MereFileFieldOption.RemovedDecimal4))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".0000").Replace(".", "");
//                if (options.Contains(MereFileFieldOption.RemovedDecimal5))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".00000").Replace(".", "");
//                if (options.Contains(MereFileFieldOption.RemovedDecimal6))
//                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".000000").Replace(".", "");
//                //end removed decimal
//
//                //check Ebcidic
//                if (options.Contains(MereFileFieldOption.Ebcidic))
//                {
//                    var pos = new[] { '{', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', ' ' };
//                    var neg = new[] { '}', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', ' ' };
//
//                    v = v.Trim();
//                    var l = string.IsNullOrEmpty(v) ? 0 : int.Parse(v.Last().ToString());
//                    l = l < 10 ? l : 10;
//                    var isNeg = v.StartsWith("-");
//                    var c = isNeg ? neg[l] : pos[l];
//                    v = string.Join("", v.Take(v.Length - 1)).TrimStart('-') + c;
//                }
//                //end check Ebcidic
//
//                //check trailing neg
//                if (options.Contains(MereFileFieldOption.TrailingNegative))
//                {
//                    v = v.Trim();
//                    if (v.StartsWith("-"))
//                        v = v.TrimStart('-') + "-";
//                }
//                //end check trailing neg
//
//                if (!delimited)
//                {
//                    //pad to width
//                    if (options.Contains(MereFileFieldOption.AlignLeft))
//                        v = v.PadRight(field.FlatFileFieldAttr.Length, field.FlatFileFieldAttr.PadChar.First());
//                    else
//                        v = v.PadLeft(field.FlatFileFieldAttr.Length, field.FlatFileFieldAttr.PadChar.First());
//                    //end pad to width
//                }
//            }
//            else if (!delimited && field.FlatFileFieldAttr != null && !string.IsNullOrEmpty(field.FlatFileFieldAttr.PadChar))
//            {
//                v = v.PadLeft(field.FlatFileFieldAttr.Length, field.FlatFileFieldAttr.PadChar.First());
//            }
//
//            if (!delimited)
//            {
//                //check length
//                if (field.FlatFileFieldAttr != null && v.Length > field.FlatFileFieldAttr.Length)
//                    v = string.Join("", v.Take(field.FlatFileFieldAttr.Length));
//                //end check length
//            }
//
//            return v;
//        }
//    }
//}
