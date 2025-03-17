using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

namespace repository.utils
{
	public abstract class ConnectionFactory
	{
		protected ConnectionFactory()
		{
		}

		private static ConnectionFactory? instance;

		public static ConnectionFactory getInstance()
		{
			if (instance == null)
			{
				Assembly assem = Assembly.GetExecutingAssembly();
				Type[] types = assem.GetTypes();
				foreach (var type in types)
				{
					if (type.IsSubclassOf(typeof(ConnectionFactory)))
					{
						instance = (ConnectionFactory)Activator.CreateInstance(type) ?? 
							throw new InvalidOperationException($"Failed to create instance of {type.Name}");
						break;
					}
				}
				if (instance == null)
				{
					throw new InvalidOperationException("No ConnectionFactory implementation found");
				}
			}
			return instance;
		}

		public abstract  IDbConnection createConnection(IDictionary<string,string> props);
	}




}
