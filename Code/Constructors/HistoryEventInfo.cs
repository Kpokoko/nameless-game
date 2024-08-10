using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.Constructors;

public class HistoryEventInfo
{
    public Action Action { get; set; }
    public HistoryEventType EventType { get; set; }
    public bool IsSeparator { get => EventType == HistoryEventType.Separator; }
    public bool IsInGroup { get => EventType == HistoryEventType.Group; }


    public HistoryEventInfo(Action action, HistoryEventType eventType = HistoryEventType.Solo)
    {
        Action = action;
        EventType = eventType; 
    }

    public HistoryEventInfo(HistoryEventType eventType)
    {
        EventType = eventType;
    }

    public void Invoke()
    {
        Action.Invoke();
    }
}
