using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Interfaces
{
    public interface IStartService:IService
    {
        Task RunProgram(); 
    }
}
