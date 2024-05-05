﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace iSchool.Svs.Domain.Enum
{
    public enum ResponseCode
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功")]
        Success = 200,
        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败")]
        Failed = 201,
        /// <summary>
        /// 没有登录
        /// </summary>
        [Description("没有登录")]
        NoLogin = 402,
        /// <summary>
        /// 权限不足
        /// </summary>
        [Description("权限不足")]
        NoAuth = 403,
        /// <summary>
        /// 会员权限不足
        /// </summary>
        [Description("会员权限不足")]
        UnAuth = 10403,
        /// <summary>
        /// 调用方法找不到
        /// </summary>
        [Description("调用方法找不到")]
        NoFound = 10831,
        /// <summary>
        /// 您发布的内容包含敏感词，<br>请重新编辑后再发布。
        /// </summary>
        [Description("您发布的内容包含敏感词，<br>请重新编辑后再发布。")]
        GarbageContent = 40003,
        /// <summary>
        /// 未绑定手机号
        /// </summary>
        [Description("未绑定手机号")]
        NotBindMobile = 40004,
        /// <summary>
        /// 系统异常
        /// </summary>
        [Description("系统异常")]
        Error = 500,

    }
}
