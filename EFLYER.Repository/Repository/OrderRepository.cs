using EFLYER.DTOs;
using EFLYER.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLYER.Repository.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;

        public OrderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string SqlConnection()
        {
            return _configuration.GetConnectionString("DbConnection").ToString();
        }

        public void AddToCart(OrderDTO order)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddToCart", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                    cmd.Parameters.AddWithValue("@PricePerUnit", order.PricePerUnit);
                    cmd.Parameters.AddWithValue("@RegCId", order.RegCId);
                    cmd.Parameters.AddWithValue("@ProductCId", order.ProductCId);
                    cmd.Parameters.AddWithValue("@CategoryCId", order.CategoryCId);

                    con.Open();
                    cmd.ExecuteNonQuery();
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
                            Description = dr["Description"] != DBNull.Value ? Convert.ToString(dr["Description"]) : string.Empty
                        });
                    }
                    return list;
                }
            }
        }

        public void DeleteCartItem(int CartId)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteCartItem", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CartId", CartId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public decimal GetProductPrice(int productCId)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("GetProductPrice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductCId", productCId);

                    con.Open();
                    var result = cmd.ExecuteScalar();
                    con.Close();

                    return Convert.ToDecimal(result);
                }
            }
        }

        public void EditQuantity(int NewQuantity, int RegCId, int ProductCId)
        {
           
            decimal price = GetProductPrice(ProductCId);
            decimal TotalPrice = price * NewQuantity;

            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("EditQuantity", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NewQuantity", NewQuantity);
                    cmd.Parameters.AddWithValue("@RegCId", RegCId);
                    cmd.Parameters.AddWithValue("@ProductCId", ProductCId);
                    cmd.Parameters.AddWithValue("@TotalPrice", TotalPrice);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddOrder(int UserId, decimal TotalAmount)
        {
            using (SqlConnection con = new SqlConnection(this.SqlConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddOrder", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RegOId", UserId);
                    cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
