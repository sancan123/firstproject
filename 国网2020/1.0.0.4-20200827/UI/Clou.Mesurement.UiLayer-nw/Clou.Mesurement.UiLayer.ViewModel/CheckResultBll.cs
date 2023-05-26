using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.DAL.DataBaseView;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using System.Collections.Generic;
using System.Linq;

namespace Mesurement.UiLayer.ViewModel
{
    /// 检定结论存取业务类
    /// <summary>
    /// 检定结论存取业务类
    /// </summary>
    public class CheckResultBll
    {
        #region 单例
        private static CheckResultBll instance = null;
        public static CheckResultBll Instance
        {
            get
            {
                if (instance == null)
                { instance = new CheckResultBll(); }
                return instance;
            }
        }
        #endregion
        #region 私有成员
        /// <summary>
        /// 台体编号
        /// </summary>
        private string equipNo
        {
            get { return EquipmentData.Equipment.ID; }
        }
        /// 要检表编号列表,从1开始
        /// <summary>
        /// 要检表编号列表,从1开始
        /// </summary>
        private List<int> GetYaoJianList()
        {
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            List<int> listTemp = new List<int>();
            for (int i = 0; i < EquipmentData.MeterGroupInfo.YaoJian.Length; i++)
            {
                if (yaojianTemp[i])
                {
                    listTemp.Add(i + 1);
                }
            }
            return listTemp;
        }
        /// 获取主结论视图
        /// <summary>
        /// 获取主结论视图
        /// </summary>
        /// <param name="displayModel"></param>
        /// <returns></returns>
        private Dictionary<string, Dictionary<string, List<string>>> GetResultView(TableDisplayModel displayModel)
        {
            Dictionary<string, Dictionary<string, List<string>>> dictionaryAll = new Dictionary<string, Dictionary<string, List<string>>>();
            #region 主结论视图字典
            Dictionary<string, List<string>> pkDictionary = new Dictionary<string, List<string>>();
            if (displayModel.ColumnModelList.Count > 0)
            {
                for (int i = 0; i < displayModel.ColumnModelList.Count; i++)
                {
                    string fieldName = displayModel.ColumnModelList[i].Field;
                    string[] nameArray = displayModel.ColumnModelList[i].DisplayName.Split('|');
                    if (pkDictionary.ContainsKey(fieldName))
                    {
                        pkDictionary[fieldName].AddRange(nameArray);
                    }
                    else
                    {
                        pkDictionary.Add(fieldName, new List<string>(nameArray));
                    }
                }
                dictionaryAll.Add("", pkDictionary);
            }
            #endregion
            #region 副结论视图字典
            for (int i = 0; i < displayModel.FKDisplayModelList.Count; i++)
            {
                string fkKey = "_" + displayModel.FKDisplayModelList[i].Key;
                string fkField = displayModel.FKDisplayModelList[i].Field;
                List<string> nameList = displayModel.FKDisplayModelList[i].DisplayNames;
                if (!dictionaryAll.ContainsKey(fkKey))
                {
                    dictionaryAll.Add(fkKey, new Dictionary<string, List<string>>());
                }
                Dictionary<string, List<string>> fieldDictionary = dictionaryAll[fkKey];
                if (fieldDictionary.ContainsKey(fkField))
                {
                    fieldDictionary[fkField].AddRange(nameList);
                }
                else
                {
                    fieldDictionary.Add(fkField, nameList);
                }
            }
            #endregion

            return dictionaryAll;
        }
        #endregion
        #region 公用的,存取检定结论
        /// <summary>
        /// 从数据库加载某一点的检定结论,仅限从临时库加载,因为查询条件里面没有表的唯一ID
        /// </summary>
        /// <param name="checkNode">要加载的检定点</param>
        public void LoadCheckResult(CheckNodeViewModel checkNode)
        {
            #region 加载视图
            string itemKey = checkNode.ItemKey;
            TableDisplayModel displayModel = checkNode.DisplayModel;
            if (displayModel == null)
            { return; }
            List<string> keyList = new List<string>();
            string tableName = string.Format("TMP_{0}", displayModel.TableName);
            Dictionary<string, Dictionary<string, List<string>>> dictionaryAll = GetResultView(displayModel);
            for (int i = 0; i < dictionaryAll.Count; i++)
            {
                keyList.Add(string.Format("{0}{1}", itemKey, dictionaryAll.Keys.ElementAt(i)));
            }
            if (keyList.Count == 0)
            { return; }
            #endregion
            #region 获取数据库中检定点的结论
            List<DynamicModel> resultRows = new List<DynamicModel>();
            for (int i = 0; i < keyList.Count; i++)
            {
                string where = string.Format(" AVR_DEVICE_ID = '{0}' and AVR_PROJECT_NO = '{1}'", equipNo, keyList[i]);
                resultRows.AddRange(DALManager.MeterTempDbDal.GetList(tableName, where));
            }
            #endregion
            #region 将获取到的每一个结论加载到结论模型中
            for (int i = 0; i < resultRows.Count; i++)
            {
                DynamicModel resultRow = resultRows[i];
                #region 提取表位号
                int meterIndex = 1;
                //
                if (!int.TryParse(resultRow.GetProperty("LNG_BENCH_POINT_NO").ToString(), out meterIndex) || meterIndex < 1 || meterIndex > checkNode.CheckResults.Count)
                {
                    continue;
                }
                meterIndex = meterIndex - 1;
                #endregion
                #region 提取结论视图
                string itemKeyTemp = resultRow.GetProperty("AVR_PROJECT_NO") as string;
                int keyIndex = keyList.IndexOf(itemKeyTemp);
                if (keyIndex < 0)
                {
                    continue;
                }
                Dictionary<string, List<string>> fieldDictionary = dictionaryAll.Values.ElementAt(keyIndex);
                #endregion
                DynamicViewModel checkResult = checkNode.CheckResults[meterIndex];
                #region 解析结论
                for (int j = 0; j < fieldDictionary.Count; j++)
                {
                    string fieldName = fieldDictionary.Keys.ElementAt(j);
                    string fieldValue = resultRow.GetProperty(fieldName) as string;
                    if (string.IsNullOrEmpty(fieldValue))
                    { continue; }
                    string[] valueArray = fieldValue.Split('^');
                    //显示名称列表
                    List<string> displayNameList = fieldDictionary.Values.ElementAt(j);
                    for (int k = 0; k < displayNameList.Count; k++)
                    {
                        if (valueArray.Length > k)
                        {
                            checkResult.SetProperty(displayNameList[k], valueArray[k]);
                        }
                    }
                }
                #endregion
            }
            #endregion
        }
        /// <summary>
        /// 保存检定结论
        ///采用先删除后插入的方式保存
        /// </summary>
        public void SaveCheckResult(CheckNodeViewModel checkNode)
        {
            #region 加载视图
            string itemKey = checkNode.ItemKey;
            List<int> yaoJianList = GetYaoJianList();
            if (yaoJianList.Count == 0)
            { return; }
            TableDisplayModel displayModel = checkNode.DisplayModel;
            if (displayModel == null)
            { return; }
            List<string> keyList = new List<string>();
            string tableName = string.Format("TMP_{0}", displayModel.TableName);
            Dictionary<string, Dictionary<string, List<string>>> dictionaryAll = GetResultView(displayModel);
            for (int i = 0; i < dictionaryAll.Count; i++)
            {
                keyList.Add(string.Format("{0}{1}", itemKey, dictionaryAll.Keys.ElementAt(i)));
            }
            if (keyList.Count == 0)
            { return; }
            #endregion
            List<string> sqlList = new List<string>();
            #region 获取删除检定结论的Sql语句
            //FK_LNG_METER_ID 所检表的唯一编号
            //LNG_BENCH_POINT_NO 表位号    这三个为前提
            //AVR_PROJECT_NO 检定点编号
            //AVR_DEVICE_ID 台体编号
            for (int i = 0; i < keyList.Count; i++)
            {
                for (int j = 0; j < yaoJianList.Count; j++)
                {
                    sqlList.Add(string.Format("delete from {0} where AVR_DEVICE_ID = '{1}' and AVR_PROJECT_NO = '{2}' and LNG_BENCH_POINT_NO ={3}", tableName, equipNo, keyList[i], yaoJianList[j]));
                }
            }
            #endregion
            #region 插入检定结论
            //结论编号循环
            for (int keyIndex = 0; keyIndex < dictionaryAll.Count; keyIndex++)
            {
                Dictionary<string, List<string>> fieldDictionary = dictionaryAll.Values.ElementAt(keyIndex);
                if (displayModel.ColumnModelList.Count > 0)
                {
                    //所有要检表
                    for (int i = 0; i < yaoJianList.Count; i++)
                    {
                        List<string> fieldValues = new List<string>();
                        //要存储的数据库字段名列表
                        for (int j = 0; j < fieldDictionary.Count; j++)
                        {
                            List<string> valueList = new List<string>();
                            string fieldTemp = fieldDictionary.Keys.ElementAt(j);
                            //字段的值
                            for (int k = 0; k < fieldDictionary[fieldTemp].Count; k++)
                            {
                                valueList.Add(checkNode.CheckResults[yaoJianList[i] - 1].GetProperty(fieldDictionary[fieldTemp][k]) as string);
                            }
                            fieldValues.Add(string.Format("'{0}'", string.Join("^", valueList)));
                        }
                        object meterId = EquipmentData.MeterGroupInfo.Meters[yaoJianList[i] - 1].GetProperty("PK_LNG_METER_ID");
                        sqlList.Add(string.Format("insert into {0} (FK_LNG_METER_ID,AVR_PROJECT_NAME,AVR_DEVICE_ID,AVR_PROJECT_NO,LNG_BENCH_POINT_NO,{1}) values ('{2}','{7}','{3}','{4}',{5},{6})", tableName, string.Join(",", fieldDictionary.Keys), meterId, equipNo, keyList[keyIndex], yaoJianList[i], string.Join(",", fieldValues), checkNode.Name));
                        
                    }
                }
            }
            #endregion

            DALManager.MeterTempDbDal.ExecuteOperation(sqlList);
            if (EquipmentData.CheckResults.ResultCollection.Count > EquipmentData.Controller.Index && EquipmentData.Controller.Index > 0)
            {
                LogManager.AddMessage(string.Format("更新检定项 {0} 的结论", EquipmentData.CheckResults.ResultCollection[EquipmentData.Controller.Index].Name), EnumLogSource.数据库存取日志);
            }
        }
        #endregion
        #region 单个表的检定结论
        /// <summary>
        /// 从数据库加载一块表的所有结论
        /// </summary>
        /// <param name="isTemp">true:临时库;false:正式库</param>
        /// <param name="meterPK">单个表检定结论的唯一编号</param>
        /// <returns>未被提取的结论</returns>
        private List<DynamicModel> LoadOneModels(bool isTemp, string meterPK)
        {
            List<DynamicModel> models = new List<DynamicModel>();
            GeneralDal dal = isTemp ? DALManager.MeterTempDbDal : DALManager.MeterDbDal;
            List<string> tableNames = dal.GetTableNames();
            if(isTemp)
            {
                tableNames.Remove("TMP_METER_INFO");
            }
            else
            {
                tableNames.Remove("METER_INFO");
            }
            List<string> sqlList = new List<string>();
            for (int i = 0; i < tableNames.Count; i++)
            {
                sqlList.Add(string.Format("select * from {0} where FK_LNG_METER_ID='{1}'", tableNames[i], meterPK));
            }
            models = dal.GetList(tableNames, sqlList);
            return models;
        }
        /// <summary>
        /// 加载单个表位单个检定项的检定结论
        /// </summary>
        /// <param name="isTemp">true:临时库;false:正式库</param>
        /// <param name="meterPK">单个表检定结论的唯一编号</param>
        /// <param name="checkKey">检定点编号</param>
        /// <param name="models">数据库中的行</param>
        /// <returns>编号对应的详细结论</returns>
        private Dictionary<string, string> LoadOneResult(string meterPK, string checkKey, List<DynamicModel> models)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string paraNo = checkKey.Split('_')[0];
            TableDisplayModel displayModel = ResultViewHelper.GetParaNoDisplayModel(paraNo);
            if (displayModel == null)
            {
                return dictionary;
            }
            List<string> keyList = new List<string>();
            Dictionary<string, Dictionary<string, List<string>>> dictionaryAll = GetResultView(displayModel);
            for (int i = 0; i < dictionaryAll.Count; i++)
            {
                keyList.Add(string.Format("{0}{1}", checkKey, dictionaryAll.Keys.ElementAt(i)));
            }

            for (int i = 0; i < keyList.Count; i++)
            {
                DynamicModel resultRow = models.FirstOrDefault(item => (item.GetProperty("AVR_PROJECT_NO") as string) == keyList[i]);
                if (resultRow == null)
                { continue; }
                if (i == 0)
                {
                    if (dictionary.ContainsKey("项目号"))
                    {
                        dictionary["项目号"] = keyList[0];
                    }
                    else
                    {
                        dictionary.Add("项目号", keyList[0]);
                    }
                    if (dictionary.ContainsKey("项目名"))
                    {
                        dictionary["项目名"] = resultRow.GetProperty("AVR_PROJECT_NAME") as string;
                    }
                    else
                    {
                        dictionary.Add("项目名", resultRow.GetProperty("AVR_PROJECT_NAME") as string);
                    }
                }
                #region 提取结论视图
                string itemKeyTemp = resultRow.GetProperty("AVR_PROJECT_NO") as string;
                int keyIndex = keyList.IndexOf(itemKeyTemp);
                if (keyIndex < 0)
                {
                    continue;
                }
                Dictionary<string, List<string>> fieldDictionary = dictionaryAll.Values.ElementAt(keyIndex);
                #endregion
                #region 解析结论
                for (int j = 0; j < fieldDictionary.Count; j++)
                {
                    string fieldName = fieldDictionary.Keys.ElementAt(j);
                    string fieldValue = resultRow.GetProperty(fieldName) as string;
                    if (string.IsNullOrEmpty(fieldValue))
                    { continue; }
                    string[] valueArray = fieldValue.Split('^');
                    //显示名称列表
                    List<string> displayNameList = fieldDictionary.Values.ElementAt(j);
                    for (int k = 0; k < displayNameList.Count; k++)
                    {
                        if (valueArray.Length > k)
                        {
                            if (dictionary.ContainsKey(displayNameList[k]))
                            {
                                dictionary[displayNameList[k]] = valueArray[k];
                            }
                            else
                            {
                                dictionary.Add(displayNameList[k], valueArray[k]);
                            }
                        }
                    }
                }
                #endregion
            }

            return dictionary;
        }
        /// <summary>
        /// 一块表的结论字典
        /// </summary>
        /// <param name="isTemp">临时库还是正式库</param>
        /// <param name="meterPk">表结论的唯一编号</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> LoadOneResult(bool isTemp, string meterPk)
        {
            Dictionary<string, Dictionary<string, string>> dictionaryAll = new Dictionary<string, Dictionary<string, string>>();
            List<DynamicModel> models = LoadOneModels(isTemp, meterPk);
            for (int i = 0; i < models.Count; i++)
            {
                string keyTemp = models[i].GetProperty("AVR_PROJECT_NO") as string;
                if (IsPrimaryKey(keyTemp))
                {
                    Dictionary<string, string> dictionaryTemp = LoadOneResult(meterPk, keyTemp, models);
                    if (dictionaryAll.ContainsKey(keyTemp))
                    {
                        dictionaryAll[keyTemp] = dictionaryTemp;
                    }
                    else
                    {
                        dictionaryAll.Add(keyTemp, dictionaryTemp);
                    }
                }
            }
            return dictionaryAll;
        }

        /// <summary>
        /// 检定店编号是否为主结论编号
        /// </summary>
        /// <param name="checkKey"></param>
        /// <returns></returns>
        private bool IsPrimaryKey(string checkKey)
        {
            if (string.IsNullOrEmpty(checkKey))
            {
                return false;
            }
            string[] arrayTemp = checkKey.Split('_');
            string paraNo = arrayTemp[0];
            DynamicModel model = SchemaFramework.GetParaFormat(paraNo);
            if (model == null)
            {
                return false;
            }
            else
            {
                string keyFormat = model.GetProperty("PARA_KEY_RULE") as string;

                if (string.IsNullOrEmpty(keyFormat))
                {
                    return arrayTemp.Length == 1;
                }
                else
                {
                    string[] arrayKeyRule = keyFormat.Split('|');
                    for (int i = 0; i < arrayKeyRule.Length; i++)
                    {
                        bool isParaKey = false;
                        if (bool.TryParse(arrayKeyRule[i], out isParaKey) && isParaKey)
                        {
                            return arrayTemp.Length == 2;
                        }
                    }
                    return arrayTemp.Length == 1;
                }
            }
        }
        #endregion

        /// <summary>
        /// 删除临时数据库中的表结论
        /// </summary>
        public void DeleteResultFromTempDb()
        {
            #region 获取删除条件
            string whereDelete = string.Format("avr_device_id='{0}'", EquipmentData.Equipment.ID);// string.Join(" or ", whereConditions);
            #endregion
            List<string> sqlList = new List<string>();
            List<string> tableNames = DALManager.MeterTempDbDal.GetTableNames();
            for (int i = 0; i < tableNames.Count; i++)
            {
                if (tableNames[i].ToLower().Contains("meter_info"))
                {
                    continue;
                }
                else
                {
                    sqlList.Add(string.Format("delete from {0} where {1}", tableNames[i], whereDelete));
                }
            }
            int countDelete = DALManager.MeterTempDbDal.ExecuteOperation(sqlList);
            LogManager.AddMessage(string.Format("从临时库删除上一次的检定结论完成,共删除{0}条记录", countDelete), EnumLogSource.数据库存取日志);
        }
    }
}
