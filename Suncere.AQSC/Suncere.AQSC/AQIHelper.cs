using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Suncere.AQSC
{
    /// <summary>
    /// AQI计算工具类
    /// </summary>
    public class AQIHelper
    {
        #region 字段参数
        #region 浓度限值
        /// <summary>
        /// 空气质量分指数限值数组
        /// </summary>
        private static int[] IAQILimits = { 0, 50, 100, 150, 200, 300, 400, 500 };
        /// <summary>
        /// 二氧化硫（SO2）24小时平均浓度限值
        /// </summary>
        private static int[] SO2DayConcentrationLimits = { 0, 50, 150, 475, 800, 1600, 2100, 2620 };
        /// <summary>
        /// 二氧化硫（SO2）1小时平均浓度限值
        /// </summary>
        private static int[] SO2HourConcentrationLimits = { 0, 150, 500, 650, 800 };
        /// <summary>
        /// 二氧化氮（NO2）24小时平均浓度限值
        /// </summary>
        private static int[] NO2DayConcentrationLimits = { 0, 40, 80, 180, 280, 565, 750, 940 };
        /// <summary>
        /// 二氧化氮（NO2）1小时平均浓度限值
        /// </summary>
        private static int[] NO2HourConcentrationLimits = { 0, 100, 200, 700, 1200, 2340, 3090, 3840 };
        /// <summary>
        /// 可吸入颗粒物（PM10）24小时平均浓度限值
        /// </summary>
        private static int[] PM10DayConcentrationLimits = { 0, 50, 150, 250, 350, 420, 500, 600 };
        /// <summary>
        /// 可吸入颗粒物（PM10）24小时平均浓度限值
        /// </summary>
        private static int[] PM10HourConcentrationLimits = { 0, 50, 150, 250, 350, 420, 500, 600 };
        /// <summary>
        /// 一氧化碳（CO）24小时平均浓度限值
        /// </summary>
        private static int[] CODayConcentrationLimits = { 0, 2, 4, 14, 24, 36, 48, 60 };
        /// <summary>
        /// 一氧化碳（CO）1小时平均浓度限值
        /// </summary>
        private static int[] COHourConcentrationLimits = { 0, 5, 10, 35, 60, 90, 120, 150 };
        /// <summary>
        /// 臭氧（O3）8小时滑动平均浓度限值
        /// </summary>
        private static int[] O3DayConcentrationLimits = { 0, 100, 160, 215, 265, 800 };
        /// <summary>
        /// 臭氧（O3）1小时平均浓度限值
        /// </summary>
        private static int[] O3HourConcentrationLimits = { 0, 160, 200, 300, 400, 800, 1000, 1200 };
        /// <summary>
        /// 细颗粒物（PM2.5）24小时平均浓度限值
        /// </summary>
        private static int[] PM25DayConcentrationLimits = { 0, 35, 75, 115, 150, 250, 350, 500 };
        /// <summary>
        /// 细颗粒物（PM2.5）24小时平均浓度限值
        /// </summary>
        private static int[] PM25HourConcentrationLimits = { 0, 35, 75, 115, 150, 250, 350, 500 };
        #endregion
        #region 其他
        /// <summary>
        /// 首要污染物限值
        /// </summary>
        private static int primaryPollutantLimit = 50;
        /// <summary>
        /// 超标污染物限值
        /// </summary>
        private static int nonAttainmentPollutantLimit = 100;
        /// <summary>
        /// IAirData属性
        /// </summary>
        private static PropertyInfo[] IAirDataProperties;
        /// <summary>
        /// IAQIReport分指数属性
        /// </summary>
        private static Dictionary<string, PropertyInfo> IAQIPropertiesDic;
        /// <summary>
        /// 对健康影响情况数组
        /// </summary>
        private static string[] effects;
        /// <summary>
        /// 建议采取的措施数组
        /// </summary>
        private static string[] measures;
        /// <summary>
        /// 日报浓度限值字典
        /// </summary>
        private static Dictionary<string, int[]> dayConcentrationLimitsDic;
        /// <summary>
        /// 实时报浓度限值字典
        /// </summary>
        private static Dictionary<string, int[]> hourConcentrationLimitsDic;
        #endregion
        #endregion

        static AQIHelper()
        {
            effects = new string[] { "空气质量令人满意，基本无空气污染", "空气质量可接受，但某些污染物可能对极少数异常敏感人群健康有较弱影响", "易感人群症状有轻度加剧，健康人群出现刺激症状", "进一步加剧易感人群症状，可能对健康人群心脏、呼吸系统有影响", "心脏病和肺病患者症状显著加剧，运动耐受力降低，健康人群普遍出现症状", "健康人群运动耐受力降低，有明显强烈症状，提前出现某些疾病" };
            measures = new string[] { "各类人群可正常活动", "极少数异常敏感人群应减少户外活动", "儿童、老年人及心脏病、呼吸系统疾病患者应减少长时间、高强度的户外锻炼", "儿童、老年人及心脏病、呼吸系统疾病患者避免长时间、高强度的户外锻炼，一般人群适量减少户外运动", "儿童、老年人和心脏病、肺病患者应停留在室内，停止户外运动，一般人群减少户外运动", "儿童、老年人和病人应当留在室内，避免体力消耗，一般人群应避免户外活动" };
            dayConcentrationLimitsDic = new Dictionary<string, int[]>(){
                {"SO2",SO2DayConcentrationLimits},
                {"NO2",NO2DayConcentrationLimits},
                {"PM10",PM10DayConcentrationLimits},
                {"CO",CODayConcentrationLimits},
                {"O3",O3DayConcentrationLimits},
                {"PM25",PM25DayConcentrationLimits}
            };
            hourConcentrationLimitsDic = new Dictionary<string, int[]>(){
                {"SO2",SO2HourConcentrationLimits},
                {"NO2",NO2HourConcentrationLimits},
                {"PM10",PM10HourConcentrationLimits},
                {"CO",COHourConcentrationLimits},
                {"O3",O3HourConcentrationLimits},
                {"PM25",PM25HourConcentrationLimits}
            };
            IAirDataProperties = typeof(IAirData).GetProperties();
            PropertyInfo[] IAQIReportProperties = typeof(IAQIReport).GetProperties();
            IAQIPropertiesDic = new Dictionary<string, PropertyInfo>();
            foreach (string pollutant in ParameterHelper.PollutantDic.Keys)
            {
                IAQIPropertiesDic.Add(pollutant, IAQIReportProperties.First(o => o.Name == string.Format("I{0}", pollutant)));
            }
        }

        #region 私有方法
        /// <summary>
        /// 计算IAQI字典
        /// </summary>
        /// <param name="data">空气质量指数计算接口</param>
        /// <param name="concentrationLimitsDic">浓度限值数组字典</param>
        /// <returns></returns>
        private static Dictionary<string, int?> GetIAQIDic(IAirData data, Dictionary<string, int[]> concentrationLimitsDic)
        {
            Dictionary<string, int?> IAQIDic = new Dictionary<string, int?>();
            foreach (PropertyInfo property in IAirDataProperties)
            {
                object value = property.GetValue(data, null);
                if (value != null)
                {
                    IAQIDic.Add(property.Name, GetIAQI(concentrationLimitsDic[property.Name], (decimal)value));
                }
            }
            return IAQIDic;
        }

        /// <summary>
        /// 计算IAQI
        /// </summary>
        /// <param name="data">空气质量指数日报/实时报接口</param>
        /// <param name="IAQIDic">空气质量分指数字典</param>
        private static void CalculateIAQI(IAQIReport data, Dictionary<string, int?> IAQIDic)
        {
            foreach (var iaqi in IAQIDic)
            {
                IAQIPropertiesDic[iaqi.Key].SetValue(data, iaqi.Value, null);
            }
        }

        /// <summary>
        /// 计算AQI、PrimaryPollutant等
        /// </summary>
        /// <param name="data">空气质量指数计算接口</param>
        /// <param name="IAQIDic">空气质量分指数字典</param>
        private static void CalculateAQI(IAQICalculate data, Dictionary<string, int?> IAQIDic)
        {
            #region 根据IAQI计算AQI、PrimaryPollutantList
            if (IAQIDic.Any())
            {
                data.AQI = IAQIDic.Max(t => t.Value);
                if (data.AQI.HasValue && data.AQI.Value > primaryPollutantLimit)
                {
                    IEnumerable<string> primaryPollutants = IAQIDic.Where(o => o.Value == data.AQI && o.Value.Value > primaryPollutantLimit).Select(o => o.Key);
                    data.PrimaryPollutant = string.Join(",", ParameterHelper.PollutantDic.Where(o => primaryPollutants.Contains(o.Key)).Select(o => o.Value));
                }
            }
            #endregion
            #region 根据AQI计算AQI级别、类别、颜色
            CalculateAQIAbout(data);
            #endregion
        }

        /// <summary>
        /// 根据AQI计算AQI级别、类别、颜色
        /// </summary>
        /// <param name="data"></param>
        private static void CalculateAQIAbout(IAQICalculate data)
        {
            if (data.AQI.HasValue)
            {
                int level = GetIAQILevel(data.AQI.Value);
                data.Level = Enum.GetName(typeof(AQILevel), level);
                data.Type = Enum.GetName(typeof(AQIType), level);
                data.Color = Enum.GetName(typeof(AQIColor), level);
            }
        }

        /// <summary>
        /// 根据IAQI计算IAQILevel
        /// </summary>
        /// <param name="iaqi">IAQI</param>
        /// <returns></returns>
        private static int GetIAQILevel(int iaqi)
        {
            int level = 0;
            for (int i = 1; i < IAQILimits.Length; i++)
            {
                if (iaqi < IAQILimits[i])
                {
                    level = i - 1;
                    break;
                }
            }
            return level;
        }

        /// <summary>
        /// 计算IAQI
        /// </summary>
        /// <param name="concentrationLimits">浓度限值数组</param>
        /// <param name="value">浓度值</param>
        /// <returns>IAQI</returns>
        private static int? GetIAQI(int[] concentrationLimits, decimal value)
        {
            int? result = null;
            if (value >= 0)
            {
                for (int i = 1; i < concentrationLimits.Length; i++)
                {
                    if (value <= concentrationLimits[i])
                    {
                        result = (int)Math.Ceiling((IAQILimits[i] - IAQILimits[i - 1]) * (value - concentrationLimits[i - 1]) / (concentrationLimits[i] - concentrationLimits[i - 1])) + IAQILimits[i - 1];
                        break;
                    }
                }
            }
            return result;
        }
        #endregion
        #region 公开方法
        /// <summary>
        /// 计算日均AQI
        /// </summary>
        /// <param name="data">空气质量指数计算接口</param>
        public static void CalculateDayAQI(IAQICalculate data)
        {
            #region 计算IAQI
            Dictionary<string, int?> IAQIDic = GetIAQIDic(data, dayConcentrationLimitsDic);
            #endregion
            #region 计算AQI、PrimaryPollutant等
            CalculateAQI(data, IAQIDic);
            #endregion
        }

        /// <summary>
        /// 计算日均AQI
        /// </summary>
        /// <param name="data">空气质量指数日报接口</param>
        public static void CalculateDayAQI(IAQIReport data)
        {
            #region 计算IAQI
            Dictionary<string, int?> IAQIDic = GetIAQIDic(data, dayConcentrationLimitsDic);
            #endregion
            #region 赋值IAQI
            CalculateIAQI(data, IAQIDic);
            #endregion
            #region 计算AQI、PrimaryPollutant等
            CalculateAQI(data, IAQIDic);
            #endregion
        }

        /// <summary>
        /// 计算小时AQI
        /// </summary>
        /// <param name="data">空气质量指数计算接口</param>
        public static void CalculateHourAQI(IAQICalculate data)
        {
            #region 计算IAQI
            Dictionary<string, int?> IAQIDic = GetIAQIDic(data, hourConcentrationLimitsDic);
            #endregion
            #region 计算AQI、PrimaryPollutant等
            CalculateAQI(data, IAQIDic);
            #endregion
        }

        /// <summary>
        /// 计算小时AQI
        /// </summary>
        /// <param name="data">空气质量指数实时报接口</param>
        public static void CalculateHourAQI(IAQIReport data)
        {
            #region 计算IAQI
            Dictionary<string, int?> IAQIDic = GetIAQIDic(data, hourConcentrationLimitsDic);
            #endregion
            #region 赋值IAQI
            CalculateIAQI(data, IAQIDic);
            #endregion
            #region 计算AQI、PrimaryPollutant等
            CalculateAQI(data, IAQIDic);
            #endregion
        }

        /// <summary>
        /// 计算日均IAQI
        /// </summary>
        /// <param name="data">空气质量指数日报接口</param>
        public static void CalculateDayIAQI(IAQIReport data)
        {
            Dictionary<string, int?> IAQIDic = GetIAQIDic(data, dayConcentrationLimitsDic);
            CalculateIAQI(data, IAQIDic);
        }

        /// <summary>
        /// 计算小时IAQI
        /// </summary>
        /// <param name="data">空气质量指数实时报接口</param>
        public static void CalculateHourIAQI(IAQIReport data)
        {
            Dictionary<string, int?> IAQIDic = GetIAQIDic(data, hourConcentrationLimitsDic);
            CalculateIAQI(data, IAQIDic);
        }

        /// <summary>
        /// 计算日均IAQI
        /// </summary>
        /// <param name="pollutant">污染物监测项</param>
        /// <param name="value">浓度值</param>
        /// <returns></returns>
        public static int? GetDayIAQI(string pollutant, decimal? value)
        {
            int? iaqi;
            if (value.HasValue && ParameterHelper.PollutantDic.ContainsKey(pollutant))
            {
                iaqi = GetIAQI(dayConcentrationLimitsDic[pollutant], value.Value);
            }
            else
            {
                iaqi = null;
            }
            return iaqi;
        }

        /// <summary>
        /// 计算小时IAQI
        /// </summary>
        /// <param name="pollutant">污染物监测项</param>
        /// <param name="value">浓度值</param>
        /// <returns></returns>
        public static int? GetHourIAQI(string pollutant, decimal? value)
        {
            int? iaqi;
            if (value.HasValue && ParameterHelper.PollutantDic.ContainsKey(pollutant))
            {
                iaqi = GetIAQI(hourConcentrationLimitsDic[pollutant], value.Value);
            }
            else
            {
                iaqi = null;
            }
            return iaqi;
        }
        #endregion
    }

    /// <summary>
    /// 空气质量指数级别
    /// </summary>
    public enum AQILevel
    {
        /// <summary>
        /// 优
        /// </summary>
        一级,
        /// <summary>
        /// 良
        /// </summary>
        二级,
        /// <summary>
        /// 轻度污染
        /// </summary>
        三级,
        /// <summary>
        /// 中度污染
        /// </summary>
        四级,
        /// <summary>
        /// 重度污染
        /// </summary>
        五级,
        /// <summary>
        /// 严重污染
        /// </summary>
        六级
    }

    /// <summary>
    /// 空气质量指数类别
    /// </summary>
    public enum AQIType
    {
        /// <summary>
        /// 一级
        /// </summary>
        优,
        /// <summary>
        /// 二级
        /// </summary>
        良,
        /// <summary>
        /// 三级
        /// </summary>
        轻度污染,
        /// <summary>
        /// 四级
        /// </summary>
        中度污染,
        /// <summary>
        /// 五级
        /// </summary>
        重度污染,
        /// <summary>
        /// 六级
        /// </summary>
        严重污染
    }

    /// <summary>
    /// 空气质量指数类别颜色
    /// </summary>
    public enum AQIColor
    {
        /// <summary>
        /// 0,228,0   #00E400
        /// </summary>
        绿色,
        /// <summary>
        /// 255,255,0   #FFFF00
        /// </summary>
        黄色,
        /// <summary>
        /// 255,126,0   #FF7E00
        /// </summary>
        橙色,
        /// <summary>
        /// 255,0,0   #FF0000
        /// </summary>
        红色,
        /// <summary>
        /// 153,0,76   #99004C
        /// </summary>
        紫色,
        /// <summary>
        /// 126,0,35   #7E0023
        /// </summary>
        褐红色
    }
}
