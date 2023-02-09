using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGameService
{
    void Init();
    bool IsInitialized { get; set; }
}

public static class GameServices
{
    private static List<IGameService> services = new List<IGameService>();
	
    public static T Get<T>()
        where T : class, IGameService
    {
        return services.Find(s => s is T) as T;
    }
	
    public static IGameService Get(string serviceName)
    {
        return services.Find(s => s.GetType().ToString() == serviceName);
    }
	
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeAll()
    {
        foreach (var service in services)
        {
            if (!service.IsInitialized)
            {
                service.IsInitialized = true;
                service.Init();
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void RegisterAll()
    {
        var types = new List<Type>(typeof(IGameService).Assembly.GetTypes())
            .FindAll(type => typeof(IGameService).IsAssignableFrom(type) && !type.IsInterface);

        foreach (var type in types)
        {
            if (services.Find(s => s.GetType() == type) is null)
            {
                IGameService instance = (IGameService)Activator.CreateInstance(type);
                services.Add(instance);
            }
        }
    }
}