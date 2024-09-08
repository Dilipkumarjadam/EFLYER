using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLYER.DTOs;

namespace EFLYER.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IConfiguration _configuration;

        public AdminRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string SqlConnection()
        {
            return _configuration.GetConnectionString("DbConnection").ToString();
        }

        public void AddProduct(ProductDTO productDTO)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertProduct", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductName", productDTO.ProductName);
                    cmd.Parameters.AddWithValue("@Description", productDTO.Description);
                    cmd.Parameters.AddWithValue("@Price", productDTO.Price);
                    cmd.Parameters.AddWithValue("@ProductImagePath", productDTO.ProductImagePath);
                    cmd.Parameters.AddWithValue("@CategoryPId", productDTO.CategoryPId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<DropDownDTO> GetCategory()
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("CategoryDropdown", con))
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
                            CategoryId = Convert.ToInt32(dr["CategoryId"]),
                            CategoryName = Convert.ToString(dr["CategoryName"])
                        });
                    }
                    return list;
                }
            }
        }

        #region--------------------------------------UPDATE DATA LOGIC----------------------------------------------------

        public bool EditProduct(ProductDTO ProductDTO)
        {

            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("EditProduct", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductId", ProductDTO.ProductId);
                    cmd.Parameters.AddWithValue("@ProductName", ProductDTO.ProductName);
                    cmd.Parameters.AddWithValue("@Description", ProductDTO.Description);
                    cmd.Parameters.AddWithValue("@Price", ProductDTO.Price);
                    cmd.Parameters.AddWithValue("@CategoryPId", ProductDTO.CategoryPId);
                    cmd.Parameters.AddWithValue("@ProductImagePath", ProductDTO.ProductImagePath);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        #endregion-------------------------

        #region ------------------------------------------UPDATE GET DATA LOGIC--------------------------------------------------
        public List<ProductDTO> EditProductGetData()
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("GetProduct", con))
                {
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
        }

        #endregion

        #region-----------------------------------------GETDATA LOGIC------------------------------------------------------
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
        #endregion-----------------------------------------------------------------------------------------------------

        public void DeleteProduct(int Id)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteProduct", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", Id);
                con.Open();
                int i = cmd.ExecuteNonQuery();
            }
        }

        public List<OrderDTO> ViewOrders()
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("Select * From Orders", con))
                {
                    cmd.CommandType = CommandType.Text;
                    List<OrderDTO> list = new List<OrderDTO>();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    con.Open();
                    da.Fill(dt);
                    con.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new OrderDTO
                        {
                            OrderId= Convert.ToInt32(dr["OrderId"]),
                            RegOId = Convert.ToInt32(dr["RegOId"]),
                            OrderDate = Convert.ToString(dr["OrderDate"]),
                            TotalAmount = Convert.ToDecimal(dr["TotalAmount"]),
                        });
                    }
                    return list;
                }
            }
        }

        public List<OrderDTO> GetAllCartData()
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("GetCartData", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<OrderDTO> list = new List<OrderDTO>();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    con.Open();
                    da.Fill(dt);
                    con.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new OrderDTO
                        {
                            CartId = dr["CartId"] != DBNull.Value ? Convert.ToInt32(dr["CartId"]) : 0,
                            RegCId = dr["RegCId"] != DBNull.Value ? Convert.ToInt32(dr["RegCId"]) : 0,
                            ProductCId = dr["ProductCId"] != DBNull.Value ? Convert.ToInt32(dr["ProductCId"]) : 0,
                            Quantity = dr["Quantity"] != DBNull.Value ? Convert.ToInt32(dr["Quantity"]) : 0,
                            PricePerUnit = dr["PricePerUnit"] != DBNull.Value ? Convert.ToDecimal(dr["PricePerUnit"]) : 0m,
                            TotalPrice = dr["TotalPrice"] != DBNull.Value ? Convert.ToDecimal(dr["TotalPrice"]) : 0m,
                            ProductName = dr["ProductName"] != DBNull.Value ? Convert.ToString(dr["ProductName"]) : string.Empty,
                            ProductImagePath = dr["ProductImagePath"] != DBNull.Value ? Convert.ToString(dr["ProductImagePath"]) : string.Empty,
                            Description = dr["Description"] != DBNull.Value ? Convert.ToString(dr["Description"]) : string.Empty,
                            CategoryName = dr["CategoryName"] != DBNull.Value ? Convert.ToString(dr["CategoryName"]) : string.Empty
                        });
                    }
                    return list;
                }
            }
        }

    }
}
