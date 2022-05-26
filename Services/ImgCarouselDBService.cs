using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using HwApp1410931031.Models;

namespace HwApp1410931031.Services
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
                    ImgCarousel Data = new ImgCarousel
                    {
                        pid = Convert.ToInt32(dr["pid"]),
                        pfile = dr["pfile"].ToString()
                    };
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