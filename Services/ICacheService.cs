using ApiPeliculas.Entities;
using System.Collections.Generic;

namespace ApiPeliculas.Services
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveData(string key);

        List<string> GetAllkeys(string pattern);
        //T GetAllkeys<T>(string pattern);
    }

}
