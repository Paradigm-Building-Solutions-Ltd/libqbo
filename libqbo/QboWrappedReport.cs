using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intuit.Ipp.Data;

namespace libqbo;

public class QboWrappedReport
{
    private readonly Report _report;

    public QboWrappedReport(Report report)
    {
        _report = report;
    }

    public IEnumerable<QboReportSection> Sections 
    { 
        get
        {
            foreach (var item in _report.Rows)
            {
                if (item.type == RowTypeEnum.Section)
                {
                    yield return new QboReportSection(item);
                }
            }
        } 
    }
}

public class QboReportSection
{
    private readonly Row _data;

    public string Group => _data.group;
    public string Id => _data.id;
    public string ParentId => _data.parentId;

    public QboReportSection(Row data)
    {
        _data = data;
    }

    public IEnumerable<ColData> Data
    {
        get
        {
            for (int i = 0; i < _data.ItemsElementName.Length; i++)
            {
                var itemName = _data.ItemsElementName[i];
                if (itemName == ItemsChoiceType1.ColData)
                {
                    foreach (var item in (ColData[])_data.AnyIntuitObjects[i])
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
