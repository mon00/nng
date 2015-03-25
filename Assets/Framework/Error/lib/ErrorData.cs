using System;
namespace Lab70_Framework.Error
{
    public class ErrorData
    {
        public bool Damaged = false;

        public int SerialNumber;
        public string Subject;
        public string Message;
        public DateTime Time;

        public string[] AdditionalComponents = new string[0];

        public ErrorData(int serialNumber, string subject, string message, string[] additionalComponents = null)
        {
            SerialNumber = serialNumber;
            Subject = subject;
            Message = message;
            Time = DateTime.Now;
        }
    }
}