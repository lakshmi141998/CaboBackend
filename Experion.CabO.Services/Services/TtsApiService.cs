using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Experion.CabO.Services.DTOs;

namespace Experion.CabO.Services.Services
{
    public class TtsApiService
    {
        private static readonly HttpClient client = new HttpClient();
        public async Task<TTSUserDetailsByEmail> GetTTSUserDetailsByEmail(string email)
        {
            var streamTask = client.GetStreamAsync("http://ttsqa.experionglobal.com:81/api/GetUserByEmailId/"+ email);
            var serializer = new DataContractJsonSerializer(typeof(TTSUserDetailsByEmail));
            var user = (TTSUserDetailsByEmail)serializer.ReadObject(streamTask.Result);
            return user;
        }
        public async Task<IList<TTSProjectsByUserId>> GetTTSProjectsByUserId(int id)
        {
            var streamTask = client.GetStreamAsync("http://ttsqa.experionglobal.com:81/api/EmployeeProjectsByUserId/" + id);
            var serializer = new DataContractJsonSerializer(typeof(IList<TTSProjectsByUserId>));
            var projects = (IList<TTSProjectsByUserId>)serializer.ReadObject(streamTask.Result);
            return projects;
        }
        public async Task<TTSUserDetailsById> GetTTSUserDetailsById(int id)
        {
            var streamTask = client.GetStreamAsync("http://ttsqa.experionglobal.com:81/api/users/" + id);
            var serializer = new DataContractJsonSerializer(typeof(TTSUserDetailsById));
            var user = (TTSUserDetailsById)serializer.ReadObject(streamTask.Result);
            return user;
        }
        public async Task<ICollection<TTSUserDetailsById>> GetAdminDetails(string role)
        {
            var streamTask = client.GetStreamAsync("http://ttsqa.experionglobal.com:81/api/users/GetUsersByDesignation/" + role);
            var serializer = new DataContractJsonSerializer(typeof(ICollection<TTSUserDetailsById>));
            var user = (ICollection<TTSUserDetailsById>)serializer.ReadObject(streamTask.Result);
            return user;
        }
    }
}
