using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesurement.UiLayer.ViewModel.WcfService
{
    public class MisDataService : IMisData
    {
        private int idRandom = -1;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int RequestId()
        {
            Random random = new Random(100000);
            idRandom = random.Next(999999);
            return idRandom;
        }
        /// <summary>
        /// 根据条码号获取表唯一编号
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="isHistory"></param>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetMeterPk(int connectionId, bool isHistory, string barCode)
        {
            GeneralDal dal = isHistory ? DALManager.MeterDbDal : DALManager.MeterTempDbDal;
            string tableName = isHistory ? "meter_info" : "tmp_meter_info";
            Dictionary<string, string> dictionaryResult = new Dictionary<string, string>();
            List<DynamicModel> models = dal.GetList(tableName, string.Format("AVR_BAR_CODE='{0}' order by DTM_TEST_DATE", barCode));
            for (int i = 0; i < models.Count; i++)
            {
                string meterPk = models[i].GetProperty("PK_LNG_METER_ID") as string;
                string testTime = models[i].GetProperty("DTM_TEST_DATE") as string;
                if (!string.IsNullOrEmpty(meterPk))
                {
                    dictionaryResult.Add(meterPk, testTime);
                }
            }
            return dictionaryResult;
        }
        /// <summary>
        /// 获取一块表的检定结论
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="isHistory">是否为历史记录</param>
        /// <param name="meterPk">表唯一编号</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> GetOneMeterResult(int connectionId, bool isHistory, string meterPk)
        {
            //if (connectionId != idRandom)
            //{
            //    return null;
            //}
            Dictionary<string, Dictionary<string, string>> dictionaryResult = new Dictionary<string, Dictionary<string, string>>();
            OneMeterResult meterResult = new OneMeterResult(meterPk, !isHistory);
            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                Model.AsyncObservableCollection<string> namesTemp = meterResult.Categories[i].Names;
                for (int j = 0; j < meterResult.Categories[i].ResultUnits.Count; j++)
                {
                    DynamicViewModel modelTemp = meterResult.Categories[i].ResultUnits[j];
                    string itemKey = modelTemp.GetProperty("项目号") as string;
                    if (!string.IsNullOrEmpty(itemKey) && !dictionaryResult.ContainsKey(itemKey))
                    {
                        Dictionary<string, string> dictionaryTemp = new Dictionary<string, string>();
                        for (int k = 0; k < namesTemp.Count; k++)
                        {
                            if (string.IsNullOrEmpty(namesTemp[k]) || dictionaryTemp.ContainsKey(namesTemp[k]))
                            {
                                continue;
                            }
                            dictionaryTemp.Add(namesTemp[k], modelTemp.GetProperty(namesTemp[k]) as string);
                        }
                        dictionaryResult.Add(itemKey, dictionaryTemp);
                    }
                }
            }
            return dictionaryResult;
        }

        /// <summary>
        /// 根据表的号码查出表唯一编号
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="isHistory"></param>
        /// <param name="codeType"></param>
        /// <param name="codeValue"></param>
        /// <param name="checkDate"></param>
        /// <returns></returns>
        private bool GetMeterPkByCodeType(int codeType, string codeValue, string checkDate, ref string PK_LNG_METER_ID, ref string DTM_TEST_DATE)
        {
            GeneralDal dal = DALManager.MeterDbDal;
            string tableName = "meter_info";

            try
            {
                List<DynamicModel> MeterInfoMdels = null;

                string strSql = "";
                string strSqlDate = "";
                var checkDateTmp = (object)checkDate;
                string orderBy = "order by DTM_TEST_DATE";
                if (!string.IsNullOrEmpty(checkDate))
                {
                    strSqlDate = string.Format("And DTM_TEST_DATE = #{0}# ", checkDateTmp);
                }
                switch (codeType)
                {
                    case 0://0-	出厂编号
                        strSql = string.Format("AVR_MADE_NO='{0}'", codeValue);
                        break;
                    case 1://1-	局资产编号（默认值）
                        strSql = string.Format("AVR_ASSET_NO='{0}'", codeValue);
                        break;
                    case 2://2-	物资资产编号
                        strSql = string.Format("AVR_MADE_NO='{0}'", codeValue);
                        break;
                    default://1-	局资产编号（默认值）
                        strSql = string.Format("AVR_ASSET_NO='{0}'", codeValue);
                        break;
                }
                strSql += strSqlDate + orderBy;
                MeterInfoMdels = dal.GetList(tableName, strSql);
                for (int i = 0; i < MeterInfoMdels.Count; i++)
                {
                    string meterPk = MeterInfoMdels[i].GetProperty("PK_LNG_METER_ID") as string;
                    string testTime = MeterInfoMdels[i].GetProperty("DTM_TEST_DATE") as string;

                    if (!string.IsNullOrEmpty(meterPk))
                    {
                        PK_LNG_METER_ID = meterPk;
                        DTM_TEST_DATE = testTime;
                        return true;
                    }
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        /// <summary>
        /// 根据表唯一编号查出费控结果
        /// </summary>
        /// <param name="strPk"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool GetMeterFkResultByPk(string strPk, out string[] result)
        {
            GeneralDal dal = DALManager.MeterDbDal;
            result = null;
            string tableName = "METER_RATES_CONTROL";
            try
            {
                List<DynamicModel> models = dal.GetList(tableName, string.Format("FK_LNG_METER_ID='{0}' ", strPk));

                if (models != null)
                {
                    string[] resultArr = new string[models.Count];
                    for (int i = 0; i < models.Count; i++)
                    {
                        string AVR_PROJECT_NO = models[i].GetProperty("AVR_PROJECT_NO") as string;
                        string AVR_CONCLUSION = models[i].GetProperty("AVR_CONCLUSION") as string;
                        if (!string.IsNullOrEmpty(AVR_PROJECT_NO) && !string.IsNullOrEmpty(AVR_CONCLUSION))
                        {
                            resultArr[i] = AVR_PROJECT_NO + "|" + AVR_CONCLUSION;
                        }
                    }
                    result = resultArr;
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        /// <summary>
        /// 根据指定参数，查询获取对应电能表费控功能检测结果
        /// </summary>
        /// <param name="codeType">编号类型:0-	出厂编号,1-	局资产编号（默认值）,2-	物资资产编号</param>
        /// <param name="codeValue">编号值</param>
        /// <param name="checkDate">检测日期（允许为空）格式：yyyymmdd</param>
        /// <param name="xmlValue">费控结果：参见XML格式定义</param>
        /// <returns></returns>
        public int GetCostConsByBarcode(int codeType, string codeValue, string checkDate, ref string xmlValue)
        {
            string RetxmlValue = "";
            string strPk = "";
            string strTestDate = "";
            string[] resultArr = null;
            try
            {
                bool result = GetMeterPkByCodeType(codeType, codeValue, checkDate, ref strPk, ref strTestDate);
                if (result)
                {
                    GetMeterFkResultByPk(strPk, out resultArr);
                    if (resultArr != null && resultArr.Length > 0)
                    {
                        RetxmlValue = GetXml(codeValue, strTestDate, resultArr);
                    }
                }
            }
            catch (Exception)
            {
                xmlValue = RetxmlValue;
                return 1;
            }
            xmlValue = RetxmlValue;
            return 0;
        }

        private string ConvertProjectNo(string ProjectNo)
        {
            if (string.IsNullOrEmpty(ProjectNo)) return "";
            string RetProject = "";
            switch (ProjectNo)
            {

                case "13003001":    //初始化
                    RetProject = "F0101";
                    break;
                case "13003002":
                    RetProject = "F0102";
                    break;
                case "13004001":  //开户
                    RetProject = "F0201";
                    break;
                case "13004002":
                    RetProject = "F0202";
                    break;
                case "13004003":
                    RetProject = "F0203";
                    break;
                case "13005001":  //充值
                    RetProject = "F0301";
                    break;
                case "13005002":
                    RetProject = "F0302";
                    break;
                case "13005003":
                    RetProject = "F0303";
                    break;
                case "13006001":    //补卡
                    RetProject = "F0401";
                    break;
                case "13006002":
                    RetProject = "F0402";
                    break;
                case "13007001":    //返写
                    RetProject = "F0501";
                    break;
                case "13007002":
                    RetProject = "F0502";
                    break;
                case "12004001":  //参数更新
                case "13008001":
                    RetProject = "F0601";
                    break;
                case "13008002":
                    RetProject = "F0602";
                    break;
                case "12004002":
                case "13008003":
                    RetProject = "F0603";
                    break;
                case "13008004":
                    RetProject = "F0604";
                    break;
                case "13008006":
                    RetProject = "F0605";
                    break;
                case "13008007":
                    RetProject = "F0606";
                    break;
                case "13008008":
                    RetProject = "F0607";
                    break;
                case "13008009":
                    RetProject = "F0608";
                    break;
                case "13008010":
                    RetProject = "F0609";
                    break;
                case "13008011":
                    RetProject = "F0610";
                    break;
                case "12003001":  //密钥更新
                case "13009001":
                    RetProject = "F0701";
                    break;
                case "12003002":
                case "13009002":
                    RetProject = "F0702";
                    break;
                case "12003003":
                case "13009003":
                    RetProject = "F0703";
                    break;
                case "13010": //数据回抄
                    RetProject = "F0800";
                    break;
                case "12005001"://远程控制
                case "13011001":
                    RetProject = "F0901";
                    break;
                case "12005002":
                case "13011002":
                    RetProject = "F0902";
                    break;
                case "12005003":
                case "13011003":
                    RetProject = "F0903";
                    break;
                case "12005004":
                case "13011004":
                    RetProject = "F0904";
                    break;
                case "12005005":
                    RetProject = "F0905";
                    break;
                case "12005006":
                    RetProject = "F0906";
                    break;
                case "12005007":
                case "13011007":
                    RetProject = "F0907";
                    break;
                case "12005008":
                case "13011008":
                    RetProject = "F0908";
                    break;
                case "13017": //清零
                    RetProject = "F1001";
                    break;
                case "12006":
                    RetProject = "F1002";
                    break;
                case "12007": //模式切换
                    RetProject = "F1001";
                    break;
                case "13012":
                    RetProject = "F1002";
                    break;
                case "13013": //钱包退费
                    RetProject = "F1200";
                    break;
                case "13018001"://费控结算
                    RetProject = "F1301";
                    break;
                case "13018002":
                    RetProject = "F1302";
                    break;
                case "13018003":
                    RetProject = "F1303";
                    break;
                case "13018004":
                    RetProject = "F1304";
                    break;
                case "12002001": //远程身份认证
                case "13001001":
                    RetProject = "F1401";
                    break;
                case "12002002":
                case "13001002":
                    RetProject = "F1402";
                    break;
                case "12002003":
                case "13001003":
                    RetProject = "F1403";
                    break;
                case "12002004":
                case "13001004":
                    RetProject = "F1404";
                    break;
                case "12002005":
                case "13001005":
                    RetProject = "F1405";
                    break;
                case "12002006":
                case "13001006":
                    RetProject = "F1406";
                    break;
                case "13002"://本地身份认证
                    RetProject = "F1500";
                    break;
                case "12009"://红外认证
                case "13015":
                    RetProject = "F1600";
                    break;
                case "13016"://防伪造卡攻击
                    RetProject = "F1700";
                    break;
                case "13019009"://快捷功能
                    RetProject = "F1801";
                    break;
                case "13019001":
                    RetProject = "F1802";
                    break;
                case "12010001":
                    RetProject = "F1803";
                    break;
                case "12010002":
                case "13019002":
                    RetProject = "F1804";
                    break;
                case "12010003":
                case "13019003":
                    RetProject = "F1805";
                    break;
                case "13019004":
                    RetProject = "F1806";
                    break;
                case "12010004":
                    RetProject = "F1807";
                    break;
                case "12010006":
                case "13019007":
                    RetProject = "F1808";
                    break;
                case "12010007":
                case "13019008":
                    RetProject = "F1809";
                    break;
                case "12010005":
                case "13019006":
                    RetProject = "F1810";
                    break;
            }
            return RetProject;
        }

        private string ConvertResult(string Result)
        {
            if (string.IsNullOrEmpty(Result)) return "";
            if (Result == "合格")
            {
                return "0";
            }
            else if (Result == "不合格")
            {
                return "1";
            }
            else
            {
                return "2"; //未检
            }
        }

        private string GetModel(string ProjectNo)
        {
            string strRetData = "";
            if (string.IsNullOrEmpty(ProjectNo)) return strRetData;

            if (ProjectNo.Substring(0, 2) == "12")
            {
                strRetData = "1";
            }
            else
            {
                strRetData = "0";
            }
            return strRetData;
        }

        private string GetXml(string codeValue, string strCheckDate, string[] result)
        {
            string xml = "";
            StringBuilder sb = new StringBuilder();
            if (result != null && result.Length > 0)
            {
                sb.Append("<RECORD>");
                sb.Append("<R Idex='1'>");
                sb.Append("<IDENTIFIER>" + codeValue + "</IDENTIFIER>");
                sb.Append("<CHECKDATE>" + strCheckDate + "</CHECKDATE>");
                for (int i = 0; i < result.Length; i++)
                {
                    string strProjectNo = ConvertProjectNo(result[i].Split('|')[0]);
                    string strResult = ConvertResult(result[i].Split('|')[1]);
                    string strModel = GetModel(result[i].Split('|')[0]);
                    sb.Append("<ITEM ID='" + strProjectNo + "'");
                    sb.Append(" RESULTFLAG='" + strResult + "'");
                    sb.Append(" MODELFLAG='" + strModel + "'");
                    sb.Append("/>");
                }
                sb.Append("</R>");
                sb.Append("</RECORD>");
                xml = sb.ToString();
            }
            return xml;
        }
    }
}
