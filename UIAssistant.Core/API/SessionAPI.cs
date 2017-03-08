using UIAssistant.Interfaces.API;
using UIAssistant.Interfaces.Session;
using UIAssistant.Infrastructure.Session;

namespace UIAssistant.Core.API
{
    public class SessionAPI : ISessionAPI
    {
        public ISession Create()
        {
            return new Session();
        }
    }
}