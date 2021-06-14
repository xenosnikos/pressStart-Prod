using System;

namespace PressStart.Models{
    public class ErrorViewModel{
        public string RequestId{get; set;}
        public bool showRequestId => !string.IsNullOrEmpty(RequestId);
    }
}