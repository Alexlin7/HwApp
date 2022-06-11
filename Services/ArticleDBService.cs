using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using HwApp1410931031.Models;


namespace HwApp1410931031.Services
{
    public class ArticleDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["BsTest"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢一筆資料
        //藉由編號取得單筆資料的方法
        public Article GetArticleDataById(int A_Id)
        {
            Article Data = new Article();
            //Sql語法

            string sql = $@" select m.*,d.Name,d.Image from Article m inner join Members d on m.Account = d.Account where m.A_Id =  {A_Id} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read(); //獲得下一筆資料直到沒有資料
                Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                Data.Account = dr["Account"].ToString();
                Data.Title = dr["Title"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Watch = Convert.ToInt32(dr["Watch"]);
                Data.Member.Name = dr["Name"].ToString();
                Data.Member.Image = dr["Image"].ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //沒有資料傳回null
                Data = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳根據編號所取得的資料
            return Data;
        }
        #endregion

        #region 查詢陣列資料
        //根據搜尋來取得資料陣列的方法
        public List<Article> GetDataList(ForPaging Paging, string Search, string Account)
        {
            //宣告要接受全部搜尋資料的物件
            List<Article> DataList = new List<Article>();
            //Sql語法
            if (!string.IsNullOrWhiteSpace(Search))
            {
                //有搜尋條件時
                SetMaxPaging(Paging, Search, Account);
                DataList = GetAllDataList(Paging, Search, Account);
            }
            else
            {
                //無搜尋條件時
                SetMaxPaging(Paging, Account);
                DataList = GetAllDataList(Paging, Account);
            }
            return DataList;
        }
        //無搜尋值的搜尋資料方法
        public List<Article> GetAllDataList(ForPaging paging, string Account)
        {
            //宣告要回傳的搜尋資料為資料庫中的Article資料表
            List<Article> DataList = new List<Article>();
            //Sql語法
            string sql = $@" select m.*, d.Name from (select row_number() over(order by A_Id) as sort,* from Article where Account = '{Account}' ) m inner join Members d on m.Account = d.Account Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }

        //有搜尋值的搜尋資料方法
        public List<Article> GetAllDataList(ForPaging paging, string Search, string Account)
        {
            //宣告要回傳的搜尋資料為資料庫中的Article資料表
            List<Article> DataList = new List<Article>();
            //Sql語法
            string sql = $@" select m.*,d.Name from (select row_number() over(order by A_Id) as sort,* from Article where (Title like '%{Search}%' or Content like '%{Search}%') and Account = '{Account}' ) m inner join Members d on m.Account = d.Account Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }
        #region 設定最大頁數方法
        //無搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging, string Account)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Article where Account = '{Account}' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }

        //有搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging, string Search, string Account)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Article Where ( Title like '%{Search}%' or Content like '%{Search}%' ) and Account = '{Account}' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }
        #endregion
        #endregion


        #region 新增文章
        //新增資料方法
        public void InsertArticle(Article newData)
        {
            //取得最新一筆A_Id
            newData.A_Id = LastArticleFinder();
            //Sql新增語法
            //設定新增時間為現在
            string sql = $@" INSERT INTO Article(A_Id,Title,Content,Account,CreateTime,Watch)
VALUES ( {newData.A_Id},'{newData.Title}','{newData.Content}','{newData.Account}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}',0 ) ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 計算目前文章最新一筆的A_Id
        public int LastArticleFinder()
        {
            // 宣告要回傳的值
            int Id;
            //Sql查詢語法
            string sql = $@" select top 1 * from article order by A_Id desc";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                // 取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["A_Id"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //沒資料時Id為0
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id + 1;
        }
        #endregion

        #region 修改文章
        //修改文章方法
        public void UpdateArticle(Article UpdateData)
        {
            //Sql修改語法
            string sql = $@" update Article set Content = '{UpdateData.Content}' where A_Id = {UpdateData.A_Id} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 刪除文章
        //刪除資料方法
        public void DeleteArticle(int A_Id)
        {
            //Sql語法
            //必須先將該文章的留言刪除
            string DeleteMessage = $@" Delete from Message where A_Id = {A_Id} ";
            //再根據文章Id取得要刪除的文章
            string DeleteArticle = $@" Delete from Article where A_Id = {A_Id} ";
            //將兩段SQL語法一起放入SQL執行，能避免一直開啟資料庫連線，降低資料庫的負擔
            string CombineSql = DeleteMessage + DeleteArticle;
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(CombineSql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 修改檢查
        //修改文章檢查判斷的方法
        public bool CheckUpdate(int A_Id)
        {
            //根據Id取得要修改的資料
            Article Data = GetArticleDataById(A_Id);
            //抓取文章內的留言
            //留言筆數
            int MessageCount = 0;
            //Sql語法
            string sql = $@" select * from Message Where A_Id = {A_Id} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    MessageCount++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //判斷並回傳(判斷是否有資料以及是否有回覆)
            return (Data != null && MessageCount == 0);
        }
        #endregion

        #region 人氣查詢
        public List<Article> GetPopularList(string Account)
        {
            List<Article> popularList = new List<Article>();
            //查詢top5 watch
            string sql = $@" SELECT TOP 5 * FROM Article m inner join  Members d on m.Account = d.Account where m.Account = '{Account}' order by watch desc";
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Title = dr["Title"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Watch = Convert.ToInt32(dr["Watch"]);
                    Data.Member.Name = dr["Name"].ToString();
                    popularList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return popularList;
        }
        #endregion


        #region 增加觀看人數
        public void AddWatch(int A_id)
        {
            string sql = $@" update Article set Watch = Watch + 1 where A_Id = '{A_id}' ";
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

    }
}