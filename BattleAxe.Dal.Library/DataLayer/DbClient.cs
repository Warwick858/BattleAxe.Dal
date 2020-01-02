// ******************************************************************************************************************
//  This file is part of BattleAxe.Dal.
//
//  BattleAxe.Dal - web service for the data access layer.
//  Copyright(C)  2020  James LoForti
//  Contact Info: jamesloforti@gmail.com
//
//  BattleAxe.Dal is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.If not, see<https://www.gnu.org/licenses/>.
//									     ____.           .____             _____  _______   
//									    |    |           |    |    ____   /  |  | \   _  \  
//									    |    |   ______  |    |   /  _ \ /   |  |_/  /_\  \ 
//									/\__|    |  /_____/  |    |__(  <_> )    ^   /\  \_/   \
//									\________|           |_______ \____/\____   |  \_____  /
//									                             \/          |__|        \/ 
//
// ******************************************************************************************************************
//
using BattleAxe.Dal.Common.Model;
using Dapper;
using Serilog;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace BattleAxe.Dal.Library.DataLayer
{
	/// <summary>
	/// CRUD methods for database.
	/// </summary>
	public class DbClient
	{
		private readonly DbConnectionCreator _dbConnectionCreator;

		public DbClient(DbConnectionCreator dbConnectionCreator)
		{
			_dbConnectionCreator = dbConnectionCreator;
		}

		/// <summary>
		/// CREATE
		/// </summary>
		public int CreateRecord(GenericRequest record)
		{
			Log.Information($"Attempting to create record: {JsonSerializer.Serialize(record)} in the database.");

			using var dbConnection = _dbConnectionCreator.CreateConnection();

			var parms = new DynamicParameters();
			parms.Add("Message", record.Message);
			parms.Add("Id", record.Id);

			//Return parm
			parms.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

			var id = dbConnection.ExecuteScalar<int>("stored_proc_name", parms, commandType: CommandType.StoredProcedure);

			Log.Information($"Created record {record} and received id {id} from the database.");

			return id;
		} // end method

		/// <summary>
		/// READ SINGLE RECORD
		/// </summary>
		public string GetRecord(int id)
		{
			Log.Information($"Attempting to get record using id {id} from the database.");

			using var dbConnection = _dbConnectionCreator.CreateConnection();

			var record = dbConnection.QuerySingle<string>("stored_proc_name", id, commandType: CommandType.StoredProcedure);

			Log.Information($"Received record {record} using id {id} from the database.");

			return record;
		} // end method

		/// <summary>
		/// READ SINGLE RECORD
		/// </summary>
		public List<GenericRequest> GetRecords()
		{
			Log.Information($"Attempting to get all records from the database.");

			using var dbConnection = _dbConnectionCreator.CreateConnection();

			var records = dbConnection.Query<GenericRequest>("stored_proc_name", commandType: CommandType.StoredProcedure).ToList();

			Log.Information($"Received records {JsonSerializer.Serialize(records)} from the database.");

			return records;
		} // end method

		/// <summary>
		/// UPDATE
		/// </summary>
		public bool UpdateRecord(GenericRequest record)
		{
			Log.Information($"Attempting to update record: {JsonSerializer.Serialize(record)} in the database.");

			using var dbConnection = _dbConnectionCreator.CreateConnection();

			var parms = new DynamicParameters();
			parms.Add("Message", record.Message);
			parms.Add("Id", record.Id);

			//Return parm
			parms.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

			var result = dbConnection.Execute("stored_proc_name", parms, commandType: CommandType.StoredProcedure);
			var outcome = (result == -1);

			Log.Information($"Updated record {record} and received result {outcome} from the database.");

			return outcome;
		} // end method

		/// <summary>
		/// DELETE RECORD
		/// </summary>
		public void DeleteRecord(int id)
		{
			Log.Information($"Attempting to delete record using id {id} from the database.");

			using var dbConnection = _dbConnectionCreator.CreateConnection();

			var parms = new DynamicParameters();
			parms.Add("Id", id);

			var record = dbConnection.ExecuteScalar("stored_proc_name", parms, commandType: CommandType.StoredProcedure);

			Log.Information($"Deleted record using id {id} from the database.");
		} // end method
	} // end class
} // end namespace
