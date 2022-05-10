using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ImageBed.Data.Access
{
    public class SqlRecordData
    {
        private OurDbContext _context { get; set; }
        public SqlRecordData(OurDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取符合条件的所有记录数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<RecordEntity> GetAll(Func<RecordEntity, bool> predicate)
        {
            if ((_context != null) && (_context.Records != null))
            {
                return _context.Records.Where(predicate);
            }
            return Array.Empty<RecordEntity>();
        }


        /// <summary>
        /// 获取符合条件的第一条记录数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<RecordEntity?> GetFirstAsync(Expression<Func<RecordEntity, bool>> predicate)
        {
            if ((_context != null) && (_context.Records != null))
            {
                return await _context?.Records?.FirstOrDefaultAsync(predicate);
            }
            return null;
        }


        /// <summary>
        /// 更新记录信息
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(RecordEntity record)
        {
            if ((_context != null) && (_context.Records != null))
            {
                try
                {
                    if (record != null)
                    {
                        _context.Records.Update(record);
                        if(await _context.SaveChangesAsync() > 0)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Update record({record.Date}) failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(RecordEntity record)
        {
            if ((_context != null) && (_context.Records != null))
            {
                try
                {
                    await _context.Records.AddAsync(record);
                    if(await _context.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Add record ({record.Date}) failed, {ex.Message}");
                }
            }
            return false;
        }
    }
}
