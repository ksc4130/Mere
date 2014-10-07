using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Mere.Attributes;

namespace Mere.Files
{

    public static class MereFileFieldParser
    {
        public static string Parse(MereFileField field, string value, bool delimited)
        {
            var toReturn = value.Trim();
            var options = field.FileFieldParsingOptions;

            if (options != null)
            {
                var l = string.IsNullOrEmpty(toReturn) ? ' ' : toReturn.Last();

                if (options.Contains(MereFileParsingOption.Ebcidic))
                {
                    var pos = new[] { '{', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', ' ' };
                    var neg = new[] { '}', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', ' ' };
                    if (neg.Contains(l) || pos.Contains(l))
                    {
                        var isNeg = neg.Contains(l);
                        var i = isNeg ? Array.IndexOf(neg, l) : Array.IndexOf(pos, l);

                        if (isNeg)
                            toReturn = "-" + toReturn;

                        toReturn = string.Join("", toReturn.Take(toReturn.Length - 1)) + i;
                    }
                }

                foreach (var option in options)
                {
                    int precision;
                    switch (option)
                    {
                        case MereFileParsingOption.TrailingNegative:

                            if (l == '-')
                            {
                                return "-" + toReturn.TrimEnd('-');
                            }
                            break;
                        //case MereFileParsingOption.RemoveAllSpaces:
                        //var regex = new Regex("\s/g");
                        //case MereFileParsingOption.TrimStartSpaces:

                        //case MereFileParsingOption.TrimEndSpaces:

                        //case MereFileParsingOption.Trim:

                        //case MereFileParsingOption.RemoveDoubleSpaces:

                        case MereFileParsingOption.RemovedDecimal1:
                            precision = 1;
                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
                            break;
                        case MereFileParsingOption.RemovedDecimal2:
                            precision = 2;
                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
                            break;
                        case MereFileParsingOption.RemovedDecimal3:
                            precision = 3;
                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
                            break;
                        case MereFileParsingOption.RemovedDecimal4:
                            precision = 4;
                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
                            break;
                        case MereFileParsingOption.RemovedDecimal5:
                            precision = 5;
                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
                            break;
                        case MereFileParsingOption.RemovedDecimal6:
                            precision = 6;
                            toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(toReturn.Length - precision));
                            break;
                        //don't that these are needed here just for writing maybe round?
                        //case MereFileParsingOption.DecimalPrecision1:
                        //    precision = 1;
                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
                        //    break;
                        //case MereFileParsingOption.DecimalPrecision2:
                        //    precision = 2;
                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
                        //    break;
                        //case MereFileParsingOption.DecimalPrecision3:
                        //    precision = 3;
                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
                        //    break;
                        //case MereFileParsingOption.DecimalPrecision4:
                        //    precision = 4;
                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
                        //    break;
                        //case MereFileParsingOption.DecimalPrecision5:
                        //    precision = 5;
                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
                        //    break;
                        //case MereFileParsingOption.DecimalPrecision6:
                        //    precision = 6;
                        //    toReturn = string.Join("", toReturn.Take(toReturn.Length - precision)) + "." + string.Join("", toReturn.Skip(precision));
                        //    break;
                        //end don't that these are needed here just for writing maybe round?
                    }
                }//end foreach option
            }



            var toStringFormat = field.ToStringFormat;
            if (!string.IsNullOrEmpty(toReturn) && toStringFormat != null)
            {

                if (field.MereColumnForField.PropertyDescriptor.PropertyType == typeof(DateTime) || field.MereColumnForField.PropertyDescriptor.PropertyType == typeof(DateTime?))
                {
                    DateTime d;
                    if (DateTime.TryParseExact(toReturn, toStringFormat, new CultureInfo("en-US"), DateTimeStyles.None, out d)) // field.Get(value);
                        toReturn = d.ToString();
                }
            }

            if (options != null && options.Contains(MereFileParsingOption.WrapWithDoubleQuotes))
            {
                toReturn = "\"" + toReturn + "\"";
            }

            return toReturn;
        }

        public static string ParseFieldForWrite(MereColumn mereColumn, string toStringFormat, List<MereFileParsingOption> options, int fieldLength, string padChar, object value, bool fixedWidth)
        {
            string v;
            if (!string.IsNullOrWhiteSpace(toStringFormat))
            {
                if (mereColumn.PropertyDescriptor.PropertyType == typeof(DateTime))
                {
                    var d = (DateTime)value; // field.Get(value);
                    v = d.ToString(toStringFormat);
                }
                else if (mereColumn.PropertyDescriptor.PropertyType == typeof(DateTime?))
                {
                    var o = value; // field.Get(value);
                    if (o != null)
                    {
                        var d = (DateTime)o;
                        v = d.ToString(toStringFormat);
                    }
                    else
                    {
                        v = "";
                    }

                }
                else if (mereColumn.PropertyDescriptor.PropertyType == typeof(int))
                {
                    var d = (int)value;
                    v = d.ToString(toStringFormat);
                }
                else if (mereColumn.PropertyDescriptor.PropertyType == typeof(int?))
                {
                    var o = value; // field.Get(value);
                    if (o != null)
                    {
                        var d = (int)o;
                        v = d.ToString(toStringFormat);
                    }
                    else
                    {
                        v = "";
                    }

                }
                else if (mereColumn.PropertyDescriptor.PropertyType == typeof(decimal))
                {
                    var d = (decimal)value;
                    v = d.ToString(toStringFormat);
                }
                else if (mereColumn.PropertyDescriptor.PropertyType == typeof(decimal?))
                {
                    var o = value; //field.Get(value);
                    if (o != null)
                    {
                        var d = (decimal)o;
                        v = d.ToString(toStringFormat);
                    }
                    else
                    {
                        v = "";
                    }

                }
                else
                {
                    //var o = field.Get(value);
                    v = value == null ? "" : value.ToString();
                }
            }
            else
            {
                v = value == null ? "" : value.ToString();
            }

            if (options != null && options.Any())
            {
                //decimal precision
                if (options.Contains(MereFileParsingOption.DecimalPrecision1))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".0");
                if (options.Contains(MereFileParsingOption.DecimalPrecision2))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".00");
                if (options.Contains(MereFileParsingOption.DecimalPrecision3))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".000");
                if (options.Contains(MereFileParsingOption.DecimalPrecision4))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".0000");
                if (options.Contains(MereFileParsingOption.DecimalPrecision5))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".00000");
                if (options.Contains(MereFileParsingOption.DecimalPrecision6))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".000000");
                //end decimal precision

                //removed decimal
                if (options.Contains(MereFileParsingOption.RemovedDecimal1))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".0").Replace(".", "");
                if (options.Contains(MereFileParsingOption.RemovedDecimal2))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".00").Replace(".", "");
                if (options.Contains(MereFileParsingOption.RemovedDecimal3))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".000").Replace(".", "");
                if (options.Contains(MereFileParsingOption.RemovedDecimal4))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".0000").Replace(".", "");
                if (options.Contains(MereFileParsingOption.RemovedDecimal5))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".00000").Replace(".", "");
                if (options.Contains(MereFileParsingOption.RemovedDecimal6))
                    v = decimal.Parse(string.IsNullOrEmpty(v.Trim()) ? "0" : v).ToString(".000000").Replace(".", "");
                //end removed decimal

                //check Ebcidic
                if (options.Contains(MereFileParsingOption.Ebcidic))
                {
                    var pos = new[] { '{', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', ' ' };
                    var neg = new[] { '}', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', ' ' };

                    v = v.Trim();
                    var l = string.IsNullOrEmpty(v) ? 0 : int.Parse(v.Last().ToString());
                    l = l < 10 ? l : 10;
                    var isNeg = v.StartsWith("-");
                    var c = isNeg ? neg[l] : pos[l];
                    v = string.Join("", v.Take(v.Length - 1)).TrimStart('-') + c;
                }
                //end check Ebcidic

                //check trailing neg
                if (options.Contains(MereFileParsingOption.TrailingNegative))
                {
                    v = v.Trim();
                    if (v.StartsWith("-"))
                        v = v.TrimStart('-') + "-";
                }
                //end check trailing neg
                if (options.Contains(MereFileParsingOption.TrimStartSpaces))
                    v = v.TrimStart();
                if (options.Contains(MereFileParsingOption.TrimEndSpaces))
                    v = v.TrimEnd();
                if (options.Contains(MereFileParsingOption.Trim))
                    v = v.Trim();

                if (fixedWidth)
                {
                    //pad to width
                    if (options.Contains(MereFileParsingOption.AlignLeft))
                        v = v.PadRight(fieldLength, padChar.First());
                    else
                        v = v.PadLeft(fieldLength, padChar.First());
                    //end pad to width
                }
            }
            else if (fixedWidth && !string.IsNullOrEmpty(padChar))
            {
                v = v.PadLeft(fieldLength, padChar.First());
            }

            if (options != null && options.Contains(MereFileParsingOption.WrapWithDoubleQuotes))
            {
                v = "\"" + v + "\"";
            }

            if (fixedWidth)
            {
                //check length
                if (v.Length > fieldLength)
                    v = string.Join("", v.Take(fieldLength));
                //end check length
            }

            return v;
        }

        public static string ParseFieldForWrite(MereFileField field, object value, bool fixedWidth)
        {
            return ParseFieldForWrite(field.MereColumnForField, field.ToStringFormat, field.FileFieldParsingOptions,
                field.FieldLength, field.PadChar, value, fixedWidth);
        }

        public static string ParseFieldForWrite(List<MereFileParsingOption> options, MereFileField field, object value, bool fixedWidth)
        {
            return ParseFieldForWrite(field.MereColumnForField, field.ToStringFormat, options,
                field.FieldLength, field.PadChar, value, fixedWidth);
        }

        public static string ParseFieldForWrite(List<MereFileParsingOption> options, string toStringFormat, MereFileField field, object value, bool fixedWidth)
        {
            return ParseFieldForWrite(field.MereColumnForField, toStringFormat, options,
                field.FieldLength, field.PadChar, value, fixedWidth);
        }
    }
}
