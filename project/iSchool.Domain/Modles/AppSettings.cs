using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Domain.Modles
{
    public class AppSettings
    {
        public static bool IsDebugMode
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// 权限过滤是否开启
        /// </summary>
        public bool IsQxFilterOpened { get; set; }
        /// <summary>
        /// 上传url
        /// </summary>
        public string UploadUrl { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// CDN文件路径
        /// </summary>
        public string CDNFileUrl { get; set; }
        /// <summary>
        /// 登录url
        /// </summary>
        public string LoginUrl { get; set; }
        /// <summary>
        /// 运营平台url
        /// </summary>
        public string OperationPlatformUrl { get; set; }

        /// <summary>
        /// DataApiUrl
        /// </summary>
        public string DataApiUrl { get; set; }

        /// <summary>
        /// 用于查询编辑角色的所有user
        /// </summary>
        public Guid GidEditor { get; set; }
        /// <summary>
        /// 用于查询审核角色的所有user
        /// </summary>
        public Guid GidAuditor { get; set; }
        /// <summary>
        /// 用于查询拥有编辑权限的所有user
        /// </summary>
        public Guid GidQxEdit { get; set; }
        /// <summary>
        /// 用于查询拥有审核权限的所有user
        /// </summary>
        public Guid GidQxAudit { get; set; }
        /// <summary>
        /// 用于查询拥有审核权限的所有user
        /// </summary>
        public string QqMapKey { get; set; }
        /// <summary>
        /// ESearchUrl
        /// </summary>
        public string ESearchUrl { get; set; }
    }
}
