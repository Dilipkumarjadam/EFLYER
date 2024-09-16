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
        #region----------------------------ADD METHODS IMPLEMENTATION---------------------------------------------
        public void AddUserData(RegisteredUserDTO modelDTO);
        public void EditUserDetails(RegisteredUserDTO modelDTO);
        public void ChangePassword(int RegId, string newPassword);
        public bool Login(string Email, string Password, string Type);
        public bool CheckEmail(string Email, int Id, string Type);
        public void SendEmail(string address, string subject, string body);
        #endregion

        #region----------------------------GET METHODS IMPLEMENTATION---------------------------------------------
        public List<RegisteredUserDTO> GetAdminData();
        public List<RegisteredUserDTO> GetUserData();
        public List<DropDownDTO> GetCountry();
        public List<ProductDTO> GetProduct();
        #endregion
        public void SendSms(string toPhoneNumber, string message);
    }
}
