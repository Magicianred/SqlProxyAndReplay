﻿using System.ServiceModel;
using ProductiveRage.SqlProxyAndReplay.DataProviderInterface.IDs;

namespace ProductiveRage.SqlProxyAndReplay.DataProviderInterface.Interfaces
{
	[ServiceContract]
	public interface IRemoteSqlParameterSet
	{
		[OperationContract]
		int Add(CommandId commandId, ParameterId parameterId);

		[OperationContract]
		ParameterId GetParameterByIndex(CommandId commandId, int index);
		[OperationContract]
		void SetParameterByIndex(CommandId commandId, int index, ParameterId parameterId);

		[OperationContract]
		ParameterId GetParameterByName(CommandId commandId, string parameterName);

		[OperationContract]
		void SetParameterByName(CommandId commandId, string parameterName, ParameterId parameterId);

		[OperationContract]
		int GetCount(CommandId commandId);

		[OperationContract]
		void Clear(CommandId commandId);
	}
}