using EFLYER.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLYER.Repository
{
    public interface IEflyerRepository
    {
        public void AddUserData(RegisteredUserDTO modelDTO);
        public List<RegisteredUserDTO> GetAdminData();
        public RegisteredUserDTO GetUserByEmail(string email);
        public List<RegisteredUserDTO> GetUserData();
        public bool Login(string Email, string Password, string Type);
        public bool CheckEmail(string Email, int Id, string Type);
        public void SendEmail(string address, string subject, string body);
        public List<DropDownDTO> GetCountry();
        public List<ProductDTO> GetProduct();
    }
}
