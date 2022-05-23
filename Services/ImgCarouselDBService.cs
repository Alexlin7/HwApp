using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.Services
{
    public class ImgCarouselDBService
    {
        private readonly static string cnstr =ConfigurationManager.ConnectionStrings["BsTest"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        public List<ImgCarousel> GetDataList()
        {
            List<ImgCarousel> DataList = new List<ImgCarousel>();
            string sql = @" SELECT * FROM ImgCarousel; ";
            try
            {
                conn.Open(); // 開啟資料庫連線
                SqlCommand cmd = new SqlCommand(sql, conn); // 執行Sql 指令
                SqlDataReader dr = cmd.ExecuteReader(); // 取得Sql 資料
                while (dr.Read()) // 獲得下一筆資料直到沒有資料
                {
                    ImgCarousel Data = new ImgCarousel();
                    Data.pid = Convert.ToInt32(dr["pid"]);
                    Data.pfile = dr["pfile"].ToString();
                    if (!dr["ptitle"].Equals(DBNull.Value))
                    {
                        Data.ptitle = dr["ptitle"].ToString();
                    }
                    if (!dr["pinfo"].Equals(DBNull.Value))
                    {
                        Data.pinfo = dr["pinfo"].ToString();
                    }
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList
;
        }
    
    }
}