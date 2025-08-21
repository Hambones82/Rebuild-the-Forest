
using System.Net;

public interface IGameManager
{
    
    public void SelfInit(ServiceLocator serviceLocator);
    public void MutualInit();
    
}
