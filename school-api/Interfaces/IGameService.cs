using school_api.Data.Models;
using school_api.Data.Dto;

namespace school_api.Interfaces
{
    public interface IGameService
    {
        //RequestSessionResponse RequestSession(SessionRequest request);
        List<Session> GetAllSessionInformation();
        Session GetSessionInformationId(Guid sessionId);

        void StartGameManegement();
        bool Ping(PingRequest request);
    }
}
