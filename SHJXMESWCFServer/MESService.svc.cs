using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SHJXMESWCFServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [Serializable]
 
    public class MESService : IMESService

    {
        // Token: 0x04000021 RID: 33
        
        [DataMember]
        public string Err { get;  set; }
        [DataMember]
        public string Login_user { get; set; }
        [DataMember]
        public int Cur { get; set; }
       
        public DataTable Dtprocess { get; set; }
        
         public DataTable DtItems { get; set; }
        [DataMember]
        public bool Islast { get; set; }
        [DataMember]
        public string Userno { get; set; }
        [DataMember]
        public string Role { get; set; }
        [DataMember]
        public string UserDpt { get; set; }
        [DataMember]
        public string User { get; set; }
        [DataMember]
        public string Duty { get; set; }
        [DataMember]
        public string Scanner { get; set; }
        [DataMember]
        public string UserDptType { get; set; }
        [DataMember]
        public string UserDptName { get; set; }
        [DataMember]
        public int Scanid { get; set; }
        //public bool Btn_start { get => btn_start; set => btn_start = value; }
        //public bool Btn_submit { get => btn_submit; set => btn_submit = value; }
        //public bool Btn_retrieve { get => btn_retrieve; set => btn_retrieve = value; }
        [DataMember]
        public int Kbid { get; set; }
        [DataMember]
        public string BoardNo { get; set; }
        [DataMember]
        public int LineID { get; set; }
        [DataMember]
        public string Statusname { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public int Next_status { get; set; }
        [DataMember]
        public string Next_statusname { get; set; }
        [DataMember]
        public string Tb_Board { get; set; }
        [DataMember]
        public string Lb_operName { get; set; }
        [DataMember]
        public string Tb_cPro { get; set; }
        [DataMember]
        public string Tb_nPro { get; set; }
      

        public DataTable GetProcess(int id)
        {
            string sql = "select a.fInterID,fConfigID,fProTecID,fCode,fName from LKM_MCCPTEntry a left join lkm_CommonBill b on a.fProTecID=b.fInterID where fConfigID=" + id + "order by fOrders asc";
            return (DataTable)DataHelper.ExecuteSql(sql, DataHelper.CreateSqlConn("shjxmes"),DataHelper.RetrunType.dataTable);

        }

        public DataTable GetSeries(string number)
        {
            
            string sql = "select * from vw_lines where ftype = 1 and  fitemcode in (" + number + ")";
            return (DataTable)DataHelper.ExecuteSql(sql, DataHelper.CreateSqlConn("shjxmes"),DataHelper.RetrunType.dataTable);
        }
        public int HandleData(int kbid,int scanid,int lineid,int status,int nextStatus,string user, int type)
        {
            this.Kbid = kbid;
            this.Scanid = scanid;
            this.LineID = lineid;
            this.Status = status;
            this.Next_status = nextStatus;
            this.User = user;
            SqlParameter[] array = new SqlParameter[]
             {
                new SqlParameter("@kbID",SqlDbType.BigInt ),
                new SqlParameter("@curKBID", SqlDbType.BigInt),
                new SqlParameter("@bustype", SqlDbType.Int),
                new SqlParameter("@opertype", SqlDbType.Int),
                new SqlParameter("@lineID", SqlDbType.Int),
                new SqlParameter("@cStatus", SqlDbType.Int),
                new SqlParameter("@nStatus", SqlDbType.Int),
                new SqlParameter("@operName",SqlDbType.NVarChar, 50)
             };
            array[0].Value = this.Kbid;
            array[1].Value = this.Scanid;
            array[2].Value = 1;
            array[3].Value = type;
            array[4].Value = this.LineID;
            array[5].Value = this.Status;
            array[6].Value = this.Next_status;
            array[7].Value = this.User;
            return DataHelper.ExecuteSqlprocedure("pro_ExecKanbanScanHandle_lansq",array, DataHelper.CreateSqlConn("shjxmes"));
            
        }
        
        [WebGet(UriTemplate ="login?info={loginInfo}",ResponseFormat = WebMessageFormat.Xml)]
        
        public string Login(string loginInfo)
        {
            string[] array = loginInfo.Replace('，', ',').Split(new char[]
             {
                ','
             });
            if (array.Length > 1)
            {
                string sql = string.Concat(new string[]
                {
                    "SELECT b.RoleName  ,a.Number ,d.DepartCode ,d.DepartName ,d.fAdmin FROM LKM_SysUser a  inner join LKM_Member c  on a.Number=c.Number  inner join LKM_Depart d  on c.DepartID=d.id  left JOIN LKM_UserInRole b ON a.UserInRole_id = b.id  where a.number='",
                    array[0],
                    "' and a.status=1 and a.pwd='",
                    array[1],
                    "'"
                });
                DataTable dataTable =(DataTable)DataHelper.ExecuteSql(sql,DataHelper.CreateSqlConn("shjxmes"), DataHelper.RetrunType.dataTable);
                if (dataTable.Rows.Count > 0)
                {
                   
                    this.Lb_operName = array[2];
                    this.Userno = array[0];
                    this.Role = dataTable.Rows[0]["RoleName"].ToString();
                    this.UserDpt = dataTable.Rows[0]["DepartCode"].ToString();
                    this.UserDptName = dataTable.Rows[0]["DepartName"].ToString();
                    this.UserDptType = ((this.UserDpt == "01.07.04") ? "B" : "C");
                    this.UserDptType = ((this.UserDpt == "00001") ? "A" : this.UserDptType);
                   
                }
                else
                {
                  
                   Err ="登录信息不正确!";
                }
            }
            
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);

        }
        [WebGet(UriTemplate = "bscan?kbno={kbno}", ResponseFormat = WebMessageFormat.Xml,BodyStyle = WebMessageBodyStyle.Wrapped)]
       // [WebInvoke(Method ="POST" , UriTemplate = "bscan?kbno={kbno}", RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Xml,BodyStyle = WebMessageBodyStyle.Bare)]
        public string Scan(string kbno)
        {
            this.Tb_Board = kbno;
            if (this.Tb_Board.IndexOf("-") > -1)
            {
                this.BoardNo = this.Tb_Board;
                string a = this.BoardNo.Substring(0, 1);
                if (a != "S")
                {
                   
                    this.Tb_Board = "";                   
                    Err = "错误提示" + "请扫描生产看板";
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder("select vv.finterid,isnull(vv.fCurScanID,0) as fCurScanID,vv.fbillno,vv.fstatus,vv.fstatuName from lkm_exec_kanban v inner join lkm_exec_kanban_entry vv on v.finterid = vv.fmainid ");
                    stringBuilder.Append(" where v.fstatus = 1 and vv.fstatus != 2 and  vv.fbillno='" + this.BoardNo + "'");
                    DataTable dataTable = (DataTable)DataHelper.ExecuteSql(stringBuilder.ToString(),DataHelper.CreateSqlConn("shjxmes"),DataHelper.RetrunType.dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        this.Status = int.Parse(dataTable.Rows[0]["fstatus"].ToString());
                        this.Statusname = dataTable.Rows[0]["fstatuName"].ToString();
                        this.Kbid = int.Parse(dataTable.Rows[0]["finterid"].ToString());
                        this.Scanid = int.Parse(dataTable.Rows[0]["fCurScanID"].ToString());
                        //this.Btn_start = (this.Status == 0 || this.Status == 1);
                        //this.Btn_submit = (this.Status != 0 && this.Status != 1 && this.Status != -1);
                        //this.Btn_retrieve = (this.Status == -1);
                        StringBuilder stringBuilder2 = new StringBuilder("select fitemid,fitemcode,fitemname,ftransfer_batch from vw_ExecKB_Detail_List ");
                        stringBuilder2.Append("  where fbillno='" + this.BoardNo + "'");
                        this.DtItems =(DataTable) DataHelper.ExecuteSql(stringBuilder2.ToString(),DataHelper.CreateSqlConn("shjxmes"),DataHelper.RetrunType.dataTable);
                        if (this.DtItems.Rows.Count > 0)
                        {                          
                            string text = "";
                            for (int i = 0; i <= this.DtItems.Rows.Count - 1; i++)
                            {
                                if (i < this.DtItems.Rows.Count - 1)
                                {
                                    text = text + "'" + this.DtItems.Rows[i]["fitemcode"].ToString() + "',";
                                }
                                else
                                {
                                    text = text + "'" + this.DtItems.Rows[i]["fitemcode"].ToString() + "'";
                                }
                            }
                            try
                            {
                                this.Islast = false;
                                DataTable series = this.GetSeries(text);
                                if (series.Rows.Count < 1)
                                {
                                    
                                    this.DtItems.Clear();
                                    //this.tb_Board.Enabled = true;
                                    //this.tb_Board.Focus();
                                    //this.tb_Board.Text = null;
                                    Err =  "错误提示：[" + text + "]未绑定产线物料";
                                }
                                else
                                {
                                    this.LineID = int.Parse(series.Rows[0]["fconfigid"].ToString());
                                    this.Dtprocess = this.GetProcess(this.LineID);
                                    if (this.Dtprocess.Rows.Count > 0)
                                    {
                                        StringBuilder stringBuilder3 = new StringBuilder("select a.fstatus,a.fstatuName,c.fName,a.finterid,b.fpersons,b.fPsnItemInfo,a.fbillno,b.fProTecID ");
                                        stringBuilder3.Append(" from lkm_exec_kanban_entry a ");
                                        stringBuilder3.Append(" left join LKM_MCCPTEntry b on a.fStatus=b.fInterID ");
                                        stringBuilder3.Append(" left join lkm_CommonBill c on b.fProTecID=c.fInterID ");
                                        stringBuilder3.Append(" where a.fstatus<>2 and a.fbillno='" + this.BoardNo + "'");
                                        DataTable dataTable2 =(DataTable) DataHelper.ExecuteSql(stringBuilder3.ToString(), DataHelper.CreateSqlConn("shjxmes"), DataHelper.RetrunType.dataTable);
                                        if (dataTable2.Rows.Count > 0)
                                        {
                                            this.User = ((dataTable2.Rows[0]["fpersons"].ToString() == "") ? this.Lb_operName : dataTable2.Rows[0]["fpersons"].ToString());
                                            this.Duty = dataTable2.Rows[0]["fPsnItemInfo"].ToString();
                                            this.Scanner = this.Lb_operName;
                                            if (int.Parse(dataTable2.Rows[0]["fstatus"].ToString()) == 0 || int.Parse(dataTable2.Rows[0]["fstatus"].ToString()) == 1)
                                            {
                                                this.Status = int.Parse(this.Dtprocess.Rows[0]["finterid"].ToString());
                                                this.Statusname = this.Dtprocess.Rows[0]["fName"].ToString();
                                            }
                                            else
                                            {
                                                this.Statusname = dataTable2.Rows[0]["fstatuName"].ToString();
                                                this.Status = int.Parse(dataTable2.Rows[0]["fstatus"].ToString());
                                            }
                                            if (this.Status == int.Parse(this.Dtprocess.Rows[this.Dtprocess.Rows.Count - 1]["finterid"].ToString()))
                                            {
                                                this.Islast = true;
                                            }
                                            for (int i = 0; i < this.Dtprocess.Rows.Count; i++)
                                            {
                                                if (this.Status == int.Parse(this.Dtprocess.Rows[i]["finterid"].ToString()))
                                                {
                                                    this.Cur = i;
                                                }
                                            }
                                            if (this.Islast)
                                            {
                                                this.Next_status = -1;
                                                this.Next_statusname = "待回收";
                                            }
                                            else if (this.Status == -1)
                                            {
                                                this.Next_status = 1;
                                                this.Next_statusname = "可用";
                                            }
                                            else
                                            {
                                                this.Next_status = int.Parse(this.Dtprocess.Rows[this.Cur + 1]["finterid"].ToString());
                                                this.Next_statusname = this.Dtprocess.Rows[this.Cur + 1]["fname"].ToString();
                                            }
                                            this.Tb_cPro = this.Statusname;
                                            this.Tb_nPro = this.Next_statusname;
                                            this.Lb_operName = this.Login_user;
                                        }
                                    }
                                   
                                }
                               
                            }
                            catch (Exception ex)
                            {
                                
                                this.Tb_Board = "";
                                //this.tb_Board.Enabled = true;
                                //this.tb_Board.Focus();
                                Err= "错误提示:看板扫描错误!" + ex.Message;
                            }
                        }
                    }
                    else
                    {
                        string text2 = "错误提示:看板[" + this.BoardNo + "]不可用，请重新采集";
                        
                        //this.tb_Board.Enabled = true;
                        //this.tb_Board.Focus();
                        this.Tb_Board = "";
                       
                    }
                   
                }
               
            }
            //DataContractSerializer contractSerializer = new DataContractSerializer(typeof(MESService));

            //MemoryStream stream = new MemoryStream();

            //contractSerializer.WriteObject(stream, this);
            //byte[] value = new byte[stream.Length];
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Read( value, 0, (int)stream.Length);
            //return Encoding.UTF8.GetString(value);
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
       // [WebGet(UriTemplate = "sscan?kbno={kbno}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare)]
        public string StartScan()        {

           

            if (this.Tb_Board.IndexOf("-") > -1)
            {
                if (this.HandleData(this.Kbid, this.Scanid, this.LineID, this.Status, this.Next_status, this.User, -1) == -1)
                {
                    string text = "【开工】提交失败，请重新采集看板";
                    Err = text;
                }
                //this.tb_Board.Enabled = true;
                //this.tb_Board.Focus();
                this.Tb_Board = "";
                this.Tb_cPro = "";
                this.Tb_nPro = "";
                //this.DtItems.Clear();
                //this.Dtprocess.Clear();
                //this.btn_start.Enabled = false;
                
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        [WebGet(UriTemplate = "submit?kbno={kbno}",ResponseFormat = WebMessageFormat.Xml,BodyStyle = WebMessageBodyStyle.Wrapped)]
        public string Submit(string kbno)
        {
            this.Tb_Board = kbno;
            if ((string.IsNullOrEmpty(this.Tb_Board)?"": this.Tb_Board).IndexOf("-") > -1)
            {
                for (int i = 0; i <= this.DtItems.Rows.Count - 1; i++)
                {
                    int num = int.Parse(this.DtItems.Rows[i]["ftransfer_batch"].ToString());
                    string sql = string.Concat(new string[]
                    {
                        "select isnull(fcount,0) as fcount from lkm_blank_inventory where fitemid=",
                        this.DtItems.Rows[i]["fitemid"].ToString(),
                        " and flineid = ",
                        this.LineID.ToString(),
                        " and fproid = (select fProTecID from LKM_MCCPTEntry where finterid = ",
                        this.Status.ToString(),
                        ")"
                    });
                    int num2 = int.Parse(DataHelper.ExecuteSql(sql, DataHelper.CreateSqlConn("shjxmes"), DataHelper.RetrunType.integer).ToString());
                    if (num2 - num < 0)
                    {
                        string text = "当前物料[" + this.DtItems.Rows[i]["fitemname"].ToString() + "]库存不足，不允许提交";
                        Err = text;
                        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
                    }
                }
                if (this.HandleData(this.Kbid,this.Scanid,this.LineID,this.Status,this.Next_status,this.User, 0) == -1)
                {
                    string text = "提交失败，请重新采集看板";
                    Err = text;
                }
                //this.tb_Board.Enabled = true;
                //this.tb_Board.Focus();
                this.Tb_Board= "";
                this.Tb_cPro= "";
                this.Tb_nPro= "";
                this.DtItems.Clear();
                this.Dtprocess.Clear();
                //this.btn_start.Enabled = false;
                //this.btn_submit.Enabled = false;               
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string SubmitScanData()
        {
            if (this.Tb_Board.IndexOf("-") > -1)
            {
                for (int i = 0; i <= this.DtItems.Rows.Count - 1; i++)
                {
                    int num = int.Parse(this.DtItems.Rows[i]["ftransfer_batch"].ToString());
                    string sql = string.Concat(new string[]
                    {
                        "select isnull(fcount,0) as fcount from lkm_blank_inventory where fitemid=",
                        this.DtItems.Rows[i]["fitemid"].ToString(),
                        " and flineid = ",
                        this.LineID.ToString(),
                        " and fproid = (select fProTecID from LKM_MCCPTEntry where finterid = ",
                        this.Status.ToString(),
                        ")"
                    });
                    int num2 = int.Parse(DataHelper.ExecuteSql(sql, DataHelper.CreateSqlConn("shjxmes"), DataHelper.RetrunType.integer).ToString());
                    if (num2 - num < 0)
                    {
                        string text = "当前物料[" + this.DtItems.Rows[i]["fitemname"].ToString() + "]库存不足，不允许提交";
                        Err = text;
                        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
                    }
                }
                if (this.HandleData(this.Kbid, this.Scanid, this.LineID, this.Status, this.Next_status, this.User, 0) == -1)
                {
                    string text = "提交失败，请重新采集看板";
                    Err = text;
                }
                //this.tb_Board.Enabled = true;
                //this.tb_Board.Focus();
                this.Tb_Board = "";
                this.Tb_cPro = "";
                this.Tb_nPro= "";
                this.DtItems.Clear();
                this.Dtprocess.Clear();
               
                //this.btn_start.Enabled = false;
                //this.btn_submit.Enabled = false;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
