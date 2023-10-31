using System.Runtime.Serialization;

namespace RueI.Parsing
{
    [Serializable]
    public class ParamProcessorUnfinishedException : Exception
    {
        public ParamProcessorUnfinishedException()
        {
        }

        public ParamProcessorUnfinishedException(string message) : base(message)
        {
        }

        public ParamProcessorUnfinishedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ParamProcessorUnfinishedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}