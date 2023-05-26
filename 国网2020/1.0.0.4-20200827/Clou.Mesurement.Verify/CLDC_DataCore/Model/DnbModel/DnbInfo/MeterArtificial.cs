using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_DataCore.Model.DnbModel.DnbInfo
{
    /// <summary>
    /// 人工检定数据结论
    /// </summary>
    [Serializable()]
    public class MeterArtificial : MeterErrorBase
    {
        /// <summary>
        /// 检定结论
        /// </summary>
        public string Md_jl;

        /// <summary>
        /// 检定数据
        /// </summary>
        public string md_jdsj;

    }
}
