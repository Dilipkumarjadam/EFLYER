using EFLYER.DTOs;
using EFLYER.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EFLYER.Repository
{
    public class EflyerRepository : IEflyerRepository
    {
        private readonly IConfiguration _configuration;

        public EflyerRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SqlConnection()
        {
            return _configuration.GetConnectionString("DbConnection").ToString();
        }

        #region----------------------------ADD METHODS---------------------------------------------
        public void AddUserData(RegisteredUserDTO modelDTO)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertRegData", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FullName", modelDTO.FullName);
                    cmd.Parameters.AddWithValue("@Mobile", modelDTO.Mobile);
                    cmd.Parameters.AddWithValue("@CountryRId", modelDTO.CountryRId);
                    cmd.Parameters.AddWithValue("@Email", modelDTO.Email);
                    cmd.Parameters.AddWithValue("@Password", modelDTO.Password);
                    cmd.Parameters.AddWithValue("@ImagePath", modelDTO.ImagePath);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool Login(string Email, string Password, string Type)
        {
            using (SqlConnection connection = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("ValidateUser", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@Type", Type);

                    connection.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    return dt.Rows.Count > 0;
                }
            }
        }

        public bool CheckEmail(string Email, int Id, string Type)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                SqlCommand cmd = new SqlCommand("CheckEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@RegId", Id);
                cmd.Parameters.AddWithValue("@Type", Type);
                con.Open();

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sqlDataAdapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
        #region----------------------------GET METHODS---------------------------------------------
        public List<ProductDTO> GetProduct()
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            using (SqlCommand cmd = new SqlCommand("GetProduct", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);
                List<ProductDTO> ProductList = new List<ProductDTO>();


                foreach (DataRow dr in dt.Rows)
                {
                    ProductList.Add(new ProductDTO
                    {
                        ProductId = Convert.ToInt32(dr["ProductId"]),
                        ProductName = Convert.ToString(dr["ProductName"]),
                        Description = Convert.ToString(dr["Description"]),
                        Price = Convert.ToInt32(dr["Price"]),
                        ProductImagePath = Convert.ToString(dr["ProductImagePath"]),
                        CategoryPId = Convert.ToInt32(dr["CategoryPId"]),
                        CategoryName = Convert.ToString(dr["CategoryName"])
                    });
                }
                return ProductList;
            }
        }

        public List<RegisteredUserDTO> GetUserData()
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("GetRegData", con))
                {
                    List<RegisteredUserDTO> List = new List<RegisteredUserDTO>();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    con.Open();
                    sda.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        List.Add(new RegisteredUserDTO
                        {
                            RegId = Convert.ToInt32(dr["RegId"]),
                            FullName = Convert.ToString(dr["FullName"]),
                            CountryRId = Convert.ToInt32(dr["CountryRId"]),
                            CountryName = Convert.ToString(dr["CountryName"]),
                            Mobile = Convert.ToString(dr["Mobile"]),
                            Email = Convert.ToString(dr["Email"]),
                            Password = Convert.ToString(dr["Password"]),
                            ImagePath = Convert.ToString(dr["ImagePath"])
                        });
                    }

                    return List;
                }
            }
        }

        public List<RegisteredUserDTO> GetAdminData()
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("Select * From AdminUser", con))
                {
                    List<RegisteredUserDTO> List = new List<RegisteredUserDTO>();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    con.Open();
                    sda.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        List.Add(new RegisteredUserDTO
                        {
                            AdminEmail = Convert.ToString(dr["AdminEmail"]),
                            AdminPassword = Convert.ToString(dr["AdminPassword"])
                        });
                    }

                    return List;
                }
            }
        }

        public List<DropDownDTO> GetCountry()
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("COUNTRYDROPDOWN", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<DropDownDTO> list = new List<DropDownDTO>();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    con.Open();
                    da.Fill(dt);
                    con.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new DropDownDTO
                        {
                            CountryId = Convert.ToInt32(dr["CountryId"]),
                            CountryName = Convert.ToString(dr["CountryName"])
                        });
                    }
                    return list;
                }
            }
        }

        #endregion
        #region----------------------------EDIT METHODS---------------------------------------------
        public void EditUserDetails(RegisteredUserDTO modelDTO)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("EditUserDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RegId", modelDTO.RegId);
                    cmd.Parameters.AddWithValue("@FullName", modelDTO.FullName);
                    cmd.Parameters.AddWithValue("@Mobile", modelDTO.Mobile);
                    cmd.Parameters.AddWithValue("@CountryRId", modelDTO.CountryRId);
                    cmd.Parameters.AddWithValue("@Email", modelDTO.Email);
                    cmd.Parameters.AddWithValue("@ImagePath", modelDTO.ImagePath);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion
        #region----------------------------DELETE METHODS---------------------------------------------
        #endregion
        #region --------------------------SEND EMAIL LOGIC----------------------------------
        public void SendEmail(string address, string subject, string body)
        {
            using (MailMessage mm = new MailMessage())
            {
                mm.From = new MailAddress("corporatehuntofficial@gmail.com");
                mm.To.Add(address);
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;

                    NetworkCredential nc = new NetworkCredential("corporatehuntofficial@gmail.com", "tsjs nnlw kpim noqo");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = nc;
                    smtp.Port = 587;

                    smtp.Send(mm);
                }
            }
        }

        public void ChangePassword(int RegId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void SendSms(string toPhoneNumber, string message)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
