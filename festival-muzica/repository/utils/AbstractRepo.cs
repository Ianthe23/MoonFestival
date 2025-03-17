using System;
using System.Collections.Generic;

namespace festival_muzica.repository.utils
{
    /// <summary>
    /// Abstract class for a database repository
    /// </summary>
    /// <typeparam name="Id">type E must have an attribute of type Id</typeparam>
    /// <typeparam name="E">type of entities saved in repository</typeparam>
    public abstract class AbstractRepo<Id, E> : IRepository<Id, E> where E : Entity<Id>
    {
        protected IValidator<E> validator;
        protected String table;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validator">the validator</param>
        /// <param name="table">the table name</param>
        public AbstractRepo(IValidator<E> validator, String table)
        {
            this.validator = validator;
            this.table = table;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AbstractRepo()
        {
        }

        public abstract E FindOne(Id id);
        public abstract IEnumerable<E> FindAll();
        public abstract E Save(E entity);
        public abstract E Delete(Id id);
        public abstract E Update(E entity);
    }
}
