using System;
namespace CLDC_DataCore.WuChaDeal
{
    class SDTQError:WuChaBase
        {

         public SDTQError(Struct.StWuChaDeal wuChaDeal)
             : base(wuChaDeal)
         { 
            
         }

        /// <summary>
        /// 计算时段投切误差
        /// </summary>
        /// <param name="t1">理论时间</param>
        /// <param name="t2">实际时间</param>
        /// <returns>返回</returns>
        public Struct.StWuChaResult getWuCha(DateTime t1, DateTime t2)
            {

            Struct.StWuChaResult stResult = new Struct.StWuChaResult();
            int intPastSecond = CLDC_DataCore.Function.DateTimes.DateDiff(t1, t2);
            stResult.Result =CLDC_DataCore.Function.Common.ConverResult( intPastSecond <= 300 ? true : false);
            stResult.Data = String.Format("{0}|{1}|{2}", t1.TimeOfDay.ToString(), t2.TimeOfDay.ToString(), intPastSecond.ToString());
            return stResult;
            }
        }
    }
