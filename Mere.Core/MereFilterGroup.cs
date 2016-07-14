using System.Collections.Generic;
using System.Linq;

namespace Mere.Core
{
    public class MereFilterGroup
    {
        private readonly List<MereFilter> _filters;
        private readonly List<MereFilterGroup> _filterGroups;
        private MereFilterGroup _curFilterGroup = null;
        public int FilterLevel { get; set; }
        private int _curFilterLevel;
        public MereFilterGroup()
        {
            AndOr = " AND ";
            _filterGroups = new List<MereFilterGroup>();
            _filters = new List<MereFilter>();
            _curFilterLevel = 0;
        }

        public MereFilterGroup(bool or)
        {
            AndOr = or ? " OR " : " AND ";
            _filters = new List<MereFilter>();
            _filterGroups = new List<MereFilterGroup>();
            _curFilterLevel = 0;
        }

        public MereFilterGroup(string andOr)
        {
            AndOr = andOr;
            _filters = new List<MereFilter>();
            _filterGroups = new List<MereFilterGroup>();
            _curFilterLevel = 0;
        }

        public string AndOr { get; set; }

        public bool HasFilters
        {
            get { return _filters.Count > 0; }
        }

        public void AddFilter(string andOr, string filter, bool newGroup = false, int backup = 0)
        {
            if (newGroup && backup <= 0)
            {
                if (_curFilterGroup != null)
                {
                    _curFilterGroup.AddFilter(andOr, filter, true, 0);
                }
                else
                {
                    _curFilterLevel++;
                    var newFilter = new MereFilter(_curFilterLevel, andOr, filter);
                    _filters.Add(newFilter);
                }
            }
            else if (backup > 0)
            {
                if (_curFilterGroup != null)
                    _filterGroups.Add(_curFilterGroup);

                if (backup > _curFilterLevel)
                    backup = _curFilterLevel;

                _curFilterLevel = _curFilterLevel - backup;
                var newGrp = new MereFilterGroup(andOr);
                newGrp.FilterLevel = _curFilterLevel;
                newGrp.AddFilter(andOr, filter, false, 0);
                _curFilterGroup = newGrp;
            }
            else
            {
                if (_curFilterGroup != null)
                {
                    _curFilterGroup.AddFilter(andOr, filter, false, 0);
                }
                else
                {
                    var newFilter = new MereFilter(_curFilterLevel, andOr, filter);
                    _filters.Add(newFilter);
                }

            }
        }

        public void And(string filter)
        {
            AddFilter(" AND ", filter);
        }

        public void AndGroup(string filter)
        {
            AddFilter(" AND ", filter, true);
        }

        public void AndGroup(string filter, int backup)
        {
            AddFilter(" AND ", filter, true, backup);
        }

        public void Or(string filter)
        {
            AddFilter(" Or ", filter);
        }

        public void OrGroup(string filter)
        {
            AddFilter(" Or ", filter, true);
        }

        public void OrGroup(string filter, int backup)
        {
            AddFilter(" Or ", filter, true, backup);
        }

        public string WhereString
        {
            get
            {
                var whereStr = "";
                var whereStrEnder = "";

                if (!_filters.Any() && !_filterGroups.Any())
                    return "";

                var filterDepth = _filters.Any() ? _filters.Max(x => x.FilterLevel) : 0;
                if (_curFilterGroup != null && _curFilterGroup.FilterLevel > filterDepth)
                    filterDepth = _curFilterGroup.FilterLevel;
                for (var i = 0; i <= filterDepth; i++)
                {
                    var level = i;

                    var curLevelFilters = _filters.Where(x => x.FilterLevel == level).ToList();
                    var curLevelFilterGroups = _filterGroups.Where(x => x.FilterLevel == level).ToList();

                    var first = curLevelFilters.FirstOrDefault();
                    var firstGrp = curLevelFilterGroups.FirstOrDefault();



                    //whereStr += " (";
                    if (_filters.Any() && first != null)
                    {
                        if (level > 0)
                        {
                            whereStr += first.Key + " (";
                            whereStrEnder += ") ";
                        }

                        whereStr += first.Value + string.Join(" ", curLevelFilters.Skip(1).Select(s => " " + s.Key + " " + s.Value));
                    }


                    if (_filterGroups.Any() && firstGrp != null)
                    {
                        if (curLevelFilters.Any())
                        {
                            whereStr += string.Join(" ", curLevelFilterGroups.Select(s => " " + s.AndOr + " " + s.WhereString));
                        }
                        else
                        {
                            whereStr += firstGrp.WhereString + string.Join(" ", curLevelFilterGroups.Skip(1).Select(s => " " + s.AndOr + " " + s.WhereString));
                        }
                    }

                    if (_curFilterGroup != null && _curFilterGroup.FilterLevel == level)
                    {
                        if (curLevelFilters.Any())
                        {
                            whereStr += " " + _curFilterGroup.AndOr + " " + _curFilterGroup.WhereString;
                        }
                        else
                        {
                            whereStr += _curFilterGroup.WhereString;
                        }
                    }

                    //whereStr += ") ";
                }
                return " (" + whereStr + whereStrEnder + ") ";
            }
        }
    }
}