using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Suncere.AQSC
{
    /// <summary>
    /// 参数助手
    /// </summary>
    public static class ParameterHelper
    {
        /// <summary>
        /// 字符串空值
        /// </summary>
        public static string EmptyValueString { get; private set; }
        /// <summary>
        /// 空气质量基本监测项字典
        /// </summary>
        public static Dictionary<string, string> PollutantDic { get; set; }

        static ParameterHelper()
        {
            EmptyValueString = "—";
            PollutantDic = new Dictionary<string, string>(){
                {"SO2","二氧化硫"},
                {"NO2","二氧化氮"},
                {"PM10","颗粒物"},
                {"CO","一氧化碳"},
                {"O3","臭氧"},
                {"PM25","细颗粒物"}
            };
            #region 获取配置
            string temp = ConfigurationManager.AppSettings["EmptyValueString"];
            if (!string.IsNullOrEmpty(temp)) EmptyValueString = temp;
            foreach (string pollutant in PollutantDic.Keys.ToList())
            {
                temp = ConfigurationManager.AppSettings[pollutant];
                if (!string.IsNullOrEmpty(temp)) PollutantDic[pollutant] = temp;
            }
            #endregion
        }
    }
}
