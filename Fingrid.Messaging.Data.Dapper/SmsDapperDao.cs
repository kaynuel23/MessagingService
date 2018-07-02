using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fingrid.Messaging.Core;
using Dapper;

namespace Fingrid.Messaging.Data.Dapper
{
    public class SmsDapperDao : DapperRepository<Sms>, ISmsDao
    {
        public SmsDapperDao() : base("[MessagesRealtime]")
        {
        }

        public async Task<bool> LogMessage(Sms sms)
        {
            try
            {
                string insertQuery = $@"INSERT INTO [dbo].{this._tableName}([MfbID],[To],[InstitutionName],[Body],[Status],[StatusMsg],[DateSent],
                                    [ReferenceNo],[NoOfAttempts],[AccountNo],[DateCreated],[IgnoreChargeOnInstitution],[NoOfParts],[UniqueId])
                                        OUTPUT inserted.ID 
                                        VALUES (@MfbID,@To,@InstitutionName,@Body,@StatusMsg,@StatusMsg,@DateSent,@ReferenceNo,@NoOfAttempts,
                                        @AccountNo,@DateCreated,@IgnoreChargeOnInstitution,@NoOfParts,@UniqueId)";
                await WithConnection(async c =>
                {
                    IEnumerable<int> result = await c.QueryAsync<int>(insertQuery, new
                    {
                        MfbID = sms.MfbID,
                        To = sms.To,
                        InstitutionName = sms.InstitutionName,
                        Body = sms.Body,
                        Status = sms.Status,
                        StatusMsg = sms.StatusMsg,
                        DateSent = DateTime.Now,
                        ReferenceNo = sms.ReferenceNo,
                        NoOfAttempts = sms.NoOfAttempts,
                        AccountNo = sms.AccountNo,
                        DateCreated = DateTime.Now,
                        IgnoreChargeOnInstitution = sms.IgnoreChargeOnInstitution,
                        NoOfParts = sms.NoOfParts,
                        UniqueId = sms.UniqueId
                    });
                    return sms.ID = result.SingleOrDefault();
                });
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> UpdateSmsStatus(Sms sms)
        {
            string updateQuery = $@"UPDATE {_tableName} SET StatusMsg = @StatusMsg, Status = @Status, SequenceNumber = @SequenceNumber,
                                    DeliveryStatus = @DeliveryStatus, MessageID = @MessageID, SMSCMSGID = @SMSCMSGID
                                    WHERE ID=@ID";
            await WithConnection(async c => {
                return await c.ExecuteAsync(updateQuery, new
                {
                    StatusMsg = sms.StatusMsg,
                    Status = sms.Status,
                    SequenceNumber = sms.SequenceNumber,
                    DeliveryStatus = sms.DeliveryStatus,
                    SMSCMSGID = sms.SMSCMSGID,
                    MessageID = sms.MessageID,
                    ID = sms.ID
                });
            });
            return true;
        }

        public async Task<Sms> GetBySequenceNumber(string sequenceNumber)
        {
            return await WithConnection(async c => {
                var results = await c.QueryFirstOrDefaultAsync<Sms>($"SELECT * FROM {_tableName} where SequenceNumber = @SequenceNumber", new { SequenceNumber = sequenceNumber });
                return results;
            });
        }
        public async Task<Sms> GetByUniqueID(string uniqueID)
        {
            return await WithConnection(async c => {
                var results = await c.QueryFirstOrDefaultAsync<Sms>($"SELECT * FROM {_tableName} where UniqueID = @UniqueID", new { UniqueID = uniqueID });
                return results;
            });
        }
        public async Task<Sms> GetByMessageID(string messageID)
        {
            return await WithConnection(async c => {
                var results = await c.QueryFirstOrDefaultAsync<Sms>($"SELECT * FROM {_tableName} where MessageID = @MessageID", new { MessageID = messageID });
                return results;
            });
        }

        public async Task<int> UniqueIDExists(string uniqueID)
        {
            return await WithConnection(async c => {
                var result = await c.QueryFirstOrDefaultAsync<int>($"SELECT count(*) FROM {_tableName} where UniqueID = @UniqueID", new { UniqueID = uniqueID });
                return result;
            });
        }
    }
}
