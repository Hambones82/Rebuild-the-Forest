using System;
using System.Collections.Generic;

[System.Serializable]

public class ServiceLocator 
{
    //have a registry of the managers.  every manager needs to set this thing.  set their own internal things 
    //from the registry
    private Dictionary<Type, IGameManager> services = new Dictionary<Type, IGameManager>();

    public T LocateService<T>() where T : class
    {
        if(services.TryGetValue(typeof(T), out var service))
        {
            return service as T;
        }        
        return null;
    }

    //true if service was registered.  false if service was not registered
    public bool RegisterService<T>(T serviceToRegister) where T: class, IGameManager
    {
        if (serviceToRegister == null) throw new ArgumentNullException("cannot register a null service");
        if(services.ContainsKey(typeof(T))) 
        {
            return false;
        }
        else
        {
            services[typeof(T)] = serviceToRegister;
            return true;
        }
    }    
}
