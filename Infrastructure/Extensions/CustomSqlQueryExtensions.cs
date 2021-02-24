using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class CustomSqlQueryExtensions
    {

        public static List<T> RawSqlQuery<T>(DatabaseContext db, string query, Func<IDataRecord, T> map) where T : new()
        {
            var command = db.Database.GetDbConnection().CreateCommand();

            command.CommandText = query;
            command.CommandType = CommandType.Text;

            db.Database.OpenConnection();

            var result = command.ExecuteReader();

            var entities = new List<T>();

            T tmpValue = new T();

            while (result.Read())
            {
                if (typeof(T).IsClass)
                {
                    tmpValue = map(result);
                    entities.Add(tmpValue);
                }
                else
                {
                    if (result.GetFieldType(0).Name == "Decimal")
                    {
                        var z = Math.Round(result.GetFloat(0), 2);
                        tmpValue = (T)Convert.ChangeType(z, typeof(T));
                    }
                    else
                    {
                        tmpValue = map(result);
                    }
                    entities.Add(tmpValue);
                }
            }

            return entities;

        }

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }
    }
}