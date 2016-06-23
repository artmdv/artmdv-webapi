using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace artmdv_webapi.Areas.v2.DataAccess
{
    public class EventStoreAccess
    {
        protected IEventStoreConnection BuildConnection()
        {
            var settings = ConnectionSettings.Create()
                .LimitRetriesForOperationTo(1)
                .LimitAttemptsForOperationTo(1)
                .LimitConcurrentOperationsTo(1)
                .FailOnNoServerResponse()
                .LimitReconnectionsTo(5);
            return EventStoreConnection.Create(settings, new Uri(Configuration.Get("EventStoreAddress")));
        }
    }
}
