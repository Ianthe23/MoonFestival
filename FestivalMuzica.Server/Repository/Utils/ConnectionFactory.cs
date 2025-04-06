using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

namespace FestivalMuzica.Server.Repository.Utils
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
						var newInstance = Activator.CreateInstance(type);
						if (newInstance == null)
						{
							throw new InvalidOperationException($"Failed to create instance of {type.Name}");
						}
						instance = (ConnectionFactory)newInstance;
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
