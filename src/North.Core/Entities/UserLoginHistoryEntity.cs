﻿using SqlSugar;

namespace North.Core.Entities
{
    [SugarTable("UserLoginHistories")]
    public class UserLoginHistoryEntity : Entity
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        [SugarColumn(Length = 64)]
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// 登录地址
        /// </summary>
        [SugarColumn(Length = 16)]
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// 登录IP所在区域
        /// </summary>
        [SugarColumn(Length = 32)]
        public string IPRegion { get; set; } = string.Empty;

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// 关联用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
