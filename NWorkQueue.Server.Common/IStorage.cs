﻿// <copyright file="IStorage.cs" company="Michael Silver">
// Copyright (c) Michael Silver. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using NWorkQueue.Common;
using NWorkQueue.Common.Models;
using NWorkQueue.Server.Common.Models;

namespace NWorkQueue.Server.Common
{
    /// <summary>
    /// The interface for storing and retrieving queue information from a storage mechinism, usually a database.
    /// When implementing IStorage, use the StorageSqlite as an example.  There should be no business logic in
    /// classes that implement IStorage.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Called when Queue process starts.  Connections to the storage should be made here, etc.
        /// </summary>
        /// <param name="deleteExistingData">Should all existing queues and messages be deleted.</param>
        /// <returns>ValueTask.</returns>
        ValueTask InitializeStorage(bool deleteExistingData);
        /*
        /// <summary>
        /// Mark the Queue Transaction as closed.
        /// </summary>
        /// <param name="transId">Queue Transaction to mark as closed.</param>
        /// <param name="storageTrans">The db transaction to assodicate with this update.</param>
        /// <param name="closeDateTime">Datetime transaction was closed.</param>
        void CloseTransaction(long transId, IStorageTransaction storageTrans, DateTime closeDateTime);

        /// <summary>
        /// Delete all messages by their queue transaction id.
        /// </summary>
        /// <param name="transId">Id of the queue transaction.</param>
        /// <param name="storageTrans">Storage Transaction.</param>
        void DeleteMessagesByTransId(long transId, IStorageTransaction storageTrans);

        /// <summary>
        /// Close messages that have too many retries.
        /// </summary>
        /// <param name="transId">Queue transaction id.</param>
        /// <param name="storageTrans">Storage Transaction.</param>
        /// <param name="closeDateTime">Datetime the message should be marked as closed.</param>
        void CloseRetriedMessages(long transId, IStorageTransaction storageTrans, DateTime closeDateTime);

        /// <summary>
        /// Expire messages past their expiration date/time.
        /// </summary>
        /// <param name="transId">Queue transaction id.</param>
        /// <param name="storageTrans">Storage Transaction.</param>
        /// <param name="closeDateTime">Datetime the message should be marked as closed.</param>
        /// <param name="expiryDateTime">Messages with expiry times before this will be marked closed.</param>
        void ExpireOlderMessages(long transId, IStorageTransaction storageTrans, DateTime closeDateTime, DateTime expiryDateTime);

        /// <summary>
        /// Update retry counts on rolledback transaction.
        /// </summary>
        /// <param name="transId">Queue transaction id.</param>
        /// <param name="storageTrans">Storage Transaction.</param>
        void UpdateRetriesOnRollbackedMessages(long transId, IStorageTransaction storageTrans);

        /// <summary>
        /// Commit added messages marking them as queued.
        /// </summary>
        /// <param name="transId">Queue transaction id.</param>
        /// <param name="storageTrans">Storage Transaction.</param>
        void CommitAddedMessages(long transId, IStorageTransaction storageTrans);

        /// <summary>
        /// Commit pulled messages marking them as complete.
        /// </summary>
        /// <param name="transId">Queue transaction id.</param>
        /// <param name="storageTrans">Storage Transaction.</param>
        /// <param name="commitDateTime">Datetime of the commit.</param>
        void CommitPulledMessages(long transId, IStorageTransaction storageTrans, DateTime commitDateTime);

        /// <summary>
        /// Commit a queue transaction.
        /// </summary>
        /// <param name="transId">Queue transaction id.</param>
        /// <param name="storageTrans">Storage Transaction.</param>
        /// <param name="commitDateTime">Datetime of the commit.</param>
        void CommitMessageTransaction(long transId, IStorageTransaction storageTrans, DateTime commitDateTime);
        */
        /// <summary>
        /// Create a new Queue in storage.
        /// </summary>
        /// <param name="name">Queue name.</param>
        /// <returns>ValueTask.</returns>
        ValueTask<long> AddQueue(string name);

        /// <summary>
        /// Delete a Queue and ALL messages in the Queue.
        /// </summary>
        /// <param name="id">Queue Id of the queue to delete.</param>
        /// <returns>ValueTask.</returns>
        ValueTask DeleteQueue(long id);

        /// <summary>
        /// Returns a Transaction object or null value if not found.
        /// </summary>
        /// <param name="transId">The Id of the transaction to lookup.</param>
        /// <returns>Transaction object or null if not found.</returns>
        ValueTask<Transaction?> GetTransactionById(long transId);

        /// <summary>
        /// Add a message to the storage.
        /// </summary>
        /// <param name="transId">Queue Transaction ID.</param>
        /// <param name="queueId">ID of the storage queue.</param>
        /// <param name="payload">Message data.</param>
        /// <param name="addDateTime">Datetime the message was added.</param>
        /// <param name="metaData">String metadata on message data.</param>
        /// <param name="priority">Message priority.</param>
        /// <param name="maxRetries">How many retries before expires.</param>
        /// <param name="expiryDateTime">Datetime the message will expire.</param>
        /// <param name="correlation">Correlation ID.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="transactionAction">Always will be "add".</param>
        /// <param name="messageState">"Always will be in transaction.</param>
        /// <returns>Message ID.</returns>
        ValueTask<long> AddMessage(
            long transId,
            long queueId,
            byte[] payload,
            DateTime addDateTime,
            string metaData,
            int priority,
            int maxRetries,
            DateTime? expiryDateTime,
            int correlation,
            string groupName,
            int transactionAction,
            int messageState);

        /// <summary>
        /// Returns the id and name of the Queue.  If no queue is found, returns null.
        /// </summary>
        /// <remarks>
        /// This search should be case sensitive, only use LIKE with SQLite.
        /// </remarks>
        /// <param name="name">Name of the queue to lookup.</param>
        /// <returns>QueueInfo containing ID and Name of queue.  Null if not found.</returns>
        ValueTask<QueueInfo?> GetQueueInfoByName(string name);

        /// <summary>
        /// Returns the id and name of the Queue.  If no queue is found, returns null.
        /// </summary>
        /// <remarks>
        /// This search should be case sensitive, only use LIKE with SQLite.
        /// </remarks>
        /// <param name="queueId">ID of the queue to lookup.</param>
        /// <returns>QueueInfo containing ID and Name of queue.  Null if not found.</returns>
        ValueTask<QueueInfo?> GetQueueInfoById(long queueId);

        /// <summary>
        /// Starts a transaction for use when adding or pulling messages.
        /// </summary>
        /// <param name="startDateTime">When the transaction started.</param>
        /// <param name="expiryDateTime">When the transaction will end.</param>
        /// <returns>Returns transaction ID.</returns>
        ValueTask<long> StartTransaction(DateTime startDateTime, DateTime expiryDateTime);

        ValueTask<int> UpdateMessages(IStorageTransaction storageTrans, long transId, int transactionAction, int oldMessageState, int newMessageState);

        /// <summary>
        /// Extends the transaction's expiration datetime.
        /// </summary>
        /// <param name="transId">The id of the transaction to update.</param>
        /// <param name="expiryDateTime">The new expiration datetime.</param>
        /// <returns>ValueTask.</returns>
        ValueTask ExtendTransaction(long transId, DateTime expiryDateTime);

        /// <summary>
        /// Update the transaction's state and end datetime.
        /// </summary>
        /// <param name="transId">The id of the transaction to update.</param>
        /// <param name="state">The new state of the transaction.</param>
        /// <param name="endDateTime">Datetime the transaction was closed (or null if not closed).</param>
        /// <returns>ValueTask.</returns>
        ValueTask UpdateTransactionState(long transId, TransactionState state, DateTime? endDateTime = null);

        /// <summary>
        /// Starts a storage (database) transaction, not a queue transaction.
        /// </summary>
        /// <remarks>
        /// Not all Storage classes will have internal transactions, so this can return a dummy class that performs no actions.
        /// </remarks>
        /// <returns>Returns a class represented by IStorageTransaction which can commit or rollbacl the transaction.</returns>
        IStorageTransaction BeginStorageTransaction();

        /*
        /// <summary>
        /// Does a Quque exist for the specified id.
        /// </summary>
        /// <param name="id">Quque ID.</param>
        /// <returns>true if quque exists.</returns>
        bool DoesQueueExist(long id);

        /// <summary>
        /// Delete all messages in a specified queue.
        /// </summary>
        /// <param name="queueId">Queue id.</param>
        /// <param name="storageTrans">Storage transaction.</param>
        void DeleteMessagesByQueueId(long queueId, IStorageTransaction storageTrans);
        */
    }
}