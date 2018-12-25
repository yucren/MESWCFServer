using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SHJXMESWCFServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“MES”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 MES.svc 或 MES.svc.cs，然后开始调试。
    public class MES : IMES
    {
        [WebGet(UriTemplate = "bscan?kbno={kbno}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped)]

        public string Scan(string kbno)
        {
            MESService service = new MESService();
          return  service.Scan(kbno);
        }

        public string StartScan(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);
            var ddd = streamReader.ReadToEnd();


            var dd = Newtonsoft.Json.JsonConvert.DeserializeObject<MESService>(ddd);

            streamReader.Close();
          return   dd.StartScan();
        }
    }
}
