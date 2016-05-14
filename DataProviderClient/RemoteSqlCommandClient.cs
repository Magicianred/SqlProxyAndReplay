﻿using System;
using System.Data;
using ProductiveRage.SqlProxyAndReplay.DataProviderInterface;
using ProductiveRage.SqlProxyAndReplay.DataProviderInterface.IDs;
using ProductiveRage.SqlProxyAndReplay.DataProviderInterface.Interfaces;

namespace ProductiveRage.SqlProxyAndReplay.DataProviderClient
{
	internal sealed class RemoteSqlCommandClient : IDbCommand
	{
		private readonly IRemoteSqlConnection _connection;
		private readonly IRemoteSqlCommand _command;
		private readonly IRemoteSqlTransaction _transaction;
		private readonly IRemoteSqlDataReader _reader;
		private readonly CommandId _commandId;
		private bool _disposed;
		public RemoteSqlCommandClient(IRemoteSqlConnection connection, IRemoteSqlCommand command, IRemoteSqlTransaction transaction, IRemoteSqlDataReader reader, CommandId commandId)
		{
			if (connection == null)
				throw new ArgumentNullException(nameof(connection));
			if (command == null)
				throw new ArgumentNullException(nameof(command));
			if (transaction == null)
				throw new ArgumentNullException(nameof(transaction));
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));

			_connection = connection;
			_command = command;
			_transaction = transaction;
			_reader = reader;
			_commandId = commandId;
			_disposed = false;
		}
		~RemoteSqlCommandClient()
		{
			Dispose(false);
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
				_command.Dispose(_commandId); // Tell the service that the current Command is finished with

			_disposed = true;
		}

		public string CommandText
		{
			get { ThrowIfDisposed(); return _command.GetCommandText(_commandId); }
			set { ThrowIfDisposed(); _command.SetCommandText(_commandId, value); }
		}

		public int CommandTimeout
		{
			get { ThrowIfDisposed(); return _command.GetCommandTimeout(_commandId); }
			set { ThrowIfDisposed(); _command.SetCommandTimeout(_commandId, value); }
		}

		public CommandType CommandType
		{
			get { ThrowIfDisposed(); return _command.GetCommandType(_commandId); }
			set { ThrowIfDisposed(); _command.SetCommandType(_commandId, value); }
		}

		public IDbConnection Connection
		{
			get 
			{
				ThrowIfDisposed();
				var connectionId = _command.GetConnection(_commandId);
				if (connectionId == null)
					return null;
				return new RemoteSqlConnectionClient(_connection, _command, _transaction, _reader, connectionId);
			}
			set
			{
				ThrowIfDisposed();
				if (value == null)
					throw new ArgumentNullException(nameof(value));
				var remoteSqlConnection = value as RemoteSqlConnectionClient;
				if (remoteSqlConnection == null)
					throw new ArgumentException($"Connection must be a {typeof(RemoteSqlConnectionClient)}");
				_command.SetConnection(_commandId, remoteSqlConnection.ConnectionId);
			}
		}

		public IDataParameterCollection Parameters
		{
			get
			{
				ThrowIfDisposed();
				throw new NotImplementedException(); // TODO
			}
		}
		public IDbTransaction Transaction
		{
			get
			{
				ThrowIfDisposed();
				var transactionId = _command.GetTransaction(_commandId);
				if (transactionId == null)
					return null;
				return new RemoteSqlTransactionClient(_connection, _command, _transaction, _reader, transactionId);
			}
			set
			{
				ThrowIfDisposed();
				if (value == null)
					throw new ArgumentNullException(nameof(value));
				var remoteSqlTransaction = value as RemoteSqlTransactionClient;
				if (remoteSqlTransaction == null)
					throw new ArgumentException($"Transaction must be a {typeof(RemoteSqlTransactionClient)}");
				_command.SetTransaction(_commandId, remoteSqlTransaction.TransactionId);
			}
		}

		public UpdateRowSource UpdatedRowSource
		{
			get { ThrowIfDisposed(); return _command.GetUpdatedRowSource(_commandId); }
			set { ThrowIfDisposed(); _command.SetUpdatedRowSource(_commandId, value); }
		}
		public IDbDataParameter CreateParameter()
		{
			ThrowIfDisposed();
			throw new NotImplementedException(); // TODO
		}

		public void Prepare() { ThrowIfDisposed(); _command.Prepare(_commandId); }
		public void Cancel() { ThrowIfDisposed(); _command.Cancel(_commandId); }

		public int ExecuteNonQuery() { ThrowIfDisposed(); return _command.ExecuteNonQuery(_commandId); }
		public IDataReader ExecuteReader() { ThrowIfDisposed(); return ExecuteReader(CommandBehavior.Default); }
		public IDataReader ExecuteReader(CommandBehavior behavior) { ThrowIfDisposed(); return new RemoteSqlDataReaderClient(_reader, _command.ExecuteReader(_commandId, behavior)); }
		public object ExecuteScalar() { ThrowIfDisposed(); return _command.ExecuteScalar(_commandId); }

		private void ThrowIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException("command");
		}
	}
}
