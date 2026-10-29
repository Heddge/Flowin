using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowin.Objects
{
    public enum RequestStatus { Success, NotFound, ServiceUnavaliable, IncorrectNote, IncorrectApiUrl, IncorrectJson }
    /// <summary>
    /// Uses for corncreting request feedback for synchronizing (SycnrhonizeManager)
    /// </summary>
    public class RequestResultStatus<T>
    {
        public RequestStatus Status { get; set; }
        public T? data { get; set; }

        public RequestResultStatus()
        {
            Status = RequestStatus.ServiceUnavaliable;
            this.data = default;
        }

        public RequestResultStatus(RequestStatus status, T data)
        {
            Status = status;
            this.data = data;
        }
    }
}
