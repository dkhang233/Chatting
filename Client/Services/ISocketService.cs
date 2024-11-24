using ChitChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChitChat.Services
{
    public interface ISocketService
    {
        Task<UserModel> PostLoginCredentialsAsync(UserCredentials userCredentials);
    }
}
