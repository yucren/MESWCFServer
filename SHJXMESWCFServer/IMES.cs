using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SHJXMESWCFServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IMES”。
    [ServiceContract]
    public interface IMES
    {
        [OperationContract]
        string StartScan(Stream stream);
        [OperationContract]
        string Scan(string kbno);
    }
}
